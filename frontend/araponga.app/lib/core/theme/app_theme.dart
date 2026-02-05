import 'package:flutter/material.dart';

import 'app_design_tokens.dart';
import '../config/constants.dart';

/// Tema Ará: clean, leve, harmônico. Fundo escuro como padrão.
/// Cores e componentes vêm de [AppDesignTokens] para manutenção em um só lugar.
/// Ver docs/DESIGN_SYSTEM.md.
class AppTheme {
  AppTheme._();

  static ThemeData _baseTheme(Brightness brightness) {
    final isDark = brightness == Brightness.dark;
    final colors = isDark ? AppColors.dark : AppColors.light;
    return ThemeData(
      useMaterial3: true,
      brightness: brightness,
      colorScheme: ColorScheme(
        brightness: brightness,
        primary: colors.primary,
        onPrimary: isDark ? AppDesignTokens.onSurfaceDark : Colors.white,
        secondary: colors.primary,
        onSecondary: isDark ? AppDesignTokens.onSurfaceDark : Colors.white,
        surface: colors.surface,
        onSurface: colors.onSurface,
        surfaceContainerHighest: colors.surfaceContainer,
        onSurfaceVariant: colors.onSurfaceVariant,
        outline: colors.outline,
        error: Colors.red.shade400,
        onError: Colors.white,
      ),
      extensions: [colors],
      appBarTheme: AppBarTheme(
        centerTitle: true,
        elevation: 0,
        scrolledUnderElevation: isDark ? 0.5 : 0,
        backgroundColor: colors.surface,
        foregroundColor: colors.onSurface,
      ),
      scaffoldBackgroundColor: colors.surface,
      bottomNavigationBarTheme: BottomNavigationBarThemeData(
        type: BottomNavigationBarType.fixed,
        selectedItemColor: colors.primary,
        unselectedItemColor: colors.onSurfaceVariant,
        backgroundColor: colors.surface,
      ),
      cardTheme: CardTheme(
        color: colors.surfaceContainer,
        elevation: 0,
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(AppDesignTokens.radiusCard),
        ),
      ),
      snackBarTheme: SnackBarThemeData(
        behavior: SnackBarBehavior.floating,
        insetPadding: AppDesignTokens.snackBarInsets,
        contentTextStyle: const TextStyle(fontSize: AppDesignTokens.fontSizeSnackBar),
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(AppDesignTokens.radiusSnackBar),
        ),
      ),
      iconButtonTheme: IconButtonThemeData(
        style: IconButton.styleFrom(
          minimumSize: const Size(AppConstants.minTouchTargetSize, AppConstants.minTouchTargetSize),
          tapTargetSize: MaterialTapTargetSize.shrinkWrap,
        ),
      ),
      filledButtonTheme: FilledButtonThemeData(
        style: FilledButton.styleFrom(
          minimumSize: const Size(88, AppConstants.minTouchTargetSize),
          padding: const EdgeInsets.symmetric(
            horizontal: AppConstants.spacingLg,
            vertical: AppConstants.spacingSm,
          ),
        ),
      ),
      textButtonTheme: TextButtonThemeData(
        style: TextButton.styleFrom(
          minimumSize: const Size(64, AppConstants.minTouchTargetSize),
          padding: const EdgeInsets.symmetric(horizontal: AppConstants.spacingMd),
        ),
      ),
      inputDecorationTheme: InputDecorationTheme(
        border: const OutlineInputBorder(),
        contentPadding: const EdgeInsets.symmetric(
          horizontal: AppConstants.spacingMd,
          vertical: AppConstants.spacingSm,
        ),
        filled: true,
      ),
    );
  }

  /// Tema claro.
  static ThemeData get light => _baseTheme(Brightness.light);

  /// Tema escuro (padrão).
  static ThemeData get dark => _baseTheme(Brightness.dark);
}
