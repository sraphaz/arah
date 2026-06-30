import '../../data/models/map_pin.dart';

/// Resolve a rota de detalhe (deep-link) para um pin do mapa.
/// Retorna `null` quando o tipo do pin não possui tela de detalhe navegável.
///
/// - `event` → `/events` (com `territoryId` quando disponível)
/// - `asset` → `/assets`
/// - `alert` → `/alerts`
/// - `post`  → `/home` (feed; não há rota de detalhe de post)
String? mapPinDeepLink(MapPin pin, {String? territoryId}) {
  switch (pin.pinType.toLowerCase()) {
    case 'event':
      final hasTid = territoryId != null && territoryId.isNotEmpty;
      return hasTid ? '/events?territoryId=$territoryId' : '/events';
    case 'asset':
      return '/assets';
    case 'alert':
      return '/alerts';
    case 'post':
      return '/home';
    default:
      return null;
  }
}
