/// Item de notificação in-app. Alinhado à API notifications (NotificationResponse).
class NotificationItem {
  const NotificationItem({
    required this.id,
    required this.title,
    this.body,
    required this.kind,
    this.dataJson,
    required this.createdAtUtc,
    this.readAtUtc,
  });

  final String id;
  final String title;
  final String? body;
  final String kind;
  final String? dataJson;
  final DateTime createdAtUtc;
  final DateTime? readAtUtc;

  bool get isRead => readAtUtc != null;

  factory NotificationItem.fromJson(Map<String, dynamic> json) {
    final id = json['id'];
    final createdAt = json['createdAtUtc'];
    final readAt = json['readAtUtc'];
    return NotificationItem(
      id: id == null ? '' : id.toString(),
      title: json['title'] as String? ?? '',
      body: json['body'] as String?,
      kind: json['kind'] as String? ?? 'info',
      dataJson: json['dataJson'] as String?,
      createdAtUtc: createdAt != null
          ? DateTime.tryParse(createdAt.toString()) ?? DateTime.now()
          : DateTime.now(),
      readAtUtc: readAt != null ? DateTime.tryParse(readAt.toString()) : null,
    );
  }
}
