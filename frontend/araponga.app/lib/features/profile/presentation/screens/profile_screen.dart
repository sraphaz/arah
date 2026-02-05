import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../core/config/constants.dart';
import '../../../../core/network/api_exception.dart';
import '../../../../core/widgets/app_snackbar.dart';
import '../../../../l10n/app_localizations.dart';
import '../../data/models/me_profile.dart';
import '../../../auth/presentation/providers/auth_state_provider.dart';
import '../providers/me_profile_provider.dart';
import '../widgets/interests_sheet.dart';
import '../widgets/preferences_sheet.dart';

/// Perfil: dados do usuário via BFF me/profile; edição (nome, bio) em bottom sheet.
class ProfileScreen extends ConsumerWidget {
  const ProfileScreen({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final auth = ref.watch(authStateProvider);
    final session = auth.valueOrNull;

    if (session == null) {
      return Scaffold(
        appBar: AppBar(title: Text(AppLocalizations.of(context)!.profile)),
        body: Center(
          child: Padding(
            padding: const EdgeInsets.all(AppConstants.spacingLg),
            child: Column(
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                Text(
                  'Ará',
                  style: Theme.of(context).textTheme.headlineMedium?.copyWith(
                        color: Theme.of(context).colorScheme.primary,
                        fontWeight: FontWeight.bold,
                      ),
                ),
                const SizedBox(height: AppConstants.spacingMd),
                Text(
                  'Entre na sua conta para acessar perfil, publicar e notificações.',
                  textAlign: TextAlign.center,
                  style: Theme.of(context).textTheme.bodyLarge?.copyWith(
                        color: Theme.of(context).colorScheme.onSurfaceVariant,
                      ),
                ),
                const SizedBox(height: AppConstants.spacingLg),
                FilledButton(
                  onPressed: () => context.push('/login'),
                  child: Text(AppLocalizations.of(context)!.login),
                ),
              ],
            ),
          ),
        ),
      );
    }

    final profileAsync = ref.watch(meProfileProvider);
    final authUser = session.user;

    return Scaffold(
      appBar: AppBar(
        title: Text(AppLocalizations.of(context)!.profile),
        actions: [
          IconButton(
            icon: const Icon(Icons.settings),
            onPressed: () => _showSettingsSheet(context),
          ),
        ],
      ),
      body: profileAsync.when(
        data: (profile) => _ProfileBody(
          profile: profile,
          onEditTap: () => _showEditProfileSheet(context, ref, profile),
          onLogout: () async {
            await ref.read(authStateProvider.notifier).logout();
            if (context.mounted) context.go('/login');
          },
        ),
        loading: () => Center(
          child: Column(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              if (authUser != null) ...[
                Text(
                  authUser.displayName,
                  style: Theme.of(context).textTheme.titleLarge,
                ),
                if (authUser.email != null && authUser.email!.isNotEmpty)
                  Padding(
                    padding: const EdgeInsets.only(top: AppConstants.spacingSm),
                    child: Text(
                      authUser.email!,
                      style: Theme.of(context).textTheme.bodySmall?.copyWith(
                            color: Theme.of(context).colorScheme.onSurfaceVariant,
                          ),
                    ),
                  ),
                const SizedBox(height: AppConstants.spacingLg),
              ],
              const CircularProgressIndicator(),
            ],
          ),
        ),
        error: (err, _) => Center(
          child: Padding(
            padding: const EdgeInsets.all(AppConstants.spacingLg),
            child: Column(
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                Icon(Icons.error_outline, size: AppConstants.iconSizeLg, color: Theme.of(context).colorScheme.error),
                const SizedBox(height: AppConstants.spacingMd),
                Text(
                  err is ApiException ? (err as ApiException).userMessage : AppLocalizations.of(context)!.errorLoad,
                  textAlign: TextAlign.center,
                  style: Theme.of(context).textTheme.bodyLarge,
                ),
                const SizedBox(height: AppConstants.spacingMd),
                FilledButton.tonal(
                  onPressed: () => ref.invalidate(meProfileProvider),
                  child: Text(AppLocalizations.of(context)!.tryAgain),
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }

  void _showSettingsSheet(BuildContext context) {
    showModalBottomSheet<void>(
      context: context,
      shape: RoundedRectangleBorder(
        borderRadius: BorderRadius.vertical(top: Radius.circular(AppConstants.radiusLg)),
      ),
      builder: (ctx) => SafeArea(
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            ListTile(
              leading: const Icon(Icons.interests),
              title: Text(AppLocalizations.of(ctx)!.myInterests),
              onTap: () {
                Navigator.of(ctx).pop();
                showModalBottomSheet<void>(
                  context: context,
                  isScrollControlled: true,
                  useSafeArea: true,
                  shape: RoundedRectangleBorder(
                    borderRadius: BorderRadius.vertical(top: Radius.circular(AppConstants.radiusLg)),
                  ),
                  builder: (_) => const InterestsSheet(),
                );
              },
            ),
            ListTile(
              leading: const Icon(Icons.notifications_active_outlined),
              title: Text(AppLocalizations.of(ctx)!.notificationPreferences),
              onTap: () {
                Navigator.of(ctx).pop();
                showModalBottomSheet<void>(
                  context: context,
                  isScrollControlled: true,
                  useSafeArea: true,
                  shape: RoundedRectangleBorder(
                    borderRadius: BorderRadius.vertical(top: Radius.circular(AppConstants.radiusLg)),
                  ),
                  builder: (_) => const PreferencesSheet(),
                );
              },
            ),
          ],
        ),
      ),
    );
  }

  void _showEditProfileSheet(BuildContext context, WidgetRef ref, MeProfile profile) {
    final displayNameController = TextEditingController(text: profile.displayName);
    final bioController = TextEditingController(text: profile.bio ?? '');

    showModalBottomSheet<void>(
      context: context,
      isScrollControlled: true,
      useSafeArea: true,
      shape: RoundedRectangleBorder(
        borderRadius: BorderRadius.vertical(top: Radius.circular(AppConstants.radiusLg)),
      ),
      builder: (ctx) => Padding(
        padding: EdgeInsets.only(
          bottom: MediaQuery.of(ctx).viewInsets.bottom,
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
                  color: Theme.of(ctx).colorScheme.onSurfaceVariant.withOpacity(0.4),
                  borderRadius: BorderRadius.circular(2),
                ),
              ),
            ),
            Text(
                AppLocalizations.of(ctx)!.editProfile,
                style: Theme.of(ctx).textTheme.titleLarge,
              ),
              const SizedBox(height: AppConstants.spacingMd),
              TextField(
                controller: displayNameController,
                decoration: InputDecoration(
                  labelText: AppLocalizations.of(ctx)!.name,
                  border: const OutlineInputBorder(),
                ),
                textCapitalization: TextCapitalization.words,
              ),
              const SizedBox(height: AppConstants.spacingMd),
              TextField(
                controller: bioController,
                decoration: InputDecoration(
                  labelText: AppLocalizations.of(ctx)!.bioOptional,
                  border: const OutlineInputBorder(),
                  alignLabelWithHint: true,
                ),
                maxLines: 3,
              ),
              const SizedBox(height: AppConstants.spacingLg),
              FilledButton(
                onPressed: () async {
                  final repo = ref.read(meProfileRepositoryProvider);
                  final name = displayNameController.text.trim();
                  if (name.isEmpty) {
                    if (ctx.mounted) showErrorSnackBar(ctx, AppLocalizations.of(ctx)!.nameRequired);
                    return;
                  }
                  try {
                    await repo.updateDisplayName(name);
                    await repo.updateBio(
                      bioController.text.trim().isEmpty ? null : bioController.text.trim(),
                    );
                    if (ctx.mounted) {
                      ref.invalidate(meProfileProvider);
                      Navigator.of(ctx).pop();
                      showSuccessSnackBar(ctx, AppLocalizations.of(ctx)!.profileUpdated);
                    }
                  } catch (e) {
                    if (ctx.mounted) {
                      showErrorSnackBar(ctx, e is ApiException ? (e as ApiException).userMessage : 'Erro ao salvar');
                    }
                  }
                },
                child: Text(AppLocalizations.of(ctx)!.save),
              ),
            ],
          ),
        ),
    );
  }
}

class _ProfileBody extends StatelessWidget {
  const _ProfileBody({
    required this.profile,
    required this.onEditTap,
    required this.onLogout,
  });

  final MeProfile profile;
  final VoidCallback onEditTap;
  final VoidCallback onLogout;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    final initial = profile.displayName.isNotEmpty
        ? profile.displayName[0].toUpperCase()
        : '?';

    return SingleChildScrollView(
      child: Column(
        children: [
          const SizedBox(height: AppConstants.spacingLg),
          CircleAvatar(
            radius: AppConstants.avatarRadiusProfile,
            backgroundColor: Theme.of(context).colorScheme.primaryContainer,
            backgroundImage: profile.avatarUrl != null && profile.avatarUrl!.isNotEmpty
                ? NetworkImage(profile.avatarUrl!)
                : null,
            child: profile.avatarUrl == null || profile.avatarUrl!.isEmpty
                ? Text(
                    initial,
                    style: Theme.of(context).textTheme.headlineMedium?.copyWith(
                          color: Theme.of(context).colorScheme.onPrimaryContainer,
                        ),
                  )
                : null,
          ),
          const SizedBox(height: AppConstants.spacingMd),
          Text(
            profile.displayName,
            style: Theme.of(context).textTheme.titleLarge,
          ),
          if (profile.email != null && profile.email!.isNotEmpty) ...[
            const SizedBox(height: AppConstants.spacingSm),
            Text(
              profile.email!,
              style: Theme.of(context).textTheme.bodySmall?.copyWith(
                    color: Theme.of(context).colorScheme.onSurfaceVariant,
                  ),
            ),
          ],
          if (profile.bio != null && profile.bio!.isNotEmpty) ...[
            const SizedBox(height: AppConstants.spacingSm),
            Padding(
              padding: const EdgeInsets.symmetric(horizontal: AppConstants.spacingLg),
              child: Text(
                profile.bio!,
                textAlign: TextAlign.center,
                style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                      color: Theme.of(context).colorScheme.onSurfaceVariant,
                    ),
              ),
            ),
          ],
          const SizedBox(height: AppConstants.spacingXl),
          ListTile(
            leading: const Icon(Icons.person_outline),
            title: Text(l10n.editProfile),
            onTap: onEditTap,
          ),
          ListTile(
            leading: const Icon(Icons.terrain_outlined),
            title: Text(l10n.myTerritory),
            onTap: () {},
          ),
          ListTile(
            leading: const Icon(Icons.notifications_outlined),
            title: Text(l10n.notifications),
            onTap: () {},
          ),
          const Divider(),
          ListTile(
            leading: Icon(Icons.logout, color: Theme.of(context).colorScheme.error),
            title: Text(
              l10n.logout,
              style: Theme.of(context).textTheme.bodyLarge?.copyWith(
                    color: Theme.of(context).colorScheme.error,
                  ),
            ),
            onTap: onLogout,
          ),
        ],
      ),
    );
  }
}
