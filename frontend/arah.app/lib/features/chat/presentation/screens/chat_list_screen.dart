import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../core/config/constants.dart';
import '../../../../core/network/api_exception.dart';
import '../../../../core/providers/territory_provider.dart';
import '../../../territories/presentation/widgets/territory_indicator_bar.dart';
import '../providers/chat_provider.dart';

class ChatListScreen extends ConsumerWidget {
  const ChatListScreen({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final territoryId = ref.watch(selectedTerritoryIdValueProvider);
    final state = ref.watch(chatListProvider);
    final notifier = ref.read(chatListProvider.notifier);

    if (territoryId == null || territoryId.isEmpty) {
      return Scaffold(
        appBar: AppBar(title: const Text('Chat')),
        body: const Center(child: Text('Escolha um território primeiro.')),
      );
    }

    return Scaffold(
      appBar: AppBar(title: const Text('Chat')),
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
    if (state.isLoading && state.channels.isEmpty) {
      return const ListView(
        physics: AlwaysScrollableScrollPhysics(),
        children: [SizedBox(height: 200, child: Center(child: CircularProgressIndicator()))],
      );
    }
    if (state.error != null && state.channels.isEmpty) {
      return ListView(
        physics: const AlwaysScrollableScrollPhysics(),
        children: [
          Padding(
            padding: const EdgeInsets.all(AppConstants.spacingLg),
            child: Text(
              state.error is ApiException
                  ? (state.error as ApiException).userMessage
                  : 'Erro ao carregar canais.',
              textAlign: TextAlign.center,
            ),
          ),
        ],
      );
    }
    if (state.channels.isEmpty) {
      return const ListView(
        physics: AlwaysScrollableScrollPhysics(),
        children: [SizedBox(height: 200, child: Center(child: Text('Nenhum canal disponível.')))],
      );
    }
    return ListView.builder(
      physics: const AlwaysScrollableScrollPhysics(),
      itemCount: state.channels.length,
      itemBuilder: (context, index) {
        final channel = state.channels[index];
        return ListTile(
          leading: const Icon(Icons.chat_bubble_outline),
          title: Text(channel.name),
          subtitle: Text('${channel.kind} · ${channel.status}'),
          onTap: () => context.push('/chat/${channel.id}?title=${Uri.encodeComponent(channel.name)}'),
        );
      },
    );
  }
}
