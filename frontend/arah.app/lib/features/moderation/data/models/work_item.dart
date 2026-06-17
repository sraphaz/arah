import 'dart:convert';

class WorkItem {
  const WorkItem({
    required this.id,
    required this.type,
    required this.status,
    required this.subjectType,
    required this.createdAtUtc,
    this.subjectId,
    this.payloadJson,
  });

  factory WorkItem.fromJson(Map<String, dynamic> json) {
    return WorkItem(
      id: json['id']?.toString() ?? '',
      type: json['type']?.toString() ?? '',
      status: json['status']?.toString() ?? '',
      subjectType: json['subjectType']?.toString() ?? '',
      createdAtUtc: DateTime.tryParse(json['createdAtUtc']?.toString() ?? '') ?? DateTime.now().toUtc(),
      subjectId: json['subjectId']?.toString(),
      payloadJson: json['payloadJson']?.toString(),
    );
  }

  final String id;
  final String type;
  final String status;
  final String subjectType;
  final DateTime createdAtUtc;
  final String? subjectId;
  final String? payloadJson;

  bool get isPending => status.toUpperCase() == 'PENDING';
  bool get isModerationCase => type.toUpperCase() == 'MODERATIONCASE';
  bool get isResidencyVerification => type.toUpperCase() == 'RESIDENCYVERIFICATION';

  String? get evidenceId {
    final raw = payloadJson;
    if (raw == null || raw.isEmpty) return null;
    try {
      final map = jsonDecode(raw) as Map<String, dynamic>;
      return map['evidenceId']?.toString();
    } catch (_) {
      return null;
    }
  }
}
