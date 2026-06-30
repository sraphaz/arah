import 'package:flutter_test/flutter_test.dart';
import 'package:arah_app/core/theme/app_design_tokens.dart';

void main() {
  group('AppDesignTokens', () {
    test('pinColorForType returns semantic colors', () {
      expect(AppDesignTokens.pinColorForType('alert'), AppDesignTokens.warning);
      expect(AppDesignTokens.pinColorForType('event'), AppDesignTokens.link);
      expect(AppDesignTokens.pinColorForType('post'), AppDesignTokens.primary);
      expect(AppDesignTokens.pinColorForType('asset'), AppDesignTokens.earth);
    });

    test('elevation returns box shadows for levels 1-4', () {
      expect(AppDesignTokens.elevation(1), isNotEmpty);
      expect(AppDesignTokens.elevation(4), isNotEmpty);
    });
  });
}
