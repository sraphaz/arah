import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/config/constants.dart';
import '../../../../core/network/api_exception.dart';
import '../../../../core/providers/territory_provider.dart';
import '../../../../core/widgets/app_snackbar.dart';
import '../../../territories/presentation/widgets/territory_indicator_bar.dart';
import '../providers/marketplace_provider.dart';

class MarketplaceScreen extends ConsumerStatefulWidget {
  const MarketplaceScreen({super.key});

  @override
  ConsumerState<MarketplaceScreen> createState() => _MarketplaceScreenState();
}

class _MarketplaceScreenState extends ConsumerState<MarketplaceScreen> {
  final _queryController = TextEditingController();

  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) {
      ref.read(marketplaceProvider.notifier).search('');
    });
  }

  @override
  void dispose() {
    _queryController.dispose();
    super.dispose();
  }

  int _cartItemCount(Map<String, dynamic>? cart) {
    final items = cart?['items'] as List? ?? [];
    return items.length;
  }

  @override
  Widget build(BuildContext context) {
    final territoryId = ref.watch(selectedTerritoryIdValueProvider);
    final state = ref.watch(marketplaceProvider);
    final notifier = ref.read(marketplaceProvider.notifier);

    if (territoryId == null || territoryId.isEmpty) {
      return Scaffold(
        appBar: AppBar(title: const Text('Marketplace')),
        body: const Center(child: Text('Escolha um território primeiro.')),
      );
    }

    return Scaffold(
      appBar: AppBar(
        title: const Text('Marketplace'),
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
              label: Text('Checkout (${_cartItemCount(state.cart)})'),
            ),
        ],
      ),
      body: Column(
        children: [
          const TerritoryIndicatorBar(),
          Padding(
            padding: const EdgeInsets.all(AppConstants.spacingMd),
            child: TextField(
              controller: _queryController,
              decoration: InputDecoration(
                hintText: 'Buscar itens',
                prefixIcon: const Icon(Icons.search),
                border: const OutlineInputBorder(),
                suffixIcon: IconButton(
                  icon: const Icon(Icons.search),
                  onPressed: () => notifier.search(_queryController.text.trim()),
                ),
              ),
              onSubmitted: notifier.search,
            ),
          ),
          Expanded(child: _buildList(context, state, notifier)),
        ],
      ),
    );
  }

  Widget _buildList(BuildContext context, MarketplaceState state, MarketplaceNotifier notifier) {
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
      return const Center(child: Text('Nenhum item encontrado.'));
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
                  if (mounted) showSuccessSnackBar(context, 'Adicionado ao carrinho.');
                } catch (e) {
                  if (mounted) {
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
