import 'package:flutter/material.dart';

import '../theme/app_design_tokens.dart';

/// Indicador de carregamento com cor primária do tema.
class ArahLoadingIndicator extends StatelessWidget {
  const ArahLoadingIndicator({
    super.key,
    this.size = 32,
    this.strokeWidth = 3,
  });

  final double size;
  final double strokeWidth;

  @override
  Widget build(BuildContext context) {
    final colors = context.appColors;
    return SizedBox(
      width: size,
      height: size,
      child: CircularProgressIndicator(
        strokeWidth: strokeWidth,
        color: colors.primary,
      ),
    );
  }
}
