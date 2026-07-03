import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/config/constants.dart';
import '../../../../core/network/api_exception.dart';
import '../../../../core/widgets/app_snackbar.dart';
import '../../../../l10n/app_localizations.dart';
import '../../data/models/onboarding_models.dart';
import '../providers/onboarding_providers.dart';
import 'propose_territory_sheet.dart';

/// Quando não há territórios próximos, permite cadastrar o município via contorno IBGE.
class OnboardingRegisterMunicipalityCard extends ConsumerStatefulWidget {
  const OnboardingRegisterMunicipalityCard({
    super.key,
    required this.latitude,
    required this.longitude,
    required this.completing,
    required this.onTerritoryProvisioned,
  });

  final double latitude;
  final double longitude;
  final bool completing;
  final void Function(TerritorySuggestion) onTerritoryProvisioned;

  @override
  ConsumerState<OnboardingRegisterMunicipalityCard> createState() =>
      _OnboardingRegisterMunicipalityCardState();
}

class _OnboardingRegisterMunicipalityCardState
    extends ConsumerState<OnboardingRegisterMunicipalityCard> {
  bool _loading = false;
  bool _ibgeFailed = false;

  Future<void> _register() async {
    if (_loading || widget.completing) return;
    setState(() {
      _loading = true;
      _ibgeFailed = false;
    });
    final l10n = AppLocalizations.of(context)!;
    try {
      final repo = ref.read(onboardingRepositoryProvider);
      final territory = await repo.suggestMunicipality(
        latitude: widget.latitude,
        longitude: widget.longitude,
      );
      if (!mounted) return;
      ref.invalidate(suggestedTerritoriesProvider((lat: widget.latitude, lng: widget.longitude)));
      widget.onTerritoryProvisioned(territory);
      showSuccessSnackBar(context, l10n.registerMunicipalitySuccess(territory.name));
    } on ApiException catch (e) {
      if (mounted) {
        setState(() => _ibgeFailed = true);
        showErrorSnackBar(context, e.userMessage);
      }
    } catch (e) {
      if (mounted) {
        setState(() => _ibgeFailed = true);
        showErrorSnackBar(context, e.toString());
      }
    } finally {
      if (mounted) setState(() => _loading = false);
    }
  }

  Future<void> _openProposeSheet() async {
    if (_loading || widget.completing) return;
    final territory = await showProposeTerritorySheet(
      context: context,
      initialLatitude: widget.latitude,
      initialLongitude: widget.longitude,
    );
    if (!mounted || territory == null) return;
    widget.onTerritoryProvisioned(territory);
    showSuccessSnackBar(context, AppLocalizations.of(context)!.proposeTerritorySuccess(territory.name));
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(AppConstants.spacingMd),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            Text(
              l10n.noTerritoryInRegion,
              style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                    color: Theme.of(context).colorScheme.onSurfaceVariant,
                  ),
            ),
            const SizedBox(height: AppConstants.spacingMd),
            Text(
              l10n.registerMunicipalityTitle,
              style: Theme.of(context).textTheme.titleSmall?.copyWith(
                    color: Theme.of(context).colorScheme.primary,
                    fontWeight: FontWeight.w600,
                  ),
            ),
            const SizedBox(height: AppConstants.spacingSm),
            Text(
              l10n.registerMunicipalityDescription,
              style: Theme.of(context).textTheme.bodySmall?.copyWith(
                    color: Theme.of(context).colorScheme.onSurfaceVariant,
                  ),
            ),
            const SizedBox(height: AppConstants.spacingMd),
            FilledButton.icon(
              onPressed: _loading || widget.completing ? null : _register,
              icon: _loading
                  ? SizedBox(
                      width: 20,
                      height: 20,
                      child: CircularProgressIndicator(
                        strokeWidth: 2,
                        color: Theme.of(context).colorScheme.onPrimary,
                      ),
                    )
                  : const Icon(Icons.map_outlined),
              label: Text(_loading ? l10n.registerMunicipalityLoading : l10n.registerMunicipalityButtonGeneric),
            ),
            if (_ibgeFailed) ...[
              const SizedBox(height: AppConstants.spacingSm),
              Text(
                l10n.registerMunicipalityFailed,
                style: Theme.of(context).textTheme.bodySmall?.copyWith(
                      color: Theme.of(context).colorScheme.error,
                    ),
              ),
            ],
            const SizedBox(height: AppConstants.spacingSm),
            OutlinedButton.icon(
              onPressed: _loading || widget.completing ? null : _openProposeSheet,
              icon: const Icon(Icons.draw_outlined),
              label: Text(l10n.proposeTerritoryButton),
            ),
          ],
        ),
      ),
    );
  }
}
