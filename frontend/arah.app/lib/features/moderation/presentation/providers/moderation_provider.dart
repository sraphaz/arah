import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/providers/app_providers.dart';
import '../../../../core/providers/territory_provider.dart';
import '../../data/models/work_item.dart';
import '../../data/repositories/moderation_repository.dart';

enum ModerationTab { queue, cases, evidences }

class ModerationState {
  const ModerationState({
    this.items = const [],
    this.isLoading = false,
    this.error,
    this.tab = ModerationTab.queue,
  });

  final List<WorkItem> items;
  final bool isLoading;
  final Object? error;
  final ModerationTab tab;

  ModerationState copyWith({
    List<WorkItem>? items,
    bool? isLoading,
    Object? error,
    ModerationTab? tab,
  }) {
    return ModerationState(
      items: items ?? this.items,
      isLoading: isLoading ?? this.isLoading,
      error: error,
      tab: tab ?? this.tab,
    );
  }
}

class ModerationNotifier extends StateNotifier<ModerationState> {
  ModerationNotifier(this._ref) : super(const ModerationState()) {
    refresh();
  }

  final Ref _ref;

  ModerationRepository get _repo => ModerationRepository(client: _ref.read(bffClientProvider));

  void setTab(ModerationTab tab) {
    state = state.copyWith(tab: tab);
    refresh();
  }

  Future<void> refresh() async {
    final territoryId = _ref.read(selectedTerritoryIdValueProvider);
    if (territoryId == null || territoryId.isEmpty) {
      state = const ModerationState();
      return;
    }
    state = state.copyWith(items: state.items, isLoading: true, error: null);
    try {
      final type = switch (state.tab) {
        ModerationTab.queue => null,
        ModerationTab.cases => 'MODERATIONCASE',
        ModerationTab.evidences => 'RESIDENCYVERIFICATION',
      };
      final items = await _repo.listWorkItems(
        territoryId,
        type: type,
        status: state.tab == ModerationTab.queue ? null : 'PENDING',
      );
      state = state.copyWith(items: items, isLoading: false);
    } catch (e) {
      state = state.copyWith(error: e, isLoading: false);
    }
  }

  Future<void> decideItem(WorkItem item, String outcome, {String? notes}) async {
    final territoryId = _requireTerritoryId();
    if (item.isModerationCase) {
      await _repo.decideModerationCase(
        territoryId: territoryId,
        workItemId: item.id,
        outcome: outcome,
        notes: notes,
      );
    } else if (item.isResidencyVerification) {
      await _repo.decideResidencyVerification(
        territoryId: territoryId,
        workItemId: item.id,
        outcome: outcome,
        notes: notes,
      );
    }
    await refresh();
  }

  Future<int> downloadEvidence(WorkItem item) async {
    final territoryId = _requireTerritoryId();
    final evidenceId = item.evidenceId;
    if (evidenceId == null || evidenceId.isEmpty) {
      throw StateError('Evidência não encontrada no item.');
    }
    final bytes = await _repo.downloadEvidence(territoryId: territoryId, evidenceId: evidenceId);
    return bytes.length;
  }

  String _requireTerritoryId() {
    final territoryId = _ref.read(selectedTerritoryIdValueProvider);
    if (territoryId == null || territoryId.isEmpty) {
      throw StateError('Território não selecionado');
    }
    return territoryId;
  }
}

final moderationProvider =
    StateNotifierProvider.autoDispose<ModerationNotifier, ModerationState>((ref) {
  return ModerationNotifier(ref);
});
