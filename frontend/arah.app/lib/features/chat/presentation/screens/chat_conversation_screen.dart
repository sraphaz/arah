import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:intl/intl.dart';

import '../../../../core/config/constants.dart';
import '../../../../core/network/api_exception.dart';
import '../../../../core/theme/app_design_tokens.dart';
import '../../../../core/widgets/app_snackbar.dart';
import '../../../../core/widgets/arah_scaffold.dart';
import '../../../../l10n/app_localizations.dart';
import '../../../auth/presentation/providers/auth_state_provider.dart';
import '../providers/chat_provider.dart';

class ChatConversationScreen extends ConsumerStatefulWidget {
  const ChatConversationScreen({super.key, required this.conversationId, this.title});

  final String conversationId;
  final String? title;

  @override
  ConsumerState<ChatConversationScreen> createState() => _ChatConversationScreenState();
}

class _ChatConversationScreenState extends ConsumerState<ChatConversationScreen> {
  final _controller = TextEditingController();
  bool _sending = false;

  @override
  void dispose() {
    _controller.dispose();
    super.dispose();
  }

  Future<void> _send() async {
    if (_sending) return;
    setState(() => _sending = true);
    try {
      await ref.read(chatConversationProvider(widget.conversationId).notifier).send(_controller.text);
      _controller.clear();
    } catch (e) {
      if (mounted) {
        final l10n = AppLocalizations.of(context)!;
        showErrorSnackBar(
          context,
          e is ApiException ? e.userMessage : l10n.errorSendMessage,
        );
      }
    } finally {
      if (mounted) setState(() => _sending = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    final colors = context.appColors;
    final state = ref.watch(chatConversationProvider(widget.conversationId));
    final currentUserId = ref.watch(currentUserProvider)?.id;
    final timeFormat = DateFormat('HH:mm');

    return ArahScaffold(
      appBar: AppBar(title: Text(widget.title ?? l10n.conversation)),
      body: Column(
        children: [
          Expanded(
            child: state.isLoading && state.messages.isEmpty
                ? Center(
                    child: SizedBox(
                      width: AppConstants.loadingIndicatorSize,
                      height: AppConstants.loadingIndicatorSize,
                      child: CircularProgressIndicator(
                        strokeWidth: 2,
                        color: colors.primary,
                      ),
                    ),
                  )
                : ListView.builder(
                    padding: const EdgeInsets.all(AppConstants.spacingMd),
                    itemCount: state.messages.length,
                    itemBuilder: (context, index) {
                      final msg = state.messages[index];
                      final isMine = currentUserId != null &&
                          currentUserId.isNotEmpty &&
                          msg.senderUserId == currentUserId;
                      return Align(
                        alignment: isMine ? Alignment.centerRight : Alignment.centerLeft,
                        child: Container(
                          constraints: BoxConstraints(
                            maxWidth: MediaQuery.sizeOf(context).width * 0.78,
                          ),
                          margin: const EdgeInsets.only(bottom: AppConstants.spacingSm),
                          padding: const EdgeInsets.symmetric(
                            horizontal: AppConstants.spacingMd,
                            vertical: AppConstants.spacingSm,
                          ),
                          decoration: BoxDecoration(
                            color: isMine
                                ? colors.primary.withValues(alpha: 0.18)
                                : colors.surfaceContainer,
                            borderRadius: BorderRadius.circular(AppConstants.radiusMd),
                            border: Border.all(
                              color: isMine ? colors.accentBorder : colors.outlineSubtle,
                            ),
                          ),
                          child: Column(
                            crossAxisAlignment:
                                isMine ? CrossAxisAlignment.end : CrossAxisAlignment.start,
                            children: [
                              Text(
                                msg.text,
                                style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                                      color: colors.onSurface,
                                    ),
                              ),
                              const SizedBox(height: AppConstants.spacingXs),
                              Text(
                                timeFormat.format(msg.createdAtUtc.toLocal()),
                                style: Theme.of(context).textTheme.labelSmall?.copyWith(
                                      color: colors.onSurfaceSubtle,
                                    ),
                              ),
                            ],
                          ),
                        ),
                      );
                    },
                  ),
          ),
          SafeArea(
            child: Padding(
              padding: const EdgeInsets.all(AppConstants.spacingSm),
              child: Row(
                children: [
                  Expanded(
                    child: TextField(
                      controller: _controller,
                      decoration: InputDecoration(
                        hintText: l10n.messageHint,
                        border: const OutlineInputBorder(),
                      ),
                      onSubmitted: (_) => _send(),
                    ),
                  ),
                  IconButton(
                    onPressed: _sending ? null : _send,
                    icon: _sending
                        ? SizedBox(
                            width: AppConstants.loadingIndicatorSize,
                            height: AppConstants.loadingIndicatorSize,
                            child: CircularProgressIndicator(
                              strokeWidth: 2,
                              color: colors.primary,
                            ),
                          )
                        : Icon(Icons.send, color: colors.primary),
                  ),
                ],
              ),
            ),
          ),
        ],
      ),
    );
  }
}
