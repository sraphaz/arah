import '../../data/models/map_pin.dart';

/// Resolve a rota de detalhe (deep-link) para um pin do mapa.
/// Retorna `null` quando o tipo do pin não possui tela de detalhe navegável.
///
/// - `event` → `/events` (com `territoryId` quando disponível)
/// - `asset` → `/assets`
/// - `alert` → `/alerts`
/// - `post`  → `/post` (detalhe do post, quando há `postId` e `territoryId`); senão `/home`
String? mapPinDeepLink(MapPin pin, {String? territoryId}) {
  final hasTid = territoryId != null && territoryId.isNotEmpty;
  switch (pin.pinType.toLowerCase()) {
    case 'event':
      return hasTid ? '/events?territoryId=$territoryId' : '/events';
    case 'asset':
      return '/assets';
    case 'alert':
      return '/alerts';
    case 'post':
      final postId = pin.postId;
      if (hasTid && postId != null && postId.isNotEmpty) {
        return '/post?territoryId=$territoryId&postId=$postId';
      }
      return '/home';
    default:
      return null;
  }
}
