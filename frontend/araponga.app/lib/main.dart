import 'package:firebase_core/firebase_core.dart';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'app.dart';

void main() async {
  WidgetsFlutterBinding.ensureInitialized();
  try {
    await Firebase.initializeApp();
  } catch (_) {
    // Firebase não configurado (ex.: Web sem options, ou google-services.json ausente).
  }
  runApp(
    const ProviderScope(
      child: ArapongaApp(),
    ),
  );
}
