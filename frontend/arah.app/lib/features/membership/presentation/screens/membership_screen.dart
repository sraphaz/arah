import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/config/constants.dart';
import '../../../../core/geo/geo_location_provider.dart';
import '../../../../core/network/api_exception.dart';
import '../../../../core/providers/territory_provider.dart';
import '../../../../core/widgets/app_snackbar.dart';
import '../../../../core/widgets/arah_scaffold.dart';
import '../../../../l10n/app_localizations.dart';
import '../../../territories/presentation/widgets/territory_indicator_bar.dart';
import '../providers/membership_provider.dart';

class MembershipScreen extends ConsumerWidget {
  const MembershipScreen({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final l10n = AppLocalizations.of(context)!;
    final territoryId = ref.watch(selectedTerritoryIdValueProvider);
    final state = ref.watch(membershipProvider);
    final notifier = ref.read(membershipProvider.notifier);

    if (territoryId == null || territoryId.isEmpty) {
      return ArahScaffold(
        appBar: AppBar(title: Text(l10n.membership)),
        body: Center(child: Text(l10n.chooseTerritoryFirst)),
      );
    }

    return ArahScaffold(
      appBar: AppBar(title: Text(l10n.membership)),
      body: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          const TerritoryIndicatorBar(),
          Expanded(
            child: RefreshIndicator(
              onRefresh: () => notifier.refresh(),
              child: _buildBody(context, ref, state, notifier),
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildBody(
    BuildContext context,
    WidgetRef ref,
    MembershipState state,
    MembershipNotifier notifier,
  ) {
    final l10n = AppLocalizations.of(context)!;
    if (state.isLoading && state.membership == null && state.error == null) {
      return ListView(
        physics: AlwaysScrollableScrollPhysics(),
        children: [SizedBox(height: 200, child: Center(child: CircularProgressIndicator()))],
      );
    }

    if (state.error != null && state.membership == null) {
      final msg = state.error is ApiException
          ? (state.error as ApiException).userMessage
          : 'Erro ao carregar membership.';
      return ListView(
        physics: const AlwaysScrollableScrollPhysics(),
        children: [
          Padding(
            padding: const EdgeInsets.all(AppConstants.spacingLg),
            child: Column(
              children: [
                Text(msg, textAlign: TextAlign.center),
                const SizedBox(height: AppConstants.spacingMd),
                FilledButton.tonal(onPressed: () => notifier.refresh(), child: Text(l10n.tryAgain)),
              ],
            ),
          ),
        ],
      );
    }

    final membership = state.membership;
    final role = membership?.role ?? 'VISITOR';
    final isResident = membership?.isResident ?? false;

    return ListView(
      physics: const AlwaysScrollableScrollPhysics(),
      padding: const EdgeInsets.all(AppConstants.spacingMd),
      children: [
        Card(
          child: Padding(
            padding: const EdgeInsets.all(AppConstants.spacingMd),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(l10n.yourRole, style: Theme.of(context).textTheme.titleMedium),
                const SizedBox(height: AppConstants.spacingSm),
                Text(role, style: Theme.of(context).textTheme.headlineSmall),
                if (membership?.residencyVerification != null) ...[
                  const SizedBox(height: AppConstants.spacingSm),
                  Text(l10n.verificationLabel(membership!.residencyVerification!)),
                ],
              ],
            ),
          ),
        ),
        const SizedBox(height: AppConstants.spacingLg),
        if (!isResident) ...[
          FilledButton.icon(
            onPressed: () async {
              try {
                await notifier.becomeResident(message: 'Solicitação via app');
                if (context.mounted) showSuccessSnackBar(context, 'Solicitação enviada.');
              } catch (e) {
                if (context.mounted) {
                  showErrorSnackBar(
                    context,
                    e is ApiException ? e.userMessage : 'Erro ao solicitar residência.',
                  );
                }
              }
            },
            icon: const Icon(Icons.home_work_outlined),
            label: Text(l10n.requestResidency),
          ),
          const SizedBox(height: AppConstants.spacingMd),
          OutlinedButton.icon(
            onPressed: () async {
              final geo = ref.read(geoLocationStateProvider);
              if (geo == null) {
                if (context.mounted) showErrorSnackBar(context, 'Ative a localização primeiro.');
                return;
              }
              try {
                await notifier.verifyByGeo(geo.latitude, geo.longitude);
                if (context.mounted) showSuccessSnackBar(context, 'Residência verificada por geo.');
              } catch (e) {
                if (context.mounted) {
                  showErrorSnackBar(
                    context,
                    e is ApiException ? e.userMessage : 'Erro na verificação.',
                  );
                }
              }
            },
            icon: const Icon(Icons.location_on_outlined),
            label: Text(l10n.verifyByLocation),
          ),
        ] else
          Text(
            'Você já é morador neste território.',
            style: Theme.of(context).textTheme.bodyLarge?.copyWith(
                  color: Theme.of(context).colorScheme.primary,
                ),
          ),
      ],
    );
  }
}
