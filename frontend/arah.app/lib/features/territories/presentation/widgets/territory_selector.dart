import 'dart:async';

import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/config/constants.dart';
import '../../../../core/network/api_exception.dart';
import '../../../../core/providers/territory_provider.dart';
import '../../../../core/widgets/app_snackbar.dart';
import '../../../../core/widgets/shimmer_skeleton.dart';
import '../../../../l10n/app_localizations.dart';
import '../providers/territories_list_provider.dart';

class TerritorySelector extends ConsumerStatefulWidget {
  const TerritorySelector({super.key, this.onSelected, this.subtitle = 'Toque para ver o feed da região'});
  final VoidCallback? onSelected;
  final String subtitle;

  @override
  ConsumerState<TerritorySelector> createState() => _TerritorySelectorState();
}

class _TerritorySelectorState extends ConsumerState<TerritorySelector> {
  final _searchController = TextEditingController();
  Timer? _debounce;
  String _query = '';

  @override
  void dispose() {
    _debounce?.cancel();
    _searchController.dispose();
    super.dispose();
  }

  void _onSearchChanged(String value) {
    _debounce?.cancel();
    _debounce = Timer(const Duration(milliseconds: 400), () {
      if (mounted) setState(() => _query = value.trim());
    });
  }

  void _clearSearch() {
    _debounce?.cancel();
    _searchController.clear();
    setState(() => _query = '');
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    final isSearching = _query.isNotEmpty;
    final async = isSearching
        ? ref.watch(territoriesSearchProvider(_query))
        : ref.watch(territoriesListProvider);
    final selectedId = ref.watch(selectedTerritoryIdValueProvider);

    return Column(
      children: [
        Padding(
          padding: const EdgeInsets.fromLTRB(
            AppConstants.spacingMd,
            AppConstants.spacingSm,
            AppConstants.spacingMd,
            0,
          ),
          child: TextField(
            controller: _searchController,
            onChanged: _onSearchChanged,
            textInputAction: TextInputAction.search,
            decoration: InputDecoration(
              hintText: l10n.searchTerritoriesHint,
              prefixIcon: const Icon(Icons.search),
              suffixIcon: isSearching
                  ? IconButton(
                      icon: const Icon(Icons.clear),
                      tooltip: l10n.clear,
                      onPressed: _clearSearch,
                    )
                  : null,
              border: const OutlineInputBorder(),
              isDense: true,
            ),
          ),
        ),
        Expanded(child: _buildList(context, async, selectedId, isSearching)),
      ],
    );
  }

  Widget _buildList(
    BuildContext context,
    AsyncValue<List<TerritoryItem>> async,
    String? selectedId,
    bool isSearching,
  ) {
    return async.when(
      data: (list) {
        if (list.isEmpty) {
          return Center(
            child: Padding(
              padding: const EdgeInsets.all(AppConstants.spacingLg),
              child: Column(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  Icon(isSearching ? Icons.search_off : Icons.terrain_outlined, size: AppConstants.iconSizeLg, color: Theme.of(context).colorScheme.onSurfaceVariant),
                  const SizedBox(height: AppConstants.spacingMd),
                  Text(isSearching ? AppLocalizations.of(context)!.noSearchResults : AppLocalizations.of(context)!.noTerritoryAvailable, style: Theme.of(context).textTheme.titleMedium, textAlign: TextAlign.center),
                ],
              ),
            ),
          );
        }
        return ListView.builder(
          padding: const EdgeInsets.symmetric(horizontal: AppConstants.spacingMd, vertical: AppConstants.spacingSm),
          itemCount: list.length,
          itemBuilder: (context, index) {
            final t = list[index];
            final isSelected = selectedId == t.id;
            return Card(
              margin: const EdgeInsets.only(bottom: AppConstants.spacingSm),
              child: ListTile(
                leading: CircleAvatar(
                  backgroundColor: isSelected ? Theme.of(context).colorScheme.primary : Theme.of(context).colorScheme.surfaceContainerHighest,
                  child: Icon(Icons.terrain, color: isSelected ? Theme.of(context).colorScheme.onPrimary : Theme.of(context).colorScheme.onSurfaceVariant),
                ),
                title: Text(t.name),
                subtitle: t.description != null && t.description!.isNotEmpty ? Text(t.description!, maxLines: 2, overflow: TextOverflow.ellipsis) : null,
                trailing: isSelected ? Icon(Icons.check_circle, color: Theme.of(context).colorScheme.primary) : null,
                onTap: () async {
                  try {
                    await ref.read(territoriesRepositoryProvider).enterTerritory(t.id);
                    await ref.read(selectedTerritoryIdProvider.notifier).setTerritoryId(t.id);
                    if (context.mounted) widget.onSelected?.call();
                  } catch (e) {
                    if (!context.mounted) return;
                    final msg = e is ApiException ? e.userMessage : 'Não foi possível entrar no território.';
                    showErrorSnackBar(context, msg);
                  }
                },
              ),
            );
          },
        );
      },
      loading: () => ListView.builder(
        padding: const EdgeInsets.symmetric(horizontal: AppConstants.spacingMd, vertical: AppConstants.spacingSm),
        itemCount: 4,
        itemBuilder: (_, __) => Card(
          margin: const EdgeInsets.only(bottom: AppConstants.spacingSm),
          child: Padding(
            padding: const EdgeInsets.all(AppConstants.spacingMd),
            child: Row(
              children: [
                ShimmerBox(
                  width: AppConstants.avatarSizeSm,
                  height: AppConstants.avatarSizeSm,
                  borderRadius: BorderRadius.circular(AppConstants.avatarSizeSm / 2),
                ),
                const SizedBox(width: AppConstants.spacingMd),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      ShimmerBox(width: double.infinity, height: 16, borderRadius: BorderRadius.circular(4)),
                      const SizedBox(height: 6),
                      ShimmerBox(width: 120, height: 12, borderRadius: BorderRadius.circular(4)),
                    ],
                  ),
                ),
              ],
            ),
          ),
        ),
      ),
      error: (err, _) => Center(
        child: Padding(
          padding: const EdgeInsets.all(AppConstants.spacingLg),
          child: Column(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              Icon(Icons.wifi_off, size: AppConstants.iconSizeLg, color: Theme.of(context).colorScheme.error),
              const SizedBox(height: AppConstants.spacingMd),
              Text(err is ApiException ? err.userMessage : err.toString().replaceFirst('ApiException: ', ''), textAlign: TextAlign.center, style: Theme.of(context).textTheme.bodyMedium),
            ],
          ),
        ),
      ),
    );
  }
}
