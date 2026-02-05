import '../../../../core/config/constants.dart';
import '../../../../core/network/bff_client.dart';
import '../models/notification_item.dart';

/// Repositório da jornada BFF notifications (lista paginada, marcar como lida).
class NotificationsRepository {
  NotificationsRepository({required BffClient client}) : _client = client;

  final BffClient _client;

  /// GET notifications/paged?pageNumber=&pageSize=
  Future<NotificationsPage> getPaged({int pageNumber = 1, int pageSize = AppConstants.defaultPageSize}) async {
    final path = 'paged?pageNumber=$pageNumber&pageSize=$pageSize';
    final response = await _client.get('notifications', path);
    final data = response.data as Map<String, dynamic>?;
    if (data == null) return NotificationsPage(items: [], hasMore: false, pageNumber: pageNumber);

    final itemsList = data['items'] as List? ?? [];
    final items = itemsList
        .map((e) => NotificationItem.fromJson(e as Map<String, dynamic>))
        .toList();
    final totalCount = (data['totalCount'] as num?)?.toInt() ?? 0;
    final totalPages = (data['totalPages'] as num?)?.toInt() ?? 1;
    final hasMore = pageNumber < totalPages;

    return NotificationsPage(
      items: items,
      hasMore: hasMore,
      pageNumber: pageNumber,
      totalCount: totalCount,
    );
  }

  /// POST notifications/{id}/read
  Future<void> markAsRead(String id) async {
    await _client.post('notifications', '$id/read');
  }
}

class NotificationsPage {
  const NotificationsPage({
    required this.items,
    required this.hasMore,
    required this.pageNumber,
    this.totalCount,
  });

  final List<NotificationItem> items;
  final bool hasMore;
  final int pageNumber;
  final int? totalCount;
}
