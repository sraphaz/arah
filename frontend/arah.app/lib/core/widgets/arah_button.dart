import 'package:flutter/material.dart';

import '../config/constants.dart';
import '../theme/app_design_tokens.dart';
import '../theme/arah_motion.dart';

enum ArahButtonVariant { primary, secondary, text }

/// Botão do design system com touch target mínimo e estados de loading.
class ArahButton extends StatelessWidget {
  const ArahButton({
    super.key,
    required this.label,
    required this.onPressed,
    this.variant = ArahButtonVariant.primary,
    this.icon,
    this.loading = false,
    this.expand = false,
  });

  final String label;
  final VoidCallback? onPressed;
  final ArahButtonVariant variant;
  final IconData? icon;
  final bool loading;
  final bool expand;

  @override
  Widget build(BuildContext context) {
    final enabled = onPressed != null && !loading;

    Widget button;
    switch (variant) {
      case ArahButtonVariant.primary:
        button = FilledButton(
          onPressed: enabled ? () => _handlePress() : null,
          child: _buildChild(context),
        );
      case ArahButtonVariant.secondary:
        button = OutlinedButton(
          onPressed: enabled ? () => _handlePress() : null,
          child: _buildChild(context),
        );
      case ArahButtonVariant.text:
        button = TextButton(
          onPressed: enabled ? () => _handlePress() : null,
          child: _buildChild(context),
        );
    }

    return expand
        ? SizedBox(width: double.infinity, child: button)
        : button;
  }

  void _handlePress() {
    ArahMotion.lightTap();
    onPressed?.call();
  }

  Widget _buildChild(BuildContext context) {
    if (loading) {
      return SizedBox(
        width: AppConstants.loadingIndicatorSize,
        height: AppConstants.loadingIndicatorSize,
        child: CircularProgressIndicator(
          strokeWidth: 2,
          color: variant == ArahButtonVariant.primary
              ? Theme.of(context).colorScheme.onPrimary
              : context.appColors.primary,
        ),
      );
    }

    if (icon == null) return Text(label);

    return Row(
      mainAxisSize: MainAxisSize.min,
      children: [
        Icon(icon, size: AppConstants.iconSizeSm + 4),
        const SizedBox(width: AppConstants.spacingSm),
        Text(label),
      ],
    );
  }
}
