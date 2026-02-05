# PR #241 – fix/cd-flutter-package-name → feat/conexoes-circulo-amigos

## Resumo

Esta PR incorpora correções de CI/CD (Flutter package name, Dockerfile), login com Google e Firebase Auth no app Flutter, melhorias no BFF (CORS, documentação), mitigação de CVE no backend, migrações unificadas, scripts de stack local e documentação de setup Android, mantendo a base em `feat/conexoes-circulo-amigos`.

---

## Objetivo

- Alinhar o build e o deploy ao layout real do repositório (backend em `backend/`, sem `Core/`).
- Habilitar login social (Google) e exibição de usuário no app Flutter.
- Garantir que o BFF funcione em origem Flutter web (CORS) e que a URL base não quebre chamadas (`auth/social`).
- Mitigar CVE-2024-43483 (Microsoft.Extensions.Caching.Memory) e usar .NET 8.0.x disponível no CI.
- Unificar migrações EF e melhorar scripts de desenvolvimento local (limpeza de binários, seed).

---

## Principais mudanças

### Backend (API + módulos)

| Área | Alteração |
|------|------------|
| **Estrutura** | Projetos na raiz de `backend/` (não em `backend/Core/`); referências e solution ajustadas. |
| **Migrações** | Migrações antigas substituídas por **UnifiedInitialCreate**; snapshot e mappers atualizados. |
| **Territórios** | Raio em km (`RadiusKm`), geo-convergência bypass, ajustes de domínio e controllers. |
| **Feed** | API e jornada BFF de feed; controller de jornada e DI. |
| **Segurança** | CVE-2024-43483: `Microsoft.Extensions.Caching.Memory` 9.0.0 no BFF; `DependencyInjection` 8.0.1 em Tests.Modules.Moderation. |
| **Testes** | Caminho correto `backend/Tests/Araponga.Tests`; projetos BFF e ApiSupport; testes por módulo (Moderation, Marketplace, etc.). |
| **Warnings** | Resolução de NU1603, CS8601, xUnit2002, ASP0019 onde aplicável. |

### BFF (Backend for Frontend)

| Área | Alteração |
|------|------------|
| **Aplicação** | BFF como aplicação separada; cache e jornadas (incl. Marketplace). |
| **Jornadas** | Registro completo das jornadas; `GET /bff/journeys`; documentação do endpoint `auth/social`. |
| **Proxy** | Dispose de `HttpResponseMessage` no proxy para evitar socket exhaustion. |
| **CORS** | CORS configurado para origens localhost (qualquer porta), permitindo Flutter web. |
| **Docs** | Documentação do retry 429 e uso de constantes nomeadas. |

### App Flutter (`frontend/araponga.app`)

| Área | Alteração |
|------|------------|
| **Estrutura** | Auth, onboarding, feed, explorar, publicar, perfil, notificações, eventos, mapa; UX e i18n (pt/en). |
| **Login** | Login com **Google Sign-In** + **Firebase Auth**; botão oficial “Entrar com Google” (`sign_in_button`); opção “Não tem conta? Criar conta”; envio para BFF `auth/social`. |
| **Rede** | `BffClient` com baseUrl com barra final (`/api/v2/journeys/`) para não gerar `journeysauth`; timeout e retry configuráveis. |
| **Perfil** | Exibição de nome e e-mail do usuário logado (sessão + `me/profile`); fallback durante loading. |
| **Dependências** | `intl` em faixa `>=0.19.0 <0.21.0` para compatibilidade com CI e SDK local. |
| **Docs** | README, ARCHITECTURE, RUN_LOCAL, **GOOGLE_SIGNIN_ANDROID_SETUP** (passo a passo SHA-1, Console, Firebase, Gradle). |
| **CI/CD** | Flutter analyze; testes widget com locale pt; `--project-name araponga_app` no `flutter create` para CD. |

### Docker e CI/CD

| Área | Alteração |
|------|------------|
| **Dockerfile (raiz)** | COPY e WORKDIR alinhados ao layout real: `backend/Araponga.Api`, `backend/Araponga.Application`, módulos em `backend/Araponga.Modules.*`; imagens base `sdk:8.0` e `aspnet:8.0`. |
| **CI** | `dotnet-version: '8.0.x'` (8.0.12 não disponível no install script); testes em `backend/Tests/Araponga.Tests`. |
| **CD** | Ajustes para Flutter (package name, project name). |

### Scripts e operações

| Área | Alteração |
|------|------------|
| **run-local-stack.ps1** | Limpeza de binários antigos (pastas `bin/` e `obj/` no backend + `dotnet clean` no BFF) antes de subir; numeração dos passos (0–3). |
| **Seed** | Scripts de seed Camburi (`run-seed-camburi.ps1`, `seed-camburi.sql`, `fix-migration-history.sql`, `reset-for-migration.sql`). |
| **OAuth (gcloud)** | Script `gcloud-oauth-client.ps1` para criar/listar clientes OAuth IAM (referência; Client ID do “Sign in with Google” segue pelo Console). |
| **.gitignore** | Inclusão da pasta `logs/`. |

### Documentação

| Documento | Conteúdo |
|-----------|----------|
| **LOCAL_CHANGES_PENDENTES** | Atualizado (ex.: .NET 8.0.12 onde aplicável). |
| **RELEASE.md** | Passos para release e versionamento. |
| **backend/docs** | IMPROVEMENTS, BACKEND_LAYERS, TEST_SEPARATION, README (BFF, testes, fronteiras). |
| **frontend/araponga.app/docs** | GOOGLE_SIGNIN_ANDROID_SETUP, RUN_LOCAL, NEXT_STEPS, ARCHITECTURE, entre outros. |

---

## Como testar

1. **Backend e BFF**  
   - `.\scripts\run-local-stack.ps1` (limpa binários, sobe API + BFF).  
   - Verificar `http://localhost:8080/health` e `http://localhost:5001`.  

2. **App Flutter (web)**  
   - `cd frontend\araponga.app` e `flutter run -d chrome --dart-define=BFF_BASE_URL=http://localhost:5001`.  
   - Tela de login: “Entrar com Google” (em web exige `GOOGLE_SIGN_IN_CLIENT_ID`); login por e-mail (dev) deve funcionar.  

3. **App Flutter (Android)**  
   - Seguir `docs/GOOGLE_SIGNIN_ANDROID_SETUP.md` (SHA-1, Console, Firebase, `google-services.json`).  
   - `flutter run` em dispositivo ou emulador.  

4. **Docker (imagem da API)**  
   - Na raiz: `docker build -f Dockerfile .` (usa caminhos atuais do backend).  

5. **CI**  
   - Push em branch com as alterações; conferir workflow (setup-dotnet 8.0.x, Flutter pub get, build e testes).  

---

## Checklist (revisão)

- [x] Dockerfile raiz com caminhos reais do backend; build da imagem OK.
- [x] CI com dotnet 8.0.x e Flutter (intl compatível); testes backend e Flutter.
- [x] BFF com CORS para Flutter web; `auth/social` documentado e acessível.
- [x] App Flutter: login Google + Firebase Auth; nome/e-mail no perfil; BffClient com URL correta.
- [x] CVE-2024-43483 mitigado (Caching.Memory 9.0.0; DependencyInjection 8.0.1 onde aplicável).
- [x] Migrações unificadas; seed e scripts de stack local documentados e utilizáveis.
- [x] Documentação: GOOGLE_SIGNIN_ANDROID_SETUP, RUN_LOCAL, release e pendentes.

---

## Referências

- [CVE-2024-43483](https://nvd.nist.gov/vuln/detail/CVE-2024-43483) – Microsoft.Extensions.Caching.Memory
- [GOOGLE_SIGNIN_ANDROID_SETUP](frontend/araponga.app/docs/GOOGLE_SIGNIN_ANDROID_SETUP.md) – Setup Android (SHA-1, Firebase, Gradle)
- [RUN_LOCAL](frontend/araponga.app/docs/RUN_LOCAL.md) – Rodar app e BFF localmente
- [ADR-011 BFF](docs/10_ARCHITECTURE_DECISIONS.md) – BFF como aplicação separada
