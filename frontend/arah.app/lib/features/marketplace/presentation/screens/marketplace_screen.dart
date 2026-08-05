import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../core/config/constants.dart';
import '../../../../core/network/api_exception.dart';
import '../../../../core/providers/territory_provider.dart';
import '../../../../core/theme/app_design_tokens.dart';
import '../../../../core/widgets/app_snackbar.dart';
import '../../../../core/widgets/arah_card.dart';
import '../../../../core/widgets/arah_empty_state.dart';
import '../../../../core/widgets/arah_loading_indicator.dart';
import '../../../../core/widgets/arah_scaffold.dart';
import '../../../../l10n/app_localizations.dart';
import '../../../territories/presentation/widgets/territory_indicator_bar.dart';
import '../../data/models/marketplace_item.dart';
import '../providers/marketplace_provider.dart';

class MarketplaceScreen extends ConsumerStatefulWidget {
  const MarketplaceScreen({super.key});

  @override
  ConsumerState<MarketplaceScreen> createState() => _MarketplaceScreenState();
}

class _MarketplaceScreenState extends ConsumerState<MarketplaceScreen>
    with SingleTickerProviderStateMixin {
  final _queryController = TextEditingController();
  late final TabController _tabController;

  @override
  void initState() {
    super.initState();
    _tabController = TabController(length: 2, vsync: this);
    WidgetsBinding.instance.addPostFrameCallback((_) {
      ref.read(marketplaceProvider.notifier).search('');
      ref.read(marketplaceProvider.notifier).loadMyStore();
    });
  }

  @override
  void dispose() {
    _queryController.dispose();
    _tabController.dispose();
    super.dispose();
  }

  int _cartItemCount(Map<String, dynamic>? cart) {
    final items = cart?['items'] as List? ?? [];
    return items.length;
  }

  void _openCheckoutJourney() => context.push('/checkout-journey');

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    final colors = context.appColors;
    final territoryId = ref.watch(selectedTerritoryIdValueProvider);
    final state = ref.watch(marketplaceProvider);
    final notifier = ref.read(marketplaceProvider.notifier);
    final cartCount = _cartItemCount(state.cart);

    if (territoryId == null || territoryId.isEmpty) {
      return ArahScaffold(
        appBar: AppBar(title: Text(l10n.marketplace)),
        body: ArahEmptyState(
          icon: Icons.terrain_outlined,
          title: l10n.chooseTerritoryFirst,
        ),
      );
    }

    return ArahScaffold(
      appBar: AppBar(
        title: Text(l10n.marketplace),
        bottom: TabBar(
          controller: _tabController,
          tabs: [
            Tab(text: l10n.searchTab),
            Tab(text: l10n.myStore),
          ],
        ),
      ),
      body: Column(
        children: [
          const TerritoryIndicatorBar(),
          if (cartCount > 0)
            _CartBar(
              count: cartCount,
              label: l10n.checkoutWithCount(cartCount),
              onCheckout: _openCheckoutJourney,
              colors: colors,
            ),
          Expanded(
            child: TabBarView(
              controller: _tabController,
              children: [
                _SearchTab(
                  queryController: _queryController,
                  state: state,
                  notifier: notifier,
                ),
                _MyStoreTab(state: state, notifier: notifier),
              ],
            ),
          ),
        ],
      ),
    );
  }
}

class _CartBar extends StatelessWidget {
  const _CartBar({
    required this.count,
    required this.label,
    required this.onCheckout,
    required this.colors,
  });

  final int count;
  final String label;
  final VoidCallback onCheckout;
  final AppColors colors;

  @override
  Widget build(BuildContext context) {
    return Material(
      color: colors.accentSubtle,
      child: InkWell(
        onTap: onCheckout,
        child: Padding(
          padding: const EdgeInsets.symmetric(
            horizontal: AppConstants.spacingMd,
            vertical: AppConstants.spacingSm,
          ),
          child: Row(
            children: [
              Icon(Icons.shopping_cart_outlined, color: colors.primary, size: AppConstants.iconSizeMd),
              const SizedBox(width: AppConstants.spacingSm),
              Expanded(
                child: Text(
                  label,
                  style: Theme.of(context).textTheme.labelLarge?.copyWith(
                        color: colors.onSurface,
                      ),
                ),
              ),
              Container(
                padding: const EdgeInsets.symmetric(
                  horizontal: AppConstants.spacingSm,
                  vertical: AppConstants.spacingXs,
                ),
                decoration: BoxDecoration(
                  color: colors.primary.withValues(alpha: 0.15),
                  borderRadius: BorderRadius.circular(AppConstants.radiusSm),
                  border: Border.all(color: colors.accentBorder),
                ),
                child: Text(
                  '$count',
                  style: Theme.of(context).textTheme.labelMedium?.copyWith(
                        color: colors.primary,
                        fontWeight: FontWeight.w600,
                      ),
                ),
              ),
              const SizedBox(width: AppConstants.spacingSm),
              Icon(Icons.shopping_cart_checkout, color: colors.primary, size: AppConstants.iconSizeMd),
            ],
          ),
        ),
      ),
    );
  }
}

class _SearchTab extends StatelessWidget {
  const _SearchTab({
    required this.queryController,
    required this.state,
    required this.notifier,
  });

  final TextEditingController queryController;
  final MarketplaceState state;
  final MarketplaceNotifier notifier;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    return Column(
      children: [
        Padding(
          padding: const EdgeInsets.all(AppConstants.spacingMd),
          child: TextField(
            controller: queryController,
            decoration: InputDecoration(
              hintText: l10n.searchItemsHint,
              prefixIcon: const Icon(Icons.search),
              border: const OutlineInputBorder(),
              suffixIcon: IconButton(
                icon: const Icon(Icons.search),
                onPressed: () => notifier.search(queryController.text.trim()),
              ),
            ),
            onSubmitted: notifier.search,
          ),
        ),
        Expanded(child: _buildList(context)),
      ],
    );
  }

  Widget _buildList(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    if (state.isLoading && state.items.isEmpty) {
      return const Center(child: ArahLoadingIndicator());
    }
    if (state.error != null && state.items.isEmpty) {
      return ArahEmptyState(
        icon: Icons.error_outline,
        title: state.error is ApiException
            ? (state.error as ApiException).userMessage
            : l10n.errorSearchItems,
        actionLabel: l10n.tryAgain,
        onAction: () => notifier.search(queryController.text.trim()),
      );
    }
    if (state.items.isEmpty) {
      return ArahEmptyState(
        icon: Icons.storefront_outlined,
        title: l10n.noItemsFound,
      );
    }
    return ListView.separated(
      padding: const EdgeInsets.fromLTRB(
        AppConstants.spacingMd,
        0,
        AppConstants.spacingMd,
        AppConstants.spacingXl,
      ),
      itemCount: state.items.length,
      separatorBuilder: (_, __) => const SizedBox(height: AppConstants.spacingSm),
      itemBuilder: (context, index) {
        final item = state.items[index];
        return _ProductCard(
          item: item,
          onAddToCart: () async {
            try {
              await notifier.addToCart(item.id);
              if (context.mounted) showSuccessSnackBar(context, l10n.addedToCart);
            } catch (e) {
              if (context.mounted) {
                showErrorSnackBar(
                  context,
                  e is ApiException ? e.userMessage : l10n.errorAddToCart,
                );
              }
            }
          },
        );
      },
    );
  }
}

class _ProductCard extends StatelessWidget {
  const _ProductCard({
    required this.item,
    required this.onAddToCart,
  });

  final MarketplaceSearchItem item;
  final VoidCallback onAddToCart;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    final colors = context.appColors;
    final price = l10n.priceLabel(item.currency, item.priceAmount.toStringAsFixed(2));

    return Card(
      child: Padding(
        padding: const EdgeInsets.all(AppConstants.spacingMd),
        child: Row(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Container(
              width: AppConstants.avatarSizeLg,
              height: AppConstants.avatarSizeLg,
              decoration: BoxDecoration(
                color: colors.surfaceElevated,
                borderRadius: BorderRadius.circular(AppConstants.radiusMd),
                border: Border.all(color: colors.outlineSubtle),
              ),
              clipBehavior: Clip.antiAlias,
              child: item.primaryImageUrl != null && item.primaryImageUrl!.isNotEmpty
                  ? Image.network(
                      item.primaryImageUrl!,
                      fit: BoxFit.cover,
                      errorBuilder: (_, __, ___) => Icon(
                        Icons.inventory_2_outlined,
                        color: colors.onSurfaceSubtle,
                      ),
                    )
                  : Icon(
                      Icons.inventory_2_outlined,
                      color: colors.onSurfaceSubtle,
                    ),
            ),
            const SizedBox(width: AppConstants.spacingMd),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    item.title,
                    style: Theme.of(context).textTheme.titleMedium,
                    maxLines: 2,
                    overflow: TextOverflow.ellipsis,
                  ),
                  const SizedBox(height: AppConstants.spacingXs),
                  Text(
                    item.storeName,
                    style: Theme.of(context).textTheme.bodySmall?.copyWith(
                          color: colors.onSurfaceVariant,
                        ),
                    maxLines: 1,
                    overflow: TextOverflow.ellipsis,
                  ),
                  const SizedBox(height: AppConstants.spacingSm),
                  Text(
                    price,
                    style: Theme.of(context).textTheme.titleSmall?.copyWith(
                          color: colors.primary,
                          fontWeight: FontWeight.w600,
                        ),
                  ),
                ],
              ),
            ),
            const SizedBox(width: AppConstants.spacingSm),
            IconButton(
              onPressed: onAddToCart,
              style: IconButton.styleFrom(
                backgroundColor: colors.primary.withValues(alpha: 0.12),
                foregroundColor: colors.primary,
              ),
              icon: const Icon(Icons.add_shopping_cart_outlined),
              tooltip: l10n.addedToCart,
            ),
          ],
        ),
      ),
    );
  }
}

class _MyStoreTab extends StatefulWidget {
  const _MyStoreTab({required this.state, required this.notifier});

  final MarketplaceState state;
  final MarketplaceNotifier notifier;

  @override
  State<_MyStoreTab> createState() => _MyStoreTabState();
}

class _MyStoreTabState extends State<_MyStoreTab> {
  late final TextEditingController _nameController;
  late final TextEditingController _descriptionController;

  @override
  void initState() {
    super.initState();
    _nameController = TextEditingController(text: widget.state.myStore?.displayName ?? '');
    _descriptionController = TextEditingController(text: widget.state.myStore?.description ?? '');
  }

  @override
  void didUpdateWidget(covariant _MyStoreTab oldWidget) {
    super.didUpdateWidget(oldWidget);
    if (widget.state.myStore?.displayName != oldWidget.state.myStore?.displayName) {
      _nameController.text = widget.state.myStore?.displayName ?? '';
    }
    if (widget.state.myStore?.description != oldWidget.state.myStore?.description) {
      _descriptionController.text = widget.state.myStore?.description ?? '';
    }
  }

  @override
  void dispose() {
    _nameController.dispose();
    _descriptionController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    final colors = context.appColors;

    if (widget.state.isStoreLoading && widget.state.myStore == null) {
      return const Center(child: ArahLoadingIndicator());
    }

    final store = widget.state.myStore;

    return ListView(
      padding: const EdgeInsets.all(AppConstants.spacingMd),
      children: [
        if (store == null)
          ArahEmptyState(
            icon: Icons.storefront_outlined,
            title: l10n.openStoreEmptyTitle,
            description: l10n.openStoreEmptyDescription,
          ),
        if (store != null) ...[
          ArahCard(
            child: Row(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Container(
                  width: AppConstants.avatarSizeSm,
                  height: AppConstants.avatarSizeSm,
                  decoration: BoxDecoration(
                    color: colors.primary.withValues(alpha: 0.12),
                    borderRadius: BorderRadius.circular(AppConstants.radiusMd),
                  ),
                  child: Icon(Icons.storefront_outlined, color: colors.primary),
                ),
                const SizedBox(width: AppConstants.spacingMd),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        store.displayName,
                        style: Theme.of(context).textTheme.titleMedium,
                      ),
                      const SizedBox(height: AppConstants.spacingXs),
                      Text(
                        l10n.statusLabel(store.status),
                        style: Theme.of(context).textTheme.bodySmall?.copyWith(
                              color: colors.onSurfaceVariant,
                            ),
                      ),
                      if (store.description != null &&
                          store.description!.isNotEmpty) ...[
                        const SizedBox(height: AppConstants.spacingSm),
                        Text(
                          store.description!,
                          style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                                color: colors.onSurfaceVariant,
                              ),
                        ),
                      ],
                    ],
                  ),
                ),
              ],
            ),
          ),
          const SizedBox(height: AppConstants.spacingMd),
          Text(l10n.storePaymentsTitle, style: Theme.of(context).textTheme.titleMedium),
          const SizedBox(height: AppConstants.spacingSm),
          ArahCard(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                SwitchListTile(
                  contentPadding: EdgeInsets.zero,
                  title: Text(l10n.storePaymentsEnabled),
                  subtitle: Text(
                    store.paymentsEnabled
                        ? l10n.storePaymentsEnabledHint
                        : l10n.storePaymentsDisabledHint,
                  ),
                  value: store.paymentsEnabled,
                  onChanged: (value) async {
                    try {
                      await widget.notifier.setPaymentsEnabled(value);
                      if (context.mounted) {
                        showSuccessSnackBar(
                          context,
                          value
                              ? l10n.storePaymentsEnabled
                              : l10n.storePaymentsDisabledHint,
                        );
                      }
                    } catch (e) {
                      if (context.mounted) {
                        showErrorSnackBar(
                          context,
                          e is ApiException
                              ? e.userMessage
                              : l10n.errorSaveStore,
                        );
                      }
                    }
                  },
                ),
                const Divider(height: AppConstants.spacingLg),
                Text(
                  l10n.storePixKeyLabel,
                  style: Theme.of(context).textTheme.labelLarge,
                ),
                const SizedBox(height: AppConstants.spacingXs),
                Text(
                  l10n.storePixKeyHint,
                  style: Theme.of(context).textTheme.bodySmall?.copyWith(
                        color: colors.onSurfaceVariant,
                      ),
                ),
              ],
            ),
          ),
          const SizedBox(height: AppConstants.spacingMd),
          Row(
            children: [
              Expanded(
                child: Text(
                  l10n.myProductsTitle,
                  style: Theme.of(context).textTheme.titleMedium,
                ),
              ),
              TextButton.icon(
                onPressed: () async {
                  await context.push('/add-product-journey');
                  if (context.mounted) {
                    await widget.notifier.loadMyProducts();
                  }
                },
                icon: const Icon(Icons.add, size: 18),
                label: Text(l10n.addProduct),
              ),
            ],
          ),
          const SizedBox(height: AppConstants.spacingSm),
          if (widget.state.isProductsLoading)
            const Padding(
              padding: EdgeInsets.all(AppConstants.spacingLg),
              child: Center(child: ArahLoadingIndicator()),
            )
          else if (widget.state.myProducts.isEmpty)
            ArahCard(
              child: Text(
                l10n.myProductsEmpty,
                style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                      color: colors.onSurfaceVariant,
                    ),
              ),
            )
          else
            for (final product in widget.state.myProducts)
              ArahCard(
                margin: const EdgeInsets.only(bottom: AppConstants.spacingSm),
                onTap: () async {
                  await context.push('/add-product-journey?itemId=${product.id}');
                  if (context.mounted) {
                    await widget.notifier.loadMyProducts();
                  }
                },
                child: Row(
                  children: [
                    Icon(Icons.inventory_2_outlined, color: colors.primary),
                    const SizedBox(width: AppConstants.spacingMd),
                    Expanded(
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Text(
                            product.title,
                            style: Theme.of(context).textTheme.titleSmall,
                          ),
                          Text(
                            product.priceLabel,
                            style: Theme.of(context).textTheme.bodySmall?.copyWith(
                                  color: colors.onSurfaceVariant,
                                ),
                          ),
                        ],
                      ),
                    ),
                    IconButton(
                      tooltip: l10n.archiveProduct,
                      onPressed: () async {
                        final ok = await showDialog<bool>(
                          context: context,
                          builder: (ctx) => AlertDialog(
                            title: Text(l10n.archiveProduct),
                            content: Text(l10n.archiveProductConfirm),
                            actions: [
                              TextButton(
                                onPressed: () => Navigator.pop(ctx, false),
                                child: Text(l10n.cancel),
                              ),
                              FilledButton(
                                onPressed: () => Navigator.pop(ctx, true),
                                child: Text(l10n.archive),
                              ),
                            ],
                          ),
                        );
                        if (ok != true || !context.mounted) return;
                        try {
                          await widget.notifier.archiveProduct(product.id);
                          if (context.mounted) {
                            showSuccessSnackBar(context, l10n.productArchived);
                          }
                        } catch (e) {
                          if (context.mounted) {
                            showErrorSnackBar(
                              context,
                              e is ApiException
                                  ? e.userMessage
                                  : l10n.errorSaveProduct,
                            );
                          }
                        }
                      },
                      icon: Icon(
                        Icons.archive_outlined,
                        color: colors.onSurfaceVariant,
                      ),
                    ),
                  ],
                ),
              ),
        ],
        const SizedBox(height: AppConstants.spacingMd),
        Text(
          store == null ? l10n.createMyStore : l10n.updateStore,
          style: Theme.of(context).textTheme.titleMedium,
        ),
        const SizedBox(height: AppConstants.spacingSm),
        ArahCard(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              TextField(
                controller: _nameController,
                decoration: InputDecoration(
                  labelText: l10n.storeNameLabel,
                  border: const OutlineInputBorder(),
                ),
              ),
              const SizedBox(height: AppConstants.spacingSm),
              TextField(
                controller: _descriptionController,
                decoration: InputDecoration(
                  labelText: l10n.descriptionLabel,
                  border: const OutlineInputBorder(),
                ),
                maxLines: 3,
              ),
              const SizedBox(height: AppConstants.spacingLg),
              FilledButton(
                onPressed: () async {
                  final name = _nameController.text.trim();
                  if (name.isEmpty) {
                    showErrorSnackBar(context, l10n.informStoreName);
                    return;
                  }
                  try {
                    await widget.notifier.saveMyStore(
                      displayName: name,
                      description: _descriptionController.text.trim().isEmpty
                          ? null
                          : _descriptionController.text.trim(),
                    );
                    if (context.mounted) {
                      showSuccessSnackBar(
                        context,
                        store == null ? l10n.storeCreated : l10n.storeUpdated,
                      );
                    }
                  } catch (e) {
                    if (context.mounted) {
                      showErrorSnackBar(
                        context,
                        e is ApiException ? e.userMessage : l10n.errorSaveStore,
                      );
                    }
                  }
                },
                child: Text(store == null ? l10n.createStore : l10n.saveChanges),
              ),
            ],
          ),
        ),
      ],
    );
  }
}
