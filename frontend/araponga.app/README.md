# Ará (App Flutter)

App mobile **território-first** e **comunidade-first**. Nome: **Ará**. Interface clean, fundo escuro padrão. Conecta ao **BFF** do repositório.

**Fluxo:** Login → Onboarding (escolha de território) ou Home → Feed / Explorar / Publicar / Perfil.

## Requisitos

- [Flutter](https://flutter.dev) 3.19+ (stable)
- Dart 3.2+

## Configuração inicial

Se a pasta foi criada manualmente (sem `flutter create`):

```bash
cd frontend/araponga.app
flutter create . --project-name araponga_app --org com.araponga --platforms android,ios,web
```

Depois:

```bash
flutter pub get
flutter gen-l10n   # opcional: regenera lib/l10n a partir dos .arb
```

## BFF e ambiente

O app consome o BFF em `/api/v2/journeys/*`.

- **Desenvolvimento:** URL base padrão `http://localhost:5001`. Alterar em `lib/core/config/app_config.dart` ou:

  ```bash
  flutter run --dart-define=BFF_BASE_URL=http://SEU_IP:PORTA
  ```

- **Android emulador:** `http://10.0.2.2:5001` para localhost do host.
- **iOS simulador:** `http://localhost:5001`.

O projeto **Araponga.Api.Bff** (backend) precisa estar em execução.

## Estrutura do projeto

```
lib/
├── main.dart                 # Entry + ProviderScope
├── app.dart                  # MaterialApp, tema, go_router, l10n
├── app_router.dart           # Rotas: /, /login, /onboarding, /home
├── core/
│   ├── config/               # AppConfig (BFF URL), AppConstants (design tokens)
│   ├── geo/                  # GeoLocationNotifier (geolocator)
│   ├── network/              # BffClient (Dio), ApiException
│   ├── providers/            # bffClientProvider, territory provider
│   ├── storage/              # SecureStorage (tokens), territory_preferences
│   ├── theme/                # AppTheme (Material 3 dark/light)
│   └── widgets/              # Shimmer/skeleton, AppSnackbar, ProfileSkeleton
├── l10n/                     # app_pt.arb, app_en.arb, app_localizations.dart
└── features/
    ├── auth/                 # Login (BFF auth/social), AuthRepository, guard
    ├── home/                 # MainShellScreen (bottom nav: Feed, Explorar, Publicar, Notificações, Perfil)
    ├── feed/                 # Feed territorial (paginação, pull-to-refresh), CreatePostScreen
    ├── explore/              # Lista territórios, TerritorySelector, POST enter
    ├── onboarding/           # suggested-territories, complete, escolha de território
    ├── profile/              # me/profile GET/PUT, edição em bottom sheet
    └── territories/          # TerritoriesRepository (enter), TerritorySelector, lista paged
```

## Jornadas BFF utilizadas

| Jornada     | Uso no app |
|------------|------------|
| auth       | Login (social/dev), refresh, logout |
| me         | Perfil (profile, display-name, bio) |
| feed       | territory-feed (paginação), create-post |
| territories| paged, {id}/enter |
| onboarding | suggested-territories, complete |

## Rodar

```bash
cd frontend/araponga.app
flutter pub get
flutter run
```

## Documentação

- **[docs/NEXT_STEPS.md](docs/NEXT_STEPS.md)** – Próximos passos e estado atual.
- **[docs/ARCHITECTURE.md](docs/ARCHITECTURE.md)** – Arquitetura (BFF, providers, rotas, i18n, design).
- **[docs/DESIGN_SYSTEM.md](docs/DESIGN_SYSTEM.md)** – Manutenção do design (onde alterar cores, espaçamento, tema).
- **[docs/BEST_PRACTICES.md](docs/BEST_PRACTICES.md)** – Avaliação por boas práticas e checklist para PRs.
- **[docs/CI_CD_AND_TESTS.md](docs/CI_CD_AND_TESTS.md)** – Estado do CI/CD e testes (app no CI, primeiros testes, CD).

Documentação do repositório (planos, design, API):

- [24_FLUTTER_FRONTEND_PLAN](../../docs/24_FLUTTER_FRONTEND_PLAN.md)
- [26_FLUTTER_DESIGN_GUIDELINES](../../docs/26_FLUTTER_DESIGN_GUIDELINES.md)
- [25_FLUTTER_IMPLEMENTATION_ROADMAP](../../docs/25_FLUTTER_IMPLEMENTATION_ROADMAP.md)
- [34_FLUTTER_API_STRATEGIC_ALIGNMENT](../../docs/34_FLUTTER_API_STRATEGIC_ALIGNMENT.md)
- [BFF_FLUTTER_EXAMPLE](../../docs/BFF_FLUTTER_EXAMPLE.md)
- [32_FLUTTER_I18N_GUIDE](../../docs/32_FLUTTER_I18N_GUIDE.md)

## Produção

- **BFF_BASE_URL:** Definir em build (ex.: `--dart-define=BFF_BASE_URL=https://bff.seudominio.com`).
- **Auth:** Integrar Google/Apple Sign-In e chamar `auth/social` com dados do provedor.
- **Tokens:** `flutter_secure_storage` (KeyStore/Keychain). 401 dispara logout e redirect para `/login`.
- **Território:** ID ativo em SharedPreferences e enviado como `X-Session-Id`. Geolocalização em `X-Geo-Latitude` e `X-Geo-Longitude` quando disponível.
- **i18n:** pt (padrão) e en em `lib/l10n/`. Executar `flutter gen-l10n` para regenerar a partir dos .arb.
