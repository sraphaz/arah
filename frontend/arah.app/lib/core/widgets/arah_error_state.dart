import 'package:flutter/material.dart';

import '../config/constants.dart';
import '../theme/app_design_tokens.dart';
import 'arah_button.dart';

/// Estado de erro padronizado (APP-DS-12): mensagem + CTA "Tentar de novo".
class ArahErrorState extends StatelessWidget {
  const ArahErrorState({
    super.key,
    required this.message,
    this.onRetry,
    this.retryLabel,
  });

  final String message;
  final VoidCallback? onRetry;
  final String? retryLabel;

  @override
  Widget build(BuildContext context) {
    final colors = context.appColors;
    return Padding(
      padding: const EdgeInsets.all(AppConstants.spacingXl),
      child: Column(
        mainAxisAlignment: MainAxisAlignment.center,
        children: [
          Icon(
            Icons.error_outline,
            size: AppConstants.avatarSizeLg,
            color: colors.error.withValues(alpha: 0.85),
          ),
          const SizedBox(height: AppConstants.spacingMd),
          Text(
            message,
            textAlign: TextAlign.center,
            style: Theme.of(context).textTheme.bodyLarge,
          ),
          if (onRetry != null) ...[
            const SizedBox(height: AppConstants.spacingLg),
            ArahButton(
              label: retryLabel ?? 'Tentar de novo',
              onPressed: onRetry,
              variant: ArahButtonVariant.secondary,
            ),
          ],
        ],
      ),
    );
  }
}
