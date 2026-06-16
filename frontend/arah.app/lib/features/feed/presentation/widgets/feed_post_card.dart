import 'package:flutter/material.dart';

import '../../../../core/config/constants.dart';
import '../../../../core/theme/app_design_tokens.dart';

/// Tipo de conteúdo do feed: define cor e ícone para diferenciação visual rápida.
/// Evento = convite/calendário; Alerta = atenção; Dica = útil; Geral = neutro.
enum FeedPostType {
  general,
  alert,
  event,
  tip,
}

/// Card reutilizável de um post no feed com hierarquia clara e indicação visual por tipo.
///
/// **UX:** Borda lateral colorida + ícone pequeno permitem escanear o feed por tipo
/// (evento, aviso, dica, alerta) sem poluir; título em titleSmall, corpo em bodyMedium.
class FeedPostCard extends StatelessWidget {
  const FeedPostCard({
    super.key,
    required this.title,
    this.content,
    this.authorInitial,
    this.type = FeedPostType.general,
    this.likeCount = 0,
    this.commentCount = 0,
    this.shareCount = 0,
    this.isLiked = false,
    this.isShared = false,
    this.onMorePressed,
    this.onLikePressed,
    this.onCommentPressed,
    this.onSharePressed,
  });

  final String title;
  final String? content;
  final String? authorInitial;
  final FeedPostType type;
  final int likeCount;
  final int commentCount;
  final int shareCount;
  final bool isLiked;
  final bool isShared;
  final VoidCallback? onMorePressed;
  final VoidCallback? onLikePressed;
  final VoidCallback? onCommentPressed;
  final VoidCallback? onSharePressed;

  static Color _colorForType(FeedPostType t) {
    switch (t) {
      case FeedPostType.alert:
        return AppDesignTokens.feedTypeAlert;
      case FeedPostType.event:
        return AppDesignTokens.feedTypeEvent;
      case FeedPostType.tip:
        return AppDesignTokens.feedTypeTip;
      case FeedPostType.general:
        return AppDesignTokens.feedTypeGeneral;
    }
  }

  static IconData _iconForType(FeedPostType t) {
    switch (t) {
      case FeedPostType.alert:
        return Icons.warning_amber_rounded;
      case FeedPostType.event:
        return Icons.event_rounded;
      case FeedPostType.tip:
        return Icons.lightbulb_outline_rounded;
      case FeedPostType.general:
        return Icons.article_outlined;
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
    final theme = Theme.of(context);
    final typeColor = _colorForType(type);
    final icon = _iconForType(type);

    return Card(
      margin: const EdgeInsets.symmetric(
        horizontal: AppConstants.spacingMd,
        vertical: AppConstants.spacingSm + 2,
      ),
      clipBehavior: Clip.antiAlias,
      child: IntrinsicHeight(
        child: Row(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            // Barra lateral por tipo (diferenciação visual sem ruído)
            Container(
              width: AppDesignTokens.feedCardTypeBorderWidth,
              color: typeColor,
            ),
            Expanded(
              child: Padding(
                padding: AppDesignTokens.cardPadding,
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Row(
                      children: [
                        CircleAvatar(
                          radius: AppConstants.avatarSizeSm / 2,
                          backgroundColor: theme.colorScheme.primaryContainer,
                          child: Text(
                            (authorInitial ?? (title.isNotEmpty ? title[0] : '?')).toUpperCase(),
                            style: TextStyle(
                              color: theme.colorScheme.onPrimaryContainer,
                              fontSize: AppDesignTokens.fontSizeBodySmall,
                              fontWeight: FontWeight.w600,
                            ),
                          ),
                        ),
                        const SizedBox(width: AppConstants.spacingSm),
                        Expanded(
                          child: Text(
                            title,
                            style: theme.textTheme.titleSmall,
                            maxLines: 1,
                            overflow: TextOverflow.ellipsis,
                          ),
                        ),
                        Icon(
                          icon,
                          size: 18,
                          color: typeColor.withValues(alpha: 0.9),
                        ),
                        const SizedBox(width: AppConstants.spacingXs),
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
                    if (content != null && content!.isNotEmpty) ...[
                      const SizedBox(height: AppConstants.spacingSm),
                      Text(
                        content!,
                        style: theme.textTheme.bodyMedium,
                        maxLines: 4,
                        overflow: TextOverflow.ellipsis,
                      ),
                    ],
                    const SizedBox(height: AppConstants.spacingSm),
                    Row(
                      children: [
                        _InteractionButton(
                          icon: isLiked ? Icons.favorite : Icons.favorite_border,
                          count: likeCount,
                          color: isLiked ? theme.colorScheme.primary : null,
                          onPressed: onLikePressed,
                        ),
                        _InteractionButton(
                          icon: Icons.chat_bubble_outline,
                          count: commentCount,
                          onPressed: onCommentPressed,
                        ),
                        _InteractionButton(
                          icon: isShared ? Icons.send : Icons.send_outlined,
                          count: shareCount,
                          color: isShared ? theme.colorScheme.primary : null,
                          onPressed: onSharePressed,
                        ),
                      ],
                    ),
                  ],
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }
}

class _InteractionButton extends StatelessWidget {
  const _InteractionButton({
    required this.icon,
    required this.count,
    this.color,
    this.onPressed,
  });

  final IconData icon;
  final int count;
  final Color? color;
  final VoidCallback? onPressed;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return InkWell(
      onTap: onPressed,
      borderRadius: BorderRadius.circular(AppConstants.radiusSm),
      child: Padding(
        padding: const EdgeInsets.symmetric(horizontal: AppConstants.spacingXs),
        child: Row(
          mainAxisSize: MainAxisSize.min,
          children: [
            Icon(icon, size: 20, color: color),
            if (count > 0) ...[
              const SizedBox(width: 4),
              Text(
                count.toString(),
                style: theme.textTheme.labelSmall?.copyWith(
                  color: color ?? theme.colorScheme.onSurfaceVariant,
                ),
              ),
            ],
          ],
        ),
      ),
    );
  }
}
