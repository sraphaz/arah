import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/providers/app_providers.dart';
import '../../../../core/providers/territory_provider.dart';
import '../../data/models/alert_item.dart';
import '../../data/repositories/alerts_repository.dart';

class AlertsState {
  const AlertsState({
    this.items = const [],
    this.isLoading = false,
    this.error,
  });

  final List<AlertItem> items;
  final bool isLoading;
  final Object? error;
}

class AlertsNotifier extends StateNotifier<AlertsState> {
  AlertsNotifier(this._ref) : super(const AlertsState()) {
    refresh();
  }

  final Ref _ref;

  AlertsRepository get _repository => AlertsRepository(client: _ref.read(bffClientProvider));

  Future<void> refresh() async {
    final territoryId = _ref.read(selectedTerritoryIdValueProvider);
    if (territoryId == null || territoryId.isEmpty) {
      state = const AlertsState();
      return;
    }

    state = AlertsState(items: state.items, isLoading: true, error: null);
    try {
      final items = await _repository.listAlerts(territoryId: territoryId);
      state = AlertsState(items: items);
    } catch (e) {
      state = AlertsState(error: e);
    }
  }
}

final alertsProvider = StateNotifierProvider.autoDispose<AlertsNotifier, AlertsState>((ref) {
  return AlertsNotifier(ref);
});

final alertsRepositoryProvider = Provider<AlertsRepository>((ref) {
  return AlertsRepository(client: ref.watch(bffClientProvider));
});
