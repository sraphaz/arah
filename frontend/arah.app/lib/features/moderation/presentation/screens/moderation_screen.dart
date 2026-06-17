import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:intl/intl.dart';

import '../../../../core/config/constants.dart';
import '../../../../core/network/api_exception.dart';
import '../../../../core/providers/territory_provider.dart';
import '../../../../core/widgets/app_snackbar.dart';
import '../../../../core/widgets/arah_scaffold.dart';
import '../../../../l10n/app_localizations.dart';
import '../../../territories/presentation/widgets/territory_indicator_bar.dart';
import '../../data/models/work_item.dart';
import '../providers/moderation_provider.dart';

class ModerationScreen extends ConsumerStatefulWidget {
  const ModerationScreen({super.key});

  @override
  ConsumerState<ModerationScreen> createState() => _ModerationScreenState();
}

class _ModerationScreenState extends ConsumerState<ModerationScreen> with SingleTickerProviderStateMixin {
  late final TabController _tabController;

  @override
  void initState() {
    super.initState();
    _tabController = TabController(length: 3, vsync: this);
    _tabController.addListener(_onTabChanged);
  }

  void _onTabChanged() {
    if (_tabController.indexIsChanging) return;
    final tab = ModerationTab.values[_tabController.index];
    ref.read(moderationProvider.notifier).setTab(tab);
  }

  @override
  void dispose() {
    _tabController.removeListener(_onTabChanged);
    _tabController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    final territoryId = ref.watch(selectedTerritoryIdValueProvider);
    final state = ref.watch(moderationProvider);
    final notifier = ref.read(moderationProvider.notifier);
    final dateFormat = DateFormat('dd/MM/yyyy HH:mm');

    if (territoryId == null || territoryId.isEmpty) {
      return ArahScaffold(
        appBar: AppBar(title: Text(l10n.moderation)),
        body: Center(child: Text(l10n.chooseTerritoryFirst)),
      );
    }

    return ArahScaffold(
      appBar: AppBar(
        title: Text(l10n.moderation),
        bottom: TabBar(
          controller: _tabController,
          tabs: [
            Tab(text: l10n.moderationQueueTab),
            Tab(text: l10n.moderationCasesTab),
            Tab(text: l10n.moderationEvidencesTab),
          ],
        ),
      ),
      body: Column(
        children: [
          const TerritoryIndicatorBar(),
          Expanded(
            child: RefreshIndicator(
              onRefresh: () => notifier.refresh(),
              child: _buildBody(context, state, dateFormat, notifier),
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildBody(
    BuildContext context,
    ModerationState state,
    DateFormat dateFormat,
    ModerationNotifier notifier,
  ) {
    final l10n = AppLocalizations.of(context)!;
    if (state.isLoading && state.items.isEmpty) {
      return ListView(
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
                  : l10n.noPermissionOrError,
              textAlign: TextAlign.center,
            ),
          ),
        ],
      );
    }
    if (state.items.isEmpty) {
      return ListView(
        physics: AlwaysScrollableScrollPhysics(),
        children: [SizedBox(height: 200, child: Center(child: Text(l10n.noQueueItems)))],
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
            subtitle: Text(
              '${item.status} · ${item.subjectType}'
              '${item.evidenceId != null ? ' · ${l10n.moderationEvidenceSuffix}' : ''}'
              ' · ${dateFormat.format(item.createdAtUtc.toLocal())}',
            ),
            trailing: _buildActions(context, notifier, item),
          ),
        );
      },
    );
  }

  Widget? _buildActions(BuildContext context, ModerationNotifier notifier, WorkItem item) {
    final l10n = AppLocalizations.of(context)!;
    if (!item.isPending) return null;
    if (item.isResidencyVerification && item.evidenceId != null) {
      return Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          IconButton(
            tooltip: l10n.downloadEvidenceTooltip,
            icon: const Icon(Icons.download_outlined),
            onPressed: () => _downloadEvidence(context, notifier, item),
          ),
          IconButton(
            tooltip: l10n.approveTooltip,
            icon: const Icon(Icons.check_circle_outline),
            onPressed: () => _decide(context, notifier, item, 'APPROVED'),
          ),
          IconButton(
            tooltip: l10n.rejectTooltip,
            icon: const Icon(Icons.cancel_outlined),
            onPressed: () => _decide(context, notifier, item, 'REJECTED'),
          ),
        ],
      );
    }
    if (item.isModerationCase || item.isResidencyVerification) {
      return Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          IconButton(
            tooltip: l10n.approveTooltip,
            icon: const Icon(Icons.check_circle_outline),
            onPressed: () => _decide(context, notifier, item, 'APPROVED'),
          ),
          IconButton(
            tooltip: l10n.rejectTooltip,
            icon: const Icon(Icons.cancel_outlined),
            onPressed: () => _decide(context, notifier, item, 'REJECTED'),
          ),
        ],
      );
    }
    return null;
  }

  Future<void> _decide(
    BuildContext context,
    ModerationNotifier notifier,
    WorkItem item,
    String outcome,
  ) async {
    final l10n = AppLocalizations.of(context)!;
    try {
      await notifier.decideItem(item, outcome);
      if (context.mounted) showSuccessSnackBar(context, l10n.decisionRegistered);
    } catch (e) {
      if (context.mounted) {
        showErrorSnackBar(
          context,
          e is ApiException ? e.userMessage : l10n.errorDecideItem,
        );
      }
    }
  }

  Future<void> _downloadEvidence(
    BuildContext context,
    ModerationNotifier notifier,
    WorkItem item,
  ) async {
    final l10n = AppLocalizations.of(context)!;
    try {
      final size = await notifier.downloadEvidence(item);
      if (context.mounted) showSuccessSnackBar(context, l10n.evidenceDownloaded(size));
    } catch (e) {
      if (context.mounted) {
        showErrorSnackBar(
          context,
          e is ApiException ? e.userMessage : l10n.errorDownloadEvidence,
        );
      }
    }
  }
}
