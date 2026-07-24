import 'package:flutter/material.dart';
import 'package:flutter_map/flutter_map.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:latlong2/latlong.dart';

import '../../../../core/config/constants.dart';
import '../../../../core/theme/app_design_tokens.dart';
import '../../../territories/data/repositories/territories_repository.dart';
import '../../../territories/presentation/providers/territories_list_provider.dart';
import '../../data/models/onboarding_models.dart';
import '../providers/onboarding_providers.dart';

/// Altura do mapa na tela de onboarding.
const double _onboardingMapHeight = 220.0;
const double _onboardingMapZoom = 12.0;

/// Cor do contorno do polígono e do pin do território (verde floresta).
const _territoryBoundaryColor = AppDesignTokens.territoryBoundary;

/// Mapa compacto: sempre visível; contorno do território selecionado (ou mais próximo).
class OnboardingMap extends ConsumerWidget {
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
  Widget build(BuildContext context, WidgetRef ref) {
    final suggested = showUserMarker
        ? ref.watch(suggestedTerritoriesProvider((lat: centerLat, lng: centerLng)))
        : const AsyncValue<List<TerritorySuggestion>>.data([]);
    final detailAsync = displayedTerritoryId != null && displayedTerritoryId!.isNotEmpty
        ? ref.watch(territoryDetailProvider(displayedTerritoryId!))
        : const AsyncValue<TerritoryDetail?>.data(null);

    return ClipRRect(
      borderRadius: BorderRadius.circular(AppConstants.radiusMd),
      child: SizedBox(
        height: _onboardingMapHeight,
        child: FlutterMap(
          options: MapOptions(
            initialCenter: LatLng(centerLat, centerLng),
            initialZoom: _onboardingMapZoom,
            interactionOptions: const InteractionOptions(flags: InteractiveFlag.all),
          ),
          children: [
            TileLayer(
              urlTemplate: 'https://tile.openstreetmap.org/{z}/{x}/{y}.png',
              userAgentPackageName: 'com.araponga.app',
            ),
            // Contorno do território mais próximo (polígono ou círculo)
            detailAsync.when(
              data: (detail) => detail != null ? _boundaryLayer(context, detail) : const SizedBox.shrink(),
              loading: () => const SizedBox.shrink(),
              error: (_, __) => const SizedBox.shrink(),
            ),
            if (showUserMarker)
              MarkerLayer(
                markers: [
                  Marker(
                    point: LatLng(centerLat, centerLng),
                    width: 40,
                    height: 40,
                    child: const Icon(
                      Icons.person_pin_circle,
                      color: AppDesignTokens.locationPin,
                      size: 40,
                    ),
                  ),
                ],
              ),
            suggested.when(
              data: (list) => list.isEmpty
                  ? const SizedBox.shrink()
                  : MarkerLayer(
                      markers: list.map((t) {
                        return Marker(
                          point: LatLng(t.latitude, t.longitude),
                          width: 32,
                          height: 32,
                          child: const Icon(
                            Icons.place,
                            color: _territoryBoundaryColor,
                            size: 32,
                          ),
                        );
                      }).toList(),
                    ),
              loading: () => const SizedBox.shrink(),
              error: (_, __) => const SizedBox.shrink(),
            ),
          ],
        ),
      ),
    );
  }

  Widget _boundaryLayer(BuildContext context, TerritoryDetail detail) {
    const color = _territoryBoundaryColor;
    if (detail.boundaryPolygon != null && detail.boundaryPolygon!.length >= 3) {
      final points = detail.boundaryPolygon!.map((p) => LatLng(p.lat, p.lng)).toList();
      return PolygonLayer(
        polygons: [
          Polygon(
            points: points,
            color: color.withValues(alpha: 0.2),
            borderStrokeWidth: 2.5,
            borderColor: color,
          ),
        ],
      );
    }
    if (detail.radiusKm != null && detail.radiusKm! > 0) {
      return CircleLayer(
        circles: [
          CircleMarker(
            point: LatLng(detail.latitude, detail.longitude),
            radius: detail.radiusKm! * 1000,
            useRadiusInMeter: true,
            color: color.withValues(alpha: 0.2),
            borderStrokeWidth: 2.5,
            borderColor: color,
          ),
        ],
      );
    }
    return const SizedBox.shrink();
  }
}
