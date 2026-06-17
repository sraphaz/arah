import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/config/constants.dart';
import '../../../../core/network/api_exception.dart';
import '../../../../core/providers/territory_provider.dart';
import '../../../../core/widgets/app_snackbar.dart';
import '../../../../core/widgets/arah_scaffold.dart';
import '../../../../l10n/app_localizations.dart';
import '../../../territories/presentation/widgets/territory_indicator_bar.dart';
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

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    final territoryId = ref.watch(selectedTerritoryIdValueProvider);
    final state = ref.watch(marketplaceProvider);
    final notifier = ref.read(marketplaceProvider.notifier);

    if (territoryId == null || territoryId.isEmpty) {
      return ArahScaffold(
        appBar: AppBar(title: Text(l10n.marketplace)),
        body: Center(child: Text(l10n.chooseTerritoryFirst)),
      );
    }

    return ArahScaffold(
      appBar: AppBar(
        title: Text(l10n.marketplace),
        bottom: TabBar(
          controller: _tabController,
          tabs: const [
            Tab(text: 'Buscar'),
            Tab(text: 'Minha loja'),
          ],
        ),
        actions: [
          if (_cartItemCount(state.cart) > 0)
            TextButton.icon(
              onPressed: () async {
                try {
                  await notifier.checkout(message: 'Checkout via app');
                  if (mounted) showSuccessSnackBar(context, 'Pedido enviado.');
                } catch (e) {
                  if (mounted) {
                    showErrorSnackBar(
                      context,
                      e is ApiException ? e.userMessage : 'Erro no checkout.',
                    );
                  }
                }
              },
              icon: const Icon(Icons.shopping_cart_checkout),
              label: Text(l10n.checkoutWithCount(_cartItemCount(state.cart))),
            ),
        ],
      ),
      body: Column(
        children: [
          const TerritoryIndicatorBar(),
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
    return Column(
      children: [
        Padding(
          padding: const EdgeInsets.all(AppConstants.spacingMd),
          child: TextField(
            controller: queryController,
            decoration: InputDecoration(
              hintText: 'Buscar itens',
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
      return const Center(child: CircularProgressIndicator());
    }
    if (state.error != null && state.items.isEmpty) {
      return Center(
        child: Text(
          state.error is ApiException
              ? (state.error as ApiException).userMessage
              : 'Erro ao buscar itens.',
        ),
      );
    }
    if (state.items.isEmpty) {
      return Center(child: Text(l10n.noItemsFound));
    }
    return ListView.builder(
      padding: const EdgeInsets.symmetric(horizontal: AppConstants.spacingMd),
      itemCount: state.items.length,
      itemBuilder: (context, index) {
        final item = state.items[index];
        return Card(
          margin: const EdgeInsets.only(bottom: AppConstants.spacingSm),
          child: ListTile(
            title: Text(item.title),
            subtitle: Text('${item.storeName} · ${item.currency} ${item.priceAmount.toStringAsFixed(2)}'),
            trailing: IconButton(
              icon: const Icon(Icons.add_shopping_cart_outlined),
              onPressed: () async {
                try {
                  await notifier.addToCart(item.id);
                  if (context.mounted) showSuccessSnackBar(context, 'Adicionado ao carrinho.');
                } catch (e) {
                  if (context.mounted) {
                    showErrorSnackBar(
                      context,
                      e is ApiException ? e.userMessage : 'Erro ao adicionar.',
                    );
                  }
                }
              },
            ),
          ),
        );
      },
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
    if (widget.state.isStoreLoading && widget.state.myStore == null) {
      return const Center(child: CircularProgressIndicator());
    }

    return ListView(
      padding: const EdgeInsets.all(AppConstants.spacingMd),
      children: [
        if (widget.state.myStore != null)
          Card(
            child: ListTile(
              leading: const Icon(Icons.storefront_outlined),
              title: Text(widget.state.myStore!.displayName),
              subtitle: Text(l10n.statusLabel(widget.state.myStore!.status)),
            ),
          ),
        const SizedBox(height: AppConstants.spacingMd),
        Text(
          widget.state.myStore == null ? 'Criar minha loja' : 'Atualizar loja',
          style: Theme.of(context).textTheme.titleMedium,
        ),
        const SizedBox(height: AppConstants.spacingSm),
        TextField(
          controller: _nameController,
          decoration: const InputDecoration(
            labelText: 'Nome da loja',
            border: OutlineInputBorder(),
          ),
        ),
        const SizedBox(height: AppConstants.spacingSm),
        TextField(
          controller: _descriptionController,
          decoration: const InputDecoration(
            labelText: 'Descrição',
            border: OutlineInputBorder(),
          ),
          maxLines: 3,
        ),
        const SizedBox(height: AppConstants.spacingLg),
        FilledButton(
          onPressed: () async {
            final name = _nameController.text.trim();
            if (name.isEmpty) {
              showErrorSnackBar(context, 'Informe o nome da loja.');
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
                  widget.state.myStore == null ? 'Loja criada.' : 'Loja atualizada.',
                );
              }
            } catch (e) {
              if (context.mounted) {
                showErrorSnackBar(
                  context,
                  e is ApiException ? e.userMessage : 'Erro ao salvar loja.',
                );
              }
            }
          },
          child: Text(widget.state.myStore == null ? 'Criar loja' : 'Salvar alterações'),
        ),
      ],
    );
  }
}
