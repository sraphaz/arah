import 'package:flutter/material.dart';

import '../config/constants.dart';

/// Tokens alinhados a [frontend/shared/styles/design-tokens.css].
class AppDesignTokens {
  AppDesignTokens._();

  // ---------------------------------------------------------------------------
  // Paleta – dark mode (padrão do app)
  // ---------------------------------------------------------------------------
  static const Color primary = Color(0xFF4DD4A8);
  static const Color primaryHover = Color(0xFF3BC495);
  static const Color link = Color(0xFF7DD3FF);
  static const Color linkHover = Color(0xFF9DE3FF);

  static const Color surface = Color(0xFF0A0E12);
  static const Color surfaceElevated = Color(0xFF0F1419);
  static const Color surfaceCard = Color(0xFF141A21);
  static const Color onSurface = Color(0xFFE8EDF2);
  static const Color onSurfaceMuted = Color(0xFFB8C5D2);
  static const Color onSurfaceSubtle = Color(0xFF8A97A4);
  static const Color outline = Color(0xFF25303A);
  static const Color outlineSubtle = Color(0x1A6496B4);

  static const Color error = Color(0xFFF26D6D);
  static const Color warning = Color(0xFFF5C842);
  static const Color success = Color(0xFF4DD4A8);
  static const Color info = Color(0xFF7DD3FF);
  static const Color earth = Color(0xFF8B7355);
  static const Color textOnAccent = Color(0xFF0A0E12);

  static const Color accentSubtle = Color(0x264DD4A8);
  static const Color accentBorderSoft = Color(0x404DD4A8);
  static const Color territoryBoundary = Color(0xFF16A34A);
  static const Color locationPin = Color(0xFFF5C842);

  // Glass morphism (espelho de globals.css)
  static const Color glassBackgroundDark = Color(0xFA141A21);
  static const Color glassBorderDark = Color(0x9930393A);
  static const Color glassBackgroundLight = Color(0xFAFFFFFF);
  static const Color glassBorderLight = Color(0x66C6E3D2);
  static const double glassBlur = 24;
  static const double glassRadius = 24;

  // Watermark
  static const double watermarkOpacityDark = 0.015;
  static const double watermarkOpacityLight = 0.035;
  static const double bodyWatermarkOpacityDark = 0.01;
  static const double bodyWatermarkOpacityLight = 0.025;
  static const double bodyWatermarkSize = 720;
  static const double cardWatermarkSize = 560;

  // Light mode
  static const Color primaryLight = Color(0xFF3BC495);
  static const Color surfaceLight = Color(0xFFFAFAFA);
  static const Color surfaceCardLight = Color(0xFFF5F5F5);
  static const Color onSurfaceLight = Color(0xFF1C1C1C);
  static const Color onSurfaceMutedLight = Color(0xFF5C5C5C);
  static const Color outlineLight = Color(0xFFE0E0E0);

  // Feed – cores semânticas por tipo de post
  static const Color feedTypeAlert = warning;
  static const Color feedTypeEvent = link;
  static const Color feedTypeTip = primary;
  static const Color feedTypeGeneral = Color(0xFF9CA3AF);

  // Tipografia – escala 1.125 (CSS)
  static const double fontSizeXs = 12;
  static const double fontSizeSm = 14;
  static const double fontSizeBase = 16;
  static const double fontSizeLg = 18;
  static const double fontSizeXl = 20;
  static const double fontSize2xl = 24;
  static const double fontSize3xl = 30;
  static const double fontSize4xl = 36;

  static const double letterSpacingTight = -0.5;
  static const double letterSpacingWide = 0.5;

  static const double feedCardTypeBorderWidth = 4;
  static const EdgeInsets cardPadding = EdgeInsets.all(AppConstants.spacingMd);
  static const double fontSizeSnackBar = 15;

  static double get radiusCard => AppConstants.radiusLg;
  static double get radiusSnackBar => AppConstants.radiusMd;
  static double get radiusButton => AppConstants.radiusMd;
  static EdgeInsets get snackBarInsets =>
      const EdgeInsets.symmetric(horizontal: AppConstants.spacingMd);

  /// Gradiente sutil de fundo (equivalente a --bg-gradient-accent).
  static const List<Color> scaffoldGradientColors = [
    Color(0x084DD4A8),
    Color(0x000A0E12),
    Color(0x087DD3FF),
    Color(0x000A0E12),
  ];

  /// Cor semântica para pins no mapa por tipo.
  static Color pinColorForType(String type) {
    switch (type.toLowerCase()) {
      case 'alert':
        return warning;
      case 'event':
        return link;
      case 'post':
        return primary;
      case 'asset':
      case 'media':
        return earth;
      case 'entity':
        return territoryBoundary;
      default:
        return primary;
    }
  }

  /// Elevação Material (espelho de --elevation-*).
  static List<BoxShadow> elevation(int level, {bool isDark = true}) {
    final opacity = isDark ? 0.25 : 0.08;
    switch (level) {
      case 1:
        return [
          BoxShadow(
            color: Colors.black.withValues(alpha: opacity * 0.6),
            blurRadius: 2,
            offset: const Offset(0, 1),
          ),
        ];
      case 2:
        return [
          BoxShadow(
            color: Colors.black.withValues(alpha: opacity),
            blurRadius: 8,
            offset: const Offset(0, 2),
          ),
        ];
      case 3:
        return [
          BoxShadow(
            color: Colors.black.withValues(alpha: opacity * 1.4),
            blurRadius: 16,
            offset: const Offset(0, 4),
          ),
        ];
      default:
        return [
          BoxShadow(
            color: Colors.black.withValues(alpha: opacity * 1.8),
            blurRadius: 32,
            offset: const Offset(0, 8),
          ),
        ];
    }
  }
}

/// Extensão do tema para cores semânticas do Arah.
class AppColors extends ThemeExtension<AppColors> {
  const AppColors({
    required this.primary,
    required this.link,
    required this.surface,
    required this.surfaceElevated,
    required this.surfaceContainer,
    required this.onSurface,
    required this.onSurfaceVariant,
    required this.onSurfaceSubtle,
    required this.outline,
    required this.outlineSubtle,
    required this.error,
    required this.success,
    required this.info,
    required this.earth,
    required this.accentSubtle,
    required this.accentBorder,
    required this.glassBackground,
    required this.glassBorder,
    required this.watermarkOpacity,
    required this.bodyWatermarkOpacity,
    required this.territoryBoundary,
    required this.locationPin,
  });

  final Color primary;
  final Color link;
  final Color surface;
  final Color surfaceElevated;
  final Color surfaceContainer;
  final Color onSurface;
  final Color onSurfaceVariant;
  final Color onSurfaceSubtle;
  final Color outline;
  final Color outlineSubtle;
  final Color error;
  final Color success;
  final Color info;
  final Color earth;
  final Color accentSubtle;
  final Color accentBorder;
  final Color glassBackground;
  final Color glassBorder;
  final double watermarkOpacity;
  final double bodyWatermarkOpacity;
  final Color territoryBoundary;
  final Color locationPin;

  @override
  ThemeExtension<AppColors> copyWith({
    Color? primary,
    Color? link,
    Color? surface,
    Color? surfaceElevated,
    Color? surfaceContainer,
    Color? onSurface,
    Color? onSurfaceVariant,
    Color? onSurfaceSubtle,
    Color? outline,
    Color? outlineSubtle,
    Color? error,
    Color? success,
    Color? info,
    Color? earth,
    Color? accentSubtle,
    Color? accentBorder,
    Color? glassBackground,
    Color? glassBorder,
    double? watermarkOpacity,
    double? bodyWatermarkOpacity,
    Color? territoryBoundary,
    Color? locationPin,
  }) {
    return AppColors(
      primary: primary ?? this.primary,
      link: link ?? this.link,
      surface: surface ?? this.surface,
      surfaceElevated: surfaceElevated ?? this.surfaceElevated,
      surfaceContainer: surfaceContainer ?? this.surfaceContainer,
      onSurface: onSurface ?? this.onSurface,
      onSurfaceVariant: onSurfaceVariant ?? this.onSurfaceVariant,
      onSurfaceSubtle: onSurfaceSubtle ?? this.onSurfaceSubtle,
      outline: outline ?? this.outline,
      outlineSubtle: outlineSubtle ?? this.outlineSubtle,
      error: error ?? this.error,
      success: success ?? this.success,
      info: info ?? this.info,
      earth: earth ?? this.earth,
      accentSubtle: accentSubtle ?? this.accentSubtle,
      accentBorder: accentBorder ?? this.accentBorder,
      glassBackground: glassBackground ?? this.glassBackground,
      glassBorder: glassBorder ?? this.glassBorder,
      watermarkOpacity: watermarkOpacity ?? this.watermarkOpacity,
      bodyWatermarkOpacity: bodyWatermarkOpacity ?? this.bodyWatermarkOpacity,
      territoryBoundary: territoryBoundary ?? this.territoryBoundary,
      locationPin: locationPin ?? this.locationPin,
    );
  }

  @override
  ThemeExtension<AppColors> lerp(ThemeExtension<AppColors>? other, double t) {
    if (other is! AppColors) return this;
    return AppColors(
      primary: Color.lerp(primary, other.primary, t)!,
      link: Color.lerp(link, other.link, t)!,
      surface: Color.lerp(surface, other.surface, t)!,
      surfaceElevated: Color.lerp(surfaceElevated, other.surfaceElevated, t)!,
      surfaceContainer: Color.lerp(surfaceContainer, other.surfaceContainer, t)!,
      onSurface: Color.lerp(onSurface, other.onSurface, t)!,
      onSurfaceVariant: Color.lerp(onSurfaceVariant, other.onSurfaceVariant, t)!,
      onSurfaceSubtle: Color.lerp(onSurfaceSubtle, other.onSurfaceSubtle, t)!,
      outline: Color.lerp(outline, other.outline, t)!,
      outlineSubtle: Color.lerp(outlineSubtle, other.outlineSubtle, t)!,
      error: Color.lerp(error, other.error, t)!,
      success: Color.lerp(success, other.success, t)!,
      info: Color.lerp(info, other.info, t)!,
      earth: Color.lerp(earth, other.earth, t)!,
      accentSubtle: Color.lerp(accentSubtle, other.accentSubtle, t)!,
      accentBorder: Color.lerp(accentBorder, other.accentBorder, t)!,
      glassBackground: Color.lerp(glassBackground, other.glassBackground, t)!,
      glassBorder: Color.lerp(glassBorder, other.glassBorder, t)!,
      watermarkOpacity: watermarkOpacity + (other.watermarkOpacity - watermarkOpacity) * t,
      bodyWatermarkOpacity:
          bodyWatermarkOpacity + (other.bodyWatermarkOpacity - bodyWatermarkOpacity) * t,
      territoryBoundary: Color.lerp(territoryBoundary, other.territoryBoundary, t)!,
      locationPin: Color.lerp(locationPin, other.locationPin, t)!,
    );
  }

  static const AppColors dark = AppColors(
    primary: AppDesignTokens.primary,
    link: AppDesignTokens.link,
    surface: AppDesignTokens.surface,
    surfaceElevated: AppDesignTokens.surfaceElevated,
    surfaceContainer: AppDesignTokens.surfaceCard,
    onSurface: AppDesignTokens.onSurface,
    onSurfaceVariant: AppDesignTokens.onSurfaceMuted,
    onSurfaceSubtle: AppDesignTokens.onSurfaceSubtle,
    outline: AppDesignTokens.outline,
    outlineSubtle: AppDesignTokens.outlineSubtle,
    error: AppDesignTokens.error,
    success: AppDesignTokens.success,
    info: AppDesignTokens.info,
    earth: AppDesignTokens.earth,
    accentSubtle: AppDesignTokens.accentSubtle,
    accentBorder: AppDesignTokens.accentBorderSoft,
    glassBackground: AppDesignTokens.glassBackgroundDark,
    glassBorder: AppDesignTokens.glassBorderDark,
    watermarkOpacity: AppDesignTokens.watermarkOpacityDark,
    bodyWatermarkOpacity: AppDesignTokens.bodyWatermarkOpacityDark,
    territoryBoundary: AppDesignTokens.territoryBoundary,
    locationPin: AppDesignTokens.locationPin,
  );

  static const AppColors light = AppColors(
    primary: AppDesignTokens.primaryLight,
    link: AppDesignTokens.link,
    surface: AppDesignTokens.surfaceLight,
    surfaceElevated: AppDesignTokens.surfaceLight,
    surfaceContainer: AppDesignTokens.surfaceCardLight,
    onSurface: AppDesignTokens.onSurfaceLight,
    onSurfaceVariant: AppDesignTokens.onSurfaceMutedLight,
    onSurfaceSubtle: AppDesignTokens.onSurfaceMutedLight,
    outline: AppDesignTokens.outlineLight,
    outlineSubtle: Color(0x1A000000),
    error: AppDesignTokens.error,
    success: AppDesignTokens.success,
    info: AppDesignTokens.info,
    earth: AppDesignTokens.earth,
    accentSubtle: Color(0x1A3BC495),
    accentBorder: Color(0x333BC495),
    glassBackground: AppDesignTokens.glassBackgroundLight,
    glassBorder: AppDesignTokens.glassBorderLight,
    watermarkOpacity: AppDesignTokens.watermarkOpacityLight,
    bodyWatermarkOpacity: AppDesignTokens.bodyWatermarkOpacityLight,
    territoryBoundary: AppDesignTokens.territoryBoundary,
    locationPin: AppDesignTokens.locationPin,
  );
}

extension AppColorsExtension on BuildContext {
  AppColors get appColors => Theme.of(this).extension<AppColors>() ?? AppColors.dark;
}
