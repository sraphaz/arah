class MyStore {
  const MyStore({
    required this.id,
    required this.territoryId,
    required this.displayName,
    required this.status,
    this.description,
    this.paymentsEnabled = false,
  });

  factory MyStore.fromJson(Map<String, dynamic> json) {
    return MyStore(
      id: json['id']?.toString() ?? '',
      territoryId: json['territoryId']?.toString() ?? '',
      displayName: json['displayName']?.toString() ?? '',
      description: json['description']?.toString(),
      status: json['status']?.toString() ?? 'ACTIVE',
      paymentsEnabled: json['paymentsEnabled'] == true,
    );
  }

  final String id;
  final String territoryId;
  final String displayName;
  final String? description;
  final String status;
  final bool paymentsEnabled;
}
