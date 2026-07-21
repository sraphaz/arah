class MembershipInfo {
  const MembershipInfo({
    required this.id,
    required this.userId,
    required this.territoryId,
    required this.role,
    this.residencyVerification,
  });

  factory MembershipInfo.fromJson(Map<String, dynamic> json) {
    return MembershipInfo(
      id: json['id']?.toString() ?? '',
      userId: json['userId']?.toString() ?? '',
      territoryId: json['territoryId']?.toString() ?? '',
      role: json['role']?.toString() ?? 'VISITOR',
      residencyVerification: json['residencyVerification']?.toString(),
    );
  }

  final String id;
  final String userId;
  final String territoryId;
  final String role;
  final String? residencyVerification;

  bool get isResident => role.toUpperCase() == 'RESIDENT';

  bool get isVisitor {
    final r = role.toUpperCase();
    return r == 'VISITOR' || r == 'VISITANTE' || r.isEmpty;
  }
}
