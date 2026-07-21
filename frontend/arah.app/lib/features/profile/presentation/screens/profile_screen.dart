import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../core/config/constants.dart';
import '../../../../core/network/api_exception.dart';
import '../../../../core/theme/app_design_tokens.dart';
import '../../../../core/widgets/app_snackbar.dart';
import '../../../../core/widgets/arah_brand_header.dart';
import '../../../../core/widgets/arah_role_badge.dart';
import '../../../../core/widgets/arah_scaffold.dart';
import '../../../../core/widgets/profile_skeleton.dart';
import '../../../../l10n/app_localizations.dart';
import '../../../auth/presentation/providers/auth_state_provider.dart';
import '../../../membership/presentation/providers/membership_provider.dart';
import '../../../territories/presentation/widgets/territory_indicator_bar.dart';
import '../../data/models/me_profile.dart';
import '../providers/me_profile_provider.dart';
import '../widgets/interests_sheet.dart';
import '../widgets/preferences_sheet.dart';

/// Perfil: dados do usuário via BFF me/profile; edição (nome, bio) em bottom sheet.
class ProfileScreen extends ConsumerWidget {
  const ProfileScreen({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final l10n = AppLocalizations.of(context)!;
    final auth = ref.watch(authStateProvider);
    final session = auth.valueOrNull;

    if (session == null) {
      return ArahScaffold(
        appBar: AppBar(title: Text(l10n.profile)),
        body: Center(
          child: Padding(
            padding: const EdgeInsets.all(AppConstants.spacingLg),
            child: Column(
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                ArahBrandHeader(
                  subtitle: l10n.enterToAccess,
                  size: ArahBrandHeaderSize.medium,
                ),
                const SizedBox(height: AppConstants.spacingLg),
                FilledButton(
                  onPressed: () => context.push('/login'),
                  child: Text(l10n.login),
                ),
              ],
            ),
          ),
        ),
      );
    }

    final profileAsync = ref.watch(meProfileProvider);
    final authUser = session.user;

    return ArahScaffold(
      appBar: AppBar(
        title: Text(l10n.profile),
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
          onMyTerritory: () => TerritoryIndicatorBar.showTerritorySelectorSheet(context),
          onNotifications: () => context.push('/notifications'),
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
              const ProfileSkeleton(),
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
                  err is ApiException ? err.userMessage : l10n.errorLoad,
                  textAlign: TextAlign.center,
                  style: Theme.of(context).textTheme.bodyLarge,
                ),
                const SizedBox(height: AppConstants.spacingMd),
                FilledButton.tonal(
                  onPressed: () => ref.invalidate(meProfileProvider),
                  child: Text(l10n.tryAgain),
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }

  void _showSettingsSheet(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    showModalBottomSheet<void>(
      context: context,
      shape: const RoundedRectangleBorder(
        borderRadius: BorderRadius.vertical(top: Radius.circular(AppConstants.radiusLg)),
      ),
      builder: (ctx) => SafeArea(
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            ListTile(
              leading: const Icon(Icons.people_outline),
              title: Text(l10n.connections),
              onTap: () {
                Navigator.of(ctx).pop();
                context.push('/connections');
              },
            ),
            ListTile(
              leading: const Icon(Icons.home_work_outlined),
              title: Text(l10n.membership),
              onTap: () {
                Navigator.of(ctx).pop();
                context.push('/membership');
              },
            ),
            ListTile(
              leading: const Icon(Icons.storefront_outlined),
              title: Text(l10n.marketplace),
              onTap: () {
                Navigator.of(ctx).pop();
                context.push('/marketplace');
              },
            ),
            ListTile(
              leading: const Icon(Icons.chat_outlined),
              title: Text(l10n.chat),
              onTap: () {
                Navigator.of(ctx).pop();
                context.push('/chat');
              },
            ),
            ListTile(
              leading: const Icon(Icons.warning_amber_outlined),
              title: Text(l10n.alertsTitle),
              onTap: () {
                Navigator.of(ctx).pop();
                context.push('/alerts');
              },
            ),
            ListTile(
              leading: const Icon(Icons.how_to_vote_outlined),
              title: Text(l10n.governance),
              onTap: () {
                Navigator.of(ctx).pop();
                context.push('/governance');
              },
            ),
            ListTile(
              leading: const Icon(Icons.gavel_outlined),
              title: Text(l10n.moderation),
              onTap: () {
                Navigator.of(ctx).pop();
                context.push('/moderation');
              },
            ),
            ListTile(
              leading: const Icon(Icons.inventory_2_outlined),
              title: Text(l10n.assetsTitle),
              onTap: () {
                Navigator.of(ctx).pop();
                context.push('/assets');
              },
            ),
            ListTile(
              leading: const Icon(Icons.card_membership_outlined),
              title: Text(l10n.subscriptions),
              onTap: () {
                Navigator.of(ctx).pop();
                context.push('/subscriptions');
              },
            ),
            ListTile(
              leading: const Icon(Icons.interests),
              title: Text(AppLocalizations.of(ctx)!.myInterests),
              onTap: () {
                Navigator.of(ctx).pop();
                showModalBottomSheet<void>(
                  context: context,
                  isScrollControlled: true,
                  useSafeArea: true,
                  shape: const RoundedRectangleBorder(
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
                  shape: const RoundedRectangleBorder(
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
      shape: const RoundedRectangleBorder(
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
                  color: Theme.of(ctx).colorScheme.onSurfaceVariant.withValues(alpha: 0.4),
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
                    showErrorSnackBar(
                      ctx,
                      e is ApiException
                          ? e.userMessage
                          : AppLocalizations.of(ctx)!.errorLoad,
                    );
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

class _ProfileBody extends ConsumerWidget {
  const _ProfileBody({
    required this.profile,
    required this.onEditTap,
    required this.onMyTerritory,
    required this.onNotifications,
    required this.onLogout,
  });

  final MeProfile profile;
  final VoidCallback onEditTap;
  final VoidCallback onMyTerritory;
  final VoidCallback onNotifications;
  final VoidCallback onLogout;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final l10n = AppLocalizations.of(context)!;
    final colors = context.appColors;
    final membership = ref.watch(membershipProvider).membership;
    final role = membership?.role ?? 'VISITOR';
    final initial = profile.displayName.isNotEmpty
        ? profile.displayName[0].toUpperCase()
        : '?';

    final tools = <_ProfileTool>[
      _ProfileTool(l10n.membership, Icons.home_work_outlined, '/membership'),
      _ProfileTool(l10n.marketplace, Icons.storefront_outlined, '/marketplace'),
      _ProfileTool(l10n.connections, Icons.people_outline, '/connections'),
      _ProfileTool(l10n.governance, Icons.how_to_vote_outlined, '/governance'),
      _ProfileTool(l10n.moderation, Icons.gavel_outlined, '/moderation'),
      _ProfileTool(l10n.subscriptions, Icons.card_membership_outlined, '/subscriptions'),
      _ProfileTool(l10n.assetsTitle, Icons.inventory_2_outlined, '/assets'),
    ];

    return SingleChildScrollView(
      padding: const EdgeInsets.only(bottom: AppConstants.spacingXl),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          const SizedBox(height: AppConstants.spacingLg),
          Padding(
            padding: const EdgeInsets.symmetric(horizontal: AppConstants.spacingLg),
            child: Column(
              children: [
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
                  textAlign: TextAlign.center,
                  style: Theme.of(context).textTheme.titleLarge?.copyWith(
                        fontFamily: AppDesignTokens.fontFamilyDisplay,
                      ),
                ),
                const SizedBox(height: AppConstants.spacingSm),
                ArahRoleBadge(role: role),
                if (profile.email != null && profile.email!.isNotEmpty) ...[
                  const SizedBox(height: AppConstants.spacingSm),
                  Text(
                    profile.email!,
                    textAlign: TextAlign.center,
                    style: Theme.of(context).textTheme.bodySmall?.copyWith(
                          color: colors.onSurfaceVariant,
                        ),
                  ),
                ],
                if (profile.bio != null && profile.bio!.isNotEmpty) ...[
                  const SizedBox(height: AppConstants.spacingSm),
                  Text(
                    profile.bio!,
                    textAlign: TextAlign.center,
                    style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                          color: colors.onSurfaceVariant,
                        ),
                  ),
                ],
              ],
            ),
          ),
          const SizedBox(height: AppConstants.spacingXl),
          Padding(
            padding: const EdgeInsets.symmetric(horizontal: AppConstants.spacingMd),
            child: Text(
              l10n.profileTools,
              style: Theme.of(context).textTheme.titleMedium,
            ),
          ),
          const SizedBox(height: AppConstants.spacingSm),
          Padding(
            padding: const EdgeInsets.symmetric(horizontal: AppConstants.spacingMd),
            child: Wrap(
              spacing: AppConstants.spacingSm,
              runSpacing: AppConstants.spacingSm,
              children: [
                for (final tool in tools)
                  _ProfileToolTile(
                    label: tool.label,
                    icon: tool.icon,
                    onTap: () => context.push(tool.route),
                  ),
              ],
            ),
          ),
          const SizedBox(height: AppConstants.spacingLg),
          ListTile(
            leading: const Icon(Icons.person_outline),
            title: Text(l10n.editProfile),
            onTap: onEditTap,
          ),
          ListTile(
            leading: const Icon(Icons.terrain_outlined),
            title: Text(l10n.myTerritory),
            trailing: const Icon(Icons.chevron_right),
            onTap: onMyTerritory,
          ),
          ListTile(
            leading: const Icon(Icons.notifications_outlined),
            title: Text(l10n.notifications),
            trailing: const Icon(Icons.chevron_right),
            onTap: onNotifications,
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

class _ProfileTool {
  const _ProfileTool(this.label, this.icon, this.route);

  final String label;
  final IconData icon;
  final String route;
}

class _ProfileToolTile extends StatelessWidget {
  const _ProfileToolTile({
    required this.label,
    required this.icon,
    required this.onTap,
  });

  final String label;
  final IconData icon;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    final colors = context.appColors;
    final width = (MediaQuery.sizeOf(context).width - AppConstants.spacingMd * 2 - AppConstants.spacingSm) / 2;

    return SizedBox(
      width: width,
      child: Material(
        color: colors.surfaceContainer,
        borderRadius: BorderRadius.circular(AppConstants.radiusMd),
        child: InkWell(
          onTap: onTap,
          borderRadius: BorderRadius.circular(AppConstants.radiusMd),
          child: ConstrainedBox(
            constraints: const BoxConstraints(minHeight: AppConstants.minTouchTargetSize),
            child: Padding(
              padding: const EdgeInsets.symmetric(
                horizontal: AppConstants.spacingMd,
                vertical: AppConstants.spacingMd,
              ),
              child: Row(
                children: [
                  Icon(icon, size: AppConstants.iconSizeMd, color: colors.primary),
                  const SizedBox(width: AppConstants.spacingSm),
                  Expanded(
                    child: Text(
                      label,
                      style: Theme.of(context).textTheme.labelLarge,
                      maxLines: 2,
                      overflow: TextOverflow.ellipsis,
                    ),
                  ),
                ],
              ),
            ),
          ),
        ),
      ),
    );
  }
}
