import 'package:flutter/material.dart';

import '../config/constants.dart';
import '../theme/app_design_tokens.dart';
import '../theme/arah_motion.dart';

enum ArahButtonVariant { primary, secondary, text }

/// Botão do design system com touch target mínimo e estados de loading.
class ArahButton extends StatefulWidget {
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
  State<ArahButton> createState() => _ArahButtonState();
}

class _ArahButtonState extends State<ArahButton> {
  bool _pressed = false;

  @override
  Widget build(BuildContext context) {
    final enabled = widget.onPressed != null && !widget.loading;
    final scale = _pressed && enabled ? 0.96 : 1.0;

    Widget button;
    switch (widget.variant) {
      case ArahButtonVariant.primary:
        button = FilledButton(
          onPressed: enabled ? _handlePress : null,
          child: _buildChild(context),
        );
      case ArahButtonVariant.secondary:
        button = OutlinedButton(
          onPressed: enabled ? _handlePress : null,
          child: _buildChild(context),
        );
      case ArahButtonVariant.text:
        button = TextButton(
          onPressed: enabled ? _handlePress : null,
          child: _buildChild(context),
        );
    }

    return AnimatedScale(
      scale: scale,
      duration: ArahMotion.fast,
      child: widget.expand
          ? SizedBox(width: double.infinity, child: button)
          : button,
    );
  }

  void _handlePress() {
    ArahMotion.lightTap();
    widget.onPressed?.call();
  }

  Widget _buildChild(BuildContext context) {
    if (widget.loading) {
      return SizedBox(
        width: AppConstants.loadingIndicatorSize,
        height: AppConstants.loadingIndicatorSize,
        child: CircularProgressIndicator(
          strokeWidth: 2,
          color: widget.variant == ArahButtonVariant.primary
              ? Theme.of(context).colorScheme.onPrimary
              : context.appColors.primary,
        ),
      );
    }

    if (widget.icon == null) return Text(widget.label);

    return Row(
      mainAxisSize: MainAxisSize.min,
      children: [
        Icon(widget.icon, size: AppConstants.iconSizeSm + 4),
        const SizedBox(width: AppConstants.spacingSm),
        Text(widget.label),
      ],
    );
  }
}
