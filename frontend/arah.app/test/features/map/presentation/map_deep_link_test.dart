import 'package:arah_app/features/map/data/models/map_pin.dart';
import 'package:arah_app/features/map/presentation/screens/map_deep_link.dart';
import 'package:flutter_test/flutter_test.dart';

MapPin _pin(String type) => MapPin(
      pinType: type,
      latitude: -23.35,
      longitude: -44.89,
      title: 'Pin',
    );

void main() {
  group('mapPinDeepLink', () {
    test('event pin links to /events with territoryId', () {
      final route = mapPinDeepLink(_pin('event'), territoryId: 't1');
      expect(route, '/events?territoryId=t1');
    });

    test('event pin without territoryId links to /events', () {
      expect(mapPinDeepLink(_pin('EVENT')), '/events');
    });

    test('asset pin links to /assets', () {
      expect(mapPinDeepLink(_pin('asset')), '/assets');
    });

    test('alert pin links to /alerts', () {
      expect(mapPinDeepLink(_pin('alert')), '/alerts');
    });

    test('post pin with postId links to /post detail', () {
      final pin = MapPin(
        pinType: 'post',
        latitude: -23.35,
        longitude: -44.89,
        title: 'Post',
        postId: 'pid1',
      );
      expect(mapPinDeepLink(pin, territoryId: 't1'), '/post?territoryId=t1&postId=pid1');
    });

    test('post pin without postId falls back to /home (feed)', () {
      expect(mapPinDeepLink(_pin('post')), '/home');
    });

    test('entity and unknown pins have no deep-link target', () {
      expect(mapPinDeepLink(_pin('entity')), isNull);
      expect(mapPinDeepLink(_pin('media')), isNull);
      expect(mapPinDeepLink(_pin('whatever')), isNull);
    });
  });
}
