import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:intl/intl.dart';

import '../../../../core/config/constants.dart';
import '../../../../core/network/api_exception.dart';
import '../../../../core/widgets/app_snackbar.dart';
import '../../../../core/widgets/arah_scaffold.dart';
import '../../../../l10n/app_localizations.dart';
import '../providers/territory_events_provider.dart';

/// Formulário de criação de evento no território ativo (BFF events/create-event).
class CreateEventScreen extends ConsumerStatefulWidget {
  const CreateEventScreen({super.key, required this.territoryId});

  final String territoryId;

  @override
  ConsumerState<CreateEventScreen> createState() => _CreateEventScreenState();
}

class _CreateEventScreenState extends ConsumerState<CreateEventScreen> {
  final _formKey = GlobalKey<FormState>();
  final _titleController = TextEditingController();
  final _descController = TextEditingController();
  final _locationController = TextEditingController();

  DateTime? _startsAt;
  DateTime? _endsAt;
  bool _submitting = false;

  @override
  void dispose() {
    _titleController.dispose();
    _descController.dispose();
    _locationController.dispose();
    super.dispose();
  }

  Future<DateTime?> _pickDateTime(DateTime? initial) async {
    final now = DateTime.now();
    final base = initial ?? now;
    final date = await showDatePicker(
      context: context,
      initialDate: base,
      firstDate: now.subtract(const Duration(days: 1)),
      lastDate: now.add(const Duration(days: 365 * 2)),
    );
    if (date == null || !mounted) return null;
    final time = await showTimePicker(
      context: context,
      initialTime: TimeOfDay.fromDateTime(base),
    );
    if (time == null) return null;
    return DateTime(date.year, date.month, date.day, time.hour, time.minute);
  }

  Future<void> _submit() async {
    final l10n = AppLocalizations.of(context)!;
    if (!_formKey.currentState!.validate()) return;
    if (_startsAt == null) {
      showErrorSnackBar(context, l10n.eventStartRequired);
      return;
    }
    if (_endsAt != null && _endsAt!.isBefore(_startsAt!)) {
      showErrorSnackBar(context, l10n.eventEndBeforeStart);
      return;
    }

    setState(() => _submitting = true);
    try {
      await ref.read(territoryEventsProvider(widget.territoryId).notifier).createEvent(
            title: _titleController.text.trim(),
            description: _descController.text.trim(),
            startsAtUtc: _startsAt!,
            endsAtUtc: _endsAt,
            locationLabel: _locationController.text.trim(),
          );
      if (mounted) Navigator.of(context).pop(true);
    } catch (e) {
      if (mounted) {
        final msg = e is ApiException ? e.userMessage : l10n.errorCreateEvent;
        showErrorSnackBar(context, msg);
        setState(() => _submitting = false);
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    final dateFormat = DateFormat('dd/MM/yyyy HH:mm');

    return ArahScaffold(
      appBar: AppBar(title: Text(l10n.createEvent)),
      body: Form(
        key: _formKey,
        child: ListView(
          padding: const EdgeInsets.all(AppConstants.spacingMd),
          children: [
            TextFormField(
              controller: _titleController,
              decoration: InputDecoration(labelText: l10n.eventTitleLabel),
              textInputAction: TextInputAction.next,
              validator: (v) =>
                  (v == null || v.trim().isEmpty) ? l10n.requiredField : null,
            ),
            const SizedBox(height: AppConstants.spacingMd),
            TextFormField(
              controller: _descController,
              decoration: InputDecoration(labelText: l10n.eventDescriptionLabel),
              maxLines: 3,
            ),
            const SizedBox(height: AppConstants.spacingMd),
            _DateTimeField(
              label: l10n.eventStartLabel,
              value: _startsAt,
              placeholder: l10n.selectDateTime,
              format: dateFormat,
              onTap: () async {
                final picked = await _pickDateTime(_startsAt);
                if (picked != null) setState(() => _startsAt = picked);
              },
            ),
            const SizedBox(height: AppConstants.spacingSm),
            _DateTimeField(
              label: l10n.eventEndLabel,
              value: _endsAt,
              placeholder: l10n.selectDateTime,
              format: dateFormat,
              onClear: _endsAt == null ? null : () => setState(() => _endsAt = null),
              onTap: () async {
                final picked = await _pickDateTime(_endsAt ?? _startsAt);
                if (picked != null) setState(() => _endsAt = picked);
              },
            ),
            const SizedBox(height: AppConstants.spacingMd),
            TextFormField(
              controller: _locationController,
              decoration: InputDecoration(labelText: l10n.eventLocationLabel),
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
}

class _DateTimeField extends StatelessWidget {
  const _DateTimeField({
    required this.label,
    required this.value,
    required this.placeholder,
    required this.format,
    required this.onTap,
    this.onClear,
  });

  final String label;
  final DateTime? value;
  final String placeholder;
  final DateFormat format;
  final VoidCallback onTap;
  final VoidCallback? onClear;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return InkWell(
      onTap: onTap,
      borderRadius: BorderRadius.circular(AppConstants.radiusSm),
      child: InputDecorator(
        decoration: InputDecoration(
          labelText: label,
          suffixIcon: onClear != null
              ? IconButton(icon: const Icon(Icons.clear), onPressed: onClear)
              : const Icon(Icons.calendar_today_outlined),
        ),
        child: Text(
          value != null ? format.format(value!) : placeholder,
          style: theme.textTheme.bodyLarge?.copyWith(
            color: value != null
                ? theme.colorScheme.onSurface
                : theme.colorScheme.onSurfaceVariant,
          ),
        ),
      ),
    );
  }
}
