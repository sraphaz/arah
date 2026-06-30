import 'dart:async';

import 'package:firebase_core/firebase_core.dart';
import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'app.dart';
import 'core/storage/storage_migration.dart';

void main() async {
  // Em debug: mesma zone para ensureInitialized e runApp (evita "Zone mismatch").
  if (kDebugMode) {
    FlutterError.onError = (details) {
      FlutterError.presentError(details);
      debugPrint('[FlutterError] ${details.exception}');
      if (details.stack != null) debugPrint('[FlutterError] stackTrace:\n${details.stack}');
    };
    runZonedGuarded<void>(() async {
      WidgetsFlutterBinding.ensureInitialized();
      await _runApp();
    }, (error, stackTrace) {
      debugPrint('[Uncaught error] $error');
      debugPrint('[Uncaught error] stackTrace:\n$stackTrace');
    });
  } else {
    WidgetsFlutterBinding.ensureInitialized();
    await _runApp();
  }
}

Future<void> _runApp() async {
  try {
    await Firebase.initializeApp();
  } catch (_) {
    // Firebase não configurado (ex.: Web sem options, ou google-services.json ausente).
  }
  await StorageMigration.migrateIfNeeded();
  runApp(
    const ProviderScope(
      child: ArahApp(),
    ),
  );
}
