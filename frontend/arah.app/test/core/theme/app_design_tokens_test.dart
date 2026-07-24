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

    test('elevation: dark L1 is flat; higher levels and light have shadows', () {
      expect(AppDesignTokens.elevation(1, isDark: true), isEmpty);
      expect(AppDesignTokens.elevation(2, isDark: true), isNotEmpty);
      expect(AppDesignTokens.elevation(1, isDark: false), isNotEmpty);
      expect(AppDesignTokens.elevation(4), isNotEmpty);
    });
  });
}
