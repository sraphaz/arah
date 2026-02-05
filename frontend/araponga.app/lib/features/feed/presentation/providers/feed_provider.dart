import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/config/constants.dart';
import '../../../../core/network/bff_client.dart';
import '../../../../core/providers/app_providers.dart';
import '../../data/repositories/feed_repository.dart';

/// Estado do feed com paginação: itens carregados, página atual, se há mais.
class FeedState {
  const FeedState({
    this.items = const [],
    this.page = 1,
    this.hasMore = false,
    this.isLoading = false,
    this.error,
  });

  final List<dynamic> items;
  final int page;
  final bool hasMore;
  final bool isLoading;
  final Object? error;

  static const initial = FeedState();
}

/// Notifier do feed por território: carrega primeira página, loadMore e refresh.
class FeedNotifier extends StateNotifier<FeedState> {
  FeedNotifier(this._ref, this.territoryId) : super(FeedState.initial) {
    if (territoryId != null && territoryId!.isNotEmpty) {
      _loadPage(1, append: false);
    }
  }

  final Ref _ref;
  final String? territoryId;

  BffClient get _client => _ref.read(bffClientProvider);

  Future<void> _loadPage(int pageNumber, {required bool append}) async {
    final tid = territoryId ?? '';
    if (tid.isEmpty) return;
    state = FeedState(
      items: append ? state.items : [],
      page: state.page,
      hasMore: state.hasMore,
      isLoading: true,
      error: null,
    );
    try {
      final path = 'territory-feed?territoryId=$tid&pageNumber=$pageNumber&pageSize=${AppConstants.defaultPageSize}';
      final response = await _client.get('feed', path);
      final data = response.data as Map<String, dynamic>?;
      final list = data != null ? (data['items'] as List?) ?? [] : [];
      final hasMore = list.length >= AppConstants.defaultPageSize;
      state = FeedState(
        items: append ? [...state.items, ...list] : list,
        page: pageNumber,
        hasMore: hasMore,
        isLoading: false,
        error: null,
      );
    } catch (e) {
      state = FeedState(
        items: state.items,
        page: state.page,
        hasMore: state.hasMore,
        isLoading: false,
        error: e,
      );
    }
  }

  /// Puxar para atualizar: recarrega primeira página.
  Future<void> refresh() async {
    if (territoryId == null || territoryId!.isEmpty) return;
    await _loadPage(1, append: false);
  }

  /// Carregar próxima página (scroll infinito).
  Future<void> loadMore() async {
    if (state.isLoading || !state.hasMore) return;
    await _loadPage(state.page + 1, append: true);
  }
}

final feedNotifierProvider =
    StateNotifierProvider.autoDispose.family<FeedNotifier, FeedState, String?>((ref, territoryId) {
  return FeedNotifier(ref, territoryId);
});

final feedRepositoryProvider = Provider<FeedRepository>((ref) {
  return FeedRepository(client: ref.watch(bffClientProvider));
});

/// Provider legado (FutureProvider) para compatibilidade; preferir feedNotifierProvider.
@Deprecated('Use feedNotifierProvider(territoryId) para paginação e refresh')
final feedProvider = FutureProvider.autoDispose.family<FeedData, String?>((ref, territoryId) async {
  final client = ref.watch(bffClientProvider);
  final tid = territoryId ?? '';
  final path = 'territory-feed?territoryId=$tid&pageNumber=1&pageSize=${AppConstants.defaultPageSize}';
  final response = await client.get('feed', path);
  final data = response.data as Map<String, dynamic>?;
  final items = data != null ? (data['items'] as List?) ?? [] : [];
  return FeedData(items: items, hasMore: false);
});

class FeedData {
  const FeedData({required this.items, required this.hasMore});
  final List<dynamic> items;
  final bool hasMore;
}
