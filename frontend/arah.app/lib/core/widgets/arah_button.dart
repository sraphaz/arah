import 'package:flutter/material.dart';

import '../config/constants.dart';
import '../theme/app_design_tokens.dart';
import '../theme/arah_motion.dart';

enum ArahButtonVariant { primary, secondary, text }

/// Botão do design system: variantes, touch mínimo 44px e scale no press.
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

  bool get _enabled => widget.onPressed != null && !widget.loading;

  @override
  Widget build(BuildContext context) {
    final colors = context.appColors;

    Widget button;
    switch (widget.variant) {
      case ArahButtonVariant.primary:
        button = FilledButton(
          onPressed: _enabled ? _handlePress : null,
          style: FilledButton.styleFrom(
            minimumSize: const Size(
              AppConstants.minTouchTargetSize,
              AppConstants.minTouchTargetSize,
            ),
            foregroundColor: AppDesignTokens.textOnAccent,
            backgroundColor: colors.primary,
          ),
          child: _buildChild(context),
        );
      case ArahButtonVariant.secondary:
        button = OutlinedButton(
          onPressed: _enabled ? _handlePress : null,
          style: OutlinedButton.styleFrom(
            minimumSize: const Size(
              AppConstants.minTouchTargetSize,
              AppConstants.minTouchTargetSize,
            ),
            foregroundColor: colors.primary,
            side: BorderSide(color: colors.accentBorder),
          ),
          child: _buildChild(context),
        );
      case ArahButtonVariant.text:
        button = TextButton(
          onPressed: _enabled ? _handlePress : null,
          style: TextButton.styleFrom(
            minimumSize: const Size(
              AppConstants.minTouchTargetSize,
              AppConstants.minTouchTargetSize,
            ),
            foregroundColor: colors.primary,
          ),
          child: _buildChild(context),
        );
    }

    final scaled = AnimatedScale(
      scale: _pressed && _enabled ? 0.97 : 1.0,
      duration: ArahMotion.fast,
      child: Listener(
        onPointerDown: _enabled ? (_) => setState(() => _pressed = true) : null,
        onPointerUp: (_) => setState(() => _pressed = false),
        onPointerCancel: (_) => setState(() => _pressed = false),
        child: button,
      ),
    );

    return widget.expand
        ? SizedBox(width: double.infinity, child: scaled)
        : ConstrainedBox(
            constraints: const BoxConstraints(
              minWidth: AppConstants.minTouchTargetSize,
              minHeight: AppConstants.minTouchTargetSize,
            ),
            child: scaled,
          );
  }

  void _handlePress() {
    ArahMotion.lightTap();
    widget.onPressed?.call();
  }

  Widget _buildChild(BuildContext context) {
    final colors = context.appColors;
    if (widget.loading) {
      return SizedBox(
        width: AppConstants.loadingIndicatorSize,
        height: AppConstants.loadingIndicatorSize,
        child: CircularProgressIndicator(
          strokeWidth: 2,
          color: widget.variant == ArahButtonVariant.primary
              ? AppDesignTokens.textOnAccent
              : colors.primary,
        ),
      );
    }

    if (widget.icon == null) return Text(widget.label);

    return Row(
      mainAxisSize: MainAxisSize.min,
      mainAxisAlignment: MainAxisAlignment.center,
      children: [
        Icon(widget.icon, size: AppConstants.iconSizeSm + 4),
        const SizedBox(width: AppConstants.spacingSm),
        Flexible(child: Text(widget.label, overflow: TextOverflow.ellipsis)),
      ],
    );
  }
}
