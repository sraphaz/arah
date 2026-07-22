import 'package:flutter/material.dart';

import '../config/constants.dart';
import '../theme/app_design_tokens.dart';
import 'arah_button.dart';
import 'arah_scaffold.dart';

/// Shell de jornada multi-etapas (UI kit JourneyShell): topo, progresso, corpo e CTAs.
class ArahJourneyShell extends StatelessWidget {
  const ArahJourneyShell({
    super.key,
    required this.title,
    required this.currentStep,
    required this.totalSteps,
    required this.onClose,
    this.onBack,
    required this.child,
    required this.primaryActionLabel,
    required this.onPrimaryAction,
    this.primaryEnabled = true,
    this.secondaryActionLabel,
    this.onSecondaryAction,
    this.primaryLoading = false,
  });

  final String title;
  /// Índice 0-based do passo atual.
  final int currentStep;
  final int totalSteps;
  final VoidCallback onClose;
  final VoidCallback? onBack;
  final Widget child;
  final String primaryActionLabel;
  final VoidCallback? onPrimaryAction;
  final bool primaryEnabled;
  final String? secondaryActionLabel;
  final VoidCallback? onSecondaryAction;
  final bool primaryLoading;

  @override
  Widget build(BuildContext context) {
    final colors = context.appColors;
    final theme = Theme.of(context);
    final safeTotal = totalSteps <= 0 ? 1 : totalSteps;
    final clampedStep = currentStep.clamp(0, safeTotal - 1);
    final progress = (clampedStep + 1) / safeTotal;
    final showBack = onBack != null && clampedStep > 0;

    return ArahScaffold(
      showWatermark: false,
      body: SafeArea(
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            Padding(
              padding: const EdgeInsets.fromLTRB(
                AppConstants.spacingSm,
                AppConstants.spacingSm,
                AppConstants.spacingMd,
                AppConstants.spacingSm,
              ),
              child: Row(
                children: [
                  IconButton(
                    onPressed: showBack ? onBack : onClose,
                    icon: Icon(showBack ? Icons.arrow_back : Icons.close),
                    constraints: const BoxConstraints(
                      minWidth: AppConstants.minTouchTargetSize,
                      minHeight: AppConstants.minTouchTargetSize,
                    ),
                  ),
                  Expanded(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Text(
                          title,
                          maxLines: 1,
                          overflow: TextOverflow.ellipsis,
                          style: theme.textTheme.titleMedium?.copyWith(
                            color: colors.onSurface,
                            fontFamily: AppDesignTokens.fontFamilyDisplay,
                            letterSpacing: AppDesignTokens.letterSpacingTight,
                          ),
                        ),
                        const SizedBox(height: 2),
                        Text(
                          '${clampedStep + 1}/$safeTotal',
                          style: theme.textTheme.labelSmall?.copyWith(
                            color: colors.onSurfaceSubtle,
                          ),
                        ),
                      ],
                    ),
                  ),
                ],
              ),
            ),
            Padding(
              padding: const EdgeInsets.symmetric(horizontal: AppConstants.spacingMd),
              child: ClipRRect(
                borderRadius: BorderRadius.circular(AppConstants.radiusSm),
                child: LinearProgressIndicator(
                  value: progress,
                  minHeight: 4,
                  backgroundColor: colors.surfaceContainer,
                  color: colors.primary,
                ),
              ),
            ),
            Expanded(
              child: SingleChildScrollView(
                padding: const EdgeInsets.fromLTRB(
                  AppConstants.spacingMd + 2,
                  AppConstants.spacingLg - 4,
                  AppConstants.spacingMd + 2,
                  AppConstants.spacingMd,
                ),
                child: child,
              ),
            ),
            DecoratedBox(
              decoration: BoxDecoration(
                border: Border(top: BorderSide(color: colors.outlineSubtle)),
                color: colors.surface.withValues(alpha: 0.92),
              ),
              child: Padding(
                padding: const EdgeInsets.fromLTRB(
                  AppConstants.spacingMd + 2,
                  AppConstants.spacingMd - 4,
                  AppConstants.spacingMd + 2,
                  AppConstants.spacingMd,
                ),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.stretch,
                  children: [
                    ArahButton(
                      label: primaryActionLabel,
                      onPressed: primaryEnabled ? onPrimaryAction : null,
                      loading: primaryLoading,
                      expand: true,
                    ),
                    if (secondaryActionLabel != null && onSecondaryAction != null) ...[
                      const SizedBox(height: AppConstants.spacingSm),
                      ArahButton(
                        label: secondaryActionLabel!,
                        onPressed: onSecondaryAction,
                        variant: ArahButtonVariant.secondary,
                        expand: true,
                      ),
                    ],
                  ],
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }
}
