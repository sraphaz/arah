import 'package:flutter/material.dart';

import '../config/brand_config.dart';
import '../config/constants.dart';
import '../theme/app_design_tokens.dart';

/// Cabeçalho de marca com presença visual alinhada ao portal/wiki.
class ArahBrandHeader extends StatelessWidget {
  const ArahBrandHeader({
    super.key,
    this.subtitle,
    this.size = ArahBrandHeaderSize.large,
    this.center = true,
  });

  final String? subtitle;
  final ArahBrandHeaderSize size;
  final bool center;

  @override
  Widget build(BuildContext context) {
    final colors = context.appColors;
    final titleStyle = switch (size) {
      ArahBrandHeaderSize.large => Theme.of(context).textTheme.displaySmall?.copyWith(
            color: colors.primary,
            fontWeight: FontWeight.w700,
            letterSpacing: AppDesignTokens.letterSpacingTight,
          ),
      ArahBrandHeaderSize.medium => Theme.of(context).textTheme.headlineMedium?.copyWith(
            color: colors.primary,
            fontWeight: FontWeight.w700,
            letterSpacing: AppDesignTokens.letterSpacingTight,
          ),
      ArahBrandHeaderSize.compact => Theme.of(context).textTheme.titleLarge?.copyWith(
            color: colors.primary,
            fontWeight: FontWeight.w600,
          ),
    };

    return Column(
      crossAxisAlignment: center ? CrossAxisAlignment.center : CrossAxisAlignment.start,
      children: [
        Row(
          mainAxisSize: center ? MainAxisSize.min : MainAxisSize.max,
          mainAxisAlignment: center ? MainAxisAlignment.center : MainAxisAlignment.start,
          children: [
            Container(
              width: size == ArahBrandHeaderSize.large ? 48 : 40,
              height: size == ArahBrandHeaderSize.large ? 48 : 40,
              decoration: BoxDecoration(
                color: colors.accentSubtle,
                borderRadius: BorderRadius.circular(AppConstants.radiusMd),
                border: Border.all(color: colors.accentBorder),
              ),
              child: Icon(
                Icons.terrain_outlined,
                color: colors.primary,
                size: size == ArahBrandHeaderSize.large ? 28 : 22,
              ),
            ),
            const SizedBox(width: AppConstants.spacingSm + 4),
            Text(BrandConfig.name, style: titleStyle),
          ],
        ),
        if (subtitle != null) ...[
          SizedBox(height: size == ArahBrandHeaderSize.large ? AppConstants.spacingMd : AppConstants.spacingSm),
          Text(
            subtitle!,
            textAlign: center ? TextAlign.center : TextAlign.start,
            style: Theme.of(context).textTheme.bodyLarge?.copyWith(
                  color: colors.onSurfaceVariant,
                  height: 1.5,
                ),
          ),
        ],
      ],
    );
  }
}

enum ArahBrandHeaderSize { large, medium, compact }
