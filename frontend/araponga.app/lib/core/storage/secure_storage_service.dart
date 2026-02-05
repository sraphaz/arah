import 'package:flutter_secure_storage/flutter_secure_storage.dart';

import '../config/constants.dart';

/// Armazenamento seguro de tokens (Android KeyStore / iOS Keychain).
/// Em produção: não logar valores; usar opções de segurança por plataforma.
class SecureStorageService {
  SecureStorageService() : _storage = const FlutterSecureStorage(aOptions: AndroidOptions(encryptedSharedPreferences: true));

  final FlutterSecureStorage _storage;

  Future<void> writeAccessToken(String value) =>
      _storage.write(key: AppConstants.keyAccessToken, value: value);

  Future<String?> readAccessToken() =>
      _storage.read(key: AppConstants.keyAccessToken);

  Future<void> writeRefreshToken(String value) =>
      _storage.write(key: AppConstants.keyRefreshToken, value: value);

  Future<String?> readRefreshToken() =>
      _storage.read(key: AppConstants.keyRefreshToken);

  Future<void> writeTokenExpiry(String value) =>
      _storage.write(key: AppConstants.keyTokenExpiry, value: value);

  Future<String?> readTokenExpiry() =>
      _storage.read(key: AppConstants.keyTokenExpiry);

  Future<void> clearAuth() async {
    await _storage.delete(key: AppConstants.keyAccessToken);
    await _storage.delete(key: AppConstants.keyRefreshToken);
    await _storage.delete(key: AppConstants.keyTokenExpiry);
  }
}
