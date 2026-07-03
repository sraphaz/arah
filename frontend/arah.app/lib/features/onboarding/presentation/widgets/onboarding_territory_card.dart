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
      elevation: isSelected ? 2 : 1,
      shape: RoundedRectangleBorder(
        borderRadius: BorderRadius.circular(AppConstants.radiusMd),
        side: isSelected
            ? BorderSide(color: colorScheme.primary, width: 2)
            : BorderSide.none,
      ),
      child: ListTile(
        contentPadding: const EdgeInsets.symmetric(
          horizontal: AppConstants.spacingMd,
          vertical: AppConstants.spacingSm,
        ),
        leading: CircleAvatar(
          backgroundColor: isSelected
              ? colorScheme.primaryContainer
              : colorScheme.surfaceContainerHighest,
          child: Icon(
            isSelected ? Icons.place : Icons.place_outlined,
            color: isSelected ? colorScheme.primary : colorScheme.onSurfaceVariant,
          ),
        ),
        title: Text(
          name,
          style: Theme.of(context).textTheme.titleSmall?.copyWith(
                fontWeight: isSelected ? FontWeight.w600 : null,
              ),
        ),
        subtitle: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            if (isPending)
              Padding(
                padding: const EdgeInsets.only(top: AppConstants.spacingXs),
                child: Chip(
                  label: Text(
                    AppLocalizations.of(context)!.territoryPendingBadge,
                    style: Theme.of(context).textTheme.labelSmall,
                  ),
                  visualDensity: VisualDensity.compact,
                  materialTapTargetSize: MaterialTapTargetSize.shrinkWrap,
                ),
              ),
            if (subtitle.isNotEmpty)
              Padding(
                padding: const EdgeInsets.only(top: AppConstants.spacingXs),
                child: Text(
                  subtitle,
                  style: Theme.of(context).textTheme.bodySmall?.copyWith(
                        color: Theme.of(context).colorScheme.onSurfaceVariant,
                      ),
                ),
              ),
          ],
        ),
        trailing: onTap != null
            ? Icon(
                isSelected ? Icons.check_circle : Icons.arrow_forward_ios,
                size: AppConstants.iconSizeSm,
                color: isSelected ? colorScheme.primary : colorScheme.onSurfaceVariant,
              )
            : null,
        onTap: onTap,
      ),
    );
  }
}
