using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Application.Interfaces.Media;
using Araponga.Application.Models;
using Araponga.Application.Services.Media;
using Araponga.Domain.Chat;
using Araponga.Domain.Media;
using Araponga.Domain.Membership;
using Araponga.Domain.Users;

namespace Araponga.Application.Services;

public sealed class ChatService
{
    private readonly IChatConversationRepository _conversationRepository;
    private readonly IChatConversationParticipantRepository _participantRepository;
    private readonly IChatMessageRepository _messageRepository;
    private readonly IChatConversationStatsRepository _statsRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMediaAssetRepository _mediaAssetRepository;
    private readonly IMediaAttachmentRepository _mediaAttachmentRepository;
    private readonly TerritoryMediaConfigService _mediaConfigService;
    private readonly FeatureFlagCacheService _featureFlags;
    private readonly AccessEvaluator _accessEvaluator;
    private readonly IUnitOfWork _unitOfWork;

    public ChatService(
        IChatConversationRepository conversationRepository,
        IChatConversationParticipantRepository participantRepository,
        IChatMessageRepository messageRepository,
        IChatConversationStatsRepository statsRepository,
        IUserRepository userRepository,
        IMediaAssetRepository mediaAssetRepository,
        IMediaAttachmentRepository mediaAttachmentRepository,
        TerritoryMediaConfigService mediaConfigService,
        FeatureFlagCacheService featureFlags,
        AccessEvaluator accessEvaluator,
        IUnitOfWork unitOfWork)
    {
        _conversationRepository = conversationRepository;
        _participantRepository = participantRepository;
        _messageRepository = messageRepository;
        _statsRepository = statsRepository;
        _userRepository = userRepository;
        _mediaAssetRepository = mediaAssetRepository;
        _mediaAttachmentRepository = mediaAttachmentRepository;
        _mediaConfigService = mediaConfigService;
        _featureFlags = featureFlags;
        _accessEvaluator = accessEvaluator;
        _unitOfWork = unitOfWork;
    }

    public async Task<OperationResult<IReadOnlyList<ChatConversation>>> ListOrCreateTerritoryChannelsAsync(
        Guid territoryId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        if (!_featureFlags.IsEnabled(territoryId, FeatureFlag.ChatEnabled))
        {
            return OperationResult<IReadOnlyList<ChatConversation>>.Failure("Chat is disabled for this territory.");
        }

        // Leitura de canais exige membership (visitor ou resident)
        var role = await _accessEvaluator.GetRoleAsync(userId, territoryId, cancellationToken);
        if (role is null)
        {
            return OperationResult<IReadOnlyList<ChatConversation>>.Failure("User has no membership in territory.");
        }

        var channels = new List<ChatConversation>();

        if (_featureFlags.IsEnabled(territoryId, FeatureFlag.ChatTerritoryPublicChannel))
        {
            var publicChannel = await EnsureChannelAsync(
                territoryId,
                userId,
                ConversationKind.TerritoryPublic,
                name: "Canal público",
                cancellationToken);
            channels.Add(publicChannel);
        }

        if (_featureFlags.IsEnabled(territoryId, FeatureFlag.ChatTerritoryResidentsChannel))
        {
            var residentsChannel = await EnsureChannelAsync(
                territoryId,
                userId,
                ConversationKind.TerritoryResidents,
                name: "Canal de moradores",
                cancellationToken);
            channels.Add(residentsChannel);
        }

        return OperationResult<IReadOnlyList<ChatConversation>>.Success(channels);
    }

    public async Task<OperationResult<IReadOnlyList<ChatConversation>>> ListActiveGroupsAsync(
        Guid territoryId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        if (!_featureFlags.IsEnabled(territoryId, FeatureFlag.ChatEnabled) ||
            !_featureFlags.IsEnabled(territoryId, FeatureFlag.ChatGroups))
        {
            return OperationResult<IReadOnlyList<ChatConversation>>.Failure("Groups are disabled for this territory.");
        }

        // Listagem exige membership no território (não revela grupos para fora)
        var role = await _accessEvaluator.GetRoleAsync(userId, territoryId, cancellationToken);
        if (role is null)
        {
            return OperationResult<IReadOnlyList<ChatConversation>>.Failure("User has no membership in territory.");
        }

        var groups = await _conversationRepository.ListGroupsAsync(
            territoryId,
            ConversationStatus.Active,
            cancellationToken);

        return OperationResult<IReadOnlyList<ChatConversation>>.Success(groups);
    }

    public async Task<OperationResult<ChatConversation>> CreateGroupAsync(
        Guid territoryId,
        Guid userId,
        string name,
        CancellationToken cancellationToken)
    {
        if (!_featureFlags.IsEnabled(territoryId, FeatureFlag.ChatEnabled) ||
            !_featureFlags.IsEnabled(territoryId, FeatureFlag.ChatGroups))
        {
            return OperationResult<ChatConversation>.Failure("Groups are disabled for this territory.");
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            return OperationResult<ChatConversation>.Failure("Group name is required.");
        }

        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            return OperationResult<ChatConversation>.Failure("User not found.");
        }

        if (user.IdentityVerificationStatus != UserIdentityVerificationStatus.Verified)
        {
            return OperationResult<ChatConversation>.Failure("User must be verified to create groups.");
        }

        var isResidentValidated = await _accessEvaluator.IsResidentAsync(userId, territoryId, cancellationToken);
        if (!isResidentValidated)
        {
            return OperationResult<ChatConversation>.Failure("Only validated residents can create groups.");
        }

        var conversation = new ChatConversation(
            Guid.NewGuid(),
            ConversationKind.Group,
            ConversationStatus.PendingApproval,
            territoryId,
            name,
            userId,
            DateTime.UtcNow,
            approvedByUserId: null,
            approvedAtUtc: null,
            lockedAtUtc: null,
            lockedByUserId: null,
            disabledAtUtc: null,
            disabledByUserId: null);

        await _conversationRepository.AddAsync(conversation, cancellationToken);

        var owner = new ConversationParticipant(
            conversation.Id,
            userId,
            ConversationParticipantRole.Owner,
            DateTime.UtcNow,
            leftAtUtc: null,
            mutedUntilUtc: null,
            lastReadMessageId: null,
            lastReadAtUtc: null);

        await _participantRepository.AddAsync(owner, cancellationToken);

        await _statsRepository.UpsertAsync(
            new ChatConversationStats(conversation.Id, null, null, null, null, 0),
            cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);
        return OperationResult<ChatConversation>.Success(conversation);
    }

    public async Task<OperationResult<ChatConversation>> ApproveGroupAsync(
        Guid territoryId,
        Guid groupId,
        Guid approverUserId,
        CancellationToken cancellationToken)
    {
        var isCurator = await _accessEvaluator.HasCapabilityAsync(
            approverUserId,
            territoryId,
            MembershipCapabilityType.Curator,
            cancellationToken);
        if (!isCurator)
        {
            return OperationResult<ChatConversation>.Failure("User is not a curator for territory.");
        }

        var conversation = await _conversationRepository.GetByIdAsync(groupId, cancellationToken);
        if (conversation is null || conversation.TerritoryId != territoryId || conversation.Kind != ConversationKind.Group)
        {
            return OperationResult<ChatConversation>.Failure("Group not found.");
        }

        conversation.Approve(approverUserId, DateTime.UtcNow);
        await _conversationRepository.UpdateAsync(conversation, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return OperationResult<ChatConversation>.Success(conversation);
    }

    public async Task<OperationResult<ChatConversation>> DisableGroupAsync(
        Guid territoryId,
        Guid groupId,
        Guid actorUserId,
        CancellationToken cancellationToken)
    {
        var isModerator = await _accessEvaluator.HasCapabilityAsync(
            actorUserId,
            territoryId,
            MembershipCapabilityType.Moderator,
            cancellationToken);
        if (!isModerator)
        {
            return OperationResult<ChatConversation>.Failure("User is not a moderator for territory.");
        }

        var conversation = await _conversationRepository.GetByIdAsync(groupId, cancellationToken);
        if (conversation is null || conversation.TerritoryId != territoryId || conversation.Kind != ConversationKind.Group)
        {
            return OperationResult<ChatConversation>.Failure("Group not found.");
        }

        conversation.Disable(actorUserId, DateTime.UtcNow);
        await _conversationRepository.UpdateAsync(conversation, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return OperationResult<ChatConversation>.Success(conversation);
    }

    public async Task<OperationResult<ChatConversation>> LockGroupAsync(
        Guid territoryId,
        Guid groupId,
        Guid actorUserId,
        CancellationToken cancellationToken)
    {
        var isModerator = await _accessEvaluator.HasCapabilityAsync(
            actorUserId,
            territoryId,
            MembershipCapabilityType.Moderator,
            cancellationToken);
        if (!isModerator)
        {
            return OperationResult<ChatConversation>.Failure("User is not a moderator for territory.");
        }

        var conversation = await _conversationRepository.GetByIdAsync(groupId, cancellationToken);
        if (conversation is null || conversation.TerritoryId != territoryId || conversation.Kind != ConversationKind.Group)
        {
            return OperationResult<ChatConversation>.Failure("Group not found.");
        }

        conversation.Lock(actorUserId, DateTime.UtcNow);
        await _conversationRepository.UpdateAsync(conversation, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return OperationResult<ChatConversation>.Success(conversation);
    }

    public async Task<OperationResult<ChatConversation>> GetConversationAsync(
        Guid conversationId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var conversation = await _conversationRepository.GetByIdAsync(conversationId, cancellationToken);
        if (conversation is null)
        {
            return OperationResult<ChatConversation>.Failure("Conversation not found.");
        }

        var access = await CanAccessConversationAsync(conversation, userId, cancellationToken);
        if (!access)
        {
            return OperationResult<ChatConversation>.Failure("Forbidden.");
        }

        return OperationResult<ChatConversation>.Success(conversation);
    }

    public async Task<OperationResult<IReadOnlyList<ChatMessage>>> ListMessagesAsync(
        Guid conversationId,
        Guid userId,
        DateTime? beforeCreatedAtUtc,
        Guid? beforeMessageId,
        int limit,
        CancellationToken cancellationToken)
    {
        if (limit <= 0 || limit > 100)
        {
            limit = 50;
        }

        var conversation = await _conversationRepository.GetByIdAsync(conversationId, cancellationToken);
        if (conversation is null)
        {
            return OperationResult<IReadOnlyList<ChatMessage>>.Failure("Conversation not found.");
        }

        var access = await CanAccessConversationAsync(conversation, userId, cancellationToken);
        if (!access)
        {
            return OperationResult<IReadOnlyList<ChatMessage>>.Failure("Forbidden.");
        }

        // Para canais, criar estado de participante sob demanda (para read receipts / mute etc.)
        await EnsureParticipantForImplicitConversationsAsync(conversation, userId, cancellationToken);

        var messages = await _messageRepository.ListByConversationAsync(
            conversationId,
            beforeCreatedAtUtc,
            beforeMessageId,
            limit,
            cancellationToken);

        return OperationResult<IReadOnlyList<ChatMessage>>.Success(messages);
    }

    public async Task<OperationResult<ChatMessage>> SendTextMessageAsync(
        Guid conversationId,
        Guid userId,
        string text,
        Guid? mediaId,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return OperationResult<ChatMessage>.Failure("Message text is required.");
        }

        var conversation = await _conversationRepository.GetByIdAsync(conversationId, cancellationToken);
        if (conversation is null)
        {
            return OperationResult<ChatMessage>.Failure("Conversation not found.");
        }

        var canSend = await CanSendMessageAsync(conversation, userId, cancellationToken);
        if (!canSend)
        {
            return OperationResult<ChatMessage>.Failure("Forbidden.");
        }

        await EnsureParticipantForImplicitConversationsAsync(conversation, userId, cancellationToken);

        if (conversation.Status == ConversationStatus.Locked)
        {
            return OperationResult<ChatMessage>.Failure("Conversation is locked.");
        }

        if (conversation.Status == ConversationStatus.Disabled)
        {
            return OperationResult<ChatMessage>.Failure("Conversation is disabled.");
        }

        // Validar mídia se fornecida
        if (mediaId.HasValue && mediaId.Value != Guid.Empty)
        {
            var mediaAsset = await _mediaAssetRepository.GetByIdAsync(mediaId.Value, cancellationToken);
            if (mediaAsset is null)
            {
                return OperationResult<ChatMessage>.Failure("Media asset not found.");
            }

            if (mediaAsset.UploadedByUserId != userId || mediaAsset.IsDeleted)
            {
                return OperationResult<ChatMessage>.Failure("Media asset is invalid or does not belong to the user.");
            }

            // Validar tipo: apenas imagens e áudios em chat (vídeos não permitidos)
            if (mediaAsset.MediaType == MediaType.Video)
            {
                return OperationResult<ChatMessage>.Failure("Videos are not allowed in chat messages.");
            }
            if (mediaAsset.MediaType != MediaType.Image && mediaAsset.MediaType != MediaType.Audio)
            {
                return OperationResult<ChatMessage>.Failure("Only images and audio are allowed in chat messages.");
            }

            // Obter limites efetivos da configuração territorial (com fallback para global)
            // Nota: DMs não têm TerritoryId, então usar limites globais padrão
            if (conversation.TerritoryId.HasValue)
            {
                var chatLimits = await _mediaConfigService.GetEffectiveChatLimitsAsync(
                    conversation.TerritoryId.Value,
                    cancellationToken);

                // Validar imagens
                if (mediaAsset.MediaType == MediaType.Image)
                {
                    if (!chatLimits.ImagesEnabled)
                    {
                        return OperationResult<ChatMessage>.Failure("Images are not enabled for chat in this territory.");
                    }
                    if (mediaAsset.SizeBytes > chatLimits.MaxImageSizeBytes)
                    {
                        var maxSizeMB = chatLimits.MaxImageSizeBytes / (1024.0 * 1024.0);
                        return OperationResult<ChatMessage>.Failure($"Image size exceeds {maxSizeMB:F1}MB limit for chat.");
                    }
                    // Validar tipo MIME se configurado
                    if (chatLimits.AllowedImageMimeTypes != null && chatLimits.AllowedImageMimeTypes.Count > 0)
                    {
                        if (!chatLimits.AllowedImageMimeTypes.Contains(mediaAsset.MimeType, StringComparer.OrdinalIgnoreCase))
                        {
                            return OperationResult<ChatMessage>.Failure($"Image MIME type '{mediaAsset.MimeType}' is not allowed for chat.");
                        }
                    }
                }
                // Validar áudios
                else if (mediaAsset.MediaType == MediaType.Audio)
                {
                    if (!chatLimits.AudioEnabled)
                    {
                        return OperationResult<ChatMessage>.Failure("Audio is not enabled for chat in this territory.");
                    }
                    if (mediaAsset.SizeBytes > chatLimits.MaxAudioSizeBytes)
                    {
                        var maxSizeMB = chatLimits.MaxAudioSizeBytes / (1024.0 * 1024.0);
                        return OperationResult<ChatMessage>.Failure($"Audio size exceeds {maxSizeMB:F1}MB limit for chat.");
                    }
                    // Validar tipo MIME se configurado
                    if (chatLimits.AllowedAudioMimeTypes != null && chatLimits.AllowedAudioMimeTypes.Count > 0)
                    {
                        if (!chatLimits.AllowedAudioMimeTypes.Contains(mediaAsset.MimeType, StringComparer.OrdinalIgnoreCase))
                        {
                            return OperationResult<ChatMessage>.Failure($"Audio MIME type '{mediaAsset.MimeType}' is not allowed for chat.");
                        }
                    }
                }
            }
            else
            {
                // Para DMs (sem TerritoryId), usar limites padrão fixos como fallback
                if (mediaAsset.MediaType == MediaType.Image && mediaAsset.SizeBytes > 5 * 1024 * 1024) // 5MB
                {
                    return OperationResult<ChatMessage>.Failure("Image size exceeds 5MB limit for chat.");
                }
                if (mediaAsset.MediaType == MediaType.Audio && mediaAsset.SizeBytes > 2 * 1024 * 1024) // 2MB
                {
                    return OperationResult<ChatMessage>.Failure("Audio size exceeds 2MB limit for chat.");
                }
            }
        }

        var message = new ChatMessage(
            Guid.NewGuid(),
            conversationId,
            userId,
            MessageContentType.Text,
            text,
            payloadJson: null,
            DateTime.UtcNow,
            editedAtUtc: null,
            deletedAtUtc: null,
            deletedByUserId: null);

        await _messageRepository.AddAsync(message, cancellationToken);

        // Criar MediaAttachment se mídia foi fornecida
        if (mediaId.HasValue && mediaId.Value != Guid.Empty)
        {
            var attachment = new MediaAttachment(
                Guid.NewGuid(),
                mediaId.Value,
                MediaOwnerType.ChatMessage,
                message.Id,
                0,
                DateTime.UtcNow);

            await _mediaAttachmentRepository.AddAsync(attachment, cancellationToken);
        }

        // Atualizar stats (upsert)
        var existingStats = await _statsRepository.GetAsync(conversationId, cancellationToken);
        var newCount = (existingStats?.MessageCount ?? 0) + 1;
        var preview = message.Text is null
            ? null
            : (message.Text.Length <= 200 ? message.Text : message.Text[..200]);

        await _statsRepository.UpsertAsync(
            new ChatConversationStats(
                conversationId,
                message.Id,
                message.CreatedAtUtc,
                userId,
                preview,
                newCount),
            cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);
        return OperationResult<ChatMessage>.Success(message);
    }

    public async Task<OperationResult<IReadOnlyList<ConversationParticipant>>> ListParticipantsAsync(
        Guid conversationId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var conversation = await _conversationRepository.GetByIdAsync(conversationId, cancellationToken);
        if (conversation is null)
        {
            return OperationResult<IReadOnlyList<ConversationParticipant>>.Failure("Conversation not found.");
        }

        var access = await CanAccessConversationAsync(conversation, userId, cancellationToken);
        if (!access)
        {
            return OperationResult<IReadOnlyList<ConversationParticipant>>.Failure("Forbidden.");
        }

        // Para canais, não listamos "participantes implícitos" (território inteiro).
        if (conversation.Kind is ConversationKind.TerritoryPublic or ConversationKind.TerritoryResidents)
        {
            return OperationResult<IReadOnlyList<ConversationParticipant>>.Success(Array.Empty<ConversationParticipant>());
        }

        var participants = await _participantRepository.ListByConversationAsync(conversationId, cancellationToken);
        return OperationResult<IReadOnlyList<ConversationParticipant>>.Success(participants);
    }

    public async Task<OperationResult> AddParticipantAsync(
        Guid conversationId,
        Guid actorUserId,
        Guid newUserId,
        CancellationToken cancellationToken)
    {
        if (newUserId == Guid.Empty)
        {
            return OperationResult.Failure("UserId is required.");
        }

        var conversation = await _conversationRepository.GetByIdAsync(conversationId, cancellationToken);
        if (conversation is null)
        {
            return OperationResult.Failure("Conversation not found.");
        }

        if (conversation.Kind != ConversationKind.Group)
        {
            return OperationResult.Failure("Participants can only be managed for groups.");
        }

        var actor = await _participantRepository.GetAsync(conversationId, actorUserId, cancellationToken);
        if (actor is null || actor.Role != ConversationParticipantRole.Owner)
        {
            return OperationResult.Failure("Only group owners can add participants.");
        }

        var exists = await _participantRepository.GetAsync(conversationId, newUserId, cancellationToken);
        if (exists is not null)
        {
            return OperationResult.Success();
        }

        var participant = new ConversationParticipant(
            conversationId,
            newUserId,
            ConversationParticipantRole.Member,
            DateTime.UtcNow,
            leftAtUtc: null,
            mutedUntilUtc: null,
            lastReadMessageId: null,
            lastReadAtUtc: null);

        await _participantRepository.AddAsync(participant, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return OperationResult.Success();
    }

    public async Task<OperationResult> RemoveParticipantAsync(
        Guid conversationId,
        Guid actorUserId,
        Guid targetUserId,
        CancellationToken cancellationToken)
    {
        var conversation = await _conversationRepository.GetByIdAsync(conversationId, cancellationToken);
        if (conversation is null)
        {
            return OperationResult.Failure("Conversation not found.");
        }

        if (conversation.Kind != ConversationKind.Group)
        {
            return OperationResult.Failure("Participants can only be managed for groups.");
        }

        var actor = await _participantRepository.GetAsync(conversationId, actorUserId, cancellationToken);
        if (actor is null || actor.Role != ConversationParticipantRole.Owner)
        {
            return OperationResult.Failure("Only group owners can remove participants.");
        }

        if (targetUserId == actorUserId)
        {
            return OperationResult.Failure("Owner cannot remove themselves.");
        }

        await _participantRepository.RemoveAsync(conversationId, targetUserId, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return OperationResult.Success();
    }

    public async Task<OperationResult> MarkReadAsync(
        Guid conversationId,
        Guid userId,
        Guid messageId,
        CancellationToken cancellationToken)
    {
        var conversation = await _conversationRepository.GetByIdAsync(conversationId, cancellationToken);
        if (conversation is null)
        {
            return OperationResult.Failure("Conversation not found.");
        }

        var access = await CanAccessConversationAsync(conversation, userId, cancellationToken);
        if (!access)
        {
            return OperationResult.Failure("Forbidden.");
        }

        await EnsureParticipantForImplicitConversationsAsync(conversation, userId, cancellationToken);

        var participant = await _participantRepository.GetAsync(conversationId, userId, cancellationToken);
        if (participant is null)
        {
            return OperationResult.Failure("Participant not found.");
        }

        participant.MarkRead(messageId, DateTime.UtcNow);
        await _participantRepository.UpdateAsync(participant, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return OperationResult.Success();
    }

    private async Task<ChatConversation> EnsureChannelAsync(
        Guid territoryId,
        Guid userId,
        ConversationKind kind,
        string name,
        CancellationToken cancellationToken)
    {
        var existing = await _conversationRepository.GetTerritoryChannelAsync(territoryId, kind, cancellationToken);
        if (existing is not null)
        {
            return existing;
        }

        var channel = new ChatConversation(
            Guid.NewGuid(),
            kind,
            ConversationStatus.Active,
            territoryId,
            name,
            userId,
            DateTime.UtcNow,
            approvedByUserId: userId,
            approvedAtUtc: DateTime.UtcNow,
            lockedAtUtc: null,
            lockedByUserId: null,
            disabledAtUtc: null,
            disabledByUserId: null);

        await _conversationRepository.AddAsync(channel, cancellationToken);
        await _statsRepository.UpsertAsync(new ChatConversationStats(channel.Id, null, null, null, null, 0), cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return channel;
    }

    private async Task EnsureParticipantForImplicitConversationsAsync(
        ChatConversation conversation,
        Guid userId,
        CancellationToken cancellationToken)
    {
        if (conversation.Kind is not (ConversationKind.TerritoryPublic or ConversationKind.TerritoryResidents))
        {
            return;
        }

        var existing = await _participantRepository.GetAsync(conversation.Id, userId, cancellationToken);
        if (existing is not null)
        {
            return;
        }

        var participant = new ConversationParticipant(
            conversation.Id,
            userId,
            ConversationParticipantRole.Member,
            DateTime.UtcNow,
            leftAtUtc: null,
            mutedUntilUtc: null,
            lastReadMessageId: null,
            lastReadAtUtc: null);

        await _participantRepository.AddAsync(participant, cancellationToken);
    }

    private async Task<bool> CanAccessConversationAsync(
        ChatConversation conversation,
        Guid userId,
        CancellationToken cancellationToken)
    {
        if (conversation.Kind == ConversationKind.Direct)
        {
            // DM é território-escopado (feature flags por território).
            // Falha segura: sem TerritoryId não há como validar flag => negar acesso.
            if (conversation.TerritoryId is null)
            {
                return false;
            }

            var directTerritoryId = conversation.TerritoryId.Value;
            if (!_featureFlags.IsEnabled(directTerritoryId, FeatureFlag.ChatEnabled) ||
                !_featureFlags.IsEnabled(directTerritoryId, FeatureFlag.ChatDmEnabled))
            {
                return false;
            }

            var participant = await _participantRepository.GetAsync(conversation.Id, userId, cancellationToken);
            return participant is not null;
        }

        if (conversation.TerritoryId is null)
        {
            return false;
        }

        var territoryId = conversation.TerritoryId.Value;

        if (!_featureFlags.IsEnabled(territoryId, FeatureFlag.ChatEnabled))
        {
            return false;
        }

        if (conversation.Kind == ConversationKind.Group && !_featureFlags.IsEnabled(territoryId, FeatureFlag.ChatGroups))
        {
            return false;
        }

        if (conversation.Kind == ConversationKind.TerritoryPublic && !_featureFlags.IsEnabled(territoryId, FeatureFlag.ChatTerritoryPublicChannel))
        {
            return false;
        }

        if (conversation.Kind == ConversationKind.TerritoryResidents && !_featureFlags.IsEnabled(territoryId, FeatureFlag.ChatTerritoryResidentsChannel))
        {
            return false;
        }

        // Group: só membros
        if (conversation.Kind == ConversationKind.Group)
        {
            var participant = await _participantRepository.GetAsync(conversation.Id, userId, cancellationToken);
            return participant is not null;
        }

        // Canais: membership no território
        var role = await _accessEvaluator.GetRoleAsync(userId, territoryId, cancellationToken);
        if (role is null)
        {
            return false;
        }

        if (conversation.Kind == ConversationKind.TerritoryResidents)
        {
            var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
            if (user is null || user.IdentityVerificationStatus != UserIdentityVerificationStatus.Verified)
            {
                return false;
            }

            return await _accessEvaluator.IsResidentAsync(userId, territoryId, cancellationToken);
        }

        return true;
    }

    private async Task<bool> CanSendMessageAsync(
        ChatConversation conversation,
        Guid userId,
        CancellationToken cancellationToken)
    {
        if (!await CanAccessConversationAsync(conversation, userId, cancellationToken))
        {
            return false;
        }

        if (conversation.Kind == ConversationKind.TerritoryPublic)
        {
            if (conversation.TerritoryId is null)
            {
                return false;
            }

            var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
            if (user is null || user.IdentityVerificationStatus != UserIdentityVerificationStatus.Verified)
            {
                return false;
            }

            return await _accessEvaluator.IsResidentAsync(userId, conversation.TerritoryId.Value, cancellationToken);
        }

        if (conversation.Kind == ConversationKind.TerritoryResidents)
        {
            // já validado em CanAccessConversationAsync
            return true;
        }

        if (conversation.Kind == ConversationKind.Group)
        {
            // Para grupos, exigir que o grupo esteja ativo (aprovado) e que o user esteja verificado
            if (conversation.Status != ConversationStatus.Active)
            {
                return false;
            }

            var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
            if (user is null || user.IdentityVerificationStatus != UserIdentityVerificationStatus.Verified)
            {
                return false;
            }

            return true;
        }

        // DM (fase 2): aqui ainda seria gateado por flag e preferências/bloqueios
        return true;
    }
}

