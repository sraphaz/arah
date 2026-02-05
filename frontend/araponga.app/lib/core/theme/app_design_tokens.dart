import 'package:flutter/material.dart';

import '../config/constants.dart';

/// Tokens de design em um único lugar para manutenção fácil.
/// Alterar cores, radius ou tipografia aqui reflete em todo o app via [AppTheme].
/// Ver docs/DESIGN_SYSTEM.md.
class AppDesignTokens {
  AppDesignTokens._();

  // ---------------------------------------------------------------------------
  // Paleta – tema escuro (padrão)
  // ---------------------------------------------------------------------------
  static const Color primaryDark = Color(0xFF81C784);
  static const Color surfaceDark = Color(0xFF121212);
  static const Color surfaceVariantDark = Color(0xFF1E1E1E);
  static const Color onSurfaceDark = Color(0xFFE8E8E8);
  static const Color onSurfaceVariantDark = Color(0xFFB0B0B0);
  static const Color outlineDark = Color(0xFF404040);

  // ---------------------------------------------------------------------------
  // Paleta – tema claro
  // ---------------------------------------------------------------------------
  static const Color primaryLight = Color(0xFF1B5E20);
  static const Color surfaceLight = Color(0xFFFAFAFA);
  static const Color onSurfaceLight = Color(0xFF1C1C1C);

  // ---------------------------------------------------------------------------
  // Tipografia (tamanhos base para TextTheme; Material 3 já define scale)
  // ---------------------------------------------------------------------------
  static const double fontSizeSnackBar = 15;

  // ---------------------------------------------------------------------------
  // Componentes (usar AppConstants quando já existir)
  // ---------------------------------------------------------------------------
  static double get radiusCard => AppConstants.radiusMd;
  static double get radiusSnackBar => AppConstants.radiusSm;
  static EdgeInsets get snackBarInsets => const EdgeInsets.symmetric(horizontal: AppConstants.spacingMd);
}

/// Extensão do tema para acessar tokens e cores do app de forma semântica.
/// Uso: `context.appColors.primary` ou `Theme.of(context).extension<AppColors>()!`.
class AppColors extends ThemeExtension<AppColors> {
  const AppColors({
    required this.primary,
    required this.surface,
    required this.surfaceContainer,
    required this.onSurface,
    required this.onSurfaceVariant,
    required this.outline,
  });

  final Color primary;
  final Color surface;
  final Color surfaceContainer;
  final Color onSurface;
  final Color onSurfaceVariant;
  final Color outline;

  @override
  ThemeExtension<AppColors> copyWith({
    Color? primary,
    Color? surface,
    Color? surfaceContainer,
    Color? onSurface,
    Color? onSurfaceVariant,
    Color? outline,
  }) {
    return AppColors(
      primary: primary ?? this.primary,
      surface: surface ?? this.surface,
      surfaceContainer: surfaceContainer ?? this.surfaceContainer,
      onSurface: onSurface ?? this.onSurface,
      onSurfaceVariant: onSurfaceVariant ?? this.onSurfaceVariant,
      outline: outline ?? this.outline,
    );
  }

  @override
  ThemeExtension<AppColors> lerp(ThemeExtension<AppColors>? other, double t) {
    if (other is! AppColors) return this;
    return AppColors(
      primary: Color.lerp(primary, other.primary, t)!,
      surface: Color.lerp(surface, other.surface, t)!,
      surfaceContainer: Color.lerp(surfaceContainer, other.surfaceContainer, t)!,
      onSurface: Color.lerp(onSurface, other.onSurface, t)!,
      onSurfaceVariant: Color.lerp(onSurfaceVariant, other.onSurfaceVariant, t)!,
      outline: Color.lerp(outline, other.outline, t)!,
    );
  }

  /// Cores para tema escuro.
  static const AppColors dark = AppColors(
    primary: AppDesignTokens.primaryDark,
    surface: AppDesignTokens.surfaceDark,
    surfaceContainer: AppDesignTokens.surfaceVariantDark,
    onSurface: AppDesignTokens.onSurfaceDark,
    onSurfaceVariant: AppDesignTokens.onSurfaceVariantDark,
    outline: AppDesignTokens.outlineDark,
  );

  /// Cores para tema claro (surfaceContainer = surfaceVariant implícito no light).
  static AppColors get light => const AppColors(
        primary: AppDesignTokens.primaryLight,
        surface: AppDesignTokens.surfaceLight,
        surfaceContainer: Color(0xFFF5F5F5),
        onSurface: AppDesignTokens.onSurfaceLight,
        onSurfaceVariant: Color(0xFF5C5C5C),
        outline: Color(0xFFE0E0E0),
      );
}

/// Acesso rápido: `context.appColors`.
extension AppColorsExtension on BuildContext {
  AppColors get appColors => Theme.of(this).extension<AppColors>()!;
}
