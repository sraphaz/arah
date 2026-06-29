import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/config/constants.dart';
import '../../../../core/network/api_exception.dart';
import '../../../../core/widgets/app_snackbar.dart';
import '../../../../core/widgets/arah_scaffold.dart';
import '../../../../l10n/app_localizations.dart';
import '../providers/governance_provider.dart';
import 'voting_labels.dart';

/// Formulário de criação de votação no território.
class CreateVotingScreen extends ConsumerStatefulWidget {
  const CreateVotingScreen({super.key, required this.territoryId});

  final String territoryId;

  @override
  ConsumerState<CreateVotingScreen> createState() => _CreateVotingScreenState();
}

class _CreateVotingScreenState extends ConsumerState<CreateVotingScreen> {
  final _formKey = GlobalKey<FormState>();
  final _titleController = TextEditingController();
  final _descController = TextEditingController();
  final List<TextEditingController> _optionControllers = [
    TextEditingController(),
    TextEditingController(),
  ];

  String _type = votingTypes.first;
  String _visibility = votingVisibilities.first;
  bool _submitting = false;

  @override
  void dispose() {
    _titleController.dispose();
    _descController.dispose();
    for (final c in _optionControllers) {
      c.dispose();
    }
    super.dispose();
  }

  void _addOption() {
    setState(() => _optionControllers.add(TextEditingController()));
  }

  void _removeOption(int index) {
    if (_optionControllers.length <= 2) return;
    setState(() {
      _optionControllers.removeAt(index).dispose();
    });
  }

  Future<void> _submit() async {
    final l10n = AppLocalizations.of(context)!;
    if (!_formKey.currentState!.validate()) return;

    final options = _optionControllers
        .map((c) => c.text.trim())
        .where((t) => t.isNotEmpty)
        .toList();
    if (options.length < 2) {
      showErrorSnackBar(context, l10n.votingNeedsTwoOptions);
      return;
    }

    setState(() => _submitting = true);
    try {
      await ref
          .read(governanceProvider(widget.territoryId).notifier)
          .createVoting(
            type: _type,
            title: _titleController.text.trim(),
            description: _descController.text.trim(),
            options: options,
            visibility: _visibility,
          );
      if (mounted) Navigator.of(context).pop(true);
    } catch (e) {
      if (mounted) {
        final msg =
            e is ApiException ? e.userMessage : l10n.errorCreateVoting;
        showErrorSnackBar(context, msg);
        setState(() => _submitting = false);
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    return ArahScaffold(
      appBar: AppBar(title: Text(l10n.createVoting)),
      body: Form(
        key: _formKey,
        child: ListView(
          padding: const EdgeInsets.all(AppConstants.spacingMd),
          children: [
            TextFormField(
              controller: _titleController,
              decoration: InputDecoration(labelText: l10n.votingTitleLabel),
              textInputAction: TextInputAction.next,
              validator: (v) =>
                  (v == null || v.trim().isEmpty) ? l10n.requiredField : null,
            ),
            const SizedBox(height: AppConstants.spacingMd),
            TextFormField(
              controller: _descController,
              decoration:
                  InputDecoration(labelText: l10n.votingDescriptionLabel),
              maxLines: 3,
            ),
            const SizedBox(height: AppConstants.spacingMd),
            DropdownButtonFormField<String>(
              value: _type,
              decoration: InputDecoration(labelText: l10n.votingTypeLabel),
              items: votingTypes
                  .map((t) => DropdownMenuItem(
                        value: t,
                        child: Text(votingTypeLabel(l10n, t)),
                      ))
                  .toList(),
              onChanged: (v) => setState(() => _type = v ?? _type),
            ),
            const SizedBox(height: AppConstants.spacingMd),
            DropdownButtonFormField<String>(
              value: _visibility,
              decoration:
                  InputDecoration(labelText: l10n.votingVisibilityLabel),
              items: votingVisibilities
                  .map((v) => DropdownMenuItem(
                        value: v,
                        child: Text(votingVisibilityLabel(l10n, v)),
                      ))
                  .toList(),
              onChanged: (v) => setState(() => _visibility = v ?? _visibility),
            ),
            const SizedBox(height: AppConstants.spacingLg),
            Text(l10n.votingOptionsLabel,
                style: Theme.of(context).textTheme.titleSmall),
            const SizedBox(height: AppConstants.spacingSm),
            ..._buildOptionFields(l10n),
            Align(
              alignment: Alignment.centerLeft,
              child: TextButton.icon(
                onPressed: _addOption,
                icon: const Icon(Icons.add),
                label: Text(l10n.addOption),
              ),
            ),
            const SizedBox(height: AppConstants.spacingLg),
            FilledButton(
              onPressed: _submitting ? null : _submit,
              child: _submitting
                  ? const SizedBox(
                      height: AppConstants.loadingIndicatorSize,
                      width: AppConstants.loadingIndicatorSize,
                      child: CircularProgressIndicator(strokeWidth: 2),
                    )
                  : Text(l10n.create),
            ),
          ],
        ),
      ),
    );
  }

  List<Widget> _buildOptionFields(AppLocalizations l10n) {
    return List.generate(_optionControllers.length, (index) {
      return Padding(
        padding: const EdgeInsets.only(bottom: AppConstants.spacingSm),
        child: Row(
          children: [
            Expanded(
              child: TextFormField(
                controller: _optionControllers[index],
                decoration: InputDecoration(
                  labelText: '${l10n.optionLabel} ${index + 1}',
                ),
              ),
            ),
            if (_optionControllers.length > 2)
              IconButton(
                icon: const Icon(Icons.remove_circle_outline),
                tooltip: l10n.removeOption,
                onPressed: () => _removeOption(index),
              ),
          ],
        ),
      );
    });
  }
}
