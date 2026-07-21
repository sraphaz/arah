import 'package:flutter/material.dart';

import '../config/constants.dart';
import '../theme/app_design_tokens.dart';
import '../../l10n/app_localizations.dart';

/// Papel territorial normalizado para o badge.
enum ArahRoleKind { visitor, resident, curator }

/// Chip compacto de papel (visitante / morador / curador).
///
/// Aceita variantes EN/PT (`VISITOR`, `VISITANTE`, `RESIDENT`, `MORADOR`,
/// `CURATOR`, `CURADOR`, `MODERATOR`). Usa apenas tokens de tema.
class ArahRoleBadge extends StatelessWidget {
  const ArahRoleBadge({
    super.key,
    required this.role,
  });

  /// Papel bruto (API ou UI); normalizado internamente.
  final String role;

  static ArahRoleKind kindFrom(String role) {
    final r = role.trim().toUpperCase();
    if (r == 'RESIDENT' || r == 'MORADOR' || r == 'MORADORA') {
      return ArahRoleKind.resident;
    }
    if (r == 'CURATOR' || r == 'CURADOR' || r == 'MODERATOR') {
      return ArahRoleKind.curator;
    }
    return ArahRoleKind.visitor;
  }

  @override
  Widget build(BuildContext context) {
    final colors = context.appColors;
    final kind = kindFrom(role);
    final l10n = AppLocalizations.of(context);

    final (label, icon, bg, fg) = switch (kind) {
      ArahRoleKind.resident => (
          l10n?.resident ?? 'Morador',
          Icons.cottage_outlined,
          colors.accentSubtle,
          colors.primary,
        ),
      ArahRoleKind.curator => (
          l10n?.curator ?? 'Curador',
          Icons.verified_outlined,
          colors.info.withValues(alpha: 0.18),
          colors.info,
        ),
      ArahRoleKind.visitor => (
          l10n?.visitor ?? 'Visitante',
          Icons.explore_outlined,
          colors.onSurfaceSubtle.withValues(alpha: 0.14),
          colors.onSurfaceVariant,
        ),
    };

    return Semantics(
      label: label,
      excludeSemantics: true,
      child: Container(
        padding: const EdgeInsets.symmetric(
          horizontal: AppConstants.spacingSm,
          vertical: AppConstants.spacingXs,
        ),
        decoration: BoxDecoration(
          color: bg,
          borderRadius: BorderRadius.circular(AppConstants.radiusXl),
        ),
        child: Row(
          mainAxisSize: MainAxisSize.min,
          children: [
            Icon(icon, size: AppConstants.iconSizeSm, color: fg),
            const SizedBox(width: AppConstants.spacingXs),
            Text(
              label,
              style: Theme.of(context).textTheme.labelSmall?.copyWith(
                    color: fg,
                    fontWeight: FontWeight.w600,
                    letterSpacing: AppDesignTokens.letterSpacingWide * 0.4,
                    fontFamily: AppDesignTokens.fontFamilyBody,
                  ),
            ),
          ],
        ),
      ),
    );
  }
}
