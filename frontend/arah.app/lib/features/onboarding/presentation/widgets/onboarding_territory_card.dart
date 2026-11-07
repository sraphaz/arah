import 'package:flutter/material.dart';

import '../../../../core/config/constants.dart';
import '../../../../l10n/app_localizations.dart';

/// Card de um território sugerido na lista do onboarding.
class OnboardingTerritoryCard extends StatelessWidget {
  const OnboardingTerritoryCard({
    super.key,
    required this.name,
    required this.subtitle,
    this.isSelected = false,
    this.isPending = false,
    this.onTap,
  });

  final String name;
  final String subtitle;
  final bool isSelected;
  final bool isPending;
  final VoidCallback? onTap;

  @override
  Widget build(BuildContext context) {
    final colorScheme = Theme.of(context).colorScheme;
    return Card(
      elevation: isSelected ? 2 : 0,
      shape: RoundedRectangleBorder(
        borderRadius: BorderRadius.circular(AppConstants.radiusMd),
        side: isSelected
            ? BorderSide(color: colorScheme.primary, width: 1.5)
            : BorderSide(color: colorScheme.outline.withValues(alpha: 0.35)),
      ),
      child: Material(
        color: Colors.transparent,
        child: InkWell(
          onTap: onTap,
          borderRadius: BorderRadius.circular(AppConstants.radiusMd),
          child: Padding(
            padding: const EdgeInsets.symmetric(
              horizontal: AppConstants.spacingMd,
              vertical: 13,
            ),
            child: Row(
              children: [
                Icon(
                  isSelected ? Icons.place : Icons.place_outlined,
                  color: isSelected
                      ? colorScheme.primary
                      : colorScheme.onSurfaceVariant,
                  size: 22,
                ),
                const SizedBox(width: 12),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    mainAxisSize: MainAxisSize.min,
                    children: [
                      Text(
                        name,
                        style:
                            Theme.of(context).textTheme.titleMedium?.copyWith(
                                  fontSize: 15,
                                  fontWeight: FontWeight.w600,
                                ),
                      ),
                      if (isPending)
                        Padding(
                          padding: const EdgeInsets.only(
                              top: AppConstants.spacingXs),
                          child: Chip(
                            label: Text(
                              AppLocalizations.of(context)!
                                  .territoryPendingBadge,
                              style: Theme.of(context).textTheme.labelSmall,
                            ),
                            visualDensity: VisualDensity.compact,
                            materialTapTargetSize:
                                MaterialTapTargetSize.shrinkWrap,
                          ),
                        ),
                      if (subtitle.isNotEmpty)
                        Padding(
                          padding: const EdgeInsets.only(
                              top: AppConstants.spacingXs),
                          child: Text(
                            subtitle,
                            style:
                                Theme.of(context).textTheme.bodySmall?.copyWith(
                                      color: Theme.of(context)
                                          .colorScheme
                                          .onSurfaceVariant,
                                    ),
                          ),
                        ),
                    ],
                  ),
                ),
                if (onTap != null)
                  Icon(
                    isSelected
                        ? Icons.check_circle
                        : Icons.radio_button_unchecked,
                    size: 21,
                    color: isSelected
                        ? colorScheme.primary
                        : colorScheme.onSurfaceVariant,
                  ),
              ],
            ),
          ),
        ),
      ),
    );
  }
}
