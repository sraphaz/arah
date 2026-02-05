import 'package:flutter/material.dart';
import 'package:flutter_map/flutter_map.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:latlong2/latlong.dart';

import '../../../../core/config/constants.dart';
import '../../../../core/providers/territory_provider.dart';
import '../../../../l10n/app_localizations.dart';
import '../../../../core/geo/geo_location_provider.dart';
import '../providers/map_pins_provider.dart';
import '../../data/models/map_pin.dart';

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
    final territoryId = widget.territoryId ??
        ref.watch(selectedTerritoryIdValueProvider);
    final geo = ref.watch(geoLocationStateProvider);
    final pinsAsync = ref.watch(mapPinsProvider(territoryId));

    final hasTerritory = territoryId != null && territoryId.isNotEmpty;
    final initialCenter = _initialCenter(geo, pinsAsync.valueOrNull);
    final initialZoom = _initialZoom(geo, pinsAsync.valueOrNull);

    return Scaffold(
      appBar: AppBar(
        title: Text(AppLocalizations.of(context)!.map),
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
                      color: Theme.of(context).colorScheme.primary.withOpacity(0.5),
                    ),
                    const SizedBox(height: AppConstants.spacingMd),
                    Text(
                      AppLocalizations.of(context)!.chooseTerritory,
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
              ),
              children: [
                TileLayer(
                  urlTemplate: 'https://tile.openstreetmap.org/{z}/{x}/{y}.png',
                  userAgentPackageName: 'com.araponga.app',
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
                          color: Theme.of(context).colorScheme.primary,
                          size: 40,
                        ),
                      ),
                    ],
                  ),
                pinsAsync.when(
                  data: (pins) => _buildPinsLayer(context, pins),
                  loading: () => const SizedBox.shrink(),
                  error: (_, __) => const SizedBox.shrink(),
                ),
                const AttributionLayer(),
              ],
            ),
    );
  }

  LatLng _initialCenter(GeoPosition? geo, List<MapPin>? pins) {
    if (geo != null) return LatLng(geo.latitude, geo.longitude);
    if (pins != null && pins.isNotEmpty) {
      final lat = pins.map((p) => p.latitude).reduce((a, b) => a + b) / pins.length;
      final lng = pins.map((p) => p.longitude).reduce((a, b) => a + b) / pins.length;
      return LatLng(lat, lng);
    }
    return _defaultCenter;
  }

  double _initialZoom(GeoPosition? geo, List<MapPin>? pins) {
    if (pins != null && pins.isNotEmpty) return _territoryZoom;
    if (geo != null) return _territoryZoom;
    return _defaultZoom;
  }

  Widget _buildPinsLayer(BuildContext context, List<MapPin> pins) {
    return MarkerLayer(
      markers: pins.map((pin) {
        return Marker(
          point: LatLng(pin.latitude, pin.longitude),
          width: 36,
          height: 36,
          child: GestureDetector(
            onTap: () => _onPinTap(context, pin),
            child: Container(
              decoration: BoxDecoration(
                color: Theme.of(context).colorScheme.primaryContainer,
                shape: BoxShape.circle,
                border: Border.all(
                  color: Theme.of(context).colorScheme.primary,
                  width: 2,
                ),
              ),
              child: Icon(
                _iconForPinType(pin.pinType),
                size: 20,
                color: Theme.of(context).colorScheme.onPrimaryContainer,
              ),
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

  void _onPinTap(BuildContext context, MapPin pin) {
    showModalBottomSheet(
      context: context,
      builder: (ctx) => Padding(
        padding: const EdgeInsets.all(AppConstants.spacingMd),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            Text(
              pin.title.isNotEmpty ? pin.title : 'Pin',
              style: Theme.of(ctx).textTheme.titleMedium,
            ),
            const SizedBox(height: AppConstants.spacingSm),
            Text(
              _subtitleForPinType(ctx, pin),
              style: Theme.of(ctx).textTheme.bodySmall?.copyWith(
                    color: Theme.of(ctx).colorScheme.onSurfaceVariant,
                  ),
            ),
          ],
        ),
      ),
    );
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
