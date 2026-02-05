import 'package:araponga_app/features/territories/presentation/providers/territories_list_provider.dart';
import 'package:flutter_test/flutter_test.dart';

void main() {
  group('TerritoryItem.fromJson', () {
    test('parses minimal json', () {
      final item = TerritoryItem.fromJson({'id': 't1', 'name': 'Território 1'});
      expect(item.id, 't1');
      expect(item.name, 'Território 1');
      expect(item.description, isNull);
    });

    test('parses with description', () {
      final item = TerritoryItem.fromJson({
        'id': 't2',
        'name': 'Centro',
        'description': 'Região central',
      });
      expect(item.id, 't2');
      expect(item.name, 'Centro');
      expect(item.description, 'Região central');
    });

    test('handles missing name with title fallback', () {
      final item = TerritoryItem.fromJson({'id': 'x', 'title': 'Fallback Title'});
      expect(item.name, 'Fallback Title');
    });

    test('handles empty id', () {
      final item = TerritoryItem.fromJson({'name': 'N'});
      expect(item.id, '');
    });
  });
}
