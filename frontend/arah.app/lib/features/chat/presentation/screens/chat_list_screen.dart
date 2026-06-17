import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../core/config/constants.dart';
import '../../../../core/network/api_exception.dart';
import '../../../../core/providers/territory_provider.dart';
import '../../../../core/widgets/app_snackbar.dart';
import '../../../../core/widgets/arah_scaffold.dart';
import '../../../../l10n/app_localizations.dart';
import '../../../territories/presentation/widgets/territory_indicator_bar.dart';
import '../providers/chat_provider.dart';

class ChatListScreen extends ConsumerStatefulWidget {
  const ChatListScreen({super.key});

  @override
  ConsumerState<ChatListScreen> createState() => _ChatListScreenState();
}

class _ChatListScreenState extends ConsumerState<ChatListScreen> with SingleTickerProviderStateMixin {
  late final TabController _tabController;

  @override
  void initState() {
    super.initState();
    _tabController = TabController(length: 2, vsync: this);
    _tabController.addListener(_onTabChanged);
  }

  void _onTabChanged() {
    if (_tabController.indexIsChanging) return;
    final tab = _tabController.index == 0 ? ChatListTab.channels : ChatListTab.groups;
    ref.read(chatListProvider.notifier).setTab(tab);
    setState(() {});
  }

  @override
  void dispose() {
    _tabController.removeListener(_onTabChanged);
    _tabController.dispose();
    super.dispose();
  }

  Future<void> _showCreateGroupDialog() async {
    final l10n = AppLocalizations.of(context)!;
    final nameController = TextEditingController();
    await showDialog<void>(
      context: context,
      builder: (ctx) => AlertDialog(
        title: Text(l10n.newGroup),
        content: TextField(
          controller: nameController,
          decoration: const InputDecoration(
            labelText: 'Nome do grupo',
            border: OutlineInputBorder(),
          ),
        ),
        actions: [
          TextButton(onPressed: () => Navigator.pop(ctx), child: Text(l10n.cancel)),
          FilledButton(
            onPressed: () async {
              final name = nameController.text.trim();
              if (name.isEmpty) return;
              try {
                final group = await ref.read(chatListProvider.notifier).createGroup(name);
                if (ctx.mounted) {
                  Navigator.pop(ctx);
                  showSuccessSnackBar(context, 'Grupo criado.');
                  if (mounted) {
                    context.push('/chat/${group.id}?title=${Uri.encodeComponent(group.name)}');
                  }
                }
              } catch (e) {
                if (ctx.mounted) {
                  showErrorSnackBar(
                    ctx,
                    e is ApiException ? e.userMessage : 'Erro ao criar grupo.',
                  );
                }
              }
            },
            child: Text(l10n.create),
          ),
        ],
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    final territoryId = ref.watch(selectedTerritoryIdValueProvider);
    final state = ref.watch(chatListProvider);
    final notifier = ref.read(chatListProvider.notifier);

    if (territoryId == null || territoryId.isEmpty) {
      return ArahScaffold(
        appBar: AppBar(title: Text(l10n.chat)),
        body: Center(child: Text(l10n.chooseTerritoryFirst)),
      );
    }

    return ArahScaffold(
      appBar: AppBar(
        title: Text(l10n.chat),
        bottom: TabBar(
          controller: _tabController,
          tabs: const [
            Tab(text: 'Canais'),
            Tab(text: 'Grupos'),
          ],
        ),
      ),
      floatingActionButton: _tabController.index == 1
          ? FloatingActionButton(
              onPressed: _showCreateGroupDialog,
              child: const Icon(Icons.group_add_outlined),
            )
          : null,
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

  Widget _buildBody(BuildContext context, ChatListState state) {
    if (state.isLoading && state.activeItems.isEmpty) {
      return ListView(
        physics: AlwaysScrollableScrollPhysics(),
        children: [SizedBox(height: 200, child: Center(child: CircularProgressIndicator()))],
      );
    }
    if (state.error != null && state.activeItems.isEmpty) {
      return ListView(
        physics: const AlwaysScrollableScrollPhysics(),
        children: [
          Padding(
            padding: const EdgeInsets.all(AppConstants.spacingLg),
            child: Text(
              state.error is ApiException
                  ? (state.error as ApiException).userMessage
                  : 'Erro ao carregar conversas.',
              textAlign: TextAlign.center,
            ),
          ),
        ],
      );
    }
    if (state.activeItems.isEmpty) {
      return ListView(
        physics: AlwaysScrollableScrollPhysics(),
        children: [
          SizedBox(
            height: 200,
            child: Center(
              child: Text(
                state.tab == ChatListTab.channels
                    ? 'Nenhum canal disponível.'
                    : 'Nenhum grupo ainda. Toque + para criar.',
              ),
            ),
          ),
        ],
      );
    }
    return ListView.builder(
      physics: const AlwaysScrollableScrollPhysics(),
      itemCount: state.activeItems.length,
      itemBuilder: (context, index) {
        final conversation = state.activeItems[index];
        return ListTile(
          leading: Icon(
            state.tab == ChatListTab.channels ? Icons.chat_bubble_outline : Icons.group_outlined,
          ),
          title: Text(conversation.name),
          subtitle: Text('${conversation.kind} · ${conversation.status}'),
          onTap: () => context.push('/chat/${conversation.id}?title=${Uri.encodeComponent(conversation.name)}'),
        );
      },
    );
  }
}
