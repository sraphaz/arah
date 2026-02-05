import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/config/constants.dart';
import '../../../../core/network/api_exception.dart';
import '../../../../core/providers/territory_provider.dart';
import '../../../../core/widgets/app_snackbar.dart';
import '../../../../l10n/app_localizations.dart';
import '../providers/feed_provider.dart';

/// Tela "Publicar": título, conteúdo, tipo e visibilidade; POST feed/create-post.
class CreatePostScreen extends ConsumerStatefulWidget {
  const CreatePostScreen({super.key, this.onSuccess});

  /// Chamado após criar o post com sucesso (ex.: trocar para aba do feed).
  final VoidCallback? onSuccess;

  @override
  ConsumerState<CreatePostScreen> createState() => _CreatePostScreenState();
}

class _CreatePostScreenState extends ConsumerState<CreatePostScreen> {
  final _formKey = GlobalKey<FormState>();
  final _titleController = TextEditingController();
  final _contentController = TextEditingController();
  String _type = 'General';
  String _visibility = 'Public';
  bool _submitting = false;

  @override
  void dispose() {
    _titleController.dispose();
    _contentController.dispose();
    super.dispose();
  }

  Future<void> _submit() async {
    if (!(_formKey.currentState?.validate() ?? false)) return;
    final territoryId = ref.read(selectedTerritoryIdValueProvider);
    if (territoryId == null || territoryId.isEmpty) {
      if (mounted) showErrorSnackBar(context, 'Escolha um território antes de publicar.');
      return;
    }
    setState(() => _submitting = true);
    try {
      final repo = ref.read(feedRepositoryProvider);
      await repo.createPost(
        territoryId: territoryId,
        title: _titleController.text,
        content: _contentController.text,
        type: _type,
        visibility: _visibility,
      );
      ref.invalidate(feedNotifierProvider(territoryId));
      if (!mounted) return;
      _titleController.clear();
      _contentController.clear();
      showSuccessSnackBar(context, 'Post criado com sucesso.');
      widget.onSuccess?.call();
    } on ApiException catch (e) {
      if (mounted) showErrorSnackBar(context, e.userMessage);
    } catch (e) {
      if (mounted) showErrorSnackBar(context, e.toString());
    } finally {
      if (mounted) setState(() => _submitting = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    final territoryId = ref.watch(selectedTerritoryIdValueProvider);
    final hasTerritory = territoryId != null && territoryId.isNotEmpty;

    return Scaffold(
      appBar: AppBar(
        title: Text(AppLocalizations.of(context)!.createPost),
        actions: [
          TextButton(
            onPressed: _submitting || !hasTerritory
                ? null
                : _submit,
            child: _submitting
                ? SizedBox(
                    width: AppConstants.iconSizeMd,
                    height: AppConstants.iconSizeMd,
                    child: const CircularProgressIndicator(strokeWidth: 2),
                  )
                : Text(AppLocalizations.of(context)!.createPost),
          ),
        ],
      ),
      body: !hasTerritory
          ? Center(
              child: Padding(
                padding: const EdgeInsets.all(AppConstants.spacingXl),
                child: Column(
                  mainAxisAlignment: MainAxisAlignment.center,
                  children: [
                    Icon(
                      Icons.terrain_outlined,
                      size: AppConstants.avatarSizeLg,
                      color: Theme.of(context).colorScheme.primary.withOpacity(0.6),
                    ),
                    const SizedBox(height: AppConstants.spacingLg),
                    Text(
                      AppLocalizations.of(context)!.noTerritorySelected,
                      textAlign: TextAlign.center,
                      style: Theme.of(context).textTheme.titleMedium,
                    ),
                    const SizedBox(height: AppConstants.spacingSm),
                    Text(
                      AppLocalizations.of(context)!.chooseTerritoryInExplore,
                      textAlign: TextAlign.center,
                      style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                            color: Theme.of(context).colorScheme.onSurfaceVariant,
                          ),
                    ),
                  ],
                ),
              ),
            )
          : SingleChildScrollView(
              padding: const EdgeInsets.all(AppConstants.spacingMd),
              child: Form(
                key: _formKey,
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.stretch,
                  children: [
                    TextFormField(
                      controller: _titleController,
                      decoration: InputDecoration(
                        labelText: AppLocalizations.of(context)!.title,
                        hintText: AppLocalizations.of(context)!.titleHint,
                        border: const OutlineInputBorder(),
                      ),
                      maxLength: 200,
                      validator: (v) {
                        if (v == null || v.trim().isEmpty) return AppLocalizations.of(context)!.informTitle;
                        return null;
                      },
                    ),
                    const SizedBox(height: AppConstants.spacingMd),
                    TextFormField(
                      controller: _contentController,
                      decoration: InputDecoration(
                        labelText: AppLocalizations.of(context)!.content,
                        hintText: AppLocalizations.of(context)!.contentHint,
                        border: const OutlineInputBorder(),
                        alignLabelWithHint: true,
                      ),
                      maxLines: 5,
                      validator: (v) {
                        if (v == null || v.trim().isEmpty) return AppLocalizations.of(context)!.informContent;
                        return null;
                      },
                    ),
                    const SizedBox(height: AppConstants.spacingLg),
                    DropdownButtonFormField<String>(
                      value: _type,
                      decoration: InputDecoration(
                        labelText: AppLocalizations.of(context)!.type,
                        border: const OutlineInputBorder(),
                      ),
                      items: [
                        DropdownMenuItem(value: 'General', child: Text(AppLocalizations.of(context)!.general)),
                        DropdownMenuItem(value: 'Alert', child: Text(AppLocalizations.of(context)!.alert)),
                      ],
                      onChanged: (v) => setState(() => _type = v ?? 'General'),
                    ),
                    const SizedBox(height: AppConstants.spacingMd),
                    DropdownButtonFormField<String>(
                      value: _visibility,
                      decoration: InputDecoration(
                        labelText: AppLocalizations.of(context)!.visibility,
                        border: const OutlineInputBorder(),
                      ),
                      items: [
                        DropdownMenuItem(value: 'Public', child: Text(AppLocalizations.of(context)!.public)),
                        DropdownMenuItem(value: 'ResidentsOnly', child: Text(AppLocalizations.of(context)!.residentsOnly)),
                      ],
                      onChanged: (v) => setState(() => _visibility = v ?? 'Public'),
                    ),
                  ],
                ),
              ),
            ),
    );
  }
}
