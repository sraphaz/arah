import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/config/constants.dart';
import '../../../../core/network/api_exception.dart';
import '../../../../core/widgets/app_snackbar.dart';
import '../../../../core/widgets/arah_list_skeleton.dart';
import '../../../../core/widgets/arah_scaffold.dart';
import '../../../../l10n/app_localizations.dart';
import '../providers/subscriptions_provider.dart';

class SubscriptionsScreen extends ConsumerWidget {
  const SubscriptionsScreen({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final l10n = AppLocalizations.of(context)!;
    final state = ref.watch(subscriptionsProvider);
    final notifier = ref.read(subscriptionsProvider.notifier);

    return ArahScaffold(
      appBar: AppBar(title: Text(l10n.subscriptions)),
      body: RefreshIndicator(
        onRefresh: () => notifier.refresh(),
        child: _buildBody(context, state, notifier),
      ),
    );
  }

  Widget _buildBody(
    BuildContext context,
    SubscriptionsState state,
    SubscriptionsNotifier notifier,
  ) {
    final l10n = AppLocalizations.of(context)!;
    if (state.isLoading && state.plans.isEmpty) {
      return ListView(
        physics: AlwaysScrollableScrollPhysics(),
        children: const [ArahListSkeleton()],
      );
    }
    if (state.error != null && state.plans.isEmpty) {
      return ListView(
        physics: const AlwaysScrollableScrollPhysics(),
        children: [
          Padding(
            padding: const EdgeInsets.all(AppConstants.spacingLg),
            child: Text(
              state.error is ApiException
                  ? (state.error as ApiException).userMessage
                  : l10n.errorLoadPlans,
              textAlign: TextAlign.center,
            ),
          ),
        ],
      );
    }

    final hasPaidSubscription = state.mySubscription != null &&
        state.mySubscription!.status.toUpperCase() != 'FREE' &&
        state.mySubscription!.status.toUpperCase() != 'CANCELED';

    return ListView(
      physics: const AlwaysScrollableScrollPhysics(),
      padding: const EdgeInsets.all(AppConstants.spacingMd),
      children: [
        if (state.mySubscription != null) ...[
          Card(
            child: ListTile(
              leading: const Icon(Icons.verified_outlined),
              title: Text(l10n.mySubscription),
              subtitle: Text(l10n.statusLabel(state.mySubscription!.status)),
              trailing: hasPaidSubscription
                  ? TextButton(
                      onPressed: () async {
                        try {
                          await notifier.cancelMySubscription();
                          if (context.mounted) {
                            showSuccessSnackBar(context, l10n.subscriptionCancelled);
                          }
                        } catch (e) {
                          if (context.mounted) {
                            showErrorSnackBar(
                              context,
                              e is ApiException ? e.userMessage : l10n.errorCancelSubscription,
                            );
                          }
                        }
                      },
                      child: Text(l10n.cancelSubscription),
                    )
                  : null,
            ),
          ),
          const SizedBox(height: AppConstants.spacingMd),
        ],
        Text(l10n.availablePlans, style: Theme.of(context).textTheme.titleMedium),
        const SizedBox(height: AppConstants.spacingSm),
        ...state.plans.map(
          (plan) => Card(
            margin: const EdgeInsets.only(bottom: AppConstants.spacingSm),
            child: ListTile(
              title: Text(plan.name),
              subtitle: Text('${plan.description}\n${plan.currency} ${plan.priceAmount.toStringAsFixed(2)}'),
              isThreeLine: true,
              trailing: FilledButton(
                onPressed: () async {
                  try {
                    await notifier.subscribeToPlan(plan.id);
                    if (context.mounted) showSuccessSnackBar(context, l10n.subscriptionActivated);
                  } catch (e) {
                    if (context.mounted) {
                      showErrorSnackBar(
                        context,
                        e is ApiException ? e.userMessage : l10n.errorSubscribe,
                      );
                    }
                  }
                },
                child: Text(l10n.subscribe),
              ),
            ),
          ),
        ),
      ],
    );
  }
}
