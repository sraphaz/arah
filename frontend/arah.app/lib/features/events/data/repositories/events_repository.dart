import '../../../../core/config/constants.dart';
import '../../../../core/network/api_exception.dart';
import '../../../../core/network/bff_client.dart';
import '../models/event_item.dart';

/// Repositório da jornada BFF events (territory-events).
class EventsRepository {
  EventsRepository({required BffClient client}) : _client = client;

  final BffClient _client;

  /// GET events/territory-events?territoryId=&pageNumber=&pageSize=
  Future<EventsPage> getTerritoryEvents({
    required String territoryId,
    int pageNumber = 1,
    int pageSize = AppConstants.defaultPageSize,
    DateTime? from,
    DateTime? to,
    String? status,
  }) async {
    var path =
        'territory-events?territoryId=$territoryId&pageNumber=$pageNumber&pageSize=$pageSize';
    if (from != null) path += '&from=${from.toIso8601String()}';
    if (to != null) path += '&to=${to.toIso8601String()}';
    if (status != null && status.isNotEmpty) path += '&status=$status';

    final response = await _client.get('events', path);
    final data = response.data as Map<String, dynamic>?;
    if (data == null) return EventsPage(items: [], hasMore: false, pageNumber: pageNumber);

    final itemsList = data['items'] as List? ?? [];
    final items = itemsList
        .map((e) => EventItem.fromJson(e as Map<String, dynamic>))
        .toList();
    final pagination = data['pagination'] as Map<String, dynamic>?;
    final totalPages = (pagination?['totalPages'] as num?)?.toInt() ?? 1;
    final hasMore = pageNumber < totalPages;

    return EventsPage(
      items: items,
      hasMore: hasMore,
      pageNumber: pageNumber,
    );
  }

  /// POST events/participate — status: INTERESTED | CONFIRMED
  Future<void> participate({required String eventId, required String status}) async {
    await _client.post(
      'events',
      'participate',
      body: {'eventId': eventId, 'status': status},
    );
  }

  /// POST events/create-event — cria um evento no território ativo.
  /// Datas em UTC (ISO 8601). Retorna o evento criado.
  Future<EventItem> createEvent({
    required String territoryId,
    required String title,
    String? description,
    required DateTime startsAtUtc,
    DateTime? endsAtUtc,
    double? latitude,
    double? longitude,
    String? locationLabel,
  }) async {
    final response = await _client.post(
      'events',
      'create-event',
      body: {
        'territoryId': territoryId,
        'title': title,
        if (description != null && description.isNotEmpty) 'description': description,
        'startsAtUtc': startsAtUtc.toUtc().toIso8601String(),
        if (endsAtUtc != null) 'endsAtUtc': endsAtUtc.toUtc().toIso8601String(),
        if (latitude != null) 'latitude': latitude,
        if (longitude != null) 'longitude': longitude,
        if (locationLabel != null && locationLabel.isNotEmpty) 'locationLabel': locationLabel,
      },
    );
    final data = response.data;
    if (data is Map<String, dynamic>) {
      return EventItem.fromJson(data);
    }
    throw ApiException('Resposta inválida ao criar evento.', statusCode: response.statusCode);
  }
}

class EventsPage {
  const EventsPage({
    required this.items,
    required this.hasMore,
    required this.pageNumber,
  });

  final List<EventItem> items;
  final bool hasMore;
  final int pageNumber;
}
