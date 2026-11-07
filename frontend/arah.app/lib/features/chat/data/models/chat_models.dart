class ChatConversationSummary {
  const ChatConversationSummary({
    required this.id,
    required this.name,
    required this.kind,
    required this.status,
    this.unreadCount = 0,
  });

  factory ChatConversationSummary.fromJson(Map<String, dynamic> json) {
    return ChatConversationSummary(
      id: json['id']?.toString() ?? '',
      name: json['name']?.toString() ?? 'Conversa',
      kind: json['kind']?.toString() ?? 'CHANNEL',
      status: json['status']?.toString() ?? 'ACTIVE',
      unreadCount: (json['unreadCount'] as num?)?.toInt() ??
          (json['unread'] as num?)?.toInt() ??
          int.tryParse(json['unread']?.toString() ?? '') ??
          0,
    );
  }

  final String id;
  final String name;
  final String kind;
  final String status;
  final int unreadCount;
}

class ChatMessage {
  const ChatMessage({
    required this.id,
    required this.text,
    required this.senderUserId,
    required this.createdAtUtc,
  });

  factory ChatMessage.fromJson(Map<String, dynamic> json) {
    return ChatMessage(
      id: json['id']?.toString() ?? '',
      text: json['text']?.toString() ?? '',
      senderUserId: json['senderUserId']?.toString() ?? '',
      createdAtUtc: DateTime.tryParse(json['createdAtUtc']?.toString() ?? '') ?? DateTime.now().toUtc(),
    );
  }

  final String id;
  final String text;
  final String senderUserId;
  final DateTime createdAtUtc;
}
