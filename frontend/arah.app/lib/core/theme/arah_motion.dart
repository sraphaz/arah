import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:go_router/go_router.dart';

import '../config/constants.dart';

/// Microinterações alinhadas ao handoff (150/250ms, easing premium, press 0.975).
class ArahMotion {
  ArahMotion._();

  /// `cubic-bezier(0.16, 1, 0.3, 1)` — sem bounce.
  static const Curve emphasized = Cubic(0.16, 1.0, 0.3, 1.0);

  /// Escala de press do UI kit.
  static const double pressScale = 0.975;

  static bool animationsEnabled(BuildContext context) =>
      !MediaQuery.disableAnimationsOf(context);

  static void lightTap() => HapticFeedback.lightImpact();

  static void selectionTap() => HapticFeedback.selectionClick();

  static Duration duration(BuildContext context, int milliseconds) {
    if (!animationsEnabled(context)) return Duration.zero;
    return Duration(milliseconds: milliseconds);
  }

  static Duration get fast =>
      const Duration(milliseconds: AppConstants.animationFast);

  static Duration get normal =>
      const Duration(milliseconds: AppConstants.animationNormal);

  static Duration get slow =>
      const Duration(milliseconds: AppConstants.animationSlow);

  static Duration resolve(BuildContext context, Duration preferred) {
    if (!animationsEnabled(context)) return Duration.zero;
    return preferred;
  }

  /// Transição de tela: escurecer → surgir (fade + leve slide).
  static CustomTransitionPage<T> fadePage<T>({
    required LocalKey key,
    required Widget child,
    String? name,
  }) {
    return CustomTransitionPage<T>(
      key: key,
      name: name,
      child: child,
      transitionDuration: normal,
      reverseTransitionDuration: fast,
      transitionsBuilder: (context, animation, secondaryAnimation, child) {
        if (!animationsEnabled(context)) return child;
        final curved = CurvedAnimation(
          parent: animation,
          curve: emphasized,
          reverseCurve: emphasized,
        );
        return FadeTransition(
          opacity: curved,
          child: SlideTransition(
            position: Tween<Offset>(
              begin: const Offset(0, 0.024),
              end: Offset.zero,
            ).animate(curved),
            child: child,
          ),
        );
      },
    );
  }
}

/// Page transitions Material alinhadas ao design (fade, sem bounce).
class ArahFadePageTransitionsBuilder extends PageTransitionsBuilder {
  const ArahFadePageTransitionsBuilder();

  @override
  Widget buildTransitions<T>(
    PageRoute<T> route,
    BuildContext context,
    Animation<double> animation,
    Animation<double> secondaryAnimation,
    Widget child,
  ) {
    if (!ArahMotion.animationsEnabled(context)) return child;
    final curved = CurvedAnimation(
      parent: animation,
      curve: ArahMotion.emphasized,
      reverseCurve: ArahMotion.emphasized,
    );
    return FadeTransition(
      opacity: curved,
      child: SlideTransition(
        position: Tween<Offset>(
          begin: const Offset(0, 0.024),
          end: Offset.zero,
        ).animate(curved),
        child: child,
      ),
    );
  }
}
