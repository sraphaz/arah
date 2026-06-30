import 'dart:math' as math;

import 'package:flutter/material.dart';
import 'package:flutter_map/flutter_map.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:latlong2/latlong.dart';

import '../../../../core/config/constants.dart';
import '../../../../core/providers/main_shell_tab_provider.dart';
import '../../../../core/providers/territory_provider.dart';
import '../../../../core/theme/app_design_tokens.dart';
import '../../../../core/widgets/arah_glass_card.dart';
import '../../../../core/widgets/arah_button.dart';
import '../../../../core/widgets/arah_scaffold.dart';
import '../../../../l10n/app_localizations.dart';
import '../../../../core/geo/geo_location_provider.dart';
import '../../../territories/data/repositories/territories_repository.dart';
import '../../../territories/presentation/providers/territories_list_provider.dart';
import '../providers/map_pins_provider.dart';
import '../../data/models/map_pin.dart';
import 'map_deep_link.dart';

/// Centro padrão do mapa (Brasil) quando não há geo nem pins.
const LatLng _defaultCenter = LatLng(-14.2, -51.9);
const double _defaultZoom = 4.0;
const double _territoryZoom = 13.0;

/// Tela de mapa com pins do território (entidades, posts, eventos, etc.).
/// Usa flutter_map + OpenStreetMap; opcionalmente configurável para tiles Mapbox.
class MapScreen extends ConsumerStatefulWidget {
  const MapScreen({super.key, this.territoryId});

  /// Território a exibir; se null, usa o território selecionado.
  final String? territoryId;

  @override
  ConsumerState<MapScreen> createState() => _MapScreenState();
}

class _MapScreenState extends ConsumerState<MapScreen> {
  final MapController _mapController = MapController();

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    // Sempre priorizar o território atualmente selecionado para que a alternância no Explorar
    // reflita no mapa e apenas um contorno seja exibido por vez.
    final territoryId = ref.watch(selectedTerritoryIdValueProvider) ?? widget.territoryId;
    final geo = ref.watch(geoLocationStateProvider);
    final pinsAsync = ref.watch(mapPinsProvider(territoryId));
    final territoryDetailAsync = ref.watch(territoryDetailProvider(territoryId ?? ''));

    final hasTerritory = territoryId != null && territoryId.isNotEmpty;
    final territoryDetail = territoryDetailAsync.valueOrNull;
    final initialCenter = _initialCenter(geo, pinsAsync.valueOrNull, territoryDetail);
    final initialZoom = _initialZoom(geo, pinsAsync.valueOrNull);

    return ArahScaffold(
      appBar: AppBar(
        title: Text(l10n.map),
        actions: [
          if (geo != null)
            IconButton(
              icon: const Icon(Icons.my_location),
              onPressed: () {
                _mapController.move(
                  LatLng(geo.latitude, geo.longitude),
                  _territoryZoom,
                );
              },
            ),
        ],
      ),
      body: !hasTerritory
          ? Center(
              child: Padding(
                padding: const EdgeInsets.all(AppConstants.spacingLg),
                child: Column(
                  mainAxisAlignment: MainAxisAlignment.center,
                  children: [
                    Icon(
                      Icons.map_outlined,
                      size: AppConstants.iconSizeLg,
                      color: Theme.of(context).colorScheme.primary.withValues(alpha: 0.5),
                    ),
                    const SizedBox(height: AppConstants.spacingMd),
                    Text(
                      l10n.chooseTerritory,
                      textAlign: TextAlign.center,
                      style: Theme.of(context).textTheme.titleMedium,
                    ),
                  ],
                ),
              ),
            )
          : FlutterMap(
              mapController: _mapController,
              options: MapOptions(
                initialCenter: initialCenter,
                initialZoom: initialZoom,
                interactionOptions: const InteractionOptions(
                  flags: InteractiveFlag.all,
                ),
                onTap: (_, point) => _handleMapTap(
                  context,
                  point,
                  pinsAsync.valueOrNull,
                  territoryId,
                ),
              ),
              children: [
                TileLayer(
                  urlTemplate: 'https://tile.openstreetmap.org/{z}/{x}/{y}.png',
                  userAgentPackageName: AppConstants.mapUserAgentPackage,
                ),
                if (geo != null)
                  MarkerLayer(
                    markers: [
                      Marker(
                        point: LatLng(geo.latitude, geo.longitude),
                        width: 40,
                        height: 40,
                        child: Icon(
                          Icons.person_pin_circle,
                          color: context.appColors.locationPin,
                          size: 40,
                        ),
                      ),
                    ],
                  ),
                territoryDetailAsync.when(
                  data: (detail) {
                    if (detail == null) return const SizedBox.shrink();
                    // Key garante que apenas o contorno do território atual seja exibido ao alternar.
                    return KeyedSubtree(
                      key: ValueKey('boundary-$territoryId'),
                      child: _buildTerritoryBoundaryLayer(context, detail),
                    );
                  },
                  loading: () => const SizedBox.shrink(),
                  error: (_, __) => const SizedBox.shrink(),
                ),
                pinsAsync.when(
                  data: (pins) => _buildPinsLayer(context, pins, territoryId),
                  loading: () => const SizedBox.shrink(),
                  error: (_, __) => const SizedBox.shrink(),
                ),
                SimpleAttributionWidget(
                  source: Text(l10n.openStreetMapAttribution),
                ),
              ],
            ),
    );
  }

  LatLng _initialCenter(GeoPosition? geo, List<MapPin>? pins, TerritoryDetail? territoryDetail) {
    if (geo != null) return LatLng(geo.latitude, geo.longitude);
    if (pins != null && pins.isNotEmpty) {
      final lat = pins.map((p) => p.latitude).reduce((a, b) => a + b) / pins.length;
      final lng = pins.map((p) => p.longitude).reduce((a, b) => a + b) / pins.length;
      return LatLng(lat, lng);
    }
    if (territoryDetail != null) return LatLng(territoryDetail.latitude, territoryDetail.longitude);
    return _defaultCenter;
  }

  Widget _buildTerritoryBoundaryLayer(BuildContext context, TerritoryDetail detail) {
    final color = context.appColors.territoryBoundary;
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

  double _initialZoom(GeoPosition? geo, List<MapPin>? pins) {
    if (pins != null && pins.isNotEmpty) return _territoryZoom;
    if (geo != null) return _territoryZoom;
    return _defaultZoom;
  }

  Widget _buildPinsLayer(BuildContext context, List<MapPin> pins, String? territoryId) {
    return MarkerLayer(
      markers: pins.map((pin) {
        return Marker(
          point: LatLng(pin.latitude, pin.longitude),
          width: AppConstants.minTouchTargetSize,
          height: AppConstants.minTouchTargetSize,
          // O toque é tratado no nível do mapa (MapOptions.onTap) por ser mais
          // confiável na web do que GestureDetector dentro do marcador.
          child: Container(
            decoration: BoxDecoration(
              color: AppDesignTokens.pinColorForType(pin.pinType).withValues(alpha: 0.25),
              shape: BoxShape.circle,
              border: Border.all(
                color: AppDesignTokens.pinColorForType(pin.pinType),
                width: 2,
              ),
            ),
            child: Icon(
              _iconForPinType(pin.pinType),
              size: 20,
              color: AppDesignTokens.pinColorForType(pin.pinType),
            ),
          ),
        );
      }).toList(),
    );
  }

  IconData _iconForPinType(String type) {
    switch (type.toLowerCase()) {
      case 'entity':
        return Icons.place;
      case 'post':
        return Icons.article;
      case 'event':
        return Icons.event;
      case 'asset':
        return Icons.photo_library;
      case 'alert':
        return Icons.warning_amber;
      case 'media':
        return Icons.perm_media;
      default:
        return Icons.place;
    }
  }

  /// Distância máxima (px) entre o toque e o pin para considerar um acerto.
  /// Generosa o suficiente para toques de dedo próximos ao marcador.
  static const double _pinTapTolerancePx = 48;

  /// Trata o toque no mapa: localiza o pin mais próximo (em pixels) do ponto
  /// tocado e, se dentro da tolerância, abre a folha de detalhes.
  void _handleMapTap(
    BuildContext context,
    LatLng tapPoint,
    List<MapPin>? pins,
    String? territoryId,
  ) {
    if (pins == null || pins.isEmpty) return;
    final camera = _mapController.camera;
    final tapPx = camera.latLngToScreenOffset(tapPoint);

    MapPin? nearest;
    double nearestDistance = double.infinity;
    for (final pin in pins) {
      final pinPx = camera.latLngToScreenOffset(LatLng(pin.latitude, pin.longitude));
      final dx = pinPx.dx - tapPx.dx;
      final dy = pinPx.dy - tapPx.dy;
      final distance = math.sqrt(dx * dx + dy * dy);
      if (distance < nearestDistance) {
        nearestDistance = distance;
        nearest = pin;
      }
    }

    if (nearest != null && nearestDistance <= _pinTapTolerancePx) {
      _onPinTap(context, nearest, territoryId);
    }
  }

  void _onPinTap(BuildContext context, MapPin pin, String? territoryId) {
    final l10n = AppLocalizations.of(context)!;
    final target = mapPinDeepLink(pin, territoryId: territoryId);
    final hasTarget = target != null;
    showModalBottomSheet(
      context: context,
      showDragHandle: true,
      builder: (ctx) => Padding(
        padding: const EdgeInsets.all(AppConstants.spacingMd),
        child: ArahGlassCard(
          child: Column(
            mainAxisSize: MainAxisSize.min,
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              Text(
                pin.title.isNotEmpty ? pin.title : l10n.mapPin,
                style: Theme.of(ctx).textTheme.titleMedium,
              ),
              const SizedBox(height: AppConstants.spacingSm),
              Text(
                _subtitleForPinType(ctx, pin),
                style: Theme.of(ctx).textTheme.bodySmall?.copyWith(
                      color: Theme.of(ctx).colorScheme.onSurfaceVariant,
                    ),
              ),
              const SizedBox(height: AppConstants.spacingLg),
              ArahButton(
                label: hasTarget ? l10n.viewDetails : l10n.close,
                onPressed: () {
                  Navigator.pop(ctx);
                  if (target != null) {
                    _navigateToPinTarget(context, target);
                  }
                },
                expand: true,
              ),
            ],
          ),
        ),
      ),
    );
  }

  /// Navega para a rota de detalhe correspondente ao pin (deep-link).
  void _navigateToPinTarget(BuildContext context, String route) {
    if (route == '/home') {
      // Não há rota de detalhe de post; seleciona a aba do feed e volta ao shell.
      ref.read(mainShellTabProvider.notifier).state = 0;
      context.go('/home');
    } else {
      context.push(route);
    }
  }

  String _subtitleForPinType(BuildContext context, MapPin pin) {
    final l10n = AppLocalizations.of(context)!;
    switch (pin.pinType.toLowerCase()) {
      case 'entity':
        return l10n.mapEntity;
      case 'post':
        return l10n.mapPost;
      case 'event':
        return l10n.mapEvent;
      case 'asset':
        return l10n.mapAsset;
      case 'alert':
        return l10n.mapAlert;
      default:
        return l10n.mapPin;
    }
  }
}
