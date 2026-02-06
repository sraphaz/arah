import '../../../../core/network/api_exception.dart';
import '../../../../core/network/bff_client.dart';

/// Detalhe do território para exibir no mapa (perímetro: polígono ou círculo).
class TerritoryDetail {
  const TerritoryDetail({
    required this.id,
    required this.name,
    required this.latitude,
    required this.longitude,
    this.radiusKm,
    this.boundaryPolygon,
  });

  final String id;
  final String name;
  final double latitude;
  final double longitude;
  /// Raio em km; usado para desenhar círculo quando [boundaryPolygon] é null.
  final double? radiusKm;
  /// Polígono do perímetro (lista de pontos). Quando preenchido, desenha o contorno no mapa.
  final List<({double lat, double lng})>? boundaryPolygon;

  factory TerritoryDetail.fromJson(Map<String, dynamic> json) {
    final id = json['id'];
    final boundaryRaw = json['boundaryPolygon'] as List<dynamic>?;
    List<({double lat, double lng})>? boundaryPolygon;
    if (boundaryRaw != null && boundaryRaw.length >= 3) {
      boundaryPolygon = boundaryRaw
          .map((e) {
            final m = e as Map<String, dynamic>?;
            if (m == null) return null;
            final lat = (m['latitude'] as num?)?.toDouble();
            final lng = (m['longitude'] as num?)?.toDouble();
            if (lat == null || lng == null) return null;
            return (lat: lat, lng: lng);
          })
          .whereType<({double lat, double lng})>()
          .toList();
      if (boundaryPolygon.length < 3) boundaryPolygon = null;
    }
    return TerritoryDetail(
      id: id == null ? '' : id.toString(),
      name: json['name'] as String? ?? '',
      latitude: (json['latitude'] as num?)?.toDouble() ?? 0,
      longitude: (json['longitude'] as num?)?.toDouble() ?? 0,
      radiusKm: (json['radiusKm'] as num?)?.toDouble(),
      boundaryPolygon: boundaryPolygon,
    );
  }
}

/// Repositório da jornada de territórios (lista, detalhe, entrar como visitante).
class TerritoriesRepository {
  TerritoriesRepository({required this.client});

  final BffClient client;

  /// GET territories/{id} – detalhe do território (centro, raio, polígono) para o mapa.
  Future<TerritoryDetail?> getTerritoryById(String territoryId) async {
    if (territoryId.isEmpty) return null;
    final response = await client.get('territories', territoryId);
    final data = response.data as Map<String, dynamic>?;
    if (data == null) return null;
    return TerritoryDetail.fromJson(data);
  }

  /// POST territories/{territoryId}/enter – entra no território como visitante.
  /// Idempotente: se já for visitante, retorna sucesso.
  Future<void> enterTerritory(String territoryId) async {
    if (territoryId.isEmpty) throw ArgumentError('territoryId is required');
    final path = '$territoryId/enter';
    final response = await client.post('territories', path);
    if (response.statusCode < 200 || response.statusCode >= 300) {
      throw ApiException(
        'HTTP ${response.statusCode}',
        statusCode: response.statusCode,
        body: response.data?.toString(),
      );
    }
  }
}
