class WorkItem {
  const WorkItem({
    required this.id,
    required this.type,
    required this.status,
    required this.subjectType,
    required this.createdAtUtc,
  });

  factory WorkItem.fromJson(Map<String, dynamic> json) {
    return WorkItem(
      id: json['id']?.toString() ?? '',
      type: json['type']?.toString() ?? '',
      status: json['status']?.toString() ?? '',
      subjectType: json['subjectType']?.toString() ?? '',
      createdAtUtc: DateTime.tryParse(json['createdAtUtc']?.toString() ?? '') ?? DateTime.now().toUtc(),
    );
  }

  final String id;
  final String type;
  final String status;
  final String subjectType;
  final DateTime createdAtUtc;
}
