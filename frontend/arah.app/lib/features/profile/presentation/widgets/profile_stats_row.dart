import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/config/constants.dart';
import '../../../../core/theme/app_design_tokens.dart';
import '../../../../core/widgets/arah_card.dart';
import '../../../../core/widgets/arah_role_badge.dart';
import '../../../../l10n/app_localizations.dart';
import '../../../membership/presentation/providers/membership_provider.dart';
import '../../data/models/me_profile.dart';

/// Linha de 3 estatísticas sob o header do perfil (APP-DS-09).
///
/// Usa contagens do JSON quando presentes; senão placeholders com papel,
/// presença no território e quantidade de interesses.
class ProfileStatsRow extends ConsumerWidget {
  const ProfileStatsRow({super.key, required this.profile});

  final MeProfile profile;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final l10n = AppLocalizations.of(context)!;
    final membership = ref.watch(membershipProvider).membership;
    final role = membership?.role ?? 'VISITOR';
    final roleKind = ArahRoleBadge.kindFrom(role);
    final roleLabel = switch (roleKind) {
      ArahRoleKind.resident => l10n.resident,
      ArahRoleKind.curator => l10n.curator,
      ArahRoleKind.visitor => l10n.visitor,
    };
    final presenceLabel =
        (membership?.isResident ?? false) ? l10n.resident : l10n.visitor;
    final interestsValue = profile.interestsCount ??
        (profile.interests.isEmpty ? null : profile.interests.length);
    final dash = l10n.profileStatUnavailable;

    final List<_StatItem> items;
    if (profile.hasStatCounts) {
      items = [
        _StatItem(
          value: profile.postsCount?.toString() ?? dash,
          label: l10n.profileStatPosts,
        ),
        _StatItem(
          value: profile.connectionsCount?.toString() ?? dash,
          label: l10n.profileStatConnections,
        ),
        _StatItem(
          value: (profile.interestsCount ?? interestsValue)?.toString() ?? dash,
          label: l10n.profileStatInterests,
        ),
      ];
    } else {
      items = [
        _StatItem(value: roleLabel, label: l10n.profileStatRole),
        _StatItem(value: presenceLabel, label: l10n.profileStatPresence),
        _StatItem(
          value: interestsValue?.toString() ?? dash,
          label: l10n.profileStatInterests,
        ),
      ];
    }

    return ArahCard(
      padding: const EdgeInsets.symmetric(
        vertical: AppConstants.spacingMd,
        horizontal: AppConstants.spacingSm,
      ),
      child: Row(
        children: [
          for (var i = 0; i < items.length; i++) ...[
            if (i > 0)
              Container(
                width: 1,
                height: AppConstants.minTouchTargetSize * 0.55,
                color: context.appColors.outlineSubtle,
              ),
            Expanded(child: _StatColumn(item: items[i])),
          ],
        ],
      ),
    );
  }
}

class _StatItem {
  const _StatItem({required this.value, required this.label});

  final String value;
  final String label;
}

class _StatColumn extends StatelessWidget {
  const _StatColumn({required this.item});

  final _StatItem item;

  @override
  Widget build(BuildContext context) {
    final colors = context.appColors;
    final theme = Theme.of(context);

    return Padding(
      padding: const EdgeInsets.symmetric(horizontal: AppConstants.spacingXs),
      child: Column(
        mainAxisSize: MainAxisSize.min,
        children: [
          Text(
            item.value,
            textAlign: TextAlign.center,
            maxLines: 1,
            overflow: TextOverflow.ellipsis,
            style: theme.textTheme.titleMedium?.copyWith(
              color: colors.onSurface,
              fontWeight: FontWeight.w600,
              fontFamily: AppDesignTokens.fontFamilyDisplay,
            ),
          ),
          const SizedBox(height: AppConstants.spacingXs),
          Text(
            item.label,
            textAlign: TextAlign.center,
            maxLines: 1,
            overflow: TextOverflow.ellipsis,
            style: theme.textTheme.labelSmall?.copyWith(
              color: colors.onSurfaceVariant,
            ),
          ),
        ],
      ),
    );
  }
}
