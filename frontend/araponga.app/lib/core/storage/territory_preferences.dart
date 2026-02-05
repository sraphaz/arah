import 'package:shared_preferences/shared_preferences.dart';

import '../config/constants.dart';

/// Persistência do território selecionado (SharedPreferences).
Future<String?> loadSelectedTerritoryId() async {
  final prefs = await SharedPreferences.getInstance();
  return prefs.getString(AppConstants.keySelectedTerritoryId);
}

Future<void> saveSelectedTerritoryId(String? id) async {
  final prefs = await SharedPreferences.getInstance();
  if (id == null) {
    await prefs.remove(AppConstants.keySelectedTerritoryId);
  } else {
    await prefs.setString(AppConstants.keySelectedTerritoryId, id);
  }
}
