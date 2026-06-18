import 'package:flutter/material.dart';

import '../config/constants.dart';
import '../theme/app_design_tokens.dart';
import 'arah_button.dart';

/// Estado vazio padronizado com ícone, título, descrição e CTA opcional.
class ArahEmptyState extends StatelessWidget {
  const ArahEmptyState({
    super.key,
    required this.icon,
    required this.title,
    this.description,
    this.actionLabel,
    this.onAction,
  });

  final IconData icon;
  final String title;
  final String? description;
  final String? actionLabel;
  final VoidCallback? onAction;

  @override
  Widget build(BuildContext context) {
    final colors = context.appColors;
    return Padding(
      padding: const EdgeInsets.all(AppConstants.spacingXl),
      child: Column(
        mainAxisAlignment: MainAxisAlignment.center,
        children: [
          Icon(
            icon,
            size: AppConstants.avatarSizeLg,
            color: colors.primary.withValues(alpha: 0.5),
          ),
          const SizedBox(height: AppConstants.spacingMd),
          Text(
            title,
            textAlign: TextAlign.center,
            style: Theme.of(context).textTheme.titleMedium,
          ),
          if (description != null) ...[
            const SizedBox(height: AppConstants.spacingSm),
            Text(
              description!,
              textAlign: TextAlign.center,
              style: Theme.of(context).textTheme.bodySmall?.copyWith(
                    color: colors.onSurfaceVariant,
                  ),
            ),
          ],
          if (actionLabel != null && onAction != null) ...[
            const SizedBox(height: AppConstants.spacingLg),
            ArahButton(
              label: actionLabel!,
              onPressed: onAction,
              expand: true,
            ),
          ],
        ],
      ),
    );
  }
}
