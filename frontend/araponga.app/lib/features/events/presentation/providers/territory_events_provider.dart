import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/config/constants.dart';
import '../../../../core/providers/app_providers.dart';
import '../../data/models/event_item.dart';
import '../../data/repositories/events_repository.dart';

final eventsRepositoryProvider = Provider<EventsRepository>((ref) {
  return EventsRepository(client: ref.watch(bffClientProvider));
});

/// Estado da lista de eventos: itens, página, hasMore, loading, error.
class TerritoryEventsState {
  const TerritoryEventsState({
    this.items = const [],
    this.page = 1,
    this.hasMore = false,
    this.isLoading = false,
    this.error,
  });

  final List<EventItem> items;
  final int page;
  final bool hasMore;
  final bool isLoading;
  final Object? error;

  static const initial = TerritoryEventsState();
}

class TerritoryEventsNotifier extends StateNotifier<TerritoryEventsState> {
  TerritoryEventsNotifier(this._ref, this.territoryId) : super(TerritoryEventsState.initial) {
    if (territoryId != null && territoryId!.isNotEmpty) {
      loadPage(1, append: false);
    }
  }

  final Ref _ref;
  final String? territoryId;

  EventsRepository get _repo => _ref.read(eventsRepositoryProvider);

  Future<void> loadPage(int pageNumber, {required bool append}) async {
    final tid = territoryId ?? '';
    if (tid.isEmpty) return;

    state = TerritoryEventsState(
      items: append ? state.items : [],
      page: state.page,
      hasMore: state.hasMore,
      isLoading: true,
      error: null,
    );
    try {
      final page = await _repo.getTerritoryEvents(
        territoryId: tid,
        pageNumber: pageNumber,
        pageSize: AppConstants.defaultPageSize,
      );
      state = TerritoryEventsState(
        items: append ? [...state.items, ...page.items] : page.items,
        page: pageNumber,
        hasMore: page.hasMore,
        isLoading: false,
        error: null,
      );
    } catch (e) {
      state = TerritoryEventsState(
        items: state.items,
        page: state.page,
        hasMore: state.hasMore,
        isLoading: false,
        error: e,
      );
    }
  }

  Future<void> refresh() async {
    if (territoryId == null || territoryId!.isEmpty) return;
    await loadPage(1, append: false);
  }

  Future<void> loadMore() async {
    if (state.isLoading || !state.hasMore) return;
    await loadPage(state.page + 1, append: true);
  }

  /// Marca participação (INTERESTED ou CONFIRMED) e atualiza o item na lista.
  Future<void> participate(EventItem item, String status) async {
    if (territoryId == null || territoryId!.isEmpty) return;
    try {
      await _repo.participate(eventId: item.id, status: status);
      state = TerritoryEventsState(
        items: state.items
            .map((e) => e.id == item.id
                ? EventItem(
                    id: e.id,
                    territoryId: e.territoryId,
                    title: e.title,
                    description: e.description,
                    startsAtUtc: e.startsAtUtc,
                    endsAtUtc: e.endsAtUtc,
                    latitude: e.latitude,
                    longitude: e.longitude,
                    locationLabel: e.locationLabel,
                    status: e.status,
                    createdByDisplayName: e.createdByDisplayName,
                    participants: e.participants,
                    userParticipationStatus: status,
                  )
                : e)
            .toList(),
        page: state.page,
        hasMore: state.hasMore,
        isLoading: state.isLoading,
        error: state.error,
      );
    } catch (_) {
      rethrow;
    }
  }
}

final territoryEventsProvider = StateNotifierProvider.autoDispose
    .family<TerritoryEventsNotifier, TerritoryEventsState, String?>((ref, territoryId) {
  return TerritoryEventsNotifier(ref, territoryId);
});
