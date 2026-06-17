class AlertItem {
  const AlertItem({
    required this.id,
    required this.title,
    required this.description,
    required this.status,
    required this.createdAtUtc,
  });

  factory AlertItem.fromJson(Map<String, dynamic> json) {
    return AlertItem(
      id: json['id']?.toString() ?? '',
      title: json['title']?.toString() ?? 'Alerta',
      description: json['description']?.toString() ?? '',
      status: json['status']?.toString() ?? 'OPEN',
      createdAtUtc: DateTime.tryParse(json['createdAtUtc']?.toString() ?? '') ?? DateTime.now().toUtc(),
    );
  }

  final String id;
  final String title;
  final String description;
  final String status;
  final DateTime createdAtUtc;
}
