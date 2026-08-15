import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

import '../../../../core/config/constants.dart';
import '../../../../core/theme/app_design_tokens.dart';
import '../../../../core/widgets/arah_card.dart';
import '../../../../core/widgets/arah_journey_shell.dart';
import '../../../../l10n/app_localizations.dart';

/// Stub JourneyShell para serviços “Em breve” (APP-DS-15) — só UI, sem backend.
class ComingSoonJourneyScreen extends StatefulWidget {
  const ComingSoonJourneyScreen({
    super.key,
    required this.title,
    this.phase,
  });

  final String title;
  final String? phase;

  @override
  State<ComingSoonJourneyScreen> createState() =>
      _ComingSoonJourneyScreenState();
}

class _ComingSoonJourneyScreenState extends State<ComingSoonJourneyScreen> {
  int _step = 0;

  void _close() {
    if (context.canPop()) {
      context.pop();
    } else {
      context.go('/home');
    }
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    final theme = Theme.of(context);
    final colors = context.appColors;
    final phase = widget.phase;

    return ArahJourneyShell(
      title: l10n.comingSoonJourneyTitle,
      currentStep: _step,
      totalSteps: 2,
      onClose: _close,
      onBack: _step > 0 ? () => setState(() => _step -= 1) : null,
      primaryActionLabel:
          _step == 0 ? l10n.continueButton : l10n.comingSoonUnderstood,
      onPrimaryAction: () {
        if (_step == 0) {
          setState(() => _step = 1);
        } else {
          _close();
        }
      },
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            _step == 0
                ? l10n.comingSoonVisionTitle
                : l10n.comingSoonNotifyTitle,
            style: theme.textTheme.headlineSmall?.copyWith(
              fontFamily: AppDesignTokens.fontFamilyDisplay,
              letterSpacing: AppDesignTokens.letterSpacingTight,
            ),
          ),
          const SizedBox(height: AppConstants.spacingSm),
          Text(
            _step == 0
                ? l10n.comingSoonVisionMessage(widget.title)
                : l10n.comingSoonNotifyMessage,
            style: theme.textTheme.bodyMedium?.copyWith(
              color: colors.onSurfaceVariant,
            ),
          ),
          const SizedBox(height: AppConstants.spacingLg),
          ArahCard(
            child: Row(
              children: [
                Icon(Icons.schedule_outlined, color: colors.primary),
                const SizedBox(width: AppConstants.spacingMd),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(widget.title, style: theme.textTheme.titleSmall),
                      if (phase != null && phase.isNotEmpty) ...[
                        const SizedBox(height: AppConstants.spacingXs),
                        Text(
                          '${l10n.statusSoon} · $phase',
                          style: theme.textTheme.bodySmall?.copyWith(
                            color: colors.onSurfaceSubtle,
                          ),
                        ),
                      ],
                    ],
                  ),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}
