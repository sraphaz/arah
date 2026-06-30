import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'app_router.dart';
import 'core/providers/theme_mode_provider.dart';
import 'core/services/device_registration_listener.dart';
import 'core/config/brand_config.dart';
import 'core/theme/app_theme.dart';
import 'l10n/app_localizations.dart';

class ArahApp extends ConsumerWidget {
  const ArahApp({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final router = ref.watch(goRouterProvider);
    final themeMode = ref.watch(themeModeProvider);
    return DeviceRegistrationListener(
      child: MaterialApp.router(
        title: BrandConfig.name,
        debugShowCheckedModeBanner: false,
        theme: AppTheme.light,
        darkTheme: AppTheme.dark,
        themeMode: themeMode,
        routerConfig: router,
        locale: const Locale('pt'),
        localizationsDelegates: AppLocalizations.localizationsDelegates,
        supportedLocales: AppLocalizations.supportedLocales,
        localeResolutionCallback: (locale, supported) {
          for (final l in supported) {
            if (l.languageCode == locale?.languageCode) return l;
          }
          return const Locale('pt');
        },
      ),
    );
  }
}
