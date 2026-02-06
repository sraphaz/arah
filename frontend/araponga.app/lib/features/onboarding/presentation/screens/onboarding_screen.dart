import 'package:flutter/material.dart';
import 'package:flutter_map/flutter_map.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:latlong2/latlong.dart';

import '../../../../core/config/constants.dart';
import '../../../../core/geo/geo_location_provider.dart';
import '../../../../core/network/api_exception.dart';
import '../../../../core/providers/territory_provider.dart';
import '../../../../core/widgets/app_snackbar.dart';
import '../../../../l10n/app_localizations.dart';
import '../../../auth/presentation/providers/auth_state_provider.dart';
import '../../../territories/data/repositories/territories_repository.dart';
import '../../../territories/presentation/providers/territories_list_provider.dart';
import '../../data/models/onboarding_models.dart';
import '../providers/onboarding_providers.dart';

/// Altura do mapa na tela de onboarding.
const double _onboardingMapHeight = 220.0;
const double _onboardingMapZoom = 12.0;

/// Centro padrão do mapa quando ainda não há localização (Camburi).
const double _defaultCenterLat = -23.76281;
const double _defaultCenterLng = -45.63691;

/// Onboarding: localização → mapa + territórios próximos → seleção → Continuar → /home.
///
/// Exibida apenas após login/cadastro quando **não há preferência de território salva**.
/// Se já houvesse território selecionado, o app iria direto para o feed (sem pedir email, senha ou seleção).
/// O usuário **só se torna visitante** do território ao tocar em "Continuar" nesta tela (complete onboarding).
/// Por enquanto exibe apenas a lista de territórios próximos (suggested-territories).
class OnboardingScreen extends ConsumerStatefulWidget {
  const OnboardingScreen({super.key});

  @override
  ConsumerState<OnboardingScreen> createState() => _OnboardingScreenState();
}

class _OnboardingScreenState extends ConsumerState<OnboardingScreen> {
  bool _completing = false;
  bool _locationRequested = false;
  bool _requestingLocation = false;
  /// Território escolhido na lista; null = usar o mais próximo. Só altera mapa e botão Continuar; não completa onboarding.
  String? _selectedTerritoryId;

  @override
  void initState() {
    super.initState();
    _requestLocationOnce();
  }

  Future<void> _requestLocationOnce() async {
    if (_locationRequested) return;
    _locationRequested = true;
    await ref.read(geoLocationStateProvider.notifier).fetch();
    if (mounted) setState(() {});
  }

  Future<void> _requestLocationAndRefresh() async {
    if (_requestingLocation) return;
    setState(() => _requestingLocation = true);
    await ref.read(geoLocationStateProvider.notifier).fetch();
    if (!mounted) return;
    setState(() => _requestingLocation = false);
    // Forçar rebuild para o ícone/estado de localização atualizar logo após a permissão (evita ter de clicar de novo).
    setState(() {});
  }

  Future<void> _completeWithId(String territoryId) async {
    if (_completing) return;
    setState(() => _completing = true);
    try {
      final repo = ref.read(onboardingRepositoryProvider);
      await repo.completeOnboarding(territoryId);
      if (!mounted) return;
      await ref.read(selectedTerritoryIdProvider.notifier).setTerritoryId(territoryId);
      if (!mounted) return;
      context.go('/home');
    } on ApiException catch (e) {
      if (mounted) showErrorSnackBar(context, e.userMessage);
    } catch (e) {
      if (mounted) showErrorSnackBar(context, e.toString());
    } finally {
      if (mounted) setState(() => _completing = false);
    }
  }

  Future<void> _completeWith(TerritorySuggestion territory) async {
    await _completeWithId(territory.id);
  }

  /// Volta para a tela de login/cadastro (faz logout). O router observa authStateProvider;
  /// ao ficar sem token, o redirect envia para /login. Não chamar context.go para não
  /// disputar com o redirect (que poderia ainda ver token antigo).
  Future<void> _goBackToLogin(BuildContext context) async {
    await ref.read(selectedTerritoryIdProvider.notifier).setTerritoryId(null);
    await ref.read(authStateProvider.notifier).logout();
    // Navega na próxima frame para o estado de auth já estar atualizado no router.
    if (!context.mounted) return;
    WidgetsBinding.instance.addPostFrameCallback((_) {
      if (!context.mounted) return;
      context.go('/login');
    });
  }

  @override
  Widget build(BuildContext context) {
    final geo = ref.watch(geoLocationStateProvider);
    final hasGeo = geo != null;
    final l10n = AppLocalizations.of(context)!;
    final suggestedAsync = hasGeo
        ? ref.watch(suggestedTerritoriesProvider((lat: geo.latitude, lng: geo.longitude)))
        : const AsyncValue<List<TerritorySuggestion>>.data([]);
    final nearestSuggestion = suggestedAsync.valueOrNull?.isNotEmpty == true
        ? suggestedAsync.valueOrNull!.first
        : null;
    // Seleção inicial: mostrar o mais próximo como selecionado (mapa + destaque na lista)
    if (_selectedTerritoryId == null && nearestSuggestion != null) {
      WidgetsBinding.instance.addPostFrameCallback((_) {
        if (mounted && _selectedTerritoryId == null) {
          setState(() => _selectedTerritoryId = nearestSuggestion.id);
        }
      });
    }

    return Scaffold(
      appBar: AppBar(
        leading: IconButton(
          icon: const Icon(Icons.arrow_back),
          onPressed: () => _goBackToLogin(context),
          tooltip: l10n.back,
        ),
        title: Text(l10n.onboardingTitle),
        centerTitle: true,
      ),
      body: SafeArea(
        child: SingleChildScrollView(
          padding: const EdgeInsets.all(AppConstants.spacingMd),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              const SizedBox(height: AppConstants.spacingSm),
              Text(
                l10n.onboardingDescription,
                style: Theme.of(context).textTheme.bodyLarge,
              ),
              const SizedBox(height: AppConstants.spacingLg),
              // Fase 1: bloco de localização (sempre na mesma tela)
              Card(
                child: Padding(
                  padding: const EdgeInsets.all(AppConstants.spacingMd),
                  child: hasGeo
                      ? Column(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          mainAxisSize: MainAxisSize.min,
                          children: [
                            Row(
                              children: [
                                Icon(
                                  Icons.location_on,
                                  color: Colors.orange,
                                  size: AppConstants.iconSizeLg,
                                ),
                                const SizedBox(width: AppConstants.spacingSm),
                                Expanded(
                                  child: Text(
                                    l10n.onboardingLocationEnabled,
                                    style: Theme.of(context).textTheme.titleSmall?.copyWith(
                                          color: Theme.of(context).colorScheme.primary,
                                          fontWeight: FontWeight.w600,
                                        ),
                                  ),
                                ),
                              ],
                            ),
                            const SizedBox(height: AppConstants.spacingSm),
                            Text(
                              l10n.onboardingLocationPrivacy,
                              style: Theme.of(context).textTheme.bodySmall?.copyWith(
                                    color: Theme.of(context).colorScheme.onSurfaceVariant,
                                  ),
                            ),
                          ],
                        )
                      : Column(
                          crossAxisAlignment: CrossAxisAlignment.stretch,
                          children: [
                            OutlinedButton.icon(
                              onPressed: _completing || _requestingLocation
                                  ? null
                                  : () async {
                                      await _requestLocationAndRefresh();
                                    },
                              icon: _requestingLocation
                                  ? SizedBox(
                                      width: 20,
                                      height: 20,
                                      child: CircularProgressIndicator(
                                        strokeWidth: 2,
                                        color: Theme.of(context).colorScheme.primary,
                                      ),
                                    )
                                  : const Icon(Icons.location_on_outlined),
                              label: Text(
                                _requestingLocation ? l10n.onboardingGettingLocation : l10n.useMyLocation,
                              ),
                            ),
                            const SizedBox(height: AppConstants.spacingSm),
                            Text(
                              l10n.onboardingAllowLocationToCenter,
                              style: Theme.of(context).textTheme.bodySmall?.copyWith(
                                    color: Theme.of(context).colorScheme.onSurfaceVariant,
                                  ),
                            ),
                          ],
                        ),
                ),
              ),
              const SizedBox(height: AppConstants.spacingLg),
              // Mapa: contorno do território selecionado na lista (ou o mais próximo se nenhum selecionado)
              _OnboardingMap(
                centerLat: hasGeo ? geo.latitude : _defaultCenterLat,
                centerLng: hasGeo ? geo.longitude : _defaultCenterLng,
                showUserMarker: hasGeo,
                displayedTerritoryId: _selectedTerritoryId ?? nearestSuggestion?.id,
              ),
              const SizedBox(height: AppConstants.spacingLg),
              // Botão Continuar: ao tocar, o usuário se torna visitante do território (único momento).
              if (hasGeo) ...[
                Text(
                  l10n.onboardingVisitorOnContinue,
                  style: Theme.of(context).textTheme.bodySmall?.copyWith(
                        color: Theme.of(context).colorScheme.onSurfaceVariant,
                      ),
                ),
                const SizedBox(height: AppConstants.spacingSm),
                _ContinueButton(
                  suggestedAsync: suggestedAsync,
                  nearestSuggestion: nearestSuggestion,
                  selectedTerritoryId: _selectedTerritoryId,
                  completing: _completing,
                  onCompleteWith: _completeWith,
                  l10n: l10n,
                ),
                const SizedBox(height: AppConstants.spacingLg),
              ],
              if (hasGeo) ...[
                Text(
                  l10n.onboardingNearbyTitle,
                  style: Theme.of(context).textTheme.titleSmall?.copyWith(
                        color: Theme.of(context).colorScheme.primary,
                        fontWeight: FontWeight.w600,
                      ),
                ),
                const SizedBox(height: AppConstants.spacingSm),
                _SuggestedList(
                  latitude: geo.latitude,
                  longitude: geo.longitude,
                  selectedTerritoryId: _selectedTerritoryId,
                  onSelectTerritory: (t) => setState(() => _selectedTerritoryId = t.id),
                  completing: _completing,
                ),
                const SizedBox(height: AppConstants.spacingLg),
              ] else
                Padding(
                  padding: const EdgeInsets.only(bottom: AppConstants.spacingMd),
                  child: Text(
                    l10n.enableLocationHint,
                    style: Theme.of(context).textTheme.bodySmall?.copyWith(
                          color: Theme.of(context).colorScheme.onSurfaceVariant,
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

/// Botão Continuar: usa o território selecionado na lista (ou o mais próximo). Só completa ao tocar aqui.
class _ContinueButton extends StatelessWidget {
  const _ContinueButton({
    required this.suggestedAsync,
    required this.nearestSuggestion,
    required this.selectedTerritoryId,
    required this.completing,
    required this.onCompleteWith,
    required this.l10n,
  });

  final AsyncValue<List<TerritorySuggestion>> suggestedAsync;
  final TerritorySuggestion? nearestSuggestion;
  final String? selectedTerritoryId;
  final bool completing;
  final void Function(TerritorySuggestion) onCompleteWith;
  final AppLocalizations l10n;

  @override
  Widget build(BuildContext context) {
    final isLoading = suggestedAsync.isLoading;
    final list = suggestedAsync.valueOrNull ?? [];
    TerritorySuggestion? effective = nearestSuggestion;
    if (selectedTerritoryId != null) {
      for (final t in list) {
        if (t.id == selectedTerritoryId) {
          effective = t;
          break;
        }
      }
    }
    final canContinue = effective != null && !completing && !isLoading && !suggestedAsync.hasError;

    return FilledButton.icon(
      onPressed: canContinue ? () => onCompleteWith(effective!) : null,
      icon: isLoading
          ? SizedBox(
              width: 20,
              height: 20,
              child: CircularProgressIndicator(
                strokeWidth: 2,
                color: Theme.of(context).colorScheme.onPrimary,
              ),
            )
          : Icon(
              canContinue ? Icons.check_circle_outline : Icons.hourglass_empty,
              size: AppConstants.iconSizeMd,
            ),
      label: Text(
        isLoading
            ? l10n.onboardingLoadingTerritories
            : (effective != null
                ? l10n.onboardingContinueWith(effective.name)
                : l10n.continueButton),
      ),
    );
  }
}

/// Cor do contorno do polígono e do pin do território (verde floresta).
const _territoryBoundaryColor = Color(0xFF228B22);

/// Mapa compacto: sempre visível; contorno do território selecionado (ou mais próximo).
class _OnboardingMap extends ConsumerWidget {
  const _OnboardingMap({
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
                    child: Icon(
                      Icons.person_pin_circle,
                      color: Colors.orange,
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
                          child: Icon(
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
            color: color.withOpacity(0.2),
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
            color: color.withOpacity(0.2),
            borderStrokeWidth: 2.5,
            borderColor: color,
          ),
        ],
      );
    }
    return const SizedBox.shrink();
  }
}

class _SuggestedList extends ConsumerWidget {
  const _SuggestedList({
    required this.latitude,
    required this.longitude,
    required this.selectedTerritoryId,
    required this.onSelectTerritory,
    required this.completing,
  });

  final double latitude;
  final double longitude;
  final String? selectedTerritoryId;
  /// Só seleciona o território (atualiza mapa e destaque); não completa o onboarding.
  final void Function(TerritorySuggestion) onSelectTerritory;
  final bool completing;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final async = ref.watch(suggestedTerritoriesProvider((lat: latitude, lng: longitude)));

    return async.when(
      data: (list) {
        if (list.isEmpty) {
          return Padding(
            padding: const EdgeInsets.symmetric(vertical: AppConstants.spacingSm),
            child: Text(
              AppLocalizations.of(context)!.noTerritoryInRegion,
              style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                    color: Theme.of(context).colorScheme.onSurfaceVariant,
                  ),
            ),
          );
        }
        return ListView.separated(
          shrinkWrap: true,
          physics: const NeverScrollableScrollPhysics(),
          itemCount: list.length,
          separatorBuilder: (_, __) => const SizedBox(height: AppConstants.spacingSm),
          itemBuilder: (context, index) {
            final t = list[index];
            final isSelected = t.id == selectedTerritoryId;
            return _TerritorySuggestionCard(
              name: t.name,
              subtitle: t.description != null && t.description!.isNotEmpty
                  ? t.description!
                  : '${t.distanceKm.toStringAsFixed(1)} km de distância',
              isSelected: isSelected,
              onTap: completing ? null : () => onSelectTerritory(t),
            );
          },
        );
      },
      loading: () => const Padding(
        padding: EdgeInsets.symmetric(vertical: AppConstants.spacingMd),
        child: Center(child: SizedBox(height: 32, width: 32, child: CircularProgressIndicator(strokeWidth: 2))),
      ),
      error: (err, _) => Padding(
        padding: const EdgeInsets.symmetric(vertical: AppConstants.spacingMd),
        child: Column(
          children: [
            Icon(Icons.error_outline, size: AppConstants.iconSizeLg, color: Theme.of(context).colorScheme.error),
            const SizedBox(height: AppConstants.spacingMd),
            Text(
              err is ApiException ? err.userMessage : err.toString().replaceFirst('ApiException: ', ''),
              textAlign: TextAlign.center,
              style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                    color: Theme.of(context).colorScheme.error,
                  ),
            ),
            const SizedBox(height: AppConstants.spacingMd),
            FilledButton.tonal(
              onPressed: () => ref.invalidate(suggestedTerritoriesProvider((lat: latitude, lng: longitude))),
              child: Text(AppLocalizations.of(context)!.tryAgain),
            ),
          ],
        ),
      ),
    );
  }
}

class _TerritorySuggestionCard extends StatelessWidget {
  const _TerritorySuggestionCard({
    required this.name,
    required this.subtitle,
    this.isSelected = false,
    this.onTap,
  });

  final String name;
  final String subtitle;
  final bool isSelected;
  final VoidCallback? onTap;

  @override
  Widget build(BuildContext context) {
    final colorScheme = Theme.of(context).colorScheme;
    return Card(
      elevation: isSelected ? 2 : 1,
      shape: RoundedRectangleBorder(
        borderRadius: BorderRadius.circular(AppConstants.radiusMd),
        side: isSelected
            ? BorderSide(color: colorScheme.primary, width: 2)
            : BorderSide.none,
      ),
      child: ListTile(
        contentPadding: const EdgeInsets.symmetric(
          horizontal: AppConstants.spacingMd,
          vertical: AppConstants.spacingSm,
        ),
        leading: CircleAvatar(
          backgroundColor: isSelected
              ? colorScheme.primaryContainer
              : colorScheme.surfaceContainerHighest,
          child: Icon(
            isSelected ? Icons.place : Icons.place_outlined,
            color: isSelected ? colorScheme.primary : colorScheme.onSurfaceVariant,
          ),
        ),
        title: Text(
          name,
          style: Theme.of(context).textTheme.titleSmall?.copyWith(
                fontWeight: isSelected ? FontWeight.w600 : null,
              ),
        ),
        subtitle: subtitle.isNotEmpty
            ? Padding(
                padding: const EdgeInsets.only(top: AppConstants.spacingXs),
                child: Text(
                  subtitle,
                  style: Theme.of(context).textTheme.bodySmall?.copyWith(
                        color: Theme.of(context).colorScheme.onSurfaceVariant,
                      ),
                ),
              )
            : null,
        trailing: onTap != null
            ? Icon(
                isSelected ? Icons.check_circle : Icons.arrow_forward_ios,
                size: AppConstants.iconSizeSm,
                color: isSelected ? colorScheme.primary : colorScheme.onSurfaceVariant,
              )
            : null,
        onTap: onTap,
      ),
    );
  }
}
