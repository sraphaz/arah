import 'package:flutter/material.dart';
import 'package:flutter_svg/flutter_svg.dart';

import '../config/brand_config.dart';
import '../config/constants.dart';
import '../theme/app_design_tokens.dart';

/// Cabeçalho de marca com logo e wordmark alinhados ao portal/wiki.
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

  double get _logoSize => switch (size) {
        ArahBrandHeaderSize.large => 48,
        ArahBrandHeaderSize.medium => 40,
        _ => 32,
      };

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
            ClipRRect(
              borderRadius: BorderRadius.circular(AppConstants.radiusMd),
              child: SvgPicture.asset(
                'assets/images/arah-icon.svg',
                width: _logoSize,
                height: _logoSize,
                semanticsLabel: BrandConfig.name,
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
