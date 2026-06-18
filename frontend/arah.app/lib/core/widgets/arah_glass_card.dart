import 'dart:ui';

import 'package:flutter/material.dart';

import '../config/constants.dart';
import '../theme/app_design_tokens.dart';

/// Card com glass morphism (espelho de .glass-card no wiki).
class ArahGlassCard extends StatelessWidget {
  const ArahGlassCard({
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
  Widget build(BuildContext context) {
    final colors = context.appColors;
    final content = ClipRRect(
      borderRadius: BorderRadius.circular(AppDesignTokens.glassRadius),
      child: BackdropFilter(
        filter: ImageFilter.blur(
          sigmaX: AppDesignTokens.glassBlur,
          sigmaY: AppDesignTokens.glassBlur,
        ),
        child: DecoratedBox(
          decoration: BoxDecoration(
            color: colors.glassBackground,
            borderRadius: BorderRadius.circular(AppDesignTokens.glassRadius),
            border: Border.all(color: colors.glassBorder),
            boxShadow: AppDesignTokens.elevation(
              2,
              isDark: Theme.of(context).brightness == Brightness.dark,
            ),
          ),
          child: Padding(
            padding: padding ?? const EdgeInsets.all(AppConstants.spacingLg),
            child: child,
          ),
        ),
      ),
    );

    if (onTap == null) {
      return Padding(padding: margin ?? EdgeInsets.zero, child: content);
    }

    return Padding(
      padding: margin ?? EdgeInsets.zero,
      child: Material(
        color: Colors.transparent,
        child: InkWell(
          onTap: onTap,
          borderRadius: BorderRadius.circular(AppDesignTokens.glassRadius),
          child: content,
        ),
      ),
    );
  }
}
