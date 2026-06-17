class ConnectionItem {
  const ConnectionItem({
    required this.id,
    required this.requesterUserId,
    required this.targetUserId,
    required this.status,
    this.territoryId,
    this.direction,
  });

  factory ConnectionItem.fromJson(Map<String, dynamic> json) {
    return ConnectionItem(
      id: json['id']?.toString() ?? '',
      requesterUserId: json['requesterUserId']?.toString() ?? '',
      targetUserId: json['targetUserId']?.toString() ?? '',
      status: json['status']?.toString() ?? 'Pending',
      territoryId: json['territoryId']?.toString(),
      direction: json['direction']?.toString(),
    );
  }

  final String id;
  final String requesterUserId;
  final String targetUserId;
  final String status;
  final String? territoryId;
  final String? direction;

  bool get isPending => status.toLowerCase() == 'pending';
  bool get isIncoming => direction?.toLowerCase() == 'incoming';
}
