import 'package:flutter/material.dart';

import 'app_design_tokens.dart';
import '../config/constants.dart';

/// Tema Arah: identidade premium (ADR-021) — Sora display + Geist corpo, floresta.
class AppTheme {
  AppTheme._();

  static TextStyle _style({
    required double size,
    required FontWeight weight,
    required Color color,
    required String fontFamily,
    double? height,
    double? letterSpacing,
  }) {
    return TextStyle(
      fontFamily: fontFamily,
      fontSize: size,
      fontWeight: weight,
      color: color,
      height: height,
      letterSpacing: letterSpacing,
    );
  }

  static TextTheme _textTheme(Brightness brightness) {
    final onSurface = brightness == Brightness.dark
        ? AppDesignTokens.onSurface
        : AppDesignTokens.onSurfaceLight;
    final onMuted = brightness == Brightness.dark
        ? AppDesignTokens.onSurfaceMuted
        : AppDesignTokens.onSurfaceMutedLight;
    const display = AppDesignTokens.fontFamilyDisplay;
    const body = AppDesignTokens.fontFamilyBody;

    return TextTheme(
      displaySmall: _style(
        size: AppDesignTokens.fontSize4xl,
        weight: FontWeight.w700,
        color: onSurface,
        fontFamily: display,
        letterSpacing: AppDesignTokens.letterSpacingTight,
      ),
      headlineMedium: _style(
        size: AppDesignTokens.fontSize2xl,
        weight: FontWeight.w600,
        color: onSurface,
        fontFamily: display,
        letterSpacing: AppDesignTokens.letterSpacingTight,
      ),
      titleLarge: _style(
        size: AppDesignTokens.fontSizeXl,
        weight: FontWeight.w600,
        color: onSurface,
        fontFamily: display,
      ),
      titleMedium: _style(
        size: AppDesignTokens.fontSizeLg,
        weight: FontWeight.w600,
        color: onSurface,
        fontFamily: display,
      ),
      bodyLarge: _style(
        size: AppDesignTokens.fontSizeBase,
        weight: FontWeight.w400,
        color: onSurface,
        fontFamily: body,
        height: 1.5,
      ),
      bodyMedium: _style(
        size: AppDesignTokens.fontSizeSm,
        weight: FontWeight.w400,
        color: onMuted,
        fontFamily: body,
        height: 1.5,
      ),
      bodySmall: _style(
        size: AppDesignTokens.fontSizeXs,
        weight: FontWeight.w400,
        color: onMuted,
        fontFamily: body,
        height: 1.4,
      ),
      labelLarge: _style(
        size: AppDesignTokens.fontSizeSm,
        weight: FontWeight.w600,
        color: onSurface,
        fontFamily: body,
        letterSpacing: AppDesignTokens.letterSpacingWide,
      ),
      labelSmall: _style(
        size: AppDesignTokens.fontSizeXs,
        weight: FontWeight.w500,
        color: onMuted,
        fontFamily: body,
      ),
    );
  }

  static ThemeData _baseTheme(Brightness brightness) {
    final isDark = brightness == Brightness.dark;
    final colors = isDark ? AppColors.dark : AppColors.light;
    final textTheme = _textTheme(brightness);

    return ThemeData(
      useMaterial3: true,
      brightness: brightness,
      fontFamily: AppDesignTokens.fontFamilyBody,
      textTheme: textTheme,
      colorScheme: ColorScheme(
        brightness: brightness,
        primary: colors.primary,
        onPrimary: AppDesignTokens.textOnAccent,
        secondary: colors.link,
        onSecondary: AppDesignTokens.textOnAccent,
        tertiary: colors.primary,
        surface: colors.surface,
        onSurface: colors.onSurface,
        surfaceContainerHighest: colors.surfaceContainer,
        onSurfaceVariant: colors.onSurfaceVariant,
        outline: colors.outline,
        error: colors.error,
        onError: AppDesignTokens.textOnAccent,
      ),
      extensions: [colors],
      appBarTheme: AppBarTheme(
        centerTitle: false,
        elevation: 0,
        scrolledUnderElevation: 0,
        backgroundColor: colors.surfaceElevated.withValues(alpha: 0.92),
        foregroundColor: colors.onSurface,
        surfaceTintColor: Colors.transparent,
        titleTextStyle: textTheme.titleLarge,
      ),
      scaffoldBackgroundColor: colors.surface,
      navigationBarTheme: NavigationBarThemeData(
        elevation: 0,
        height: 72,
        backgroundColor: colors.surfaceElevated.withValues(alpha: 0.95),
        indicatorColor: colors.accentSubtle,
        labelTextStyle: WidgetStateProperty.resolveWith((states) {
          final selected = states.contains(WidgetState.selected);
          return textTheme.labelSmall?.copyWith(
            fontWeight: selected ? FontWeight.w600 : FontWeight.w500,
            color: selected ? colors.primary : colors.onSurfaceSubtle,
          );
        }),
        iconTheme: WidgetStateProperty.resolveWith((states) {
          final selected = states.contains(WidgetState.selected);
          return IconThemeData(
            color: selected ? colors.primary : colors.onSurfaceSubtle,
            size: AppConstants.iconSizeMd,
          );
        }),
      ),
      bottomNavigationBarTheme: BottomNavigationBarThemeData(
        type: BottomNavigationBarType.fixed,
        selectedItemColor: colors.primary,
        unselectedItemColor: colors.onSurfaceSubtle,
        backgroundColor: colors.surfaceElevated,
      ),
      cardTheme: CardThemeData(
        color: colors.surfaceContainer,
        elevation: 0,
        margin: EdgeInsets.zero,
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(AppDesignTokens.radiusCard),
          side: BorderSide(color: colors.outlineSubtle),
        ),
      ),
      dividerTheme: DividerThemeData(
        color: colors.outlineSubtle,
        thickness: 1,
        space: 1,
      ),
      snackBarTheme: SnackBarThemeData(
        behavior: SnackBarBehavior.floating,
        backgroundColor: colors.surfaceContainer,
        contentTextStyle: textTheme.bodyMedium?.copyWith(color: colors.onSurface),
        insetPadding: AppDesignTokens.snackBarInsets,
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(AppDesignTokens.radiusSnackBar),
          side: BorderSide(color: colors.accentBorder),
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
            vertical: AppConstants.spacingSm + 2,
          ),
          backgroundColor: colors.primary,
          foregroundColor: AppDesignTokens.textOnAccent,
          textStyle: textTheme.labelLarge,
          shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.circular(AppDesignTokens.radiusButton),
          ),
        ),
      ),
      textButtonTheme: TextButtonThemeData(
        style: TextButton.styleFrom(
          minimumSize: const Size(64, AppConstants.minTouchTargetSize),
          padding: const EdgeInsets.symmetric(horizontal: AppConstants.spacingMd),
          foregroundColor: colors.link,
          textStyle: textTheme.labelLarge?.copyWith(color: colors.link),
        ),
      ),
      inputDecorationTheme: InputDecorationTheme(
        filled: true,
        fillColor: colors.surfaceContainer.withValues(alpha: 0.6),
        border: OutlineInputBorder(
          borderRadius: BorderRadius.circular(AppConstants.radiusMd),
          borderSide: BorderSide(color: colors.outline),
        ),
        enabledBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(AppConstants.radiusMd),
          borderSide: BorderSide(color: colors.outline),
        ),
        focusedBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(AppConstants.radiusMd),
          borderSide: BorderSide(color: colors.primary, width: 1.5),
        ),
        errorBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(AppConstants.radiusMd),
          borderSide: BorderSide(color: colors.error),
        ),
        contentPadding: const EdgeInsets.symmetric(
          horizontal: AppConstants.spacingMd,
          vertical: AppConstants.spacingSm + 4,
        ),
        labelStyle: textTheme.bodyMedium,
        hintStyle: textTheme.bodyMedium?.copyWith(color: colors.onSurfaceSubtle),
      ),
      outlinedButtonTheme: OutlinedButtonThemeData(
        style: OutlinedButton.styleFrom(
          minimumSize: const Size(88, AppConstants.minTouchTargetSize),
          foregroundColor: colors.primary,
          side: BorderSide(color: colors.accentBorder),
          shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.circular(AppDesignTokens.radiusButton),
          ),
        ),
      ),
      chipTheme: ChipThemeData(
        backgroundColor: colors.surfaceContainer,
        selectedColor: colors.primary,
        disabledColor: colors.surfaceContainer.withValues(alpha: 0.5),
        labelStyle: textTheme.labelLarge!,
        secondaryLabelStyle: textTheme.labelLarge!.copyWith(color: AppDesignTokens.textOnAccent),
        padding: const EdgeInsets.symmetric(horizontal: AppConstants.spacingSm),
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(AppConstants.radiusXl),
          side: BorderSide(color: colors.outlineSubtle),
        ),
      ),
      floatingActionButtonTheme: FloatingActionButtonThemeData(
        backgroundColor: colors.primary,
        foregroundColor: AppDesignTokens.textOnAccent,
        elevation: 0,
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(AppConstants.radiusLg),
        ),
      ),
      progressIndicatorTheme: ProgressIndicatorThemeData(
        color: colors.primary,
        linearTrackColor: colors.outlineSubtle,
        circularTrackColor: colors.outlineSubtle,
      ),
      dialogTheme: DialogThemeData(
        backgroundColor: colors.glassBackground,
        surfaceTintColor: Colors.transparent,
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(AppDesignTokens.glassRadius),
          side: BorderSide(color: colors.glassBorder),
        ),
      ),
      bottomSheetTheme: BottomSheetThemeData(
        backgroundColor: colors.glassBackground,
        surfaceTintColor: Colors.transparent,
        shape: const RoundedRectangleBorder(
          borderRadius: BorderRadius.vertical(
            top: Radius.circular(AppDesignTokens.glassRadius),
          ),
        ),
      ),
    );
  }

  static ThemeData get light => _baseTheme(Brightness.light);

  static ThemeData get dark => _baseTheme(Brightness.dark);
}
