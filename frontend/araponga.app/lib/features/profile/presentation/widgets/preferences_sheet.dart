import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/config/constants.dart';
import '../../../../core/widgets/app_snackbar.dart';
import '../../../../l10n/app_localizations.dart';
import '../../data/repositories/me_profile_repository.dart';
import '../providers/me_profile_provider.dart';

/// Sheet: preferências de notificação (GET me/preferences, PUT me/preferences/notifications).
class PreferencesSheet extends ConsumerStatefulWidget {
  const PreferencesSheet({super.key});

  @override
  ConsumerState<PreferencesSheet> createState() => _PreferencesSheetState();
}

class _PreferencesSheetState extends ConsumerState<PreferencesSheet> {
  NotificationPrefs? _prefs;
  bool _loading = false;
  bool _saving = false;

  @override
  Widget build(BuildContext context) {
    final prefsAsync = ref.watch(mePreferencesProvider);

    return Padding(
      padding: EdgeInsets.only(
        bottom: MediaQuery.of(context).viewInsets.bottom,
        left: AppConstants.spacingLg,
        right: AppConstants.spacingLg,
        top: AppConstants.spacingMd,
      ),
      child: SingleChildScrollView(
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
              AppLocalizations.of(context)!.notificationPreferences,
              style: Theme.of(context).textTheme.titleLarge,
            ),
            const SizedBox(height: AppConstants.spacingLg),
            prefsAsync.when(
              data: (preferences) {
                _prefs ??= preferences.notifications;
                final p = _prefs!;
                return Column(
                  children: [
                    _SwitchRow(
                      title: AppLocalizations.of(context)!.notifPosts,
                      value: p.postsEnabled,
                      onChanged: (v) => setState(() => _prefs = NotificationPrefs(
                            postsEnabled: v,
                            commentsEnabled: p.commentsEnabled,
                            eventsEnabled: p.eventsEnabled,
                            alertsEnabled: p.alertsEnabled,
                            marketplaceEnabled: p.marketplaceEnabled,
                            moderationEnabled: p.moderationEnabled,
                            membershipRequestsEnabled: p.membershipRequestsEnabled,
                          )),
                    ),
                    _SwitchRow(
                      title: AppLocalizations.of(context)!.notifComments,
                      value: p.commentsEnabled,
                      onChanged: (v) => setState(() => _prefs = NotificationPrefs(
                            postsEnabled: p.postsEnabled,
                            commentsEnabled: v,
                            eventsEnabled: p.eventsEnabled,
                            alertsEnabled: p.alertsEnabled,
                            marketplaceEnabled: p.marketplaceEnabled,
                            moderationEnabled: p.moderationEnabled,
                            membershipRequestsEnabled: p.membershipRequestsEnabled,
                          )),
                    ),
                    _SwitchRow(
                      title: AppLocalizations.of(context)!.notifEvents,
                      value: p.eventsEnabled,
                      onChanged: (v) => setState(() => _prefs = NotificationPrefs(
                            postsEnabled: p.postsEnabled,
                            commentsEnabled: p.commentsEnabled,
                            eventsEnabled: v,
                            alertsEnabled: p.alertsEnabled,
                            marketplaceEnabled: p.marketplaceEnabled,
                            moderationEnabled: p.moderationEnabled,
                            membershipRequestsEnabled: p.membershipRequestsEnabled,
                          )),
                    ),
                    _SwitchRow(
                      title: AppLocalizations.of(context)!.notifAlerts,
                      value: p.alertsEnabled,
                      onChanged: (v) => setState(() => _prefs = NotificationPrefs(
                            postsEnabled: p.postsEnabled,
                            commentsEnabled: p.commentsEnabled,
                            eventsEnabled: p.eventsEnabled,
                            alertsEnabled: v,
                            marketplaceEnabled: p.marketplaceEnabled,
                            moderationEnabled: p.moderationEnabled,
                            membershipRequestsEnabled: p.membershipRequestsEnabled,
                          )),
                    ),
                    const SizedBox(height: AppConstants.spacingMd),
                    FilledButton(
                      onPressed: _saving ? null : () => _save(context),
                      child: _saving
                          ? const SizedBox(
                              width: 20,
                              height: 20,
                              child: CircularProgressIndicator(strokeWidth: 2),
                            )
                          : Text(AppLocalizations.of(context)!.save),
                    ),
                  ],
                );
              },
              loading: () => const Center(child: CircularProgressIndicator()),
              error: (e, _) => Text(
                AppLocalizations.of(context)!.errorLoad,
                style: Theme.of(context).textTheme.bodySmall?.copyWith(
                      color: Theme.of(context).colorScheme.error,
                    ),
              ),
            ),
          ],
        ),
      ),
    );
  }

  Future<void> _save(BuildContext context) async {
    final p = _prefs;
    if (p == null) return;
    setState(() => _saving = true);
    try {
      final repo = ref.read(meProfileRepositoryProvider);
      await repo.updateNotificationPreferences(p);
      ref.invalidate(mePreferencesProvider);
      if (context.mounted) showSuccessSnackBar(context, AppLocalizations.of(context)!.profileUpdated);
    } catch (e) {
      if (context.mounted) showErrorSnackBar(context, e.toString());
    } finally {
      if (context.mounted) setState(() => _saving = false);
    }
  }
}

class _SwitchRow extends StatelessWidget {
  const _SwitchRow({required this.title, required this.value, required this.onChanged});

  final String title;
  final bool value;
  final ValueChanged<bool> onChanged;

  @override
  Widget build(BuildContext context) {
    return SwitchListTile(
      title: Text(title),
      value: value,
      onChanged: onChanged,
    );
  }
}
