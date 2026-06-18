import 'package:flutter/material.dart';
import 'package:flutter_svg/flutter_svg.dart';

import '../config/brand_config.dart';
import '../theme/app_design_tokens.dart';

/// Marca d'água sutil do logo (identidade wiki/portal).
class ArahWatermark extends StatelessWidget {
  const ArahWatermark({
    super.key,
    this.size = AppDesignTokens.bodyWatermarkSize,
    this.alignment = Alignment.bottomRight,
  });

  final double size;
  final Alignment alignment;

  @override
  Widget build(BuildContext context) {
    final colors = context.appColors;
    return IgnorePointer(
      child: Align(
        alignment: alignment,
        child: Opacity(
          opacity: colors.bodyWatermarkOpacity,
          child: SvgPicture.asset(
            'assets/images/arah-icon.svg',
            width: size,
            height: size,
            semanticsLabel: BrandConfig.name,
          ),
        ),
      ),
    );
  }
}
