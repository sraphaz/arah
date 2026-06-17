class ChatConversationSummary {
  const ChatConversationSummary({
    required this.id,
    required this.name,
    required this.kind,
    required this.status,
  });

  factory ChatConversationSummary.fromJson(Map<String, dynamic> json) {
    return ChatConversationSummary(
      id: json['id']?.toString() ?? '',
      name: json['name']?.toString() ?? 'Conversa',
      kind: json['kind']?.toString() ?? 'CHANNEL',
      status: json['status']?.toString() ?? 'ACTIVE',
    );
  }

  final String id;
  final String name;
  final String kind;
  final String status;
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
