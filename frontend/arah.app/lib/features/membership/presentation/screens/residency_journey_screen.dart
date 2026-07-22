import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../core/config/constants.dart';
import '../../../../core/network/api_exception.dart';
import '../../../../core/providers/current_territory_name_provider.dart';
import '../../../../core/theme/app_design_tokens.dart';
import '../../../../core/widgets/app_snackbar.dart';
import '../../../../core/widgets/arah_card.dart';
import '../../../../core/widgets/arah_journey_shell.dart';
import '../../../../l10n/app_localizations.dart';
import '../providers/membership_provider.dart';

/// Jornada "Confirmar residência" (APP-DS-11 parcial): presença → mensagem → revisão → sucesso.
class ResidencyJourneyScreen extends ConsumerStatefulWidget {
  const ResidencyJourneyScreen({super.key});

  @override
  ConsumerState<ResidencyJourneyScreen> createState() =>
      _ResidencyJourneyScreenState();
}

class _ResidencyJourneyScreenState extends ConsumerState<ResidencyJourneyScreen> {
  static const int _totalSteps = 4;

  int _step = 0;
  bool _submitting = false;
  final _messageController = TextEditingController();

  @override
  void dispose() {
    _messageController.dispose();
    super.dispose();
  }

  void _close() {
    if (context.canPop()) {
      context.pop();
    } else {
      context.go('/home');
    }
  }

  Future<void> _onPrimary() async {
    final l10n = AppLocalizations.of(context)!;
    if (_step < 2) {
      setState(() => _step += 1);
      return;
    }
    if (_step == 2) {
      setState(() => _submitting = true);
      try {
        final message = _messageController.text.trim();
        await ref.read(membershipProvider.notifier).becomeResident(
              message: message.isEmpty ? null : message,
            );
        if (!mounted) return;
        setState(() {
          _submitting = false;
          _step = 3;
        });
      } catch (e) {
        if (!mounted) return;
        setState(() => _submitting = false);
        showErrorSnackBar(
          context,
          e is ApiException ? e.userMessage : l10n.errorRequestResidency,
        );
      }
      return;
    }
    _close();
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    final territoryName =
        ref.watch(currentTerritoryNameProvider) ?? l10n.residencyJourneyReviewTerritory;

    final String primaryLabel;
    switch (_step) {
      case 2:
        primaryLabel = l10n.residencyJourneySendRequest;
      case 3:
        primaryLabel = l10n.residencyJourneyUnderstood;
      default:
        primaryLabel = l10n.continueButton;
    }

    return ArahJourneyShell(
      title: l10n.residencyJourneyTitle,
      currentStep: _step,
      totalSteps: _totalSteps,
      onClose: _close,
      onBack: _step > 0 && _step < 3 ? () => setState(() => _step -= 1) : null,
      primaryActionLabel: primaryLabel,
      onPrimaryAction: _submitting ? null : _onPrimary,
      primaryEnabled: !_submitting,
      primaryLoading: _submitting,
      child: _buildStep(context, l10n, territoryName),
    );
  }

  Widget _buildStep(
    BuildContext context,
    AppLocalizations l10n,
    String territoryName,
  ) {
    switch (_step) {
      case 0:
        return _PresenceStep(l10n: l10n, territoryName: territoryName);
      case 1:
        return _MessageStep(
          l10n: l10n,
          controller: _messageController,
        );
      case 2:
        return _ReviewStep(
          l10n: l10n,
          territoryName: territoryName,
          message: _messageController.text.trim(),
        );
      default:
        return _SuccessStep(l10n: l10n);
    }
  }
}

class _PresenceStep extends StatelessWidget {
  const _PresenceStep({required this.l10n, required this.territoryName});

  final AppLocalizations l10n;
  final String territoryName;

  @override
  Widget build(BuildContext context) {
    final colors = context.appColors;
    final theme = Theme.of(context);
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          l10n.residencyJourneyBecomeResident(territoryName),
          style: theme.textTheme.headlineSmall?.copyWith(
            color: colors.onSurface,
            fontFamily: AppDesignTokens.fontFamilyDisplay,
            letterSpacing: AppDesignTokens.letterSpacingTight,
          ),
        ),
        const SizedBox(height: AppConstants.spacingSm),
        Text(
          l10n.residencyJourneyPresenceSubtitle,
          style: theme.textTheme.bodyMedium?.copyWith(
            color: colors.onSurfaceVariant,
          ),
        ),
        const SizedBox(height: AppConstants.spacingLg),
        ArahCard(
          child: Row(
            children: [
              Icon(Icons.my_location, color: colors.primary, size: 22),
              const SizedBox(width: AppConstants.spacingMd - 4),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      l10n.residencyJourneyPresenceTitle,
                      style: theme.textTheme.titleSmall?.copyWith(
                        color: colors.onSurface,
                      ),
                    ),
                    Text(
                      l10n.residencyJourneyPresenceHint,
                      style: theme.textTheme.bodySmall?.copyWith(
                        color: colors.onSurfaceSubtle,
                      ),
                    ),
                  ],
                ),
              ),
              Icon(Icons.check_circle, color: colors.success, size: 22),
            ],
          ),
        ),
      ],
    );
  }
}

class _MessageStep extends StatelessWidget {
  const _MessageStep({
    required this.l10n,
    required this.controller,
  });

  final AppLocalizations l10n;
  final TextEditingController controller;

  @override
  Widget build(BuildContext context) {
    final colors = context.appColors;
    final theme = Theme.of(context);
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          l10n.residencyJourneyProofTitle,
          style: theme.textTheme.headlineSmall?.copyWith(
            color: colors.onSurface,
            fontFamily: AppDesignTokens.fontFamilyDisplay,
            letterSpacing: AppDesignTokens.letterSpacingTight,
          ),
        ),
        const SizedBox(height: AppConstants.spacingSm),
        Text(
          l10n.residencyJourneyProofSubtitle,
          style: theme.textTheme.bodyMedium?.copyWith(
            color: colors.onSurfaceVariant,
          ),
        ),
        const SizedBox(height: AppConstants.spacingLg),
        TextField(
          controller: controller,
          maxLines: 5,
          minLines: 3,
          textInputAction: TextInputAction.done,
          decoration: InputDecoration(
            labelText: l10n.residencyJourneyMessageLabel,
            hintText: l10n.residencyJourneyMessageHint,
            alignLabelWithHint: true,
          ),
        ),
      ],
    );
  }
}

class _ReviewStep extends StatelessWidget {
  const _ReviewStep({
    required this.l10n,
    required this.territoryName,
    required this.message,
  });

  final AppLocalizations l10n;
  final String territoryName;
  final String message;

  @override
  Widget build(BuildContext context) {
    final colors = context.appColors;
    final theme = Theme.of(context);
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          l10n.residencyJourneyReviewTitle,
          style: theme.textTheme.headlineSmall?.copyWith(
            color: colors.onSurface,
            fontFamily: AppDesignTokens.fontFamilyDisplay,
            letterSpacing: AppDesignTokens.letterSpacingTight,
          ),
        ),
        const SizedBox(height: AppConstants.spacingLg),
        ArahCard(
          padding: const EdgeInsets.symmetric(
            horizontal: AppConstants.spacingMd,
            vertical: AppConstants.spacingXs,
          ),
          child: Column(
            children: [
              _ReviewRow(
                icon: Icons.terrain_outlined,
                label: l10n.residencyJourneyReviewTerritory,
                value: territoryName,
              ),
              _ReviewRow(
                icon: Icons.my_location_outlined,
                label: l10n.residencyJourneyReviewPresence,
                value: l10n.residencyJourneyReviewPresenceValue,
                accent: colors.success,
              ),
              _ReviewRow(
                icon: Icons.description_outlined,
                label: l10n.residencyJourneyReviewMessage,
                value: message.isEmpty
                    ? l10n.residencyJourneyReviewMessageEmpty
                    : message,
              ),
              _ReviewRow(
                icon: Icons.shield_outlined,
                label: l10n.residencyJourneyReviewAnalysis,
                value: l10n.residencyJourneyReviewAnalysisValue,
                showDivider: false,
              ),
            ],
          ),
        ),
      ],
    );
  }
}

class _ReviewRow extends StatelessWidget {
  const _ReviewRow({
    required this.icon,
    required this.label,
    required this.value,
    this.accent,
    this.showDivider = true,
  });

  final IconData icon;
  final String label;
  final String value;
  final Color? accent;
  final bool showDivider;

  @override
  Widget build(BuildContext context) {
    final colors = context.appColors;
    final theme = Theme.of(context);
    return DecoratedBox(
      decoration: BoxDecoration(
        border: showDivider
            ? Border(bottom: BorderSide(color: colors.outlineSubtle))
            : null,
      ),
      child: Padding(
        padding: const EdgeInsets.symmetric(vertical: AppConstants.spacingMd - 3),
        child: Row(
          children: [
            Icon(icon, size: 19, color: colors.onSurfaceSubtle),
            const SizedBox(width: AppConstants.spacingMd - 4),
            Expanded(
              child: Text(
                label,
                style: theme.textTheme.bodyMedium?.copyWith(
                  color: colors.onSurfaceVariant,
                ),
              ),
            ),
            Flexible(
              child: Text(
                value,
                textAlign: TextAlign.right,
                maxLines: 2,
                overflow: TextOverflow.ellipsis,
                style: theme.textTheme.bodyMedium?.copyWith(
                  color: accent ?? colors.onSurface,
                  fontWeight: FontWeight.w600,
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }
}

class _SuccessStep extends StatelessWidget {
  const _SuccessStep({required this.l10n});

  final AppLocalizations l10n;

  @override
  Widget build(BuildContext context) {
    final colors = context.appColors;
    final theme = Theme.of(context);
    return Padding(
      padding: const EdgeInsets.symmetric(
        horizontal: AppConstants.spacingLg,
        vertical: AppConstants.spacing2xl,
      ),
      child: Column(
        children: [
          Container(
            width: 88,
            height: 88,
            decoration: BoxDecoration(
              shape: BoxShape.circle,
              color: colors.success.withValues(alpha: 0.12),
            ),
            child: Icon(Icons.how_to_reg, size: 48, color: colors.success),
          ),
          const SizedBox(height: AppConstants.spacingLg - 2),
          Text(
            l10n.residencyJourneySuccessTitle,
            textAlign: TextAlign.center,
            style: theme.textTheme.headlineSmall?.copyWith(
              color: colors.onSurface,
              fontFamily: AppDesignTokens.fontFamilyDisplay,
              letterSpacing: AppDesignTokens.letterSpacingTight,
            ),
          ),
          const SizedBox(height: AppConstants.spacingMd - 4),
          Text(
            l10n.residencyJourneySuccessMessage,
            textAlign: TextAlign.center,
            style: theme.textTheme.bodyMedium?.copyWith(
              color: colors.onSurfaceVariant,
              height: 1.55,
            ),
          ),
        ],
      ),
    );
  }
}
