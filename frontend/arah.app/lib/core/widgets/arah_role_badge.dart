import 'package:flutter/material.dart';

import '../config/constants.dart';
import '../theme/app_design_tokens.dart';
import 'arah_material_symbol.dart';
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
          'cottage',
          colors.primary.withValues(alpha: 0.13),
          colors.primary,
        ),
      ArahRoleKind.curator => (
          l10n?.curator ?? 'Curador',
          'verified',
          colors.info.withValues(alpha: 0.18),
          colors.info,
        ),
      ArahRoleKind.visitor => (
          l10n?.visitor ?? 'Visitante',
          'explore',
          colors.onSurfaceSubtle.withValues(alpha: 0.14),
          colors.onSurfaceVariant,
        ),
    };

    return Semantics(
      label: label,
      excludeSemantics: true,
      child: Container(
        padding: const EdgeInsets.symmetric(horizontal: 7, vertical: 2),
        decoration: BoxDecoration(
          color: bg,
          borderRadius: BorderRadius.circular(AppConstants.radiusXl),
        ),
        child: Row(
          mainAxisSize: MainAxisSize.min,
          children: [
            ArahMaterialSymbol(
              icon,
              size: 12.5,
              fill:
                  kind == ArahRoleKind.resident || kind == ArahRoleKind.curator
                      ? 1
                      : 0,
              color: fg,
            ),
            const SizedBox(width: 4),
            Text(
              label,
              style: Theme.of(context).textTheme.labelSmall?.copyWith(
                    fontSize: 10.5,
                    color: fg,
                    fontWeight: FontWeight.w600,
                    letterSpacing: 0.2,
                    fontFamily: AppDesignTokens.fontFamilyBody,
                  ),
            ),
          ],
        ),
      ),
    );
  }
}
