import 'dart:async';
import 'package:intl/intl.dart';

import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/config/constants.dart';
import '../../../../core/network/api_exception.dart';
import '../../../../core/providers/territory_provider.dart';
import '../../../../core/theme/app_design_tokens.dart';
import '../../../../core/widgets/arah_material_symbol.dart';
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
            return Padding(
              padding: const EdgeInsets.only(bottom: AppConstants.spacingSm + 2),
              child: _TerritoryCard(
                territory: t,
                isSelected: isSelected,
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
        itemBuilder: (_, __) => Padding(
          padding: const EdgeInsets.only(bottom: AppConstants.spacingSm + 2),
          child: Card(
            child: Padding(
              padding: const EdgeInsets.all(AppConstants.spacingMd),
              child: Row(
                children: [
                  ShimmerBox(
                    width: AppConstants.avatarSizeSm,
                    height: AppConstants.avatarSizeSm,
                    borderRadius: BorderRadius.circular(12),
                  ),
                  const SizedBox(width: AppConstants.spacingMd),
                  Expanded(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        ShimmerBox(width: double.infinity, height: 16, borderRadius: BorderRadius.circular(4)),
                        const SizedBox(height: 6),
                        ShimmerBox(width: 120, height: 12, borderRadius: BorderRadius.circular(4)),
                        const SizedBox(height: 8),
                        ShimmerBox(width: 180, height: 12, borderRadius: BorderRadius.circular(4)),
                      ],
                    ),
                  ),
                ],
              ),
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

class _TerritoryCard extends StatelessWidget {
  const _TerritoryCard({
    required this.territory,
    required this.isSelected,
    required this.onTap,
  });

  final TerritoryItem territory;
  final bool isSelected;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    final colors = context.appColors;
    final region = territory.region;
    final description = territory.description;
    final metaParts = <String>[
      if (region != null && region.isNotEmpty) region,
      if (territory.memberCount != null)
        '${NumberFormat.decimalPattern('pt_BR').format(territory.memberCount)} moradores',
    ];

    return Material(
      color: Colors.transparent,
      child: InkWell(
        onTap: onTap,
        borderRadius: BorderRadius.circular(AppConstants.radiusCard),
        child: Ink(
          decoration: BoxDecoration(
            color: Theme.of(context).colorScheme.surfaceContainerHighest.withValues(alpha: 0.18),
            borderRadius: BorderRadius.circular(AppConstants.radiusCard),
            border: Border.all(
              color: isSelected ? colors.accentBorder : colors.outlineSubtle,
              width: isSelected ? 1.5 : 1,
            ),
          ),
          child: Padding(
            padding: const EdgeInsets.all(13),
            child: Row(
              children: [
                Container(
                  width: 40,
                  height: 40,
                  decoration: BoxDecoration(
                    borderRadius: BorderRadius.circular(12),
                    gradient: isSelected
                        ? const LinearGradient(
                            begin: Alignment.topCenter,
                            end: Alignment.bottomCenter,
                            colors: [
                              AppDesignTokens.loginButtonGradientStart,
                              AppDesignTokens.loginButtonGradientEnd,
                            ],
                          )
                        : null,
                    color: isSelected ? null : Theme.of(context).colorScheme.surfaceContainerHighest,
                  ),
                  child: Center(
                    child: ArahMaterialSymbol(
                      isSelected ? 'location_on' : 'forest',
                      size: 20,
                      fill: 1,
                      color: isSelected ? AppDesignTokens.textOnAccent : colors.primary,
                    ),
                  ),
                ),
                const SizedBox(width: 12),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        territory.name,
                        style: Theme.of(context).textTheme.titleMedium?.copyWith(
                              fontSize: 15,
                              letterSpacing: -0.2,
                            ),
                      ),
                      if (metaParts.isNotEmpty) ...[
                        const SizedBox(height: 2),
                        Text(
                          metaParts.join(' · '),
                          style: Theme.of(context).textTheme.bodySmall?.copyWith(
                                color: colors.onSurfaceSubtle,
                              ),
                          maxLines: 1,
                          overflow: TextOverflow.ellipsis,
                        ),
                      ],
                      if (description != null && description.isNotEmpty) ...[
                        const SizedBox(height: 5),
                        Text(
                          description,
                          style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                                color: colors.onSurfaceVariant,
                              ),
                          maxLines: 2,
                          overflow: TextOverflow.ellipsis,
                        ),
                      ],
                    ],
                  ),
                ),
                const SizedBox(width: AppConstants.spacingSm),
                if (isSelected)
                  ArahMaterialSymbol(
                    'check_circle',
                    size: 21,
                    fill: 1,
                    color: colors.primary,
                  )
                else
                  ArahMaterialSymbol(
                    'chevron_right',
                    size: 20,
                    color: colors.onSurfaceSubtle,
                  ),
              ],
            ),
          ),
        ),
      ),
    );
  }
}
