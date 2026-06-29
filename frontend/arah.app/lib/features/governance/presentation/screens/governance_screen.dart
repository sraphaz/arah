import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/config/constants.dart';
import '../../../../core/network/api_exception.dart';
import '../../../../core/providers/territory_provider.dart';
import '../../../../core/widgets/app_snackbar.dart';
import '../../../../core/widgets/arah_list_skeleton.dart';
import '../../../../core/widgets/arah_scaffold.dart';
import '../../../../l10n/app_localizations.dart';
import '../../../territories/presentation/widgets/territory_indicator_bar.dart';
import '../../data/models/voting.dart';
import '../providers/governance_provider.dart';
import 'create_voting_screen.dart';
import 'voting_labels.dart';

/// Governança: votações do território (BFF governance/{territoryId}/votings).
class GovernanceScreen extends ConsumerWidget {
  const GovernanceScreen({super.key, this.territoryId});

  final String? territoryId;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final l10n = AppLocalizations.of(context)!;
    final effectiveTerritoryId =
        territoryId ?? ref.watch(selectedTerritoryIdValueProvider);
    final hasTerritory =
        effectiveTerritoryId != null && effectiveTerritoryId.isNotEmpty;

    if (!hasTerritory) {
      return ArahScaffold(
        appBar: AppBar(title: Text(l10n.governance)),
        body: Center(
          child: Padding(
            padding: const EdgeInsets.all(AppConstants.spacingLg),
            child: Text(
              l10n.chooseTerritoryForGovernance,
              textAlign: TextAlign.center,
              style: Theme.of(context).textTheme.titleMedium,
            ),
          ),
        ),
      );
    }

    final state = ref.watch(governanceProvider(effectiveTerritoryId));
    final notifier = ref.read(governanceProvider(effectiveTerritoryId).notifier);

    return ArahScaffold(
      appBar: AppBar(title: Text(l10n.governance)),
      floatingActionButton: FloatingActionButton.extended(
        onPressed: () => _openCreate(context, effectiveTerritoryId),
        icon: const Icon(Icons.how_to_vote_outlined),
        label: Text(l10n.createVoting),
      ),
      body: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          const TerritoryIndicatorBar(),
          _StatusFilterBar(
            selected: state.statusFilter,
            onSelected: notifier.setStatusFilter,
          ),
          Expanded(
            child: RefreshIndicator(
              onRefresh: () => notifier.refresh(),
              child: _buildBody(context, ref, state, notifier),
            ),
          ),
        ],
      ),
    );
  }

  Future<void> _openCreate(BuildContext context, String territoryId) async {
    final created = await Navigator.of(context).push<bool>(
      MaterialPageRoute(
        builder: (_) => CreateVotingScreen(territoryId: territoryId),
      ),
    );
    if (created == true && context.mounted) {
      showSuccessSnackBar(context, AppLocalizations.of(context)!.votingCreated);
    }
  }

  Widget _buildBody(
    BuildContext context,
    WidgetRef ref,
    GovernanceState state,
    GovernanceNotifier notifier,
  ) {
    final l10n = AppLocalizations.of(context)!;

    if (state.isLoading && state.votings.isEmpty) {
      return const ArahListSkeleton();
    }

    if (state.error != null && state.votings.isEmpty) {
      final message = state.error is ApiException
          ? (state.error as ApiException).userMessage
          : l10n.errorLoadVotings;
      return ListView(
        physics: const AlwaysScrollableScrollPhysics(),
        children: [
          Padding(
            padding: const EdgeInsets.all(AppConstants.spacingLg),
            child: Column(
              children: [
                Icon(Icons.lock_outline,
                    size: AppConstants.iconSizeLg,
                    color: Theme.of(context).colorScheme.error),
                const SizedBox(height: AppConstants.spacingMd),
                Text(message, textAlign: TextAlign.center),
                const SizedBox(height: AppConstants.spacingMd),
                FilledButton.tonal(
                    onPressed: () => notifier.refresh(),
                    child: Text(l10n.tryAgain)),
              ],
            ),
          ),
        ],
      );
    }

    if (state.votings.isEmpty) {
      return ListView(
        physics: const AlwaysScrollableScrollPhysics(),
        children: [
          SizedBox(
            height: MediaQuery.of(context).size.height * 0.5,
            child: Center(
              child: Column(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  Icon(
                    Icons.how_to_vote_outlined,
                    size: AppConstants.avatarSizeLg,
                    color: Theme.of(context)
                        .colorScheme
                        .primary
                        .withValues(alpha: 0.5),
                  ),
                  const SizedBox(height: AppConstants.spacingMd),
                  Text(l10n.noVotings,
                      style: Theme.of(context).textTheme.titleMedium),
                  const SizedBox(height: AppConstants.spacingXs),
                  Padding(
                    padding: const EdgeInsets.symmetric(
                        horizontal: AppConstants.spacingLg),
                    child: Text(
                      l10n.governanceSubtitle,
                      textAlign: TextAlign.center,
                      style: Theme.of(context).textTheme.bodySmall?.copyWith(
                            color:
                                Theme.of(context).colorScheme.onSurfaceVariant,
                          ),
                    ),
                  ),
                ],
              ),
            ),
          ),
        ],
      );
    }

    return ListView.builder(
      physics: const AlwaysScrollableScrollPhysics(),
      padding: const EdgeInsets.symmetric(
          horizontal: AppConstants.spacingMd, vertical: AppConstants.spacingSm),
      itemCount: state.votings.length,
      itemBuilder: (context, index) {
        final voting = state.votings[index];
        return _VotingCard(
          voting: voting,
          alreadyVoted: state.votedIds.contains(voting.id),
          onVote: (option) async {
            try {
              await notifier.vote(voting, option);
              if (context.mounted) {
                showSuccessSnackBar(context, l10n.voteRegistered);
              }
            } catch (e) {
              if (context.mounted) {
                final msg = e is ApiException ? e.userMessage : l10n.errorVote;
                showErrorSnackBar(context, msg);
              }
            }
          },
          onShowResults: () => _showResults(context, notifier, voting),
          onClose: () async {
            try {
              await notifier.closeVoting(voting);
              if (context.mounted) {
                showSuccessSnackBar(context, l10n.votingClosedMsg);
              }
            } catch (e) {
              if (context.mounted) {
                final msg =
                    e is ApiException ? e.userMessage : l10n.errorCloseVoting;
                showErrorSnackBar(context, msg);
              }
            }
          },
        );
      },
    );
  }

  Future<void> _showResults(
    BuildContext context,
    GovernanceNotifier notifier,
    Voting voting,
  ) async {
    await showModalBottomSheet<void>(
      context: context,
      isScrollControlled: true,
      showDragHandle: true,
      builder: (ctx) => _VotingResultsSheet(
        voting: voting,
        load: () => notifier.results(voting),
      ),
    );
  }
}

class _StatusFilterBar extends StatelessWidget {
  const _StatusFilterBar({required this.selected, required this.onSelected});

  final String? selected;
  final void Function(String?) onSelected;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    return Padding(
      padding: const EdgeInsets.symmetric(
          horizontal: AppConstants.spacingMd, vertical: AppConstants.spacingSm),
      child: Wrap(
        spacing: AppConstants.spacingSm,
        children: [
          ChoiceChip(
            label: Text(l10n.filterAllVotings),
            selected: selected == null,
            onSelected: (_) => onSelected(null),
          ),
          ChoiceChip(
            label: Text(l10n.filterOpenVotings),
            selected: selected == 'Open',
            onSelected: (_) => onSelected('Open'),
          ),
          ChoiceChip(
            label: Text(l10n.filterClosedVotings),
            selected: selected == 'Closed',
            onSelected: (_) => onSelected('Closed'),
          ),
        ],
      ),
    );
  }
}

class _VotingCard extends StatefulWidget {
  const _VotingCard({
    required this.voting,
    required this.alreadyVoted,
    required this.onVote,
    required this.onShowResults,
    required this.onClose,
  });

  final Voting voting;
  final bool alreadyVoted;
  final void Function(String option) onVote;
  final VoidCallback onShowResults;
  final VoidCallback onClose;

  @override
  State<_VotingCard> createState() => _VotingCardState();
}

class _VotingCardState extends State<_VotingCard> {
  String? _selectedOption;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    final voting = widget.voting;
    final theme = Theme.of(context);
    final canVote = voting.isOpen && !widget.alreadyVoted;

    return Card(
      margin: const EdgeInsets.only(bottom: AppConstants.spacingMd),
      child: Padding(
        padding: const EdgeInsets.all(AppConstants.spacingMd),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Expanded(
                  child: Text(
                    voting.title,
                    style: theme.textTheme.titleMedium
                        ?.copyWith(fontWeight: FontWeight.w600),
                  ),
                ),
                _StatusBadge(status: voting.status),
                PopupMenuButton<String>(
                  onSelected: (value) {
                    if (value == 'results') widget.onShowResults();
                    if (value == 'close') widget.onClose();
                  },
                  itemBuilder: (ctx) => [
                    PopupMenuItem(
                        value: 'results', child: Text(l10n.viewResults)),
                    if (voting.isOpen)
                      PopupMenuItem(
                          value: 'close', child: Text(l10n.closeVoting)),
                  ],
                ),
              ],
            ),
            Text(
              '${votingTypeLabel(l10n, voting.type)} · ${votingVisibilityLabel(l10n, voting.visibility)}',
              style: theme.textTheme.labelSmall
                  ?.copyWith(color: theme.colorScheme.onSurfaceVariant),
            ),
            if (voting.description.isNotEmpty) ...[
              const SizedBox(height: AppConstants.spacingSm),
              Text(voting.description, style: theme.textTheme.bodyMedium),
            ],
            const SizedBox(height: AppConstants.spacingSm),
            if (canVote) ...[
              ...voting.options.map(
                (option) => RadioListTile<String>(
                  value: option,
                  groupValue: _selectedOption,
                  onChanged: (value) =>
                      setState(() => _selectedOption = value),
                  title: Text(option),
                  contentPadding: EdgeInsets.zero,
                  dense: true,
                ),
              ),
              const SizedBox(height: AppConstants.spacingXs),
              Align(
                alignment: Alignment.centerRight,
                child: FilledButton(
                  onPressed: _selectedOption == null
                      ? null
                      : () => widget.onVote(_selectedOption!),
                  child: Text(l10n.voteAction),
                ),
              ),
            ] else ...[
              if (widget.alreadyVoted)
                Row(
                  children: [
                    Icon(Icons.check_circle,
                        size: AppConstants.iconSizeSm,
                        color: theme.colorScheme.primary),
                    const SizedBox(width: AppConstants.spacingXs),
                    Text(l10n.alreadyVoted,
                        style: theme.textTheme.bodySmall?.copyWith(
                            color: theme.colorScheme.onSurfaceVariant)),
                  ],
                ),
              const SizedBox(height: AppConstants.spacingXs),
              Align(
                alignment: Alignment.centerRight,
                child: TextButton.icon(
                  onPressed: widget.onShowResults,
                  icon: const Icon(Icons.bar_chart_outlined),
                  label: Text(l10n.viewResults),
                ),
              ),
            ],
          ],
        ),
      ),
    );
  }
}

class _StatusBadge extends StatelessWidget {
  const _StatusBadge({required this.status});

  final String status;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    final theme = Theme.of(context);
    final isOpen = status.toLowerCase() == 'open';
    final color = isOpen ? theme.colorScheme.primary : theme.colorScheme.outline;
    return Container(
      margin: const EdgeInsets.only(left: AppConstants.spacingSm),
      padding: const EdgeInsets.symmetric(
          horizontal: AppConstants.spacingSm, vertical: 2),
      decoration: BoxDecoration(
        color: color.withValues(alpha: 0.12),
        borderRadius: BorderRadius.circular(AppConstants.radiusSm),
      ),
      child: Text(
        votingStatusLabel(l10n, status),
        style: theme.textTheme.labelSmall?.copyWith(color: color),
      ),
    );
  }
}

class _VotingResultsSheet extends StatelessWidget {
  const _VotingResultsSheet({required this.voting, required this.load});

  final Voting voting;
  final Future<VotingResults> Function() load;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    final theme = Theme.of(context);
    return Padding(
      padding: const EdgeInsets.fromLTRB(
        AppConstants.spacingLg,
        0,
        AppConstants.spacingLg,
        AppConstants.spacingLg,
      ),
      child: FutureBuilder<VotingResults>(
        future: load(),
        builder: (context, snapshot) {
          if (snapshot.connectionState == ConnectionState.waiting) {
            return const Padding(
              padding: EdgeInsets.all(AppConstants.spacingXl),
              child: Center(child: CircularProgressIndicator()),
            );
          }
          if (snapshot.hasError) {
            final err = snapshot.error;
            final msg = err is ApiException ? err.userMessage : l10n.errorLoad;
            return Padding(
              padding: const EdgeInsets.all(AppConstants.spacingLg),
              child: Text(msg, textAlign: TextAlign.center),
            );
          }
          final results = snapshot.data ?? VotingResults(votingId: voting.id, counts: const {});
          final total = results.totalVotes;
          return Column(
            mainAxisSize: MainAxisSize.min,
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(voting.title,
                  style: theme.textTheme.titleMedium
                      ?.copyWith(fontWeight: FontWeight.w600)),
              const SizedBox(height: AppConstants.spacingXs),
              Text(l10n.totalVotesLabel(total),
                  style: theme.textTheme.bodySmall?.copyWith(
                      color: theme.colorScheme.onSurfaceVariant)),
              const SizedBox(height: AppConstants.spacingMd),
              ...voting.options.map((option) {
                final count = results.counts[option] ?? 0;
                final fraction = total == 0 ? 0.0 : count / total;
                final percent = (fraction * 100).round();
                return Padding(
                  padding:
                      const EdgeInsets.only(bottom: AppConstants.spacingMd),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Row(
                        mainAxisAlignment: MainAxisAlignment.spaceBetween,
                        children: [
                          Expanded(child: Text(option)),
                          Text('$count · $percent%',
                              style: theme.textTheme.bodySmall),
                        ],
                      ),
                      const SizedBox(height: AppConstants.spacingXs),
                      ClipRRect(
                        borderRadius:
                            BorderRadius.circular(AppConstants.radiusSm),
                        child: LinearProgressIndicator(
                          value: fraction,
                          minHeight: 8,
                          backgroundColor: theme
                              .colorScheme.surfaceContainerHighest,
                        ),
                      ),
                    ],
                  ),
                );
              }),
            ],
          );
        },
      ),
    );
  }
}
