import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/providers/app_providers.dart';
import '../../../../core/providers/territory_provider.dart';
import '../../data/models/work_item.dart';
import '../../data/repositories/moderation_repository.dart';

class ModerationState {
  const ModerationState({this.items = const [], this.isLoading = false, this.error});

  final List<WorkItem> items;
  final bool isLoading;
  final Object? error;
}

class ModerationNotifier extends StateNotifier<ModerationState> {
  ModerationNotifier(this._ref) : super(const ModerationState()) {
    refresh();
  }

  final Ref _ref;

  ModerationRepository get _repo => ModerationRepository(client: _ref.read(bffClientProvider));

  Future<void> refresh() async {
    final territoryId = _ref.read(selectedTerritoryIdValueProvider);
    if (territoryId == null || territoryId.isEmpty) {
      state = const ModerationState();
      return;
    }
    state = ModerationState(items: state.items, isLoading: true);
    try {
      final items = await _repo.listWorkItems(territoryId);
      state = ModerationState(items: items);
    } catch (e) {
      state = ModerationState(error: e);
    }
  }
}

final moderationProvider =
    StateNotifierProvider.autoDispose<ModerationNotifier, ModerationState>((ref) {
  return ModerationNotifier(ref);
});
