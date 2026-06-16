import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:intl/intl.dart';

import '../../../../core/config/constants.dart';
import '../../../../core/network/api_exception.dart';
import '../../../../core/providers/territory_provider.dart';
import '../../../territories/presentation/widgets/territory_indicator_bar.dart';
import '../providers/moderation_provider.dart';

class ModerationScreen extends ConsumerWidget {
  const ModerationScreen({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final territoryId = ref.watch(selectedTerritoryIdValueProvider);
    final state = ref.watch(moderationProvider);
    final notifier = ref.read(moderationProvider.notifier);
    final dateFormat = DateFormat('dd/MM/yyyy HH:mm');

    if (territoryId == null || territoryId.isEmpty) {
      return Scaffold(
        appBar: AppBar(title: const Text('Moderação')),
        body: const Center(child: Text('Escolha um território primeiro.')),
      );
    }

    return Scaffold(
      appBar: AppBar(title: const Text('Moderação')),
      body: Column(
        children: [
          const TerritoryIndicatorBar(),
          Expanded(
            child: RefreshIndicator(
              onRefresh: () => notifier.refresh(),
              child: _buildBody(context, state, dateFormat),
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildBody(BuildContext context, ModerationState state, DateFormat dateFormat) {
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
                  : 'Sem permissão ou erro ao carregar.',
              textAlign: TextAlign.center,
            ),
          ),
        ],
      );
    }
    if (state.items.isEmpty) {
      return const ListView(
        physics: AlwaysScrollableScrollPhysics(),
        children: [SizedBox(height: 200, child: Center(child: Text('Nenhum work item pendente.')))],
      );
    }
    return ListView.builder(
      physics: const AlwaysScrollableScrollPhysics(),
      padding: const EdgeInsets.all(AppConstants.spacingMd),
      itemCount: state.items.length,
      itemBuilder: (context, index) {
        final item = state.items[index];
        return Card(
          child: ListTile(
            title: Text(item.type),
            subtitle: Text('${item.status} · ${item.subjectType} · ${dateFormat.format(item.createdAtUtc.toLocal())}'),
          ),
        );
      },
    );
  }
}
