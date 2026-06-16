import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/config/constants.dart';
import '../../../../core/network/api_exception.dart';
import '../../../../core/providers/territory_provider.dart';
import '../../../../core/widgets/app_snackbar.dart';
import '../../../territories/presentation/widgets/territory_indicator_bar.dart';
import '../providers/assets_provider.dart';

class AssetsScreen extends ConsumerWidget {
  const AssetsScreen({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final territoryId = ref.watch(selectedTerritoryIdValueProvider);
    final state = ref.watch(assetsProvider);
    final notifier = ref.read(assetsProvider.notifier);

    if (territoryId == null || territoryId.isEmpty) {
      return Scaffold(
        appBar: AppBar(title: const Text('Assets')),
        body: const Center(child: Text('Escolha um território primeiro.')),
      );
    }

    return Scaffold(
      appBar: AppBar(title: const Text('Assets')),
      floatingActionButton: FloatingActionButton(
        onPressed: () => _showCreateDialog(context, ref),
        child: const Icon(Icons.add),
      ),
      body: Column(
        children: [
          const TerritoryIndicatorBar(),
          Expanded(
            child: RefreshIndicator(
              onRefresh: () => notifier.refresh(),
              child: _buildBody(context, state),
            ),
          ),
        ],
      ),
    );
  }

  Future<void> _showCreateDialog(BuildContext context, WidgetRef ref) async {
    final nameController = TextEditingController();
    final typeController = TextEditingController(text: 'Infrastructure');
    await showDialog<void>(
      context: context,
      builder: (ctx) => AlertDialog(
        title: const Text('Novo asset'),
        content: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            TextField(controller: nameController, decoration: const InputDecoration(labelText: 'Nome')),
            TextField(controller: typeController, decoration: const InputDecoration(labelText: 'Tipo')),
          ],
        ),
        actions: [
          TextButton(onPressed: () => Navigator.pop(ctx), child: const Text('Cancelar')),
          FilledButton(
            onPressed: () async {
              try {
                await ref.read(assetsProvider.notifier).createAsset(
                      name: nameController.text.trim(),
                      type: typeController.text.trim(),
                    );
                if (ctx.mounted) {
                  Navigator.pop(ctx);
                  showSuccessSnackBar(ctx, 'Asset criado.');
                }
              } catch (e) {
                if (ctx.mounted) {
                  showErrorSnackBar(
                    ctx,
                    e is ApiException ? e.userMessage : 'Erro ao criar asset.',
                  );
                }
              }
            },
            child: const Text('Criar'),
          ),
        ],
      ),
    );
  }

  Widget _buildBody(BuildContext context, AssetsState state) {
    if (state.isLoading && state.items.isEmpty) {
      return const ListView(
        physics: AlwaysScrollableScrollPhysics(),
        children: [SizedBox(height: 200, child: Center(child: CircularProgressIndicator()))],
      );
    }
    if (state.error != null && state.items.isEmpty) {
      return ListView(
        physics: const AlwaysScrollableScrollPhysics(),
        children: [
          Padding(
            padding: const EdgeInsets.all(AppConstants.spacingLg),
            child: Text(
              state.error is ApiException
                  ? (state.error as ApiException).userMessage
                  : 'Erro ao carregar assets.',
              textAlign: TextAlign.center,
            ),
          ),
        ],
      );
    }
    if (state.items.isEmpty) {
      return const ListView(
        physics: AlwaysScrollableScrollPhysics(),
        children: [SizedBox(height: 200, child: Center(child: Text('Nenhum asset cadastrado.')))],
      );
    }
    return ListView.builder(
      physics: const AlwaysScrollableScrollPhysics(),
      padding: const EdgeInsets.all(AppConstants.spacingMd),
      itemCount: state.items.length,
      itemBuilder: (context, index) {
        final asset = state.items[index];
        return Card(
          child: ListTile(
            title: Text(asset.name),
            subtitle: Text('${asset.type} · ${asset.status}'),
          ),
        );
      },
    );
  }
}
