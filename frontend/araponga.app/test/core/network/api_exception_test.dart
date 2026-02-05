import 'package:araponga_app/core/network/api_exception.dart';
import 'package:flutter_test/flutter_test.dart';

void main() {
  group('ApiException', () {
    test('userMessage returns session expired for 401', () {
      final e = ApiException('Unauthorized', statusCode: 401);
      expect(e.isUnauthorized, isTrue);
      expect(e.userMessage, contains('Sessão expirada'));
    });

    test('userMessage returns rate limit message for 429', () {
      final e = ApiException('Too many requests', statusCode: 429);
      expect(e.isRateLimited, isTrue);
      expect(e.userMessage, contains('Muitas tentativas'));
    });

    test('userMessage returns forbidden message for 403', () {
      final e = ApiException('Forbidden', statusCode: 403);
      expect(e.isForbidden, isTrue);
      expect(e.userMessage, contains('localização'));
    });

    test('userMessage returns server error for 500', () {
      final e = ApiException('Error', statusCode: 500);
      expect(e.userMessage, contains('servidor'));
    });

    test('userMessage returns server error for 502', () {
      final e = ApiException('Bad gateway', statusCode: 502);
      expect(e.userMessage, contains('servidor'));
    });

    test('userMessage returns raw message for other status', () {
      final e = ApiException('Not found', statusCode: 404);
      expect(e.userMessage, 'Not found');
    });

    test('toString includes message and statusCode', () {
      final e = ApiException('Fail', statusCode: 400);
      expect(e.toString(), contains('ApiException'));
      expect(e.toString(), contains('Fail'));
      expect(e.toString(), contains('400'));
    });
  });
}
