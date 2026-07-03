import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/config/constants.dart';
import '../../../../l10n/app_localizations.dart';
import '../../data/models/onboarding_models.dart';

/// Botão Continuar: usa o território selecionado na lista (ou o mais próximo). Só completa ao tocar aqui.
class OnboardingContinueButton extends StatelessWidget {
  const OnboardingContinueButton({
    super.key,
    required this.suggestedAsync,
    required this.nearestSuggestion,
    required this.selectedTerritoryId,
    required this.provisionedTerritory,
    required this.completing,
    required this.onCompleteWith,
    required this.l10n,
  });

  final AsyncValue<List<TerritorySuggestion>> suggestedAsync;
  final TerritorySuggestion? nearestSuggestion;
  final String? selectedTerritoryId;
  final TerritorySuggestion? provisionedTerritory;
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
      if (effective == null || effective.id != selectedTerritoryId) {
        effective = provisionedTerritory?.id == selectedTerritoryId ? provisionedTerritory : effective;
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
