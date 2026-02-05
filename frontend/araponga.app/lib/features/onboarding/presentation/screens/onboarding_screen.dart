import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../core/config/constants.dart';
import '../../../../core/geo/geo_location_provider.dart';
import '../../../../core/network/api_exception.dart';
import '../../../../core/providers/territory_provider.dart';
import '../../../../core/widgets/app_snackbar.dart';
import '../../../../l10n/app_localizations.dart';
import '../../data/models/onboarding_models.dart';
import '../providers/onboarding_providers.dart';

/// Onboarding: localização → territórios sugeridos → seleção → complete → /home.
class OnboardingScreen extends ConsumerStatefulWidget {
  const OnboardingScreen({super.key});

  @override
  ConsumerState<OnboardingScreen> createState() => _OnboardingScreenState();
}

class _OnboardingScreenState extends ConsumerState<OnboardingScreen> {
  bool _completing = false;

  Future<void> _requestLocationAndRefresh() async {
    await ref.read(geoLocationStateProvider.notifier).fetch();
    if (mounted) setState(() {});
  }

  Future<void> _completeWith(TerritorySuggestion territory) async {
    if (_completing) return;
    setState(() => _completing = true);
    try {
      final repo = ref.read(onboardingRepositoryProvider);
      await repo.completeOnboarding(territory.id);
      if (!mounted) return;
      await ref.read(selectedTerritoryIdProvider.notifier).setTerritoryId(territory.id);
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

  @override
  Widget build(BuildContext context) {
    final geo = ref.watch(geoLocationStateProvider);
    final hasGeo = geo != null;

    return Scaffold(
      appBar: AppBar(
        title: Text(AppLocalizations.of(context)!.onboardingTitle),
        centerTitle: true,
      ),
      body: SafeArea(
        child: SingleChildScrollView(
          padding: const EdgeInsets.all(AppConstants.spacingMd),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              const SizedBox(height: AppConstants.spacingLg),
              Text(
                AppLocalizations.of(context)!.onboardingDescription,
                style: Theme.of(context).textTheme.bodyLarge,
              ),
              const SizedBox(height: AppConstants.spacingLg),
              if (!hasGeo) ...[
                OutlinedButton.icon(
                  onPressed: _completing
                      ? null
                      : () async {
                          await _requestLocationAndRefresh();
                        },
                  icon: const Icon(Icons.location_on_outlined),
                  label: Text(AppLocalizations.of(context)!.useMyLocation),
                ),
                const SizedBox(height: AppConstants.spacingMd),
                Text(
                  AppLocalizations.of(context)!.enableLocationHint,
                  style: Theme.of(context).textTheme.bodySmall?.copyWith(
                        color: Theme.of(context).colorScheme.onSurfaceVariant,
                      ),
                ),
              ] else ...[
                _SuggestedList(
                  latitude: geo.latitude,
                  longitude: geo.longitude,
                  onSelect: _completeWith,
                  completing: _completing,
                ),
              ],
            ],
          ),
        ),
      ),
    );
  }
}

class _SuggestedList extends ConsumerWidget {
  const _SuggestedList({
    required this.latitude,
    required this.longitude,
    required this.onSelect,
    required this.completing,
  });

  final double latitude;
  final double longitude;
  final void Function(TerritorySuggestion) onSelect;
  final bool completing;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final async = ref.watch(suggestedTerritoriesProvider((lat: latitude, lng: longitude)));

    return async.when(
      data: (list) {
        if (list.isEmpty) {
          return Padding(
            padding: const EdgeInsets.symmetric(vertical: AppConstants.spacingLg),
            child: Text(
              'Nenhum território encontrado nesta região. Tente aumentar o raio ou ative a localização.',
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
            return _TerritorySuggestionCard(
              name: t.name,
              subtitle: t.description != null && t.description!.isNotEmpty
                  ? t.description!
                  : '${t.distanceKm.toStringAsFixed(1)} km de distância',
              onTap: completing ? null : () => onSelect(t),
            );
          },
        );
      },
      loading: () => const Padding(
        padding: EdgeInsets.all(AppConstants.spacingLg),
        child: Center(child: CircularProgressIndicator()),
      ),
      error: (err, _) => Padding(
        padding: const EdgeInsets.symmetric(vertical: AppConstants.spacingLg),
        child: Column(
          children: [
            Icon(Icons.error_outline, size: AppConstants.iconSizeLg, color: Theme.of(context).colorScheme.error),
            const SizedBox(height: AppConstants.spacingMd),
            Text(
              err.toString().replaceFirst('ApiException: ', ''),
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
    this.onTap,
  });

  final String name;
  final String subtitle;
  final VoidCallback? onTap;

  @override
  Widget build(BuildContext context) {
    return Card(
      child: ListTile(
        contentPadding: const EdgeInsets.symmetric(
          horizontal: AppConstants.spacingMd,
          vertical: AppConstants.spacingSm,
        ),
        leading: CircleAvatar(
          backgroundColor: Theme.of(context).colorScheme.primaryContainer,
          child: Icon(Icons.place_outlined, color: Theme.of(context).colorScheme.onPrimaryContainer),
        ),
        title: Text(name, style: Theme.of(context).textTheme.titleSmall),
        subtitle: Padding(
          padding: const EdgeInsets.only(top: AppConstants.spacingXs),
          child: Text(
            subtitle,
            style: Theme.of(context).textTheme.bodySmall?.copyWith(
                  color: Theme.of(context).colorScheme.onSurfaceVariant,
                ),
          ),
        ),
        trailing: onTap != null
            ? Icon(Icons.arrow_forward_ios, size: AppConstants.iconSizeSm, color: Theme.of(context).colorScheme.onSurfaceVariant)
            : null,
        onTap: onTap,
      ),
    );
  }
}
