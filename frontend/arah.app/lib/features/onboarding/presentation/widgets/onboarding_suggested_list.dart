import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/config/constants.dart';
import '../../../../core/network/api_exception.dart';
import '../../../../l10n/app_localizations.dart';
import '../../data/models/onboarding_models.dart';
import '../providers/onboarding_providers.dart';
import 'onboarding_register_municipality_card.dart';
import 'onboarding_territory_card.dart';

/// Lista de territórios sugeridos próximos; oferece cadastro do município quando vazia.
class OnboardingSuggestedList extends ConsumerWidget {
  const OnboardingSuggestedList({
    super.key,
    required this.latitude,
    required this.longitude,
    required this.selectedTerritoryId,
    required this.provisionedTerritory,
    required this.onSelectTerritory,
    required this.onTerritoryProvisioned,
    required this.completing,
  });

  final double latitude;
  final double longitude;
  final String? selectedTerritoryId;
  final TerritorySuggestion? provisionedTerritory;

  /// Só seleciona o território (atualiza mapa e destaque); não completa o onboarding.
  final void Function(TerritorySuggestion) onSelectTerritory;
  final void Function(TerritorySuggestion) onTerritoryProvisioned;
  final bool completing;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final async = ref.watch(suggestedTerritoriesProvider((lat: latitude, lng: longitude)));

    return async.when(
      data: (list) {
        final merged = List<TerritorySuggestion>.from(list);
        if (provisionedTerritory != null &&
            merged.every((t) => t.id != provisionedTerritory!.id)) {
          merged.insert(0, provisionedTerritory!);
        }
        return Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            if (merged.isEmpty)
              OnboardingRegisterMunicipalityCard(
                latitude: latitude,
                longitude: longitude,
                completing: completing,
                onTerritoryProvisioned: onTerritoryProvisioned,
              )
            else
              ListView.separated(
                shrinkWrap: true,
                physics: const NeverScrollableScrollPhysics(),
                itemCount: merged.length,
                separatorBuilder: (_, __) => const SizedBox(height: AppConstants.spacingSm),
                itemBuilder: (context, index) {
                  final t = merged[index];
                  final isSelected = t.id == selectedTerritoryId;
                  return OnboardingTerritoryCard(
                    name: t.name,
                    subtitle: t.description != null && t.description!.isNotEmpty
                        ? t.description!
                        : '${t.distanceKm.toStringAsFixed(1)} km de distância',
                    isSelected: isSelected,
                    isPending: t.isPending,
                    onTap: completing ? null : () => onSelectTerritory(t),
                  );
                },
              ),
          ],
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
