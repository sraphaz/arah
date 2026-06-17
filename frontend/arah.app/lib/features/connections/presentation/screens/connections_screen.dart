import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/config/constants.dart';
import '../../../../core/network/api_exception.dart';
import '../../../../core/widgets/app_snackbar.dart';
import '../../data/models/connection_item.dart';
import '../../data/models/connection_user.dart';
import '../providers/connections_provider.dart';

/// Círculo de amigos: conexões aceitas, pendentes e busca para enviar solicitação.
class ConnectionsScreen extends ConsumerWidget {
  const ConnectionsScreen({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final state = ref.watch(connectionsProvider);
    final notifier = ref.read(connectionsProvider.notifier);

    return Scaffold(
      appBar: AppBar(title: const Text('Conexões')),
      floatingActionButton: FloatingActionButton.extended(
        onPressed: () => _openSearchSheet(context, ref),
        icon: const Icon(Icons.person_add_outlined),
        label: const Text('Adicionar'),
      ),
      body: RefreshIndicator(
        onRefresh: () => notifier.refresh(),
        child: _buildBody(context, state, notifier),
      ),
    );
  }

  Future<void> _openSearchSheet(BuildContext context, WidgetRef ref) async {
    await showModalBottomSheet<void>(
      context: context,
      isScrollControlled: true,
      useSafeArea: true,
      builder: (ctx) => _ConnectionSearchSheet(
        onRequest: (userId) async {
          try {
            await ref.read(connectionsProvider.notifier).sendRequest(userId);
            if (ctx.mounted) {
              showSuccessSnackBar(ctx, 'Solicitação enviada.');
              Navigator.pop(ctx);
            }
          } catch (e) {
            if (ctx.mounted) {
              showErrorSnackBar(
                ctx,
                e is ApiException ? e.userMessage : 'Erro ao enviar solicitação.',
              );
            }
          }
        },
      ),
    );
  }

  Widget _buildBody(BuildContext context, ConnectionsState state, ConnectionsNotifier notifier) {
    if (state.isLoading && state.accepted.isEmpty && state.pending.isEmpty) {
      return ListView(
        physics: AlwaysScrollableScrollPhysics(),
        children: [SizedBox(height: 240, child: Center(child: CircularProgressIndicator()))],
      );
    }

    if (state.error != null && state.accepted.isEmpty && state.pending.isEmpty) {
      final message = state.error is ApiException
          ? (state.error as ApiException).userMessage
          : 'Não foi possível carregar conexões.';
      return ListView(
        physics: const AlwaysScrollableScrollPhysics(),
        children: [
          Padding(
            padding: const EdgeInsets.all(AppConstants.spacingLg),
            child: Column(
              children: [
                Text(message, textAlign: TextAlign.center),
                const SizedBox(height: AppConstants.spacingMd),
                FilledButton.tonal(onPressed: () => notifier.refresh(), child: const Text('Tentar novamente')),
              ],
            ),
          ),
        ],
      );
    }

    return ListView(
      physics: const AlwaysScrollableScrollPhysics(),
      padding: const EdgeInsets.fromLTRB(
        AppConstants.spacingMd,
        AppConstants.spacingMd,
        AppConstants.spacingMd,
        96,
      ),
      children: [
        if (state.pending.isNotEmpty) ...[
          Text('Pendentes', style: Theme.of(context).textTheme.titleMedium),
          const SizedBox(height: AppConstants.spacingSm),
          ...state.pending.map((item) => _ConnectionTile(item: item, notifier: notifier, isPending: true)),
          const SizedBox(height: AppConstants.spacingLg),
        ],
        Text('Conexões', style: Theme.of(context).textTheme.titleMedium),
        const SizedBox(height: AppConstants.spacingSm),
        if (state.accepted.isEmpty)
          Padding(
            padding: const EdgeInsets.symmetric(vertical: AppConstants.spacingLg),
            child: Text(
              'Nenhuma conexão ainda. Toque em Adicionar para buscar pessoas.',
              style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                    color: Theme.of(context).colorScheme.onSurfaceVariant,
                  ),
            ),
          )
        else
          ...state.accepted.map((item) => _ConnectionTile(item: item, notifier: notifier)),
      ],
    );
  }
}

class _ConnectionSearchSheet extends ConsumerStatefulWidget {
  const _ConnectionSearchSheet({required this.onRequest});

  final Future<void> Function(String userId) onRequest;

  @override
  ConsumerState<_ConnectionSearchSheet> createState() => _ConnectionSearchSheetState();
}

class _ConnectionSearchSheetState extends ConsumerState<_ConnectionSearchSheet> {
  final _queryController = TextEditingController();
  List<ConnectionUser> _results = const [];
  bool _loading = false;
  String? _error;

  @override
  void initState() {
    super.initState();
    _loadSuggestions();
  }

  @override
  void dispose() {
    _queryController.dispose();
    super.dispose();
  }

  Future<void> _loadSuggestions() async {
    setState(() {
      _loading = true;
      _error = null;
    });
    try {
      final suggestions = await ref.read(connectionsProvider.notifier).getSuggestions();
      if (mounted) setState(() => _results = suggestions);
    } catch (e) {
      if (mounted) {
        setState(() {
          _error = e is ApiException ? e.userMessage : 'Erro ao carregar sugestões.';
        });
      }
    } finally {
      if (mounted) setState(() => _loading = false);
    }
  }

  Future<void> _search(String query) async {
    if (query.trim().length < 2) {
      await _loadSuggestions();
      return;
    }
    setState(() {
      _loading = true;
      _error = null;
    });
    try {
      final results = await ref.read(connectionsProvider.notifier).searchUsers(query.trim());
      if (mounted) setState(() => _results = results);
    } catch (e) {
      if (mounted) {
        setState(() {
          _error = e is ApiException ? e.userMessage : 'Erro na busca.';
          _results = const [];
        });
      }
    } finally {
      if (mounted) setState(() => _loading = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    return DraggableScrollableSheet(
      expand: false,
      initialChildSize: 0.85,
      minChildSize: 0.5,
      maxChildSize: 0.95,
      builder: (context, scrollController) => Padding(
        padding: const EdgeInsets.all(AppConstants.spacingMd),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            Text('Buscar pessoas', style: Theme.of(context).textTheme.titleLarge),
            const SizedBox(height: AppConstants.spacingMd),
            TextField(
              controller: _queryController,
              decoration: InputDecoration(
                hintText: 'Nome de exibição',
                prefixIcon: const Icon(Icons.search),
                border: const OutlineInputBorder(),
                suffixIcon: IconButton(
                  icon: const Icon(Icons.clear),
                  onPressed: () {
                    _queryController.clear();
                    _loadSuggestions();
                  },
                ),
              ),
              onSubmitted: _search,
              onChanged: (value) {
                if (value.trim().length >= 2) _search(value);
              },
            ),
            const SizedBox(height: AppConstants.spacingMd),
            if (_loading) const LinearProgressIndicator(),
            if (_error != null) ...[
              const SizedBox(height: AppConstants.spacingSm),
              Text(_error!, style: TextStyle(color: Theme.of(context).colorScheme.error)),
            ],
            const SizedBox(height: AppConstants.spacingSm),
            Expanded(
              child: _results.isEmpty && !_loading
                  ? Center(
                      child: Text(
                        'Digite ao menos 2 caracteres ou veja sugestões acima.',
                        textAlign: TextAlign.center,
                        style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                              color: Theme.of(context).colorScheme.onSurfaceVariant,
                            ),
                      ),
                    )
                  : ListView.builder(
                      controller: scrollController,
                      itemCount: _results.length,
                      itemBuilder: (context, index) {
                        final user = _results[index];
                        return ListTile(
                          leading: CircleAvatar(
                            child: Text(
                              user.displayName.isNotEmpty ? user.displayName[0].toUpperCase() : '?',
                            ),
                          ),
                          title: Text(user.displayName),
                          trailing: FilledButton.tonal(
                            onPressed: () => widget.onRequest(user.id),
                            child: const Text('Conectar'),
                          ),
                        );
                      },
                    ),
            ),
          ],
        ),
      ),
    );
  }
}

class _ConnectionTile extends StatelessWidget {
  const _ConnectionTile({
    required this.item,
    required this.notifier,
    this.isPending = false,
  });

  final ConnectionItem item;
  final ConnectionsNotifier notifier;
  final bool isPending;

  @override
  Widget build(BuildContext context) {
    return Card(
      margin: const EdgeInsets.only(bottom: AppConstants.spacingSm),
      child: ListTile(
        leading: CircleAvatar(child: Text(item.isIncoming ? '←' : '→')),
        title: Text(isPending ? 'Solicitação ${item.isIncoming ? 'recebida' : 'enviada'}' : 'Conexão ativa'),
        subtitle: Text('Status: ${item.status}'),
        trailing: isPending && item.isIncoming
            ? Row(
                mainAxisSize: MainAxisSize.min,
                children: [
                  IconButton(
                    icon: const Icon(Icons.check),
                    onPressed: () async {
                      try {
                        await notifier.accept(item.id);
                      } catch (e) {
                        if (context.mounted) {
                          showErrorSnackBar(
                            context,
                            e is ApiException ? e.userMessage : 'Erro ao aceitar.',
                          );
                        }
                      }
                    },
                  ),
                  IconButton(
                    icon: const Icon(Icons.close),
                    onPressed: () async {
                      try {
                        await notifier.reject(item.id);
                      } catch (e) {
                        if (context.mounted) {
                          showErrorSnackBar(
                            context,
                            e is ApiException ? e.userMessage : 'Erro ao rejeitar.',
                          );
                        }
                      }
                    },
                  ),
                ],
              )
            : !isPending
                ? IconButton(
                    icon: const Icon(Icons.person_remove_outlined),
                    onPressed: () async {
                      try {
                        await notifier.remove(item.id);
                      } catch (e) {
                        if (context.mounted) {
                          showErrorSnackBar(
                            context,
                            e is ApiException ? e.userMessage : 'Erro ao remover.',
                          );
                        }
                      }
                    },
                  )
                : null,
      ),
    );
  }
}
