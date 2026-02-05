import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/config/constants.dart';
import '../../../../core/widgets/app_snackbar.dart';
import '../../../../l10n/app_localizations.dart';
import '../../data/repositories/me_profile_repository.dart';
import '../providers/me_profile_provider.dart';

/// Sheet: lista de interesses, adicionar e remover (GET/POST/DELETE me/interests).
class InterestsSheet extends ConsumerStatefulWidget {
  const InterestsSheet({super.key});

  @override
  ConsumerState<InterestsSheet> createState() => _InterestsSheetState();
}

class _InterestsSheetState extends ConsumerState<InterestsSheet> {
  final _tagController = TextEditingController();
  bool _loading = false;

  @override
  void dispose() {
    _tagController.dispose();
    super.dispose();
  }

  Future<void> _addInterest() async {
    final tag = _tagController.text.trim();
    if (tag.isEmpty) return;
    setState(() => _loading = true);
    try {
      final repo = ref.read(meProfileRepositoryProvider);
      await repo.addInterest(tag);
      ref.invalidate(meInterestsProvider);
      _tagController.clear();
      if (mounted) showSuccessSnackBar(context, AppLocalizations.of(context)!.interestAdded);
    } catch (e) {
      if (mounted) showErrorSnackBar(context, e.toString());
    } finally {
      if (mounted) setState(() => _loading = false);
    }
  }

  Future<void> _removeInterest(String tag) async {
    setState(() => _loading = true);
    try {
      final repo = ref.read(meProfileRepositoryProvider);
      await repo.removeInterest(tag);
      ref.invalidate(meInterestsProvider);
      if (mounted) showSuccessSnackBar(context, AppLocalizations.of(context)!.interestRemoved);
    } catch (e) {
      if (mounted) showErrorSnackBar(context, e.toString());
    } finally {
      if (mounted) setState(() => _loading = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    final interestsAsync = ref.watch(meInterestsProvider);

    return Padding(
      padding: EdgeInsets.only(
        bottom: MediaQuery.of(context).viewInsets.bottom,
        left: AppConstants.spacingLg,
        right: AppConstants.spacingLg,
        top: AppConstants.spacingMd,
      ),
      child: Column(
        mainAxisSize: MainAxisSize.min,
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          Center(
            child: Container(
              width: AppConstants.avatarSizeSm,
              height: AppConstants.spacingXs,
              margin: const EdgeInsets.only(bottom: AppConstants.spacingMd),
              decoration: BoxDecoration(
                color: Theme.of(context).colorScheme.onSurfaceVariant.withOpacity(0.4),
                borderRadius: BorderRadius.circular(2),
              ),
            ),
          ),
          Text(
            AppLocalizations.of(context)!.myInterests,
            style: Theme.of(context).textTheme.titleLarge,
          ),
          const SizedBox(height: AppConstants.spacingSm),
          Text(
            AppLocalizations.of(context)!.myInterestsHint,
            style: Theme.of(context).textTheme.bodySmall?.copyWith(
                  color: Theme.of(context).colorScheme.onSurfaceVariant,
                ),
          ),
          const SizedBox(height: AppConstants.spacingMd),
          Row(
            children: [
              Expanded(
                child: TextField(
                  controller: _tagController,
                  decoration: InputDecoration(
                    labelText: AppLocalizations.of(context)!.interestTag,
                    border: const OutlineInputBorder(),
                  ),
                  textCapitalization: TextCapitalization.sentences,
                  onSubmitted: (_) => _addInterest(),
                ),
              ),
              const SizedBox(width: AppConstants.spacingSm),
              FilledButton(
                onPressed: _loading ? null : _addInterest,
                child: _loading
                    ? const SizedBox(
                        width: 20,
                        height: 20,
                        child: CircularProgressIndicator(strokeWidth: 2),
                      )
                    : Text(AppLocalizations.of(context)!.save),
              ),
            ],
          ),
          const SizedBox(height: AppConstants.spacingLg),
          interestsAsync.when(
            data: (list) => Wrap(
              spacing: AppConstants.spacingSm,
              runSpacing: AppConstants.spacingSm,
              children: list
                  .map((tag) => Chip(
                        label: Text(tag),
                        onDeleted: () => _removeInterest(tag),
                      ))
                  .toList(),
            ),
            loading: () => const Center(child: CircularProgressIndicator()),
            error: (_, __) => Text(
              AppLocalizations.of(context)!.errorLoad,
              style: Theme.of(context).textTheme.bodySmall?.copyWith(
                    color: Theme.of(context).colorScheme.error,
                  ),
            ),
          ),
        ],
      ),
    );
  }
}
