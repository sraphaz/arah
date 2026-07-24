import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../core/config/constants.dart';
import '../../../../core/theme/app_design_tokens.dart';
import '../../../../l10n/app_localizations.dart';
import '../../../membership/data/models/membership_info.dart';
import '../../../membership/presentation/providers/membership_provider.dart';

/// Banner-convite para visitantes (spec handoff): não esconde recurso; convida a confirmar residência.
class VisitorBanner extends ConsumerWidget {
  const VisitorBanner({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final l10n = AppLocalizations.of(context)!;
    final state = ref.watch(membershipProvider);
    final membership = state.membership;
    if (membership == null || membership.isResident || _isCurator(membership)) {
      return const SizedBox.shrink();
    }

    final colors = context.appColors;
    return Material(
      color: colors.accentSubtle,
      child: InkWell(
        onTap: () => context.push('/residency-journey'),
        child: Padding(
          padding: const EdgeInsets.symmetric(
            horizontal: AppConstants.spacingMd,
            vertical: AppConstants.spacingSm + 2,
          ),
          child: Row(
            children: [
              Icon(Icons.home_work_outlined, color: colors.primary, size: 22),
              const SizedBox(width: AppConstants.spacingSm),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      l10n.visitorBannerTitle,
                      style: Theme.of(context).textTheme.titleSmall?.copyWith(
                            color: colors.onSurface,
                            fontFamily: AppDesignTokens.fontFamilyDisplay,
                          ),
                    ),
                    Text(
                      l10n.visitorBannerCta,
                      style: Theme.of(context).textTheme.bodySmall?.copyWith(
                            color: colors.onSurfaceVariant,
                          ),
                    ),
                  ],
                ),
              ),
              Icon(Icons.chevron_right, color: colors.primary),
            ],
          ),
        ),
      ),
    );
  }

  bool _isCurator(MembershipInfo m) {
    final role = m.role.toUpperCase();
    return role == 'CURATOR' || role == 'CURADOR' || role == 'MODERATOR';
  }
}
