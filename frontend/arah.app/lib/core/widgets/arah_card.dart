import 'package:flutter/material.dart';

import '../config/constants.dart';
import '../theme/app_design_tokens.dart';
import '../theme/arah_motion.dart';

/// Card padrão do design system com elevação e padding consistentes.
class ArahCard extends StatefulWidget {
  const ArahCard({
    super.key,
    required this.child,
    this.padding,
    this.margin,
    this.onTap,
  });

  final Widget child;
  final EdgeInsetsGeometry? padding;
  final EdgeInsetsGeometry? margin;
  final VoidCallback? onTap;

  @override
  State<ArahCard> createState() => _ArahCardState();
}

class _ArahCardState extends State<ArahCard> {
  bool _pressed = false;

  @override
  Widget build(BuildContext context) {
    final colors = context.appColors;
    final isDark = Theme.of(context).brightness == Brightness.dark;
    final scale = _pressed && widget.onTap != null ? 0.98 : 1.0;

    final card = AnimatedScale(
      scale: scale,
      duration: ArahMotion.fast,
      child: DecoratedBox(
        decoration: BoxDecoration(
          color: colors.surfaceContainer.withValues(alpha: 0.9),
          borderRadius: BorderRadius.circular(AppDesignTokens.radiusCard),
          border: Border.all(color: colors.outlineSubtle),
          boxShadow: AppDesignTokens.elevation(1, isDark: isDark),
        ),
        child: Padding(
          padding: widget.padding ?? AppDesignTokens.cardPadding,
          child: widget.child,
        ),
      ),
    );

    return Padding(
      padding: widget.margin ?? EdgeInsets.zero,
      child: widget.onTap == null
          ? card
          : Material(
              color: Colors.transparent,
              child: InkWell(
                onTap: () {
                  ArahMotion.lightTap();
                  widget.onTap?.call();
                },
                onHighlightChanged: (value) => setState(() => _pressed = value),
                borderRadius: BorderRadius.circular(AppDesignTokens.radiusCard),
                child: card,
              ),
            ),
    );
  }
}
