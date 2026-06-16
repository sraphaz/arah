import 'dart:io';

import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:shared_preferences/shared_preferences.dart';

import '../../features/auth/presentation/providers/auth_state_provider.dart';
import '../../features/auth/data/repositories/auth_repository.dart';
import '../network/bff_client.dart';
import '../providers/app_providers.dart';

/// Registra dispositivo no BFF (`me/devices`) quando o usuário está autenticado.
class DeviceRegistrationListener extends ConsumerStatefulWidget {
  const DeviceRegistrationListener({super.key, required this.child});

  final Widget child;

  @override
  ConsumerState<DeviceRegistrationListener> createState() => _DeviceRegistrationListenerState();
}

class _DeviceRegistrationListenerState extends ConsumerState<DeviceRegistrationListener> {
  String? _lastRegisteredToken;

  @override
  Widget build(BuildContext context) {
    ref.listen<AsyncValue<AuthSession?>>(authStateProvider, (_, next) {
      final session = next.valueOrNull;
      if (session?.accessToken != null && session!.accessToken.isNotEmpty) {
        _registerIfNeeded(session.accessToken);
      }
    });

    return widget.child;
  }

  Future<void> _registerIfNeeded(String accessToken) async {
    final prefs = await SharedPreferences.getInstance();
    const key = 'device_token';
    var token = prefs.getString(key);
    token ??= '${DateTime.now().millisecondsSinceEpoch}-${accessToken.hashCode}';
    await prefs.setString(key, token);

    if (_lastRegisteredToken == token) return;
    _lastRegisteredToken = token;

    try {
      final client = ref.read(bffClientProvider);
      final platform = _platformName();
      await client.post(
        'me',
        'devices',
        body: {
          'deviceToken': token,
          'platform': platform,
          'deviceName': kIsWeb ? 'Web' : Platform.operatingSystem,
        },
      );
    } catch (e) {
      if (kDebugMode) debugPrint('[DeviceRegistration] failed: $e');
    }
  }

  String _platformName() {
    if (kIsWeb) return 'web';
    if (Platform.isAndroid) return 'android';
    if (Platform.isIOS) return 'ios';
    return Platform.operatingSystem;
  }
}
