# AGENTS.md

## Cursor Cloud specific instructions

This monorepo has four products: a .NET 8 backend API + BFF (`backend/`), three web apps
(`frontend/wiki`, `frontend/portal`, `frontend/devportal`), and a Flutter app (`frontend/arah.app`).
The VM snapshot already has the toolchains installed (.NET 8 SDK at `/usr/local/dotnet`,
Flutter 3.27.4 + Dart at `/usr/local/flutter`, both symlinked into `/usr/local/bin`), and the
startup update script refreshes dependencies. Standard build/test/run commands live in
`.github/workflows/ci.yml`, `docs/SETUP.md`, `docs/DEVELOPMENT.md`, and the per-app
`package.json`/`pubspec.yaml`. Notes below are the non-obvious gotchas.

### Backend (.NET 8) — `backend/`, solution `Arah.sln`
- Use **`Arah.sln`**, not the legacy `Araponga.sln`. All code/namespaces are `Arah.*` even though
  prose docs still say "Araponga".
- The API **requires** a JWT signing key to boot, even in Development. Without it `Program.cs`
  throws `JWT SigningKey must be configured...`. Set env `JWT__SIGNINGKEY=...` before running
  (any value works in Development/Testing; the literal `dev-only-change-me` is tolerated outside
  Production with a warning).
- The API defaults to the **InMemory** persistence provider (`appsettings.json` →
  `Persistence:Provider`), so it runs with **no Postgres/Redis/MinIO** for local dev and seeds
  canonical territories on boot. Redis and MinIO are optional (the app falls back to in-memory cache
  and local-disk media). For realistic persistence set `Persistence__Provider=Postgres` +
  `ConnectionStrings__Postgres=...` (see `docker-compose.yml`).
- Run the API: `dotnet run --project backend/Arah.Api` → `http://localhost:5178` (Swagger `/swagger`,
  health `/health`). `/health` reporting `Degraded` in dev is expected (storage health check with the
  Local media provider).
- Run the BFF (needed by the Flutter app): `dotnet run --project backend/Arah.Api.Bff` →
  `http://localhost:5005`. Point it at the API with `Bff__ApiBaseUrl=http://localhost:5178`. The BFF
  has no `/health` route; its proxied routes live under `/bff/journeys` and `/api/v2/journeys/*`.
- Tests: `dotnet test backend/Tests/Arah.Tests/Arah.Tests.csproj` and
  `dotnet test backend/Tests/Arah.Tests.Bff/Arah.Tests.Bff.csproj`. A couple of tests under
  `Arah.Tests/Performance` assert latency SLAs (e.g. `< 700ms`) and can flake on a slow/loaded VM —
  those failures are timing-related, not code regressions.

### Web frontends — `frontend/{wiki,portal,devportal}` (npm)
- `npm run lint` is **broken** in `wiki` and `portal`: they call `next lint`, which Next.js 16
  removed, so `next` treats `lint` as a directory and errors. Use `npm run type-check` and
  `npm test` (jest) instead for those apps.
- `wiki`: `npm run dev` serves on port **3001** under the **`/wiki`** base path
  (`http://localhost:3001/wiki`); `/` returns 404. Checks: `npm run type-check`, `npm test`.
- `portal`: `npm run dev` serves on port **3000** at `/`.
- `devportal`: static HTML; no dev server. Validate with `npm test` (jest). It is also served by the
  API at `/devportal`.

### Flutter app — `frontend/arah.app`
- `flutter pub get`, then `flutter analyze --no-fatal-infos` (CI tolerates info-level lints) and
  `flutter test`. It talks only to the **BFF**, not the API directly (`--dart-define=BFF_BASE_URL=...`).
- l10n: arb files in `lib/l10n/app_pt.arb` (template) + `app_en.arb`. `flutter gen-l10n` writes to the
  synthetic package (`.dart_tool/flutter_gen/gen_l10n/`), but the app imports the **committed**
  `lib/l10n/app_localizations*.dart`. After editing arb files, run `flutter gen-l10n` then copy the 3
  generated files from `.dart_tool/flutter_gen/gen_l10n/` over `lib/l10n/`.
- `flutter_map` 8.x markers: a `GestureDetector` inside a `Marker` child does **not** fire reliably on
  web. Handle taps via `MapOptions.onTap` + nearest-pin matching (see `map_screen.dart`).
- New app journeys must be registered in the BFF `BffJourneyRegistry` (constant + `JourneyToApiPathBase`
  + `AllEndpoints` + `CacheableGetEndpoints` + `AllPathPrefixes`), or BFF tests/`/bff/journeys` break.

### Keep docs in sync with every delivery (important)
When shipping a feature (app/BFF/API), update the relevant docs **in the same PR**:
- `README.md` (phase/status + "App (Flutter) — Entregas Recentes"), `docs/CHANGELOG.md`,
  `docs/STABLE_RELEASE_APP_ONBOARDING.md` (app implemented + próximos passos),
  `docs/FEATURE_MATRIX_API_BFF_APP.md` (API/BFF/App columns), and the phase docs under
  `docs/backlog-api/` + `docs/STATUS_FASES.md` when a backlog phase status changes.
Treat "documentação desatualizada" as a bug (see `.cursorrules`).
