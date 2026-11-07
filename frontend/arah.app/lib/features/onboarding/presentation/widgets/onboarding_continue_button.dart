import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/theme/app_design_tokens.dart';
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
        effective = provisionedTerritory?.id == selectedTerritoryId
            ? provisionedTerritory
            : effective;
      }
    }
    final canContinue = effective != null &&
        !completing &&
        !isLoading &&
        !suggestedAsync.hasError;

    final label = isLoading
        ? l10n.onboardingLoadingTerritories
        : (effective != null
            ? l10n.onboardingContinueWith(effective.name)
            : l10n.continueButton);

    return Opacity(
      opacity: canContinue || isLoading ? 1 : 0.65,
      child: DecoratedBox(
        decoration: BoxDecoration(
          gradient: const LinearGradient(
            begin: Alignment(-0.5, -0.866),
            end: Alignment(0.5, 0.866),
            colors: [
              AppDesignTokens.loginButtonGradientStart,
              AppDesignTokens.loginButtonGradientEnd,
            ],
          ),
          borderRadius: BorderRadius.circular(15),
          boxShadow: const [
            BoxShadow(
              color: AppDesignTokens.loginButtonGlow,
              blurRadius: 24,
              offset: Offset(0, 8),
            ),
          ],
        ),
        child: SizedBox(
          width: double.infinity,
          height: 60,
          child: Material(
            color: Colors.transparent,
            child: InkWell(
              onTap: canContinue ? () => onCompleteWith(effective!) : null,
              borderRadius: BorderRadius.circular(15),
              child: Padding(
                padding:
                    const EdgeInsets.symmetric(horizontal: 22, vertical: 15),
                child: Row(
                  mainAxisAlignment: MainAxisAlignment.center,
                  children: [
                    if (isLoading)
                      const SizedBox(
                        width: 20,
                        height: 20,
                        child: CircularProgressIndicator(
                          strokeWidth: 2,
                          color: AppDesignTokens.textOnAccent,
                        ),
                      )
                    else
                      const Icon(
                        Icons.arrow_forward,
                        color: AppDesignTokens.textOnAccent,
                        size: 20,
                      ),
                    const SizedBox(width: 8),
                    Flexible(
                      child: Text(
                        label,
                        overflow: TextOverflow.ellipsis,
                        style:
                            Theme.of(context).textTheme.titleMedium?.copyWith(
                                  color: AppDesignTokens.textOnAccent,
                                  fontSize: 16,
                                  fontWeight: FontWeight.w600,
                                  letterSpacing: -0.1,
                                ),
                      ),
                    ),
                  ],
                ),
              ),
            ),
          ),
        ),
      ),
    );
  }
}
