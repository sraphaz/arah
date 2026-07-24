import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/config/constants.dart';
import '../../../../l10n/app_localizations.dart';
import '../../data/models/onboarding_models.dart';
import 'onboarding_continue_button.dart';
import 'onboarding_location_card.dart';
import 'onboarding_map.dart';
import 'onboarding_suggested_list.dart';

/// Centro padrão do mapa quando ainda não há localização (Camburi).
const double _defaultCenterLat = -23.76281;
const double _defaultCenterLng = -45.63691;

/// Corpo rolável do onboarding: localização → mapa → botão Continuar → lista de
/// territórios próximos. Recebe estado e callbacks do [OnboardingScreen]; não guarda estado.
class OnboardingBody extends StatelessWidget {
  const OnboardingBody({
    super.key,
    required this.latitude,
    required this.longitude,
    required this.suggestedAsync,
    required this.nearestSuggestion,
    required this.selectedTerritoryId,
    required this.provisionedTerritory,
    required this.completing,
    required this.requestingLocation,
    required this.onRequestLocation,
    required this.onCompleteWith,
    required this.onSelectTerritory,
    required this.onTerritoryProvisioned,
  });

  /// Latitude/longitude atuais; null quando ainda não há localização.
  final double? latitude;
  final double? longitude;
  final AsyncValue<List<TerritorySuggestion>> suggestedAsync;
  final TerritorySuggestion? nearestSuggestion;
  final String? selectedTerritoryId;
  final TerritorySuggestion? provisionedTerritory;
  final bool completing;
  final bool requestingLocation;
  final Future<void> Function() onRequestLocation;
  final void Function(TerritorySuggestion) onCompleteWith;
  final void Function(TerritorySuggestion) onSelectTerritory;
  final void Function(TerritorySuggestion) onTerritoryProvisioned;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    final hasGeo = latitude != null && longitude != null;

    return SingleChildScrollView(
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
          OnboardingLocationCard(
            hasGeo: hasGeo,
            requestingLocation: requestingLocation,
            completing: completing,
            onRequestLocation: onRequestLocation,
          ),
          const SizedBox(height: AppConstants.spacingLg),
          // Mapa: contorno do território selecionado na lista (ou o mais próximo se nenhum selecionado)
          OnboardingMap(
            centerLat: hasGeo ? latitude! : _defaultCenterLat,
            centerLng: hasGeo ? longitude! : _defaultCenterLng,
            showUserMarker: hasGeo,
            displayedTerritoryId: selectedTerritoryId ?? nearestSuggestion?.id,
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
            OnboardingContinueButton(
              suggestedAsync: suggestedAsync,
              nearestSuggestion: nearestSuggestion,
              selectedTerritoryId: selectedTerritoryId,
              provisionedTerritory: provisionedTerritory,
              completing: completing,
              onCompleteWith: onCompleteWith,
              l10n: l10n,
            ),
            if (provisionedTerritory?.isPending == true) ...[
              const SizedBox(height: AppConstants.spacingSm),
              Text(
                l10n.onboardingPendingTerritoryHint,
                style: Theme.of(context).textTheme.bodySmall?.copyWith(
                      color: Theme.of(context).colorScheme.tertiary,
                    ),
              ),
            ],
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
            OnboardingSuggestedList(
              latitude: latitude!,
              longitude: longitude!,
              selectedTerritoryId: selectedTerritoryId,
              provisionedTerritory: provisionedTerritory,
              onSelectTerritory: onSelectTerritory,
              onTerritoryProvisioned: onTerritoryProvisioned,
              completing: completing,
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
    );
  }
}
