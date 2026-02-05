/// Pin do mapa (entidade, post, evento, asset, alerta). Alinhado à API map/pins.
class MapPin {
  const MapPin({
    required this.pinType,
    required this.latitude,
    required this.longitude,
    required this.title,
    this.entityId,
    this.postId,
    this.eventId,
    this.assetId,
    this.status,
  });

  final String pinType;
  final double latitude;
  final double longitude;
  final String title;
  final String? entityId;
  final String? postId;
  final String? eventId;
  final String? assetId;
  final String? status;

  factory MapPin.fromJson(Map<String, dynamic> json) {
    String? guidToString(dynamic v) =>
        v == null ? null : v is String ? v : v.toString();

    return MapPin(
      pinType: json['pinType'] as String? ?? 'entity',
      latitude: (json['latitude'] as num?)?.toDouble() ?? 0,
      longitude: (json['longitude'] as num?)?.toDouble() ?? 0,
      title: json['title'] as String? ?? '',
      entityId: guidToString(json['entityId']),
      postId: guidToString(json['postId']),
      eventId: guidToString(json['eventId']),
      assetId: guidToString(json['assetId']),
      status: json['status'] as String?,
    );
  }
}
