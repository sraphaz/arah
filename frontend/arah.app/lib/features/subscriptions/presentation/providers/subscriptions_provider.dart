import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/providers/app_providers.dart';
import '../../../../core/providers/territory_provider.dart';
import '../../data/models/subscription_models.dart';
import '../../data/repositories/subscriptions_repository.dart';

class SubscriptionsState {
  const SubscriptionsState({
    this.plans = const [],
    this.mySubscription,
    this.isLoading = false,
    this.error,
  });

  final List<SubscriptionPlan> plans;
  final MySubscription? mySubscription;
  final bool isLoading;
  final Object? error;
}

class SubscriptionsNotifier extends StateNotifier<SubscriptionsState> {
  SubscriptionsNotifier(this._ref) : super(const SubscriptionsState()) {
    refresh();
  }

  final Ref _ref;

  SubscriptionsRepository get _repo => SubscriptionsRepository(client: _ref.read(bffClientProvider));

  Future<void> refresh() async {
    state = SubscriptionsState(isLoading: true);
    try {
      final territoryId = _ref.read(selectedTerritoryIdValueProvider);
      final plans = await _repo.listPlans(territoryId: territoryId);
      MySubscription? mine;
      try {
        mine = await _repo.getMySubscription(territoryId: territoryId);
      } catch (_) {}
      state = SubscriptionsState(plans: plans, mySubscription: mine);
    } catch (e) {
      state = SubscriptionsState(error: e);
    }
  }

  Future<void> subscribeToPlan(String planId) async {
    final territoryId = _ref.read(selectedTerritoryIdValueProvider);
    await _repo.subscribe(planId: planId, territoryId: territoryId);
    await refresh();
  }

  Future<void> cancelMySubscription({bool cancelAtPeriodEnd = true}) async {
    final subscriptionId = state.mySubscription?.id;
    if (subscriptionId == null || subscriptionId.isEmpty) return;
    await _repo.cancelSubscription(
      subscriptionId: subscriptionId,
      cancelAtPeriodEnd: cancelAtPeriodEnd,
    );
    await refresh();
  }
}

final subscriptionsProvider =
    StateNotifierProvider.autoDispose<SubscriptionsNotifier, SubscriptionsState>((ref) {
  return SubscriptionsNotifier(ref);
});
