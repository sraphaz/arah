import 'package:flutter/material.dart';

import '../config/constants.dart';
import '../theme/app_design_tokens.dart';

/// Exibe SnackBar de sucesso usando tokens do tema.
void showSuccessSnackBar(BuildContext context, String message) {
  final colors = context.appColors;
  ScaffoldMessenger.of(context).showSnackBar(
    SnackBar(
      content: Text(message),
      backgroundColor: colors.success,
      behavior: SnackBarBehavior.floating,
      margin: EdgeInsets.fromLTRB(
        AppConstants.spacingMd,
        0,
        AppConstants.spacingMd,
        AppConstants.spacingLg,
      ),
      shape: RoundedRectangleBorder(
        borderRadius: BorderRadius.circular(AppDesignTokens.radiusSnackBar),
        side: BorderSide(color: colors.accentBorder),
      ),
      duration: const Duration(seconds: 2),
    ),
  );
}

/// Exibe SnackBar de erro usando tokens do tema.
void showErrorSnackBar(BuildContext context, String message) {
  final colors = context.appColors;
  ScaffoldMessenger.of(context).showSnackBar(
    SnackBar(
      content: Text(message),
      backgroundColor: colors.error,
      behavior: SnackBarBehavior.floating,
      margin: EdgeInsets.fromLTRB(
        AppConstants.spacingMd,
        0,
        AppConstants.spacingMd,
        AppConstants.spacingLg,
      ),
      shape: RoundedRectangleBorder(
        borderRadius: BorderRadius.circular(AppDesignTokens.radiusSnackBar),
      ),
      duration: const Duration(seconds: 3),
    ),
  );
}
