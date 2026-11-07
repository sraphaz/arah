import 'package:flutter/material.dart';

import '../../../../core/config/constants.dart';
import '../../../../core/theme/app_design_tokens.dart';

/// Altura do mapa na tela de onboarding.
const double _onboardingMapHeight = 180.0;

/// Cor do contorno do polígono e do pin do território (verde floresta).
const _territoryBoundaryColor = AppDesignTokens.territoryBoundary;

/// Preview compacto do território no onboarding.
///
/// Nesta etapa o mapa atua como um resumo visual da jornada, então usamos uma
/// composição Flutter determinística alinhada ao handoff mobile em vez de um
/// tile map interativo.
class OnboardingMap extends StatelessWidget {
  const OnboardingMap({
    super.key,
    required this.centerLat,
    required this.centerLng,
    required this.showUserMarker,
    this.displayedTerritoryId,
  });

  final double centerLat;
  final double centerLng;
  final bool showUserMarker;

  /// ID do território cujo contorno mostrar no mapa (seleção do usuário ou mais próximo).
  final String? displayedTerritoryId;

  @override
  Widget build(BuildContext context) {
    return ClipRRect(
      borderRadius: BorderRadius.circular(AppConstants.radiusLg + 2),
      child: SizedBox(
        height: _onboardingMapHeight,
        child: DecoratedBox(
          decoration: const BoxDecoration(color: Color(0xFF141814)),
          child: Stack(
            fit: StackFit.expand,
            children: [
              const _MapRoads(),
              Center(
                child: Container(
                  width: 152,
                  height: 152,
                  decoration: BoxDecoration(
                    shape: BoxShape.circle,
                    color: _territoryBoundaryColor.withValues(alpha: 0.1),
                    border: Border.all(
                      color: _territoryBoundaryColor.withValues(alpha: 0.9),
                      width: 2.2,
                    ),
                  ),
                ),
              ),
              if (showUserMarker)
                Center(
                  child: Container(
                    width: 18,
                    height: 18,
                    decoration: BoxDecoration(
                      color: AppDesignTokens.locationPin,
                      shape: BoxShape.circle,
                      border: Border.all(color: Colors.white, width: 3),
                      boxShadow: const [
                        BoxShadow(
                          color: Color(0x40E8A06A),
                          blurRadius: 12,
                          spreadRadius: 6,
                        ),
                      ],
                    ),
                  ),
                ),
              _pin(
                left: 92,
                top: 42,
                color: const Color(0xFFAED9C2),
                icon: Icons.storefront_rounded,
              ),
              _pin(
                left: 74,
                top: 94,
                color: AppDesignTokens.locationPin,
                icon: Icons.warning_amber_rounded,
              ),
              _pin(
                right: 88,
                top: 34,
                color: const Color(0xFF8FB5F0),
                icon: Icons.photo_camera_outlined,
              ),
              IgnorePointer(
                child: DecoratedBox(
                  decoration: const BoxDecoration(
                    gradient: RadialGradient(
                      center: Alignment(0, 0),
                      radius: 1.15,
                      colors: [
                        Colors.transparent,
                        Color(0x550B120D),
                      ],
                      stops: [0.25, 1],
                    ),
                  ),
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}

Widget _pin({
  double? left,
  double? top,
  double? right,
  required Color color,
  required IconData icon,
}) {
  return Positioned(
    left: left,
    top: top,
    right: right,
    child: Transform.rotate(
      angle: 0.785398,
      child: Container(
        width: 34,
        height: 34,
        decoration: BoxDecoration(
          color: const Color(0xDD141814),
          borderRadius: const BorderRadius.only(
            topLeft: Radius.circular(17),
            topRight: Radius.circular(17),
            bottomLeft: Radius.circular(17),
            bottomRight: Radius.circular(3),
          ),
          border: Border.all(color: color, width: 1.5),
        ),
        child: Transform.rotate(
          angle: -0.785398,
          child: Icon(icon, size: 16, color: color),
        ),
      ),
    ),
  );
}

class _MapRoads extends StatelessWidget {
  const _MapRoads();

  @override
  Widget build(BuildContext context) {
    return Stack(
      fit: StackFit.expand,
      children: [
        _roadBand(left: -24, top: 96, width: 210, angle: -0.38, opacity: 0.12),
        _roadBand(left: 108, top: 26, width: 150, angle: 0.52, opacity: 0.09),
        _roadBand(left: 204, top: 116, width: 184, angle: -0.44, opacity: 0.1),
        _roadBand(left: 12, top: 16, width: 120, angle: 0.18, opacity: 0.06),
        Positioned(
          left: -10,
          right: -10,
          bottom: 16,
          child: Transform.rotate(
            angle: -0.08,
            child: Container(
              height: 20,
              decoration: BoxDecoration(
                color: Colors.white.withValues(alpha: 0.12),
                borderRadius: BorderRadius.circular(20),
              ),
            ),
          ),
        ),
        for (final dx in [48.0, 126.0, 204.0, 282.0, 348.0])
          Positioned(
            left: dx,
            top: -12,
            bottom: -12,
            child: Transform.rotate(
              angle: 0.06,
              child: Container(
                width: 4,
                color: Colors.white.withValues(alpha: 0.08),
              ),
            ),
          ),
        Positioned(
          left: 88,
          top: 98,
          child: Container(
            width: 26,
            height: 26,
            decoration: BoxDecoration(
              color: Colors.white.withValues(alpha: 0.08),
              shape: BoxShape.circle,
            ),
          ),
        ),
      ],
    );
  }

  Widget _roadBand({
    required double left,
    required double top,
    required double width,
    required double angle,
    required double opacity,
  }) {
    return Positioned(
      left: left,
      top: top,
      child: Transform.rotate(
        angle: angle,
        child: Container(
          width: width,
          height: 18,
          decoration: BoxDecoration(
            color: Colors.white.withValues(alpha: opacity),
            borderRadius: BorderRadius.circular(18),
          ),
        ),
      ),
    );
  }
}
