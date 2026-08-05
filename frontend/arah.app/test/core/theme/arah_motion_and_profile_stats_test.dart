import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

import 'package:arah_app/core/theme/arah_motion.dart';
import 'package:arah_app/features/profile/data/models/me_profile.dart';

void main() {
  group('ArahMotion', () {
    test('press scale and durations match handoff', () {
      expect(ArahMotion.pressScale, 0.975);
      expect(ArahMotion.fast.inMilliseconds, 150);
      expect(ArahMotion.normal.inMilliseconds, 250);
      expect(ArahMotion.emphasized, isA<Curve>());
    });
  });

  group('MeProfile', () {
    test('fromJson maps postsCreated; interestsCount only from explicit fields', () {
      final profile = MeProfile.fromJson({
        'id': 'u1',
        'displayName': 'Ana',
        'createdAtUtc': '2026-01-01T00:00:00Z',
        'interests': ['cultura', 'esporte'],
        'stats': {'postsCreated': 4},
      });
      expect(profile.postsCount, 4);
      expect(profile.interestsCount, isNull);
      expect(profile.hasStatCounts, isTrue);
    });

    test('fromJson keeps interestsCount null when interests empty and no stats', () {
      final profile = MeProfile.fromJson({
        'id': 'u1',
        'displayName': 'Ana',
        'createdAtUtc': '2026-01-01T00:00:00Z',
        'interests': <String>[],
      });
      expect(profile.interestsCount, isNull);
      expect(profile.hasStatCounts, isFalse);
    });

    test('mergeStatsJson fills posts from API stats endpoint', () {
      final base = MeProfile(
        id: 'u1',
        displayName: 'Ana',
        createdAtUtc: DateTime.utc(2026),
        interests: const ['a'],
      );
      final merged = base.mergeStatsJson({
        'userId': 'u1',
        'postsCreated': 7,
        'eventsCreated': 1,
      });
      expect(merged.postsCount, 7);
      expect(merged.interestsCount, 1);
    });
  });
}
