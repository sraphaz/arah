import '../../../../core/network/api_exception.dart';
import '../../../../core/network/bff_client.dart';

/// Repositório da jornada de territórios (lista, entrar como visitante).
class TerritoriesRepository {
  TerritoriesRepository({required this.client});

  final BffClient client;

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
