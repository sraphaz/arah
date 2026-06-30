import 'package:flutter/material.dart';

import '../theme/app_design_tokens.dart';
import 'arah_watermark.dart';

/// Scaffold com fundo em gradiente sutil do design system.
class ArahScaffold extends StatelessWidget {
  const ArahScaffold({
    super.key,
    required this.body,
    this.appBar,
    this.bottomNavigationBar,
    this.floatingActionButton,
    this.extendBody = false,
    this.showWatermark = true,
  });

  final Widget body;
  final PreferredSizeWidget? appBar;
  final Widget? bottomNavigationBar;
  final Widget? floatingActionButton;
  final bool extendBody;
  final bool showWatermark;

  @override
  Widget build(BuildContext context) {
    final surface = context.appColors.surface;
    return DecoratedBox(
      decoration: BoxDecoration(
        color: surface,
        gradient: RadialGradient(
          center: const Alignment(-0.6, -0.4),
          radius: 1.2,
          colors: AppDesignTokens.scaffoldGradientColors,
          stops: const [0.0, 0.45, 0.75, 1.0],
        ),
      ),
      child: Stack(
        children: [
          if (showWatermark) const ArahWatermark(),
          Scaffold(
            backgroundColor: Colors.transparent,
            extendBody: extendBody,
            appBar: appBar,
            body: body,
            bottomNavigationBar: bottomNavigationBar,
            floatingActionButton: floatingActionButton,
          ),
        ],
      ),
    );
  }
}
