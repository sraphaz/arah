import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/providers/app_providers.dart';
import '../../../../core/providers/territory_provider.dart';
import '../../data/models/connection_item.dart';
import '../../data/models/connection_user.dart';
import '../../data/repositories/connections_repository.dart';

class ConnectionsState {
  const ConnectionsState({
    this.accepted = const [],
    this.pending = const [],
    this.isLoading = false,
    this.error,
  });

  final List<ConnectionItem> accepted;
  final List<ConnectionItem> pending;
  final bool isLoading;
  final Object? error;
}

class ConnectionsNotifier extends StateNotifier<ConnectionsState> {
  ConnectionsNotifier(this._ref) : super(const ConnectionsState()) {
    refresh();
  }

  final Ref _ref;

  ConnectionsRepository get _repository =>
      ConnectionsRepository(client: _ref.read(bffClientProvider));

  Future<void> refresh() async {
    state = ConnectionsState(
      accepted: state.accepted,
      pending: state.pending,
      isLoading: true,
      error: null,
    );
    try {
      final territoryId = _ref.read(selectedTerritoryIdValueProvider);
      final accepted = await _repository.listConnections(
        territoryId: territoryId,
        status: 'Accepted',
      );
      final pending = await _repository.listPending();
      state = ConnectionsState(accepted: accepted, pending: pending);
    } catch (e) {
      state = ConnectionsState(error: e);
    }
  }

  Future<void> accept(String connectionId) async {
    await _repository.accept(connectionId);
    await refresh();
  }

  Future<void> reject(String connectionId) async {
    await _repository.reject(connectionId);
    await refresh();
  }

  Future<void> remove(String connectionId) async {
    await _repository.remove(connectionId);
    await refresh();
  }

  Future<List<ConnectionUser>> searchUsers(String query) async {
    final territoryId = _ref.read(selectedTerritoryIdValueProvider);
    return _repository.searchUsers(query: query, territoryId: territoryId);
  }

  Future<List<ConnectionUser>> getSuggestions() async {
    final territoryId = _ref.read(selectedTerritoryIdValueProvider);
    return _repository.getSuggestions(territoryId: territoryId);
  }

  Future<void> sendRequest(String targetUserId) async {
    final territoryId = _ref.read(selectedTerritoryIdValueProvider);
    await _repository.sendRequest(targetUserId: targetUserId, territoryId: territoryId);
    await refresh();
  }
}

final connectionsProvider =
    StateNotifierProvider.autoDispose<ConnectionsNotifier, ConnectionsState>((ref) {
  return ConnectionsNotifier(ref);
});
