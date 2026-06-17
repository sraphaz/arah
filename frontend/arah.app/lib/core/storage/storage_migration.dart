import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:shared_preferences/shared_preferences.dart';

import '../config/constants.dart';

/// Migra chaves legadas `araponga_*` para `arah_*` (uma vez por instalação).
class StorageMigration {
  StorageMigration._();

  static const _migrationFlag = 'arah_storage_migrated_v1';

  static const _secureKeyPairs = <({String legacy, String current})>[
    (legacy: 'araponga_access_token', current: AppConstants.keyAccessToken),
    (legacy: 'araponga_refresh_token', current: AppConstants.keyRefreshToken),
    (legacy: 'araponga_token_expiry', current: AppConstants.keyTokenExpiry),
  ];

  static Future<void> migrateIfNeeded() async {
    final prefs = await SharedPreferences.getInstance();
    if (prefs.getBool(_migrationFlag) == true) return;

    const secureStorage = FlutterSecureStorage(
      aOptions: AndroidOptions(encryptedSharedPreferences: true),
    );

    try {
      for (final pair in _secureKeyPairs) {
        final legacyValue = await secureStorage.read(key: pair.legacy);
        if (legacyValue == null) continue;
        final currentValue = await secureStorage.read(key: pair.current);
        if (currentValue == null) {
          await secureStorage.write(key: pair.current, value: legacyValue);
        }
        await secureStorage.delete(key: pair.legacy);
      }
    } catch (_) {
      // Secure storage indisponível (ex.: testes unitários sem plugin).
    }

    const legacyTerritoryKey = 'araponga_selected_territory_id';
    final legacyTerritory = prefs.getString(legacyTerritoryKey);
    if (legacyTerritory != null && prefs.getString(AppConstants.keySelectedTerritoryId) == null) {
      await prefs.setString(AppConstants.keySelectedTerritoryId, legacyTerritory);
    }
    await prefs.remove(legacyTerritoryKey);

    await prefs.setBool(_migrationFlag, true);
  }
}
