import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/config/constants.dart';
import '../../../../core/providers/app_providers.dart';
import '../../data/models/notification_item.dart';
import '../../data/repositories/notifications_repository.dart';

final notificationsRepositoryProvider = Provider<NotificationsRepository>((ref) {
  return NotificationsRepository(client: ref.watch(bffClientProvider));
});

/// Estado da lista de notificações: itens, página, hasMore, loading, error.
class NotificationsState {
  const NotificationsState({
    this.items = const [],
    this.page = 1,
    this.hasMore = false,
    this.isLoading = false,
    this.error,
  });

  final List<NotificationItem> items;
  final int page;
  final bool hasMore;
  final bool isLoading;
  final Object? error;

  static const initial = NotificationsState();
}

class NotificationsNotifier extends StateNotifier<NotificationsState> {
  NotificationsNotifier(this._ref) : super(NotificationsState.initial) {
    loadPage(1, append: false);
  }

  final Ref _ref;

  NotificationsRepository get _repo => _ref.read(notificationsRepositoryProvider);

  Future<void> loadPage(int pageNumber, {required bool append}) async {
    state = NotificationsState(
      items: append ? state.items : [],
      page: state.page,
      hasMore: state.hasMore,
      isLoading: true,
      error: null,
    );
    try {
      final page = await _repo.getPaged(
        pageNumber: pageNumber,
        pageSize: AppConstants.defaultPageSize,
      );
      state = NotificationsState(
        items: append ? [...state.items, ...page.items] : page.items,
        page: pageNumber,
        hasMore: page.hasMore,
        isLoading: false,
        error: null,
      );
    } catch (e) {
      state = NotificationsState(
        items: state.items,
        page: state.page,
        hasMore: state.hasMore,
        isLoading: false,
        error: e,
      );
    }
  }

  Future<void> refresh() async {
    await loadPage(1, append: false);
  }

  Future<void> loadMore() async {
    if (state.isLoading || !state.hasMore) return;
    await loadPage(state.page + 1, append: true);
  }

  Future<void> markAsRead(NotificationItem item) async {
    if (item.isRead) return;
    try {
      await _repo.markAsRead(item.id);
      state = NotificationsState(
        items: state.items
            .map((n) => n.id == item.id
                ? NotificationItem(
                    id: n.id,
                    title: n.title,
                    body: n.body,
                    kind: n.kind,
                    dataJson: n.dataJson,
                    createdAtUtc: n.createdAtUtc,
                    readAtUtc: DateTime.now(),
                  )
                : n)
            .toList(),
        page: state.page,
        hasMore: state.hasMore,
        isLoading: state.isLoading,
        error: state.error,
      );
    } catch (_) {
      // keep state; optional: show snackbar
    }
  }
}

final notificationsProvider =
    StateNotifierProvider.autoDispose<NotificationsNotifier, NotificationsState>((ref) {
  return NotificationsNotifier(ref);
});
