import 'package:flutter/material.dart';
import 'package:flutter/services.dart';

import '../config/constants.dart';

/// Microinterações alinhadas ao design system (haptic + scale).
class ArahMotion {
  ArahMotion._();

  static bool animationsEnabled(BuildContext context) =>
      !MediaQuery.disableAnimationsOf(context);

  static void lightTap() => HapticFeedback.lightImpact();

  static void selectionTap() => HapticFeedback.selectionClick();

  static Duration duration(BuildContext context, int milliseconds) {
    if (!animationsEnabled(context)) return Duration.zero;
    return Duration(milliseconds: milliseconds);
  }

  static Duration get fast => const Duration(milliseconds: AppConstants.animationFast);
  static Duration get normal => const Duration(milliseconds: AppConstants.animationNormal);
  static Duration get slow => const Duration(milliseconds: AppConstants.animationSlow);
}
