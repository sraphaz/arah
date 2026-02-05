import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:geolocator/geolocator.dart';

/// Par de coordenadas (graus decimais) para enviar ao backend (X-Geo-Latitude, X-Geo-Longitude).
class GeoPosition {
  const GeoPosition({required this.latitude, required this.longitude});
  final double latitude;
  final double longitude;
}

/// Estado da última posição conhecida. Null até que [GeoLocationNotifier.fetch] seja chamado com sucesso.
/// Para convergência com território: o app deve chamar fetch (ex.: ao abrir o shell ou a tela de feed).
final geoLocationStateProvider =
    StateNotifierProvider<GeoLocationNotifier, GeoPosition?>((ref) {
  return GeoLocationNotifier();
});

class GeoLocationNotifier extends StateNotifier<GeoPosition?> {
  GeoLocationNotifier() : super(null);

  /// Obtém a posição atual e atualiza o estado. Não faz nada se permissão for negada.
  /// Chamar ao iniciar o app ou ao entrar na área do feed/território.
  Future<void> fetch() async {
    final serviceEnabled = await Geolocator.isLocationServiceEnabled();
    if (!serviceEnabled) {
      return;
    }

    var permission = await Geolocator.checkPermission();
    if (permission == LocationPermission.denied) {
      permission = await Geolocator.requestPermission();
    }
    if (permission == LocationPermission.denied ||
        permission == LocationPermission.deniedForever) {
      return;
    }

    try {
      final position = await Geolocator.getCurrentPosition(
        locationSettings: const LocationSettings(
          accuracy: LocationAccuracy.medium,
          timeLimit: Duration(seconds: 10),
        ),
      );
      state = GeoPosition(
        latitude: position.latitude,
        longitude: position.longitude,
      );
    } catch (_) {
      // Manter estado anterior ou null; não bloquear o app
    }
  }

  /// Limpa a posição (ex.: usuário desativou localização).
  void clear() {
    state = null;
  }
}
