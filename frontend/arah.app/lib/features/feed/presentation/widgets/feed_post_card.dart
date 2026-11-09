import 'package:cached_network_image/cached_network_image.dart';
import 'package:flutter/material.dart';

import '../../../../core/config/constants.dart';
import '../../../../core/theme/app_design_tokens.dart';
import '../../../../core/widgets/arah_material_symbol.dart';
import '../../../../core/widgets/arah_role_badge.dart';

enum FeedPostType {
  general,
  alert,
  event,
  tip,
}

class FeedPostCard extends StatelessWidget {
  const FeedPostCard({
    super.key,
    required this.title,
    required this.authorName,
    this.content,
    this.authorInitial,
    this.authorHandle,
    this.authorRole,
    this.authorAvatarUrl,
    this.timeLabel,
    this.type = FeedPostType.general,
    this.visibilityLabel,
    this.likeCount = 0,
    this.commentCount = 0,
    this.shareCount = 0,
    this.isLiked = false,
    this.isShared = false,
    this.mediaUrls = const [],
    this.isPinned = false,
    this.onMorePressed,
    this.onLikePressed,
    this.onCommentPressed,
    this.onSharePressed,
    this.onTap,
  });

  final String title;
  final String authorName;
  final String? content;
  final String? authorInitial;
  final String? authorHandle;
  final String? authorRole;
  final String? authorAvatarUrl;
  final String? timeLabel;
  final FeedPostType type;
  final String? visibilityLabel;
  final int likeCount;
  final int commentCount;
  final int shareCount;
  final bool isLiked;
  final bool isShared;
  final List<String> mediaUrls;
  final bool isPinned;
  final VoidCallback? onMorePressed;
  final VoidCallback? onLikePressed;
  final VoidCallback? onCommentPressed;
  final VoidCallback? onSharePressed;
  final VoidCallback? onTap;

  static Color _colorForType(FeedPostType type) {
    switch (type) {
      case FeedPostType.alert:
        return AppDesignTokens.feedTypeAlert;
      case FeedPostType.event:
        return AppDesignTokens.feedTypeEvent;
      case FeedPostType.tip:
        return AppDesignTokens.feedTypeTip;
      case FeedPostType.general:
        return AppDesignTokens.primary;
    }
  }

  static FeedPostType typeFromString(String? value) {
    if (value == null) return FeedPostType.general;
    switch (value.toLowerCase()) {
      case 'alert':
        return FeedPostType.alert;
      case 'event':
        return FeedPostType.event;
      case 'tip':
        return FeedPostType.tip;
      default:
        return FeedPostType.general;
    }
  }

  @override
  Widget build(BuildContext context) {
    final colors = context.appColors;
    final typeColor = _colorForType(type);
    final showAlertPill = type == FeedPostType.alert;
    final hasMetaLine =
        (authorHandle?.isNotEmpty ?? false) || (timeLabel?.isNotEmpty ?? false);
    final isResidentAuthor = authorRole != null &&
        ArahRoleBadge.kindFrom(authorRole!) == ArahRoleKind.resident;

    return DecoratedBox(
      decoration: BoxDecoration(
        gradient: const LinearGradient(
          begin: Alignment.topLeft,
          end: Alignment.bottomRight,
          colors: [
            AppDesignTokens.surfaceCardGradientStart,
            AppDesignTokens.surfaceCardGradientEnd,
          ],
        ),
        borderRadius: BorderRadius.circular(AppConstants.radiusCard),
        border: Border.all(
          color: isPinned
              ? AppDesignTokens.warning.withValues(alpha: 0.28)
              : colors.outline,
        ),
        boxShadow: AppDesignTokens.elevation(isPinned ? 3 : 2, isDark: true),
      ),
      child: Material(
        color: Colors.transparent,
        child: InkWell(
          borderRadius: BorderRadius.circular(AppConstants.radiusCard),
          onTap: onTap,
          child: LayoutBuilder(
            builder: (context, constraints) {
              final compact = constraints.maxWidth < 336;
              return Padding(
                padding: const EdgeInsets.all(16),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    if (isPinned) ...[
                      Row(
                        children: [
                          const ArahMaterialSymbol(
                            'push_pin',
                            size: 14,
                            fill: 1,
                            color: AppDesignTokens.warning,
                          ),
                          const SizedBox(width: 5),
                          Text(
                            'Fixado pelo conselho',
                            style: Theme.of(context)
                                .textTheme
                                .labelSmall
                                ?.copyWith(
                                  fontSize: 11.5,
                                  fontWeight: FontWeight.w600,
                                  color: AppDesignTokens.warning,
                                ),
                          ),
                        ],
                      ),
                      const SizedBox(height: 11),
                    ],
                    Row(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        _FeedAvatar(
                          name: authorInitial ?? authorName,
                          avatarUrl: authorAvatarUrl,
                          resident: isResidentAuthor,
                        ),
                        const SizedBox(width: 11),
                        Expanded(
                          child: Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              Row(
                                crossAxisAlignment: CrossAxisAlignment.start,
                                children: [
                                  Expanded(
                                    child: Text(
                                      authorName,
                                      maxLines: 2,
                                      overflow: TextOverflow.ellipsis,
                                      style: Theme.of(context)
                                          .textTheme
                                          .titleMedium
                                          ?.copyWith(
                                            fontSize: 15,
                                            letterSpacing: -0.2,
                                          ),
                                    ),
                                  ),
                                  if (authorRole != null &&
                                      authorRole!.isNotEmpty)
                                    Padding(
                                      padding: const EdgeInsets.only(left: 7),
                                      child: ArahRoleBadge(role: authorRole!),
                                    ),
                                ],
                              ),
                              if (hasMetaLine ||
                                  (visibilityLabel?.isNotEmpty ?? false)) ...[
                                const SizedBox(height: 1),
                                Wrap(
                                  spacing: 6,
                                  crossAxisAlignment: WrapCrossAlignment.center,
                                  children: [
                                    if (authorHandle != null &&
                                        authorHandle!.isNotEmpty)
                                      Text(
                                        '@$authorHandle',
                                        style: Theme.of(context)
                                            .textTheme
                                            .bodySmall
                                            ?.copyWith(
                                              fontSize: 12.5,
                                              color: colors.onSurfaceSubtle,
                                            ),
                                      ),
                                    if (authorHandle != null &&
                                        authorHandle!.isNotEmpty &&
                                        timeLabel != null &&
                                        timeLabel!.isNotEmpty)
                                      Text(
                                        '·',
                                        style: Theme.of(context)
                                            .textTheme
                                            .bodySmall
                                            ?.copyWith(
                                              fontSize: 12.5,
                                              color: colors.onSurfaceSubtle
                                                  .withValues(alpha: 0.6),
                                            ),
                                      ),
                                    if (timeLabel != null &&
                                        timeLabel!.isNotEmpty)
                                      Text(
                                        timeLabel!,
                                        style: Theme.of(context)
                                            .textTheme
                                            .bodySmall
                                            ?.copyWith(
                                              fontSize: 12.5,
                                              color: colors.onSurfaceSubtle,
                                            ),
                                      ),
                                    if (visibilityLabel != null &&
                                        visibilityLabel!.isNotEmpty) ...[
                                      Text(
                                        '·',
                                        style: Theme.of(context)
                                            .textTheme
                                            .bodySmall
                                            ?.copyWith(
                                              fontSize: 12.5,
                                              color: colors.onSurfaceSubtle
                                                  .withValues(alpha: 0.6),
                                            ),
                                      ),
                                      Row(
                                        mainAxisSize: MainAxisSize.min,
                                        children: [
                                          ArahMaterialSymbol(
                                            'lock',
                                            size: 12,
                                            color: colors.primary,
                                          ),
                                          const SizedBox(width: 3),
                                          Text(
                                            visibilityLabel!,
                                            style: Theme.of(context)
                                                .textTheme
                                                .bodySmall
                                                ?.copyWith(
                                                  fontSize: 12,
                                                  color: colors.primary,
                                                ),
                                          ),
                                        ],
                                      ),
                                    ],
                                  ],
                                ),
                              ],
                            ],
                          ),
                        ),
                        if (showAlertPill && !compact || onMorePressed != null)
                          Row(
                            mainAxisSize: MainAxisSize.min,
                            children: [
                              if (showAlertPill && !compact)
                                Container(
                                  padding: const EdgeInsets.symmetric(
                                      horizontal: 10, vertical: 5),
                                  decoration: BoxDecoration(
                                    color: AppDesignTokens.alertDim,
                                    borderRadius: BorderRadius.circular(
                                        AppConstants.radiusXl),
                                  ),
                                  child: Row(
                                    mainAxisSize: MainAxisSize.min,
                                    children: [
                                      const ArahMaterialSymbol(
                                        'warning',
                                        size: 14,
                                        fill: 1,
                                        color: AppDesignTokens.warning,
                                      ),
                                      const SizedBox(width: 5),
                                      Text(
                                        'Alerta',
                                        style: Theme.of(context)
                                            .textTheme
                                            .labelSmall
                                            ?.copyWith(
                                              fontSize: 11.5,
                                              fontWeight: FontWeight.w600,
                                              color: AppDesignTokens.warning,
                                            ),
                                      ),
                                    ],
                                  ),
                                ),
                              if (showAlertPill &&
                                  !compact &&
                                  onMorePressed != null)
                                const SizedBox(width: 6),
                              if (onMorePressed != null)
                                IconButton(
                                  icon: const Icon(Icons.more_horiz),
                                  onPressed: onMorePressed,
                                  style: IconButton.styleFrom(
                                    minimumSize: const Size(36, 36),
                                    padding: EdgeInsets.zero,
                                  ),
                                ),
                            ],
                          ),
                      ],
                    ),
                    if (showAlertPill && compact) ...[
                      const SizedBox(height: 8),
                      Container(
                        padding: const EdgeInsets.symmetric(
                            horizontal: 10, vertical: 5),
                        decoration: BoxDecoration(
                          color: AppDesignTokens.alertDim,
                          borderRadius:
                              BorderRadius.circular(AppConstants.radiusXl),
                        ),
                        child: Row(
                          mainAxisSize: MainAxisSize.min,
                          children: [
                            const ArahMaterialSymbol(
                              'warning',
                              size: 14,
                              fill: 1,
                              color: AppDesignTokens.warning,
                            ),
                            const SizedBox(width: 5),
                            Text(
                              'Alerta',
                              style: Theme.of(context)
                                  .textTheme
                                  .labelSmall
                                  ?.copyWith(
                                    fontSize: 11.5,
                                    fontWeight: FontWeight.w600,
                                    color: AppDesignTokens.warning,
                                  ),
                            ),
                          ],
                        ),
                      ),
                    ],
                    const SizedBox(height: 13),
                    Text(
                      title,
                      style: Theme.of(context).textTheme.titleLarge?.copyWith(
                            fontSize: 16.5,
                            height: 1.3,
                            letterSpacing: -0.3,
                          ),
                    ),
                    if (content != null && content!.isNotEmpty) ...[
                      const SizedBox(height: 6),
                      Text(
                        content!,
                        style: Theme.of(context).textTheme.bodyLarge?.copyWith(
                              fontSize: 13.6,
                              color: colors.onSurfaceVariant,
                              height: 1.45,
                            ),
                      ),
                    ],
                    if (mediaUrls.isNotEmpty) ...[
                      const SizedBox(height: 13),
                      ClipRRect(
                        borderRadius: BorderRadius.circular(14),
                        child: Stack(
                          children: [
                            CachedNetworkImage(
                              imageUrl: mediaUrls.first,
                              height: 168,
                              width: double.infinity,
                              fit: BoxFit.cover,
                              placeholder: (_, __) => Container(
                                height: 168,
                                color: Theme.of(context)
                                    .colorScheme
                                    .surfaceContainerHighest,
                                child: const Center(
                                    child: CircularProgressIndicator(
                                        strokeWidth: 2)),
                              ),
                              errorWidget: (_, __, ___) => Container(
                                height: 120,
                                color: Theme.of(context)
                                    .colorScheme
                                    .surfaceContainerHighest,
                                child: const Center(
                                    child: Icon(Icons.broken_image_outlined)),
                              ),
                            ),
                            Positioned.fill(
                              child: DecoratedBox(
                                decoration: BoxDecoration(
                                  gradient: LinearGradient(
                                    begin: Alignment.bottomCenter,
                                    end: Alignment.topCenter,
                                    colors: [
                                      Colors.black.withValues(alpha: 0.35),
                                      Colors.transparent,
                                    ],
                                  ),
                                ),
                              ),
                            ),
                          ],
                        ),
                      ),
                    ],
                    const SizedBox(height: 14),
                    Row(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Expanded(
                          child: Wrap(
                            spacing: 0,
                            runSpacing: 8,
                            children: [
                              _InteractionButton(
                                symbol: 'favorite',
                                fill: isLiked ? 1 : 0,
                                count: likeCount,
                                color: isLiked
                                    ? typeColor
                                    : colors.onSurfaceSubtle,
                                onPressed: onLikePressed,
                              ),
                              _InteractionButton(
                                symbol: 'mode_comment',
                                count: commentCount,
                                color: colors.onSurfaceSubtle,
                                onPressed: onCommentPressed,
                              ),
                              _InteractionButton(
                                symbol: isShared ? 'send' : 'ios_share',
                                fill: isShared ? 1 : 0,
                                count: shareCount,
                                color: isShared
                                    ? colors.primary
                                    : colors.onSurfaceSubtle,
                                onPressed: onSharePressed,
                              ),
                            ],
                          ),
                        ),
                        const ArahMaterialSymbol(
                          'bookmark_border',
                          size: 19,
                          color: AppDesignTokens.onSurfaceSubtle,
                        ),
                      ],
                    ),
                  ],
                ),
              );
            },
          ),
        ),
      ),
    );
  }
}

class _FeedAvatar extends StatelessWidget {
  const _FeedAvatar({
    required this.name,
    this.avatarUrl,
    this.resident = false,
  });

  final String name;
  final String? avatarUrl;
  final bool resident;

  @override
  Widget build(BuildContext context) {
    final initials = name
        .split(RegExp(r'[ .]'))
        .where((segment) => segment.isNotEmpty)
        .take(2)
        .map((segment) => segment[0].toUpperCase())
        .join();

    return Stack(
      clipBehavior: Clip.none,
      children: [
        Container(
          width: 42,
          height: 42,
          decoration: BoxDecoration(
            shape: BoxShape.circle,
            color: AppDesignTokens.greenDeep,
            image: avatarUrl != null && avatarUrl!.isNotEmpty
                ? DecorationImage(
                    image: CachedNetworkImageProvider(avatarUrl!),
                    fit: BoxFit.cover,
                  )
                : null,
            boxShadow: AppDesignTokens.elevation(2, isDark: true),
          ),
          alignment: Alignment.center,
          child: avatarUrl == null || avatarUrl!.isEmpty
              ? Text(
                  initials.isNotEmpty ? initials : '?',
                  style: TextStyle(
                    color: Colors.white,
                    fontFamily: AppDesignTokens.fontFamilyDisplay,
                    fontSize: 15,
                    fontWeight: FontWeight.w600,
                    letterSpacing: 0.2,
                  ),
                )
              : null,
        ),
        if (resident)
          Positioned(
            right: -1,
            bottom: -1,
            child: Container(
              width: 16.8,
              height: 16.8,
              decoration: BoxDecoration(
                shape: BoxShape.circle,
                color: AppDesignTokens.greenSolid,
                border: Border.all(
                    color: AppDesignTokens.surfaceElevated, width: 2),
              ),
              child: const Center(
                child: ArahMaterialSymbol(
                  'verified',
                  size: 11,
                  fill: 1,
                  color: AppDesignTokens.textOnAccent,
                ),
              ),
            ),
          ),
      ],
    );
  }
}

class _InteractionButton extends StatelessWidget {
  const _InteractionButton({
    required this.symbol,
    required this.count,
    required this.color,
    this.fill = 0,
    this.onPressed,
  });

  final String symbol;
  final int count;
  final Color color;
  final int fill;
  final VoidCallback? onPressed;

  @override
  Widget build(BuildContext context) {
    return InkWell(
      onTap: onPressed,
      borderRadius: BorderRadius.circular(AppConstants.radiusSm),
      child: Padding(
        padding: const EdgeInsets.only(right: 14),
        child: Row(
          mainAxisSize: MainAxisSize.min,
          children: [
            ArahMaterialSymbol(
              symbol,
              size: 19,
              fill: fill,
              color: color,
            ),
            if (count > 0) ...[
              const SizedBox(width: 6),
              Text(
                count.toString(),
                style: Theme.of(context).textTheme.bodySmall?.copyWith(
                      fontSize: 13.5,
                      fontWeight: FontWeight.w500,
                      color: color,
                    ),
              ),
            ],
          ],
        ),
      ),
    );
  }
}
