# CI/CD e testes – Araponga

Status atual (implementado) e como usar os artefatos.

---

## CI (`.github/workflows/ci.yml`)

| O que roda | Backend (.NET) | App Flutter |
|------------|----------------|-------------|
| Build | ✅ `dotnet build` | ✅ implícito (analyze) |
| Testes | ✅ `dotnet test` + cobertura | ✅ `flutter test --coverage` |
| Cobertura | ✅ Codecov (flag `unittests`) | ✅ Codecov (flag `flutter`) |
| Análise estática | — | ✅ `flutter analyze --no-fatal-infos` |
| Segurança | ✅ Trivy (fs) | — (Trivy escaneia o repo) |

- **Flutter:** job `flutter-app` em `frontend/araponga.app`: `pub get` → `analyze` → `flutter test --coverage` → upload de `coverage/lcov.info` para Codecov.

---

## CD (`.github/workflows/cd.yml`)

| Artefato | O que acontece |
|----------|----------------|
| **API** | Build e push da imagem Docker para GHCR (`araponga-api:latest`, `:$sha`). |
| **App Android** | `flutter build apk` e `flutter build appbundle`; upload dos artefatos **araponga-app-android-apk** (APK) e **araponga-app-android-aab** (AAB). |
| **App iOS** | `flutter build ios --no-codesign`; zip de `Runner.app`; upload do artefato **araponga-app-ios**. |

- **Disparo:** push em `main`, tags `v*.*.*` ou `workflow_dispatch`.
- **Plataformas:** se o projeto ainda não tiver `android/` e `ios/`, o CD executa `flutter create . --project-name araponga_app --org com.araponga --platforms=android,ios,web` antes do build (o nome do pacote deve ser `araponga_app`, pois a pasta é `araponga.app`).

### Como baixar os artefatos

1. Abra a **Actions** do repositório → último run do workflow **CD**.
2. No final da página, em **Artifacts**, aparecem:
   - **araponga-app-android-apk** – APK para instalação direta.
   - **araponga-app-android-aab** – App Bundle para publicar na Play Store.
   - **araponga-app-ios** – `Runner.app` em zip (build sem assinatura; para TestFlight/App Store é preciso configurar signing no Xcode e gerar IPA com certificado).

---

## Testes do app Flutter

| Tipo | Onde | O que cobre |
|------|------|-------------|
| **App** | `test/app_test.dart` | App sobe com `ProviderScope` e `MaterialApp`. |
| **Login** | `test/features/auth/presentation/login_screen_test.dart` | Tela de login exibe título e botão Entrar. |
| **Explorar** | `test/features/explore/presentation/explore_screen_test.dart` | Tela Explorar exibe título e seção Territórios. |
| **Perfil** | `test/features/profile/presentation/profile_screen_test.dart` | Perfil logado exibe nome e app bar. |
| **Unit** | `test/core/network/api_exception_test.dart` | `ApiException.userMessage` (401, 429, 403, 5xx, outros). |
| **Unit** | `test/features/territories/presentation/territory_item_test.dart` | `TerritoryItem.fromJson`. |

- **Cobertura:** `flutter test --coverage` gera `coverage/lcov.info`; o CI envia para o Codecov (flag `flutter`).
- **Rodar localmente:** `cd frontend/araponga.app && flutter test`.

---

## Próximos passos (opcional)

- **iOS com assinatura:** para gerar IPA para TestFlight/App Store, configurar certificados e provisioning no repositório (ex.: secrets, fastlane ou step de signing no workflow) e trocar `flutter build ios --no-codesign` por build com signing.
- **Gate de cobertura:** falhar o CI se a cobertura Flutter (ou backend) cair abaixo de um percentual definido.
- **Mais testes:** widget tests para feed, criar post e onboarding; unit tests para repositórios e parsers.
