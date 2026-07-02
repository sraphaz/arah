import 'package:flutter/material.dart';
import 'package:flutter_map/flutter_map.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:latlong2/latlong.dart';

import '../../../../core/config/constants.dart';
import '../../../../core/network/api_exception.dart';
import '../../../../core/theme/app_design_tokens.dart';
import '../../../../core/widgets/app_snackbar.dart';
import '../../../../l10n/app_localizations.dart';
import '../../data/models/onboarding_models.dart';
import '../providers/onboarding_providers.dart';

const _mapZoom = 14.0;
const _boundaryColor = AppDesignTokens.territoryBoundary;

/// Sheet para propor território: pin ajustável, cidade/UF, raio ou polígono.
Future<TerritorySuggestion?> showProposeTerritorySheet({
  required BuildContext context,
  required double initialLatitude,
  required double initialLongitude,
}) {
  return showModalBottomSheet<TerritorySuggestion>(
    context: context,
    isScrollControlled: true,
    useSafeArea: true,
    builder: (context) => ProposeTerritorySheet(
      initialLatitude: initialLatitude,
      initialLongitude: initialLongitude,
    ),
  );
}

class ProposeTerritorySheet extends ConsumerStatefulWidget {
  const ProposeTerritorySheet({
    super.key,
    required this.initialLatitude,
    required this.initialLongitude,
  });

  final double initialLatitude;
  final double initialLongitude;

  @override
  ConsumerState<ProposeTerritorySheet> createState() => _ProposeTerritorySheetState();
}

class _ProposeTerritorySheetState extends ConsumerState<ProposeTerritorySheet> {
  final _cityController = TextEditingController();
  final _stateController = TextEditingController();
  final _nameController = TextEditingController();
  final _mapController = MapController();

  late LatLng _pin;
  double _radiusKm = 2.0;
  bool _polygonMode = false;
  bool _submitting = false;
  final List<LatLng> _polygonPoints = [];

  @override
  void initState() {
    super.initState();
    _pin = LatLng(widget.initialLatitude, widget.initialLongitude);
  }

  @override
  void dispose() {
    _cityController.dispose();
    _stateController.dispose();
    _nameController.dispose();
    super.dispose();
  }

  void _onMapTap(TapPosition _, LatLng point) {
    if (_polygonMode) {
      setState(() => _polygonPoints.add(point));
      return;
    }
    setState(() => _pin = point);
  }

  Future<void> _submit() async {
    if (_submitting) return;
    final l10n = AppLocalizations.of(context)!;
    final city = _cityController.text.trim();
    final state = _stateController.text.trim().toUpperCase();
    if (city.isEmpty || state.length != 2) {
      showErrorSnackBar(context, l10n.proposeTerritoryCityStateRequired);
      return;
    }
    if (_polygonMode && _polygonPoints.length < 3) {
      showErrorSnackBar(context, l10n.proposeTerritoryPolygonMinPoints);
      return;
    }

    setState(() => _submitting = true);
    try {
      final repo = ref.read(onboardingRepositoryProvider);
      final polygon = _polygonMode
          ? _polygonPoints
              .map((p) => {'latitude': p.latitude, 'longitude': p.longitude})
              .toList()
          : null;
      final territory = await repo.proposeTerritory(
        city: city,
        state: state,
        latitude: _pin.latitude,
        longitude: _pin.longitude,
        name: _nameController.text.trim().isEmpty ? null : _nameController.text.trim(),
        radiusKm: _polygonMode ? null : _radiusKm,
        boundaryPolygon: polygon,
      );
      if (!mounted) return;
      Navigator.of(context).pop(territory);
    } on ApiException catch (e) {
      if (mounted) showErrorSnackBar(context, e.userMessage);
    } catch (e) {
      if (mounted) showErrorSnackBar(context, e.toString());
    } finally {
      if (mounted) setState(() => _submitting = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    final theme = Theme.of(context);

    return Padding(
      padding: EdgeInsets.only(bottom: MediaQuery.viewInsetsOf(context).bottom),
      child: SingleChildScrollView(
        padding: const EdgeInsets.all(AppConstants.spacingMd),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            Text(
              l10n.proposeTerritoryTitle,
              style: theme.textTheme.titleMedium?.copyWith(fontWeight: FontWeight.w600),
            ),
            const SizedBox(height: AppConstants.spacingSm),
            Text(
              l10n.proposeTerritoryDescription,
              style: theme.textTheme.bodySmall?.copyWith(
                color: theme.colorScheme.onSurfaceVariant,
              ),
            ),
            const SizedBox(height: AppConstants.spacingMd),
            ClipRRect(
              borderRadius: BorderRadius.circular(AppConstants.radiusMd),
              child: SizedBox(
                height: 220,
                child: FlutterMap(
                  mapController: _mapController,
                  options: MapOptions(
                    initialCenter: _pin,
                    initialZoom: _mapZoom,
                    onTap: _onMapTap,
                  ),
                  children: [
                    TileLayer(
                      urlTemplate: 'https://tile.openstreetmap.org/{z}/{x}/{y}.png',
                      userAgentPackageName: 'com.araponga.app',
                    ),
                    if (!_polygonMode)
                      CircleLayer(
                        circles: [
                          CircleMarker(
                            point: _pin,
                            radius: _radiusKm * 1000,
                            useRadiusInMeter: true,
                            color: _boundaryColor.withValues(alpha: 0.2),
                            borderStrokeWidth: 2,
                            borderColor: _boundaryColor,
                          ),
                        ],
                      ),
                    if (_polygonMode && _polygonPoints.length >= 3)
                      PolygonLayer(
                        polygons: [
                          Polygon(
                            points: _polygonPoints,
                            color: _boundaryColor.withValues(alpha: 0.2),
                            borderStrokeWidth: 2,
                            borderColor: _boundaryColor,
                          ),
                        ],
                      ),
                    if (_polygonMode && _polygonPoints.isNotEmpty)
                      MarkerLayer(
                        markers: _polygonPoints
                            .map(
                              (p) => Marker(
                                point: p,
                                width: 16,
                                height: 16,
                                child: const Icon(Icons.circle, size: 12, color: _boundaryColor),
                              ),
                            )
                            .toList(),
                      ),
                    MarkerLayer(
                      markers: [
                        if (!_polygonMode)
                          Marker(
                            point: _pin,
                            width: 40,
                            height: 40,
                            child: const Icon(Icons.location_on, color: AppDesignTokens.locationPin, size: 40),
                          ),
                      ],
                    ),
                  ],
                ),
              ),
            ),
            const SizedBox(height: AppConstants.spacingSm),
            Text(
              _polygonMode ? l10n.proposeTerritoryTapPolygon : l10n.proposeTerritoryTapPin,
              style: theme.textTheme.bodySmall?.copyWith(color: theme.colorScheme.onSurfaceVariant),
            ),
            const SizedBox(height: AppConstants.spacingMd),
            Row(
              children: [
                Expanded(
                  child: TextField(
                    controller: _cityController,
                    decoration: InputDecoration(labelText: l10n.proposeTerritoryCity),
                    textCapitalization: TextCapitalization.words,
                  ),
                ),
                const SizedBox(width: AppConstants.spacingSm),
                SizedBox(
                  width: 72,
                  child: TextField(
                    controller: _stateController,
                    decoration: InputDecoration(labelText: l10n.proposeTerritoryState),
                    textCapitalization: TextCapitalization.characters,
                    maxLength: 2,
                  ),
                ),
              ],
            ),
            const SizedBox(height: AppConstants.spacingSm),
            TextField(
              controller: _nameController,
              decoration: InputDecoration(labelText: l10n.proposeTerritoryNameOptional),
            ),
            const SizedBox(height: AppConstants.spacingMd),
            SwitchListTile(
              contentPadding: EdgeInsets.zero,
              title: Text(l10n.proposeTerritoryPolygonMode),
              subtitle: Text(l10n.proposeTerritoryPolygonModeHint),
              value: _polygonMode,
              onChanged: _submitting
                  ? null
                  : (value) => setState(() {
                        _polygonMode = value;
                        _polygonPoints.clear();
                      }),
            ),
            if (!_polygonMode) ...[
              Text(l10n.proposeTerritoryRadiusLabel(_radiusKm.toStringAsFixed(1))),
              Slider(
                value: _radiusKm,
                min: 0.5,
                max: 10,
                divisions: 19,
                label: '${_radiusKm.toStringAsFixed(1)} km',
                onChanged: _submitting ? null : (v) => setState(() => _radiusKm = v),
              ),
            ] else if (_polygonPoints.isNotEmpty)
              Align(
                alignment: Alignment.centerRight,
                child: TextButton(
                  onPressed: _submitting ? null : () => setState(_polygonPoints.clear),
                  child: Text(l10n.proposeTerritoryClearPolygon),
                ),
              ),
            const SizedBox(height: AppConstants.spacingMd),
            FilledButton.icon(
              onPressed: _submitting ? null : _submit,
              icon: _submitting
                  ? SizedBox(
                      width: 20,
                      height: 20,
                      child: CircularProgressIndicator(
                        strokeWidth: 2,
                        color: theme.colorScheme.onPrimary,
                      ),
                    )
                  : const Icon(Icons.send_outlined),
              label: Text(_submitting ? l10n.proposeTerritorySubmitting : l10n.proposeTerritorySubmit),
            ),
          ],
        ),
      ),
    );
  }
}
