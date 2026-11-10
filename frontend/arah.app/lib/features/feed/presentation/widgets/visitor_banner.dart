import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../core/config/constants.dart';
import '../../../../core/providers/current_territory_name_provider.dart';
import '../../../../core/theme/app_design_tokens.dart';
import '../../../../core/widgets/arah_material_symbol.dart';
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
    final territoryName = ref.watch(currentTerritoryNameProvider);
    final membership = state.membership;
    if (membership == null || membership.isResident || _isCurator(membership)) {
      return const SizedBox.shrink();
    }

    final colors = context.appColors;
    return LayoutBuilder(
      builder: (context, constraints) {
        final compact = constraints.maxWidth < 340;
        return Material(
          color: Colors.transparent,
          child: InkWell(
            onTap: () => context.push('/residency-journey'),
            borderRadius: BorderRadius.circular(AppConstants.radiusCard),
            child: Ink(
              decoration: BoxDecoration(
                borderRadius: BorderRadius.circular(AppConstants.radiusCard),
                gradient: const LinearGradient(
                  begin: Alignment.topLeft,
                  end: Alignment.bottomRight,
                  colors: [
                    AppDesignTokens.surfaceCard,
                    AppDesignTokens.surfaceElevated,
                  ],
                ),
                border: Border.all(color: colors.outline),
                boxShadow: AppDesignTokens.elevation(2, isDark: true),
              ),
              child: Padding(
                padding: EdgeInsets.all(compact ? 12 : 14),
                child: Row(
                  children: [
                    Container(
                      width: compact ? 34 : 36,
                      height: compact ? 34 : 36,
                      decoration: BoxDecoration(
                        borderRadius: BorderRadius.circular(11),
                        color: colors.accentSubtle,
                      ),
                      child: Center(
                        child: ArahMaterialSymbol(
                          'cottage',
                          size: compact ? 18 : 20,
                          fill: 1,
                          color: AppDesignTokens.primary,
                        ),
                      ),
                    ),
                    SizedBox(width: compact ? 10 : 12),
                    Expanded(
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Text(
                            territoryName != null && territoryName.isNotEmpty
                                ? l10n.visitorBannerTitleWithTerritory(
                                    territoryName)
                                : l10n.visitorBannerTitle,
                            style: Theme.of(context)
                                .textTheme
                                .titleSmall
                                ?.copyWith(
                                  fontSize: compact ? 12.8 : 13.2,
                                  color: colors.onSurface,
                                  fontFamily: AppDesignTokens.fontFamilyBody,
                                  fontWeight: FontWeight.w600,
                                ),
                          ),
                          const SizedBox(height: 1),
                          Text(
                            l10n.visitorBannerSubtitle,
                            maxLines: compact ? 2 : 1,
                            overflow: TextOverflow.ellipsis,
                            style:
                                Theme.of(context).textTheme.bodySmall?.copyWith(
                                      fontSize: 11.5,
                                      color: colors.onSurfaceSubtle,
                                    ),
                          ),
                        ],
                      ),
                    ),
                    if (!compact) ...[
                      const SizedBox(width: 8),
                      ArahMaterialSymbol(
                        'chevron_right',
                        size: 18,
                        color: colors.onSurfaceSubtle,
                      ),
                    ],
                  ],
                ),
              ),
            ),
          ),
        );
      },
    );
  }

  bool _isCurator(MembershipInfo m) {
    final role = m.role.toUpperCase();
    return role == 'CURATOR' || role == 'CURADOR' || role == 'MODERATOR';
  }
}
