class ConnectionUser {
  const ConnectionUser({
    required this.id,
    required this.displayName,
  });

  factory ConnectionUser.fromJson(Map<String, dynamic> json) {
    return ConnectionUser(
      id: json['id']?.toString() ?? '',
      displayName: json['displayName']?.toString() ?? 'Usuário',
    );
  }

  final String id;
  final String displayName;
}
