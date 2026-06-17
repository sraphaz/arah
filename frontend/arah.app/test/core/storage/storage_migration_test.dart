import 'package:arah_app/core/config/constants.dart';
import 'package:arah_app/core/storage/storage_migration.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:shared_preferences/shared_preferences.dart';

void main() {
  TestWidgetsFlutterBinding.ensureInitialized();

  test('StorageMigration copies legacy territory id to arah key', () async {
    SharedPreferences.setMockInitialValues({
      'araponga_selected_territory_id': 'territory-legacy',
    });

    await StorageMigration.migrateIfNeeded();

    final prefs = await SharedPreferences.getInstance();
    expect(prefs.getString(AppConstants.keySelectedTerritoryId), 'territory-legacy');
    expect(prefs.getString('araponga_selected_territory_id'), isNull);
    expect(prefs.getBool('arah_storage_migrated_v1'), isTrue);
  });
}
