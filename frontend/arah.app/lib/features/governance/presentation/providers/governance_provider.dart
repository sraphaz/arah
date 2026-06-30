import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/providers/app_providers.dart';
import '../../data/models/voting.dart';
import '../../data/repositories/governance_repository.dart';

final governanceRepositoryProvider = Provider<GovernanceRepository>((ref) {
  return GovernanceRepository(client: ref.watch(bffClientProvider));
});

/// Estado da lista de votações do território.
class GovernanceState {
  const GovernanceState({
    this.votings = const [],
    this.statusFilter,
    this.votedIds = const {},
    this.isLoading = false,
    this.error,
  });

  final List<Voting> votings;
  final String? statusFilter; // null = todas; Open | Closed
  final Set<String> votedIds;
  final bool isLoading;
  final Object? error;

  static const initial = GovernanceState();

  GovernanceState copyWith({
    List<Voting>? votings,
    String? statusFilter,
    bool clearStatusFilter = false,
    Set<String>? votedIds,
    bool? isLoading,
    Object? error,
    bool clearError = false,
  }) {
    return GovernanceState(
      votings: votings ?? this.votings,
      statusFilter:
          clearStatusFilter ? null : (statusFilter ?? this.statusFilter),
      votedIds: votedIds ?? this.votedIds,
      isLoading: isLoading ?? this.isLoading,
      error: clearError ? null : (error ?? this.error),
    );
  }
}

class GovernanceNotifier extends StateNotifier<GovernanceState> {
  GovernanceNotifier(this._ref, this.territoryId)
      : super(GovernanceState.initial) {
    if (territoryId != null && territoryId!.isNotEmpty) {
      load();
    }
  }

  final Ref _ref;
  final String? territoryId;

  GovernanceRepository get _repo => _ref.read(governanceRepositoryProvider);

  Future<void> load() async {
    final tid = territoryId ?? '';
    if (tid.isEmpty) return;

    state = state.copyWith(isLoading: true, clearError: true);
    try {
      final votings =
          await _repo.listVotings(tid, status: state.statusFilter);
      state = state.copyWith(votings: votings, isLoading: false);
    } catch (e) {
      state = state.copyWith(isLoading: false, error: e);
    }
  }

  Future<void> refresh() => load();

  Future<void> setStatusFilter(String? status) async {
    if (status == state.statusFilter) return;
    state = status == null
        ? state.copyWith(clearStatusFilter: true)
        : state.copyWith(statusFilter: status);
    await load();
  }

  /// Registra um voto e marca a votação como votada localmente.
  Future<void> vote(Voting voting, String option) async {
    final tid = territoryId ?? '';
    if (tid.isEmpty) return;
    await _repo.vote(
      territoryId: tid,
      votingId: voting.id,
      selectedOption: option,
    );
    state = state.copyWith(votedIds: {...state.votedIds, voting.id});
  }

  Future<VotingResults> results(Voting voting) {
    final tid = territoryId ?? '';
    return _repo.getResults(territoryId: tid, votingId: voting.id);
  }

  Future<void> closeVoting(Voting voting) async {
    final tid = territoryId ?? '';
    if (tid.isEmpty) return;
    await _repo.closeVoting(territoryId: tid, votingId: voting.id);
    await load();
  }

  Future<Voting> createVoting({
    required String type,
    required String title,
    required String description,
    required List<String> options,
    required String visibility,
    DateTime? startsAtUtc,
    DateTime? endsAtUtc,
  }) async {
    final tid = territoryId ?? '';
    final created = await _repo.createVoting(
      territoryId: tid,
      type: type,
      title: title,
      description: description,
      options: options,
      visibility: visibility,
      startsAtUtc: startsAtUtc,
      endsAtUtc: endsAtUtc,
    );
    await load();
    return created;
  }
}

final governanceProvider = StateNotifierProvider.autoDispose
    .family<GovernanceNotifier, GovernanceState, String?>((ref, territoryId) {
  return GovernanceNotifier(ref, territoryId);
});
