/// Constantes e design tokens (docs/26_FLUTTER_DESIGN_GUIDELINES).
class AppConstants {
  AppConstants._();

  static const double spacingXs = 4.0;
  static const double spacingSm = 8.0;
  static const double spacingMd = 16.0;
  static const double spacingLg = 24.0;
  static const double spacingXl = 32.0;
  static const double radiusSm = 8.0;
  static const double radiusMd = 12.0;
  static const double radiusLg = 16.0;

  /// Tamanhos de ícone (empty states, botões, listas).
  static const double iconSizeSm = 14.0;
  static const double iconSizeMd = 24.0;
  static const double iconSizeLg = 48.0;

  /// Indicador de carregamento dentro de botão.
  static const double loadingIndicatorSize = 20.0;

  /// Avatar pequeno (lista) e grande (cabeçalho/post). Raio do avatar de perfil (cabeçalho).
  static const double avatarSizeSm = 40.0;
  static const double avatarSizeLg = 64.0;
  static const double avatarRadiusProfile = 48.0;

  static const int animationFast = 150;
  static const int animationNormal = 250;
  static const double minTouchTargetSize = 44.0;
  static const int defaultPageSize = 20;
  static const int maxPageSize = 50;
  static const String keyAccessToken = 'araponga_access_token';
  static const String keyRefreshToken = 'araponga_refresh_token';
  static const String keyTokenExpiry = 'araponga_token_expiry';
  static const String keySelectedTerritoryId = 'araponga_selected_territory_id';
}
