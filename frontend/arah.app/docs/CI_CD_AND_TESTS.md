# CI/CD e testes – Arah

Status atual (implementado) e como usar os artefatos.

---

## CI (`.github/workflows/ci.yml`)

| O que roda | Backend (.NET) | App Flutter |
|------------|----------------|-------------|
| Build | ✅ `dotnet build` | ✅ implícito (analyze) |
| Testes | ✅ `dotnet test` + cobertura | ✅ `flutter test --coverage` |
| Browser smoke | — | ✅ Playwright (`flutter-playwright`) valida login + onboarding + MainShell/Feed web em viewport mobile |
| Pixel parity | — | ✅ Playwright (`flutter-playwright-pixel-parity`) compara Flutter web vs handoff mobile e bloqueia regressões de login + onboarding + MainShell/Feed |
| Cobertura | ✅ Codecov (flag `unittests`) | ✅ Codecov (flag `flutter`) |
| Análise estática | — | ✅ `flutter analyze --no-fatal-infos` |
| Segurança | ✅ Trivy (fs) | — (Trivy escaneia o repo) |

- **Flutter:** job `flutter-app` em `frontend/arah.app`: `pub get` → `analyze` → `flutter test --coverage` → upload de `coverage/lcov.info` para Codecov.
- **Playwright smoke:** job `flutter-playwright` em `frontend/arah.app/e2e`: `flutter build web --web-renderer html` → `npm ci` → `npx playwright install --with-deps chromium` → `npm run test:smoke`. O renderer HTML é usado para manter estáveis os asserts textuais/semânticos do smoke de login + onboarding + MainShell/Feed. Em falha, publica `playwright-report/` e `test-results/` como artefatos do workflow.
- **Playwright pixel parity:** job `flutter-playwright-pixel-parity` usa o build web padrão do Flutter (renderer visual padrão do runtime), renderiza o handoff `design-system/ui_kits/app` como source of truth em perfil mobile (`390x844`, `isMobile`, `hasTouch`, DPR 2), captura a implementação Flutter no mesmo perfil para login + onboarding + MainShell/Feed e falha se `maxDiffPixelRatio > 0.01`. O job é **blocking** e sobe `playwright-report/` e `test-results/` como artefatos.

---

## CD (`.github/workflows/cd.yml`)

| Artefato | O que acontece |
|----------|----------------|
| **API** | Build e push da imagem Docker para GHCR (`Arah-api:latest`, `:$sha`). |
| **App Android** | `flutter build apk` e `flutter build appbundle`; upload dos artefatos **Arah-app-android-apk** (APK) e **Arah-app-android-aab** (AAB). |
| **App iOS** | `flutter build ios --no-codesign`; zip de `Runner.app`; upload do artefato **Arah-app-ios**. |

- **Disparo:** push em `main`, tags `v*.*.*` ou `workflow_dispatch`.
- **Plataformas:** se o projeto ainda não tiver `android/` e `ios/`, o CD executa `flutter create . --project-name arah_app --org com.arah --platforms=android,ios,web` antes do build (o nome do pacote é `arah_app`).

### Como baixar os artefatos

1. Abra a **Actions** do repositório → último run do workflow **CD**.
2. No final da página, em **Artifacts**, aparecem:
   - **Arah-app-android-apk** – APK para instalação direta.
   - **Arah-app-android-aab** – App Bundle para publicar na Play Store.
   - **Arah-app-ios** – `Runner.app` em zip (build sem assinatura; para TestFlight/App Store é preciso configurar signing no Xcode e gerar IPA com certificado).

---

## Testes do app Flutter

| Tipo | Onde | O que cobre |
|------|------|-------------|
| **App** | `test/app_test.dart` | App sobe com `ProviderScope` e `MaterialApp`. |
| **Login** | `test/features/auth/presentation/login_screen_test.dart` | Tela de login exibe `Arah`, os CTAs iniciais e o fluxo de e-mail. |
| **E2E / Smoke mobile** | `e2e/tests/login.smoke.spec.js` | Flutter web exibe `Arah`, `Entrar com Google` e `Entrar com e-mail` no perfil mobile. |
| **E2E / Pixel parity** | `e2e/tests/login.pixel-parity.spec.js` | Handoff do UI kit vira a imagem esperada; a implementação Flutter é comparada pixel a pixel e deve chegar em `maxDiffPixelRatio <= 0.01`. |
| **Onboarding** | `test/features/onboarding/presentation/onboarding_screen_test.dart` | Cabeçalho, lista próxima, CTA de visitante, estado vazio e responsividade do onboarding. |
| **E2E / Smoke mobile** | `e2e/tests/onboarding.smoke.spec.js` | Fluxo real Flutter `cadastro -> onboarding -> entrar como visitante` com geolocalização e BFF mockados. |
| **E2E / Pixel parity** | `e2e/tests/onboarding.pixel-parity.spec.js` | Handoff do onboarding é comparado pixel a pixel com a implementação Flutter em `390x844` DPR 2. |
| **MainShell + Feed** | `test/features/home/presentation/main_shell_screen_test.dart` | Top bar territorial, bottom-nav canônica e entrada no feed inicial. |
| **Feed** | `test/features/feed/presentation/feed_screen_test.dart` | Filtros `Avisos`/`Vizinhança`, banner de visitante e responsividade mobile estreita. |
| **E2E / Smoke mobile** | `e2e/tests/main-shell-feed.smoke.spec.js` | Fluxo real pós-onboarding até `/home` valida top bar, banner de visitante, bottom-nav e filtros do feed. |
| **E2E / Pixel parity** | `e2e/tests/main-shell-feed.pixel-parity.spec.js` | Compara MainShell/Feed Flutter vs handoff mobile em `390x844` DPR 2, exportando handoff/implementação/diff/composite dedicados. |
| **Explorar** | `test/features/explore/presentation/explore_screen_test.dart` | Tela Explorar exibe título e seção Territórios. |
| **Perfil** | `test/features/profile/presentation/profile_screen_test.dart` | Perfil logado exibe nome e app bar. |
| **Unit** | `test/core/network/api_exception_test.dart` | `ApiException.userMessage` (401, 429, 403, 5xx, outros). |
| **Unit** | `test/features/territories/presentation/territory_item_test.dart` | `TerritoryItem.fromJson`. |

- **Cobertura:** `flutter test --coverage` gera `coverage/lcov.info`; o CI envia para o Codecov (flag `flutter`).
- **Rodar localmente:** `cd frontend/arah.app && flutter test`.

### Rodar o smoke Playwright localmente

```bash
cd frontend/arah.app
flutter pub get
flutter build web --web-renderer html
cd e2e
npm ci
npx playwright install --with-deps chromium
npm test
```

- **Smoke:** `npm test` (login + onboarding + MainShell/Feed), `npm run test:smoke:onboarding` ou `npm run test:smoke:feed`
- **Pixel parity:** `npm run test:pixel-parity` (login + onboarding + MainShell/Feed, sobre o build web padrão do Flutter), `npm run test:pixel-parity:onboarding` ou `npm run test:pixel-parity:feed`
- **Relatório HTML local:** `frontend/arah.app/e2e/playwright-report/`
- **Artefatos de falha locais:** `frontend/arah.app/e2e/test-results/`

---

## Próximos passos (opcional)

- **iOS com assinatura:** para gerar IPA para TestFlight/App Store, configurar certificados e provisioning no repositório (ex.: secrets, fastlane ou step de signing no workflow) e trocar `flutter build ios --no-codesign` por build com signing.
- **Gate de cobertura:** falhar o CI se a cobertura Flutter (ou backend) cair abaixo de um percentual definido.
- **Mais testes:** widget tests para feed e criar post; unit tests para repositórios e parsers.
