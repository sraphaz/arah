import 'package:arah_app/core/config/app_config.dart';
import 'package:arah_app/core/geo/geo_location_provider.dart';
import 'package:arah_app/core/network/bff_client.dart';
import 'package:arah_app/features/auth/data/repositories/auth_repository.dart';
import 'package:arah_app/features/auth/presentation/providers/auth_state_provider.dart';
import 'package:arah_app/features/onboarding/data/models/onboarding_models.dart';
import 'package:arah_app/features/onboarding/data/repositories/onboarding_repository.dart';
import 'package:arah_app/features/onboarding/presentation/providers/onboarding_providers.dart';
import 'package:arah_app/features/onboarding/presentation/screens/onboarding_screen.dart';
import 'package:arah_app/features/territories/data/repositories/territories_repository.dart';
import 'package:arah_app/features/territories/presentation/providers/territories_list_provider.dart';
import 'package:arah_app/l10n/app_localizations.dart';
import 'package:arah_app/core/theme/app_theme.dart';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_test/flutter_test.dart';

class _FakeAuthStateNotifier extends AuthStateNotifier {
  @override
  Future<AuthSession?> build() async =>
      const AuthSession(accessToken: 'test-token');
}

class _FakeGeoLocationNotifier extends GeoLocationNotifier {
  _FakeGeoLocationNotifier(GeoPosition? position) {
    state = position;
  }

  @override
  Future<void> fetch() async {}
}

class _FakeOnboardingRepository extends OnboardingRepository {
  _FakeOnboardingRepository(this._territories)
      : super(
            client:
                BffClient(config: const AppConfig(bffBaseUrl: 'http://test')));

  final List<TerritorySuggestion> _territories;

  @override
  Future<List<TerritorySuggestion>> getSuggestedTerritories({
    required double latitude,
    required double longitude,
    double radiusKm = 10,
  }) async {
    return _territories;
  }
}

class _FakeTerritoriesRepository extends TerritoriesRepository {
  _FakeTerritoriesRepository(this._details)
      : super(
            client:
                BffClient(config: const AppConfig(bffBaseUrl: 'http://test')));

  final Map<String, TerritoryDetail> _details;

  @override
  Future<TerritoryDetail?> getTerritoryById(String territoryId) async {
    return _details[territoryId];
  }
}

TerritorySuggestion _territory({
  required String id,
  required String name,
  required double distanceKm,
}) {
  return TerritorySuggestion(
    id: id,
    name: name,
    description: '${distanceKm.toStringAsFixed(1)} km de distancia',
    distanceKm: distanceKm,
    latitude: -23.75 + distanceKm / 1000,
    longitude: -45.63 - distanceKm / 1000,
  );
}

Widget _buildApp({
  required List<TerritorySuggestion> territories,
  required GeoPosition? position,
  MediaQueryData? mediaQuery,
}) {
  final details = <String, TerritoryDetail>{
    for (final territory in territories)
      territory.id: TerritoryDetail(
        id: territory.id,
        name: territory.name,
        latitude: territory.latitude,
        longitude: territory.longitude,
        radiusKm: 1.2,
      ),
  };

  final child = ProviderScope(
    overrides: [
      authStateProvider.overrideWith(_FakeAuthStateNotifier.new),
      geoLocationStateProvider.overrideWith(
        (ref) => _FakeGeoLocationNotifier(position),
      ),
      onboardingRepositoryProvider.overrideWithValue(
        _FakeOnboardingRepository(territories),
      ),
      territoriesRepositoryProvider.overrideWithValue(
        _FakeTerritoriesRepository(details),
      ),
    ],
    child: MaterialApp(
      theme: AppTheme.dark,
      locale: const Locale('pt'),
      localizationsDelegates: AppLocalizations.localizationsDelegates,
      supportedLocales: AppLocalizations.supportedLocales,
      home: const OnboardingScreen(),
    ),
  );

  if (mediaQuery == null) return child;
  return MediaQuery(data: mediaQuery, child: child);
}

void _setMobileSurface(WidgetTester tester, Size size) {
  tester.view.physicalSize = size;
  tester.view.devicePixelRatio = 2;
}

void main() {
  final territories = <TerritorySuggestion>[
    _territory(id: 't1', name: 'Camburi', distanceKm: 1.2),
    _territory(id: 't2', name: 'Boiçucanga', distanceKm: 4.8),
  ];

  testWidgets(
    'OnboardingScreen mostra estrutura do handoff e atualiza CTA com a selecao',
    (tester) async {
      _setMobileSurface(tester, const Size(390, 844));
      addTearDown(() {
        tester.view.resetPhysicalSize();
        tester.view.resetDevicePixelRatio();
      });

      await tester.pumpWidget(
        _buildApp(
          territories: territories,
          position: const GeoPosition(latitude: -23.7628, longitude: -45.6369),
        ),
      );
      await tester.pumpAndSettle();

      expect(find.text('Entrar no território'), findsOneWidget);
      expect(find.text('Escolha seu território'), findsOneWidget);
      expect(find.text('Próximos a você'), findsOneWidget);
      expect(find.text('Continuar com Camburi'), findsOneWidget);

      await tester.ensureVisible(find.text('Boiçucanga'));
      await tester.tap(find.text('Boiçucanga'));
      await tester.pumpAndSettle();

      expect(find.text('Continuar com Boiçucanga'), findsOneWidget);
    },
  );

  testWidgets(
    'OnboardingScreen mostra cadastro de municipio quando nao ha territorios proximos',
    (tester) async {
      _setMobileSurface(tester, const Size(390, 844));
      addTearDown(() {
        tester.view.resetPhysicalSize();
        tester.view.resetDevicePixelRatio();
      });

      await tester.pumpWidget(
        _buildApp(
          territories: const [],
          position: const GeoPosition(latitude: -23.7628, longitude: -45.6369),
        ),
      );
      await tester.pumpAndSettle();

      expect(find.text('Nenhum território cadastrado perto de você ainda.'),
          findsOneWidget);
      expect(find.text('Cadastrar seu município'), findsOneWidget);
      expect(find.text('Cadastrar meu município'), findsOneWidget);
    },
  );

  testWidgets(
    'OnboardingScreen permanece utilizavel em viewport mobile mais estreito',
    (tester) async {
      tester.view.physicalSize = const Size(360, 780);
      tester.view.devicePixelRatio = 2;
      addTearDown(() {
        tester.view.resetPhysicalSize();
        tester.view.resetDevicePixelRatio();
      });

      await tester.pumpWidget(
        _buildApp(
          territories: territories,
          position: const GeoPosition(latitude: -23.7628, longitude: -45.6369),
          mediaQuery: const MediaQueryData(
            size: Size(360, 780),
            devicePixelRatio: 2,
            textScaler: TextScaler.linear(1.2),
          ),
        ),
      );
      await tester.pumpAndSettle();

      expect(tester.takeException(), isNull);
      expect(find.text('Continuar com Camburi'), findsOneWidget);
    },
  );
}
