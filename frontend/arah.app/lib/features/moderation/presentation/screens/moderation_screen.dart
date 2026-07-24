import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:intl/intl.dart';

import '../../../../core/config/constants.dart';
import '../../../../core/network/api_exception.dart';
import '../../../../core/providers/territory_provider.dart';
import '../../../../core/theme/app_design_tokens.dart';
import '../../../../core/widgets/app_snackbar.dart';
import '../../../../core/widgets/arah_empty_state.dart';
import '../../../../core/widgets/arah_error_state.dart';
import '../../../../core/widgets/arah_list_skeleton.dart';
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

class _ModerationScreenState extends ConsumerState<ModerationScreen>
    with SingleTickerProviderStateMixin {
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
        body: ArahEmptyState(
          icon: Icons.map_outlined,
          title: l10n.chooseTerritoryFirst,
        ),
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
        physics: const AlwaysScrollableScrollPhysics(),
        children: const [ArahListSkeleton()],
      );
    }
    if (state.error != null && state.items.isEmpty) {
      return ListView(
        physics: const AlwaysScrollableScrollPhysics(),
        children: [
          SizedBox(
            height: MediaQuery.of(context).size.height * 0.55,
            child: ArahErrorState(
              message: state.error is ApiException
                  ? (state.error as ApiException).userMessage
                  : l10n.noPermissionOrError,
              retryLabel: l10n.tryAgain,
              onRetry: () => notifier.refresh(),
            ),
          ),
        ],
      );
    }
    if (state.items.isEmpty) {
      return ListView(
        physics: const AlwaysScrollableScrollPhysics(),
        children: [
          SizedBox(
            height: MediaQuery.of(context).size.height * 0.55,
            child: ArahEmptyState(
              icon: Icons.inbox_outlined,
              title: l10n.noQueueItems,
              description: l10n.moderationEmptyDescription,
            ),
          ),
        ],
      );
    }
    return ListView.separated(
      physics: const AlwaysScrollableScrollPhysics(),
      padding: const EdgeInsets.all(AppConstants.spacingMd),
      itemCount: state.items.length,
      separatorBuilder: (_, __) => const SizedBox(height: AppConstants.spacingSm),
      itemBuilder: (context, index) {
        final item = state.items[index];
        return _WorkItemCard(
          item: item,
          dateLabel: dateFormat.format(item.createdAtUtc.toLocal()),
          onApprove: item.isPending && _canDecide(item)
              ? () => _decide(context, notifier, item, 'APPROVED')
              : null,
          onReject: item.isPending && _canDecide(item)
              ? () => _decide(context, notifier, item, 'REJECTED')
              : null,
          onDownload: item.isPending &&
                  item.isResidencyVerification &&
                  item.evidenceId != null
              ? () => _downloadEvidence(context, notifier, item)
              : null,
        );
      },
    );
  }

  bool _canDecide(WorkItem item) =>
      item.isModerationCase || item.isResidencyVerification;

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

class _WorkItemCard extends StatelessWidget {
  const _WorkItemCard({
    required this.item,
    required this.dateLabel,
    this.onApprove,
    this.onReject,
    this.onDownload,
  });

  final WorkItem item;
  final String dateLabel;
  final VoidCallback? onApprove;
  final VoidCallback? onReject;
  final VoidCallback? onDownload;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    final colors = context.appColors;
    final theme = Theme.of(context);
    final hasActions = onApprove != null || onReject != null || onDownload != null;

    return Card(
      child: Padding(
        padding: AppDesignTokens.cardPadding,
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Wrap(
              spacing: AppConstants.spacingSm,
              runSpacing: AppConstants.spacingXs,
              children: [
                _MetaChip(
                  label: _typeLabel(l10n, item),
                  foreground: AppDesignTokens.info,
                  background: AppDesignTokens.info.withValues(alpha: 0.16),
                  border: AppDesignTokens.info.withValues(alpha: 0.4),
                ),
                _MetaChip(
                  label: _statusLabel(l10n, item.status),
                  foreground: _statusColor(item.status),
                  background: _statusColor(item.status).withValues(alpha: 0.16),
                  border: _statusColor(item.status).withValues(alpha: 0.4),
                ),
              ],
            ),
            const SizedBox(height: AppConstants.spacingSm),
            Text(
              item.subjectType.isNotEmpty ? item.subjectType : _typeLabel(l10n, item),
              style: theme.textTheme.titleMedium,
            ),
            const SizedBox(height: AppConstants.spacingXs),
            Text(
              _subtitle(l10n),
              style: theme.textTheme.bodySmall?.copyWith(
                color: colors.onSurfaceVariant,
              ),
            ),
            if (hasActions) ...[
              const SizedBox(height: AppConstants.spacingMd),
              Wrap(
                spacing: AppConstants.spacingSm,
                runSpacing: AppConstants.spacingXs,
                children: [
                  if (onDownload != null)
                    OutlinedButton.icon(
                      onPressed: onDownload,
                      icon: const Icon(Icons.download_outlined, size: 18),
                      label: Text(l10n.downloadEvidenceTooltip),
                    ),
                  if (onApprove != null)
                    FilledButton.icon(
                      onPressed: onApprove,
                      icon: const Icon(Icons.check_circle_outline, size: 18),
                      label: Text(l10n.approveTooltip),
                      style: FilledButton.styleFrom(
                        backgroundColor: colors.success,
                        foregroundColor: AppDesignTokens.textOnAccent,
                        minimumSize: const Size(
                          AppConstants.minTouchTargetSize,
                          AppConstants.minTouchTargetSize,
                        ),
                      ),
                    ),
                  if (onReject != null)
                    OutlinedButton.icon(
                      onPressed: onReject,
                      icon: Icon(Icons.cancel_outlined, size: 18, color: colors.error),
                      label: Text(l10n.rejectTooltip),
                      style: OutlinedButton.styleFrom(
                        foregroundColor: colors.error,
                        side: BorderSide(color: colors.error.withValues(alpha: 0.5)),
                        minimumSize: const Size(
                          AppConstants.minTouchTargetSize,
                          AppConstants.minTouchTargetSize,
                        ),
                      ),
                    ),
                ],
              ),
            ],
          ],
        ),
      ),
    );
  }

  String _subtitle(AppLocalizations l10n) {
    final parts = <String>[dateLabel];
    if (item.evidenceId != null) {
      parts.insert(0, l10n.moderationEvidenceSuffix);
    }
    return parts.join(' · ');
  }

  static String _typeLabel(AppLocalizations l10n, WorkItem item) {
    if (item.isModerationCase) return l10n.moderationCaseTypeLabel;
    if (item.isResidencyVerification) return l10n.residencyVerificationTypeLabel;
    return item.type;
  }

  static String _statusLabel(AppLocalizations l10n, String status) {
    switch (status.toUpperCase()) {
      case 'PENDING':
        return l10n.moderationStatusPending;
      case 'APPROVED':
        return l10n.moderationStatusApproved;
      case 'REJECTED':
        return l10n.moderationStatusRejected;
      default:
        return status;
    }
  }

  /// Status colors limited to warning / success / info design tokens.
  static Color _statusColor(String status) {
    switch (status.toUpperCase()) {
      case 'PENDING':
        return AppDesignTokens.warning;
      case 'APPROVED':
        return AppDesignTokens.success;
      case 'REJECTED':
        return AppDesignTokens.info;
      default:
        return AppDesignTokens.info;
    }
  }
}

class _MetaChip extends StatelessWidget {
  const _MetaChip({
    required this.label,
    required this.foreground,
    required this.background,
    required this.border,
  });

  final String label;
  final Color foreground;
  final Color background;
  final Color border;

  @override
  Widget build(BuildContext context) {
    return Chip(
      label: Text(label),
      labelStyle: Theme.of(context).textTheme.labelSmall?.copyWith(
            color: foreground,
            fontWeight: FontWeight.w600,
          ),
      backgroundColor: background,
      side: BorderSide(color: border),
      visualDensity: VisualDensity.compact,
      materialTapTargetSize: MaterialTapTargetSize.shrinkWrap,
      padding: const EdgeInsets.symmetric(horizontal: AppConstants.spacingXs),
    );
  }
}
