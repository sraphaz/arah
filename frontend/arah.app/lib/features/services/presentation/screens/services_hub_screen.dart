import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

import '../../../../core/config/constants.dart';
import '../../../../core/theme/app_design_tokens.dart';
import '../../../../l10n/app_localizations.dart';

/// Hub de Serviços: diretório categorizado (live → rota; soon → SnackBar).
class ServicesHubScreen extends StatelessWidget {
  const ServicesHubScreen({super.key});

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    final categories = _buildCategories(l10n);

    return ListView(
      padding: const EdgeInsets.fromLTRB(
        AppConstants.spacingMd,
        AppConstants.spacingMd,
        AppConstants.spacingMd,
        AppConstants.spacingXl,
      ),
      children: [
        Text(
          l10n.services,
          style: Theme.of(context).textTheme.titleLarge,
        ),
        const SizedBox(height: AppConstants.spacingXs),
        Text(
          l10n.servicesHubSubtitle,
          style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                color: Theme.of(context).colorScheme.onSurfaceVariant,
              ),
        ),
        const SizedBox(height: AppConstants.spacingLg),
        for (final category in categories) ...[
          _CategoryHeader(title: category.title, icon: category.icon),
          const SizedBox(height: AppConstants.spacingSm),
          for (final item in category.items)
            _ServiceTile(
              item: item,
              statusLiveLabel: l10n.statusLive,
              statusSoonLabel: l10n.statusSoon,
              onTap: () => _onItemTap(context, item, l10n),
            ),
          const SizedBox(height: AppConstants.spacingLg),
        ],
      ],
    );
  }

  void _onItemTap(
    BuildContext context,
    _ServiceItem item,
    AppLocalizations l10n,
  ) {
    final route = item.route;
    if (route != null) {
      context.push(route);
      return;
    }
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(content: Text(l10n.comingSoon)),
    );
  }

  List<_ServiceCategory> _buildCategories(AppLocalizations l10n) {
    return [
      _ServiceCategory(
        title: l10n.servicesCategoryTools,
        icon: Icons.handyman_outlined,
        items: [
          _ServiceItem(
            title: l10n.chat,
            icon: Icons.chat_outlined,
            route: '/chat',
          ),
          _ServiceItem(
            title: l10n.events,
            icon: Icons.event_outlined,
            route: '/events',
          ),
          _ServiceItem(
            title: l10n.map,
            icon: Icons.map_outlined,
            route: '/map',
          ),
          _ServiceItem(
            title: l10n.alertsTitle,
            icon: Icons.warning_amber_outlined,
            route: '/alerts',
          ),
        ],
      ),
      _ServiceCategory(
        title: l10n.servicesCategoryEconomy,
        icon: Icons.payments_outlined,
        items: [
          _ServiceItem(
            title: l10n.marketplace,
            icon: Icons.storefront_outlined,
            route: '/marketplace',
          ),
          _ServiceItem(
            title: l10n.serviceGroupBuy,
            icon: Icons.groups_outlined,
            phase: 'F17',
          ),
          _ServiceItem(
            title: l10n.serviceHosting,
            icon: Icons.cottage_outlined,
            phase: 'F18',
          ),
          _ServiceItem(
            title: l10n.serviceDemands,
            icon: Icons.swap_horiz_outlined,
            phase: 'F19',
          ),
          _ServiceItem(
            title: l10n.serviceTrades,
            icon: Icons.handshake_outlined,
            phase: 'F20',
          ),
          _ServiceItem(
            title: l10n.serviceDeliveries,
            icon: Icons.local_shipping_outlined,
            phase: 'F21',
          ),
          _ServiceItem(
            title: l10n.serviceWallet,
            icon: Icons.account_balance_wallet_outlined,
            phase: 'F22',
          ),
        ],
      ),
      _ServiceCategory(
        title: l10n.servicesCategoryTerritoryServices,
        icon: Icons.room_service_outlined,
        items: [
          _ServiceItem(
            title: l10n.serviceBabysitters,
            icon: Icons.child_care_outlined,
            phase: 'E10',
          ),
          _ServiceItem(
            title: l10n.serviceWellness,
            icon: Icons.self_improvement_outlined,
            phase: 'E10',
          ),
          _ServiceItem(
            title: l10n.serviceRentals,
            icon: Icons.category_outlined,
            phase: 'F46',
          ),
          _ServiceItem(
            title: l10n.serviceDigitalHub,
            icon: Icons.apps_outlined,
            phase: 'F26',
          ),
        ],
      ),
      _ServiceCategory(
        title: l10n.servicesCategoryGovernance,
        icon: Icons.how_to_vote_outlined,
        items: [
          _ServiceItem(
            title: l10n.governance,
            icon: Icons.how_to_vote_outlined,
            route: '/governance',
          ),
          _ServiceItem(
            title: l10n.moderation,
            icon: Icons.gavel_outlined,
            route: '/moderation',
          ),
          _ServiceItem(
            title: l10n.subscriptions,
            icon: Icons.card_membership_outlined,
            route: '/subscriptions',
          ),
        ],
      ),
      _ServiceCategory(
        title: l10n.servicesCategoryLife,
        icon: Icons.eco_outlined,
        items: [
          _ServiceItem(
            title: l10n.serviceTerritoryHealth,
            icon: Icons.water_drop_outlined,
            phase: 'F24',
          ),
          _ServiceItem(
            title: l10n.serviceMetrics,
            icon: Icons.insights_outlined,
            phase: 'F25',
          ),
          _ServiceItem(
            title: l10n.serviceSeeds,
            icon: Icons.yard_outlined,
            phase: 'F48',
          ),
          _ServiceItem(
            title: l10n.serviceLearning,
            icon: Icons.school_outlined,
            phase: 'F45',
          ),
          _ServiceItem(
            title: l10n.serviceAiAssistant,
            icon: Icons.auto_awesome_outlined,
            phase: 'F27',
          ),
          _ServiceItem(
            title: l10n.serviceAchievements,
            icon: Icons.workspace_premium_outlined,
            phase: 'F42',
          ),
        ],
      ),
    ];
  }
}

class _ServiceCategory {
  const _ServiceCategory({
    required this.title,
    required this.icon,
    required this.items,
  });

  final String title;
  final IconData icon;
  final List<_ServiceItem> items;
}

class _ServiceItem {
  const _ServiceItem({
    required this.title,
    required this.icon,
    this.route,
    this.phase,
  });

  final String title;
  final IconData icon;
  final String? route;
  final String? phase;

  bool get isLive => route != null;
}

class _CategoryHeader extends StatelessWidget {
  const _CategoryHeader({required this.title, required this.icon});

  final String title;
  final IconData icon;

  @override
  Widget build(BuildContext context) {
    final colors = context.appColors;
    return Row(
      children: [
        Icon(icon, size: AppConstants.iconSizeMd, color: colors.primary),
        const SizedBox(width: AppConstants.spacingSm),
        Expanded(
          child: Text(
            title,
            style: Theme.of(context).textTheme.titleMedium,
          ),
        ),
      ],
    );
  }
}

class _ServiceTile extends StatelessWidget {
  const _ServiceTile({
    required this.item,
    required this.statusLiveLabel,
    required this.statusSoonLabel,
    required this.onTap,
  });

  final _ServiceItem item;
  final String statusLiveLabel;
  final String statusSoonLabel;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    final colors = context.appColors;
    final isLive = item.isLive;
    final chipLabel = isLive
        ? statusLiveLabel
        : item.phase != null
            ? '$statusSoonLabel · ${item.phase}'
            : statusSoonLabel;

    return Card(
      margin: const EdgeInsets.only(bottom: AppConstants.spacingSm),
      child: ListTile(
        contentPadding: const EdgeInsets.symmetric(
          horizontal: AppConstants.spacingMd,
          vertical: AppConstants.spacingXs,
        ),
        leading: Icon(
          item.icon,
          color: isLive ? colors.primary : colors.onSurfaceVariant,
        ),
        title: Text(item.title),
        subtitle: item.phase != null && !isLive
            ? Text(
                item.phase!,
                style: Theme.of(context).textTheme.bodySmall?.copyWith(
                      color: colors.onSurfaceSubtle,
                    ),
              )
            : null,
        trailing: Chip(
          label: Text(chipLabel),
          labelStyle: Theme.of(context).textTheme.labelSmall?.copyWith(
                color: isLive ? colors.success : AppDesignTokens.warning,
                fontWeight: FontWeight.w600,
              ),
          backgroundColor: isLive
              ? colors.accentSubtle
              : AppDesignTokens.warning.withValues(alpha: 0.16),
          side: BorderSide(
            color: isLive ? colors.accentBorder : AppDesignTokens.warning.withValues(alpha: 0.4),
          ),
          visualDensity: VisualDensity.compact,
          materialTapTargetSize: MaterialTapTargetSize.shrinkWrap,
          padding: const EdgeInsets.symmetric(horizontal: AppConstants.spacingXs),
        ),
        onTap: onTap,
      ),
    );
  }
}
