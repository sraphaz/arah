# Correções pendentes — App Arah (Flutter)

Registro a partir do teste local (Chrome + stack BFF/API). **Implementar depois** — ordem sugerida abaixo.

---

## 1. Marca: título **Arah** (não Ará)

**Problema:** A interface exibe **Ará** (com acento). A marca oficial é **Arah** (sem acento), alinhada ao design system e ao repositório.

**Onde corrigir:**

| Arquivo | O quê |
|---------|--------|
| `lib/l10n/app_pt.arb` | `"appTitle": "Arah"` |
| `lib/l10n/app_en.arb` | idem |
| Regenerar l10n | `flutter gen-l10n` (atualiza `app_localizations*.dart`) |
| `lib/app.dart` | `MaterialApp.router(title: 'Arah', ...)` |
| `lib/features/profile/presentation/screens/profile_screen.dart` | string hardcoded `'Ará'` → `'Arah'` ou `l10n.appTitle` |
| `web/index.html` | `<title>Arah</title>`, `apple-mobile-web-app-title` |
| `web/manifest.json` | `name` / `short_name` se ainda estiver genérico |
| `test/features/auth/presentation/login_screen_test.dart` | expect `'Arah'` |
| Docs/scripts do app | README, `run-app-local.ps1` (texto "Ará" → "Arah") |

**Critério de aceite:** Nenhuma ocorrência visível de "Ará" no app; testes de login passam.

---

## 2. Splash de abertura — fade-in minimalista e conceitual

**Problema:** Hoje `/` usa `_SplashOrRedirect` com apenas `CircularProgressIndicator` — sem identidade nem transição de abertura.

**Referência visual:** `design-system/ui_kits/app/` — login premium (`screens4.jsx`: logo + **Arah** + gradiente escuro); animações em `index.html` (`fadeIn`, `screenIn` com `cubic-bezier(0.16, 1, 0.3, 1)`); gradiente radial `#15201A` → fundo do tema.

**Proposta de implementação:**

1. Nova tela `SplashScreen` (ou widget em `lib/features/splash/` ou `core/widgets/`):
   - Fundo: gradiente radial sutil (tokens `AppDesignTokens`, sem hex solto nas telas).
   - Logo Arah (asset existente ou SVG; ver `design-system/assets/arah-logo.png`).
   - Wordmark **Arah** (tipografia display — Sora/Geist quando integradas; até lá `textTheme` + peso forte).
   - Opcional: linha conceitual curta (ex.: "Território-primeiro") — uma frase, sem ruído.
2. Animação de entrada: fade-in do logo + wordmark (~400–600 ms); respeitar `MediaQuery.disableAnimations` / `prefers-reduced-motion`.
3. Após animação (ou timeout máx. ~1,5 s): delegar ao `GoRouter` redirect (auth → login / onboarding / home) — **sem** spinner genérico no centro.
4. Rota `/` continua sendo o ponto de entrada; substituir `_SplashOrRedirect` pelo novo splash.

**Critério de aceite:** Ao abrir o app, o usuário vê brevemente uma tela de marca com fade-in; em seguida navega automaticamente conforme sessão/território.

---

## 3. Login — não consegue sair da tela (bloqueio)

**Relato:** Usuário permanece preso na tela de login após tentar entrar (teste local Chrome + BFF).

**API/BFF validados:** `check-email`, `signup` e `login` respondem 200 no stack local — o backend não é o gargalo principal.

### 3.1 Hipóteses técnicas (investigar na implementação)

| # | Hipótese | Evidência no código | Ação sugerida |
|---|----------|---------------------|---------------|
| A | **GoRouter recriado** a cada mudança de `authStateProvider` / `selectedTerritoryIdProvider` | `goRouterProvider` instancia `GoRouter` novo no `Provider` | Usar `GoRouter` estável + `refreshListenable` / `redirect` reativo sem recriar instância |
| B | **Corrida** entre `context.go('/onboarding')` e redirect que ainda vê `hasToken == false` | `login_screen.dart` navega manualmente após `login()` | Confiar no redirect do router após `setSession`; ou `ref.listen` + navegar só quando token confirmado |
| C | **`flutter_secure_storage` na Web** sem `WebOptions` | `secure_storage_service.dart` só configura Android | Adicionar `WebOptions` / fallback `shared_preferences` na web para tokens em dev |
| D | **Botão Google ausente na UI** | `loginWithGoogle()` existe no repositório, mas `LoginScreen` só tem fluxo e-mail | Restaurar botão "Entrar com Google" (comportamento condicional: web exige Firebase/clientId) |
| E | **UX confusa no passo 1** | Botão diz "Continuar", não "Entrar"; sem hint de criar conta | Alinhar copy ao design system (Google + e-mail); microcopy "Não tem conta? Criar ao informar e-mail" |
| F | **Erro silencioso** | Snackbar pode passar despercebido | Mensagem inline no formulário + log em debug para falhas de rede/CORS |

### 3.2 Melhorias de produto no login (design system)

Referência: `design-system/ui_kits/app/screens4.jsx` — `LoginScreen` com:
- Logo grande + eyebrow "Território-primeiro"
- Título **Arah** (44px display)
- Subtítulo humano
- **Entrar com Google** + **Entrar com e-mail**
- Link "Criar conta"

**Implementar:** Recompor `LoginScreen` nessa hierarquia (mantendo fluxo email-first existente por baixo).

### 3.3 Dev local — credenciais de teste

Documentar em `docs/RUN_LOCAL.md` (seção curta):

- Fluxo: e-mail novo → formulário cadastro → onboarding → home.
- Ou usar conta criada, ex.: `dev@test.local` / `senha123` (após signup local).
- Pré-requisitos: BFF em `:5001`, API em `:8080`.

### 3.4 Critérios de aceite (login)

- [ ] Cadastro com e-mail/senha navega para onboarding e depois home.
- [ ] Login com conta existente idem.
- [ ] Token persiste após hot restart (web e mobile).
- [ ] Erros de rede/API visíveis na tela.
- [ ] Google Sign-In disponível onde configurado (Android/iOS/Web com clientId).
- [ ] Testes widget/integration cobrem fluxo feliz mínimo.

---

## 4. Itens menores (mesmo PR ou follow-up)

- **`profile_screen.dart`:** remover hardcode de marca; usar l10n.
- **`main.dart`:** Firebase na web — documentar se login Google na web exige `firebase_options.dart` / `--dart-define=GOOGLE_SIGN_IN_CLIENT_ID=...`.
- **Alinhar teste** `login_screen_test.dart` ao copy real ("Continuar" vs "Entrar") após redesign.

---

## Ordem sugerida de implementação

1. **Marca Arah** (rápido, baixo risco).
2. **Corrigir login** (router + storage web + UX) — desbloqueia uso local.
3. **Splash fade-in** — impacto visual alto, depende menos do backend.
4. **Redesign login** alinhado ao design system premium (pode ser junto com item 2).

---

## Referências

- Design system app premium: `design-system/ui_kits/app/screens4.jsx`, `index.html` (animações)
- Marca: `design-system/README.md` — produto **Arah**
- Stack local: `docs/RUN_LOCAL.md`
- Router atual: `lib/app_router.dart`
- Login atual: `lib/features/auth/presentation/screens/login_screen.dart`

---

## 5. Onboarding — território próximo inexistente

**Problema:** Com localização ativa, a lista só traz **Camburi** e **Boiçucanga** (litoral norte SP, ~100 km da capital). Quem testa na Grande SP ou em outra região vê lista vazia e o botão **Continuar** desabilitado — fica preso no onboarding.

**Causa:** Seeds históricos só cobrem São Sebastião/SP; `suggested-territories` usa raio fixo de **10 km** a partir do GPS (`onboarding_providers.dart`).

### Massa de socorro (já disponível)

| Ação | Comando |
|------|---------|
| Preset capital SP | `.\scripts\seed\run-seed-territory-local.ps1 -Preset sao-paulo-centro` |
| Sua cidade | `.\scripts\seed\run-seed-territory-local.ps1 -Name Centro -City "..." -State UF -Latitude ... -Longitude ...` |
| Automático no stack | `run-local-stack.ps1` aplica também `seed-sao-paulo-centro.sql` |

Documentação: `scripts/seed/README.md`.

### Produto — cadastrar cidade/região se não existir (implementar depois)

**Objetivo:** Se `suggested-territories` retornar vazio, o usuário não fica bloqueado.

**Backend (parcialmente pronto):**

- `POST /api/v1/territories/suggestions` — cria território sugerido (nome, city, state, lat, lng, radiusKm).
- Falta expor via **BFF** jornada `onboarding` (ex.: `POST suggest-territory` ou reutilizar `territories`).

**App (a fazer):**

1. Estado vazio na onboarding: mensagem clara + CTA **“Cadastrar minha região”**.
2. Reverse geocode (Nominatim ou serviço interno) → preencher **cidade/UF** a partir do GPS.
3. Formulário mínimo: nome do bairro/região (editável), cidade, UF (confirmáveis).
4. Chamar BFF → criar território → selecionar automaticamente → `complete`.
5. Moderação posterior: território pode nascer `Active` em dev; em produção, fila de validação comunitária.

**Critérios de aceite:**

- [ ] GPS em região sem seed → usuário consegue entrar no app em &lt; 2 min.
- [ ] Cidade/UF derivadas da localização (com fallback manual).
- [ ] Território criado aparece em `suggested-territories` para outros usuários próximos.
- [ ] Testes E2E: lista vazia → cadastro região → home.

**Referências:** `TerritoriesController.Suggest`, `OnboardingScreen._ContinueButton` (`canContinue` false sem `effective`).

---

## 6. Geometria territorial — fonte governamental (IBGE)

**Necessidade:** Seleção e mapa devem usar contorno **oficial** do município (ou subdivisão acordada), não só círculo manual.

**Solução imediata (dev/local):** script `scripts/seed/fetch-ibge-municipality-boundary.ps1`

```powershell
.\scripts\seed\fetch-ibge-municipality-boundary.ps1 -City Socorro -Uf SP -Apply
```

- Código IBGE Socorro/SP: **3552106**
- Malha: `GET /api/v3/malhas/municipios/{codigo}?formato=application/vnd.geo+json`
- Resultado: `BoundaryPolygonJson` com ~81 pontos (qualidade intermediária, resolução 3)
- SQL versionado: `scripts/seed/seed-socorro-SP-ibge.sql`

**Produto (implementar depois):**

| Camada | Status |
|--------|--------|
| **Backend** | ✅ `POST onboarding/suggest-municipality` — reverse geocode (Nominatim) + malha IBGE + cria território `Active` |
| **BFF** | ✅ Proxy jornada `onboarding/suggest-municipality` |
| **App** | ✅ Card “Cadastrar meu município” quando lista vazia |
| **Pendente** | Metadado `ibgeCode` no domínio; moderação em produção; cache malha IBGE |

---

## 7. Cadastro comunitário de célula (GPS impreciso / IBGE indisponível)

**Status:** ✅ MVP implementado (2026-06-29)

| Camada | Entrega |
|--------|---------|
| **Backend** | `POST onboarding/propose-territory` — pin + cidade/UF + círculo ou polígono → território `Pending` |
| **Backend** | `GET admin/territories/pending` + `POST admin/territories/{id}/review` (SystemAdmin) |
| **BFF** | Proxy `onboarding/propose-territory` |
| **App** | Sheet “Desenhar minha célula”: pin ajustável, raio, polígono; badge “Aguardando curador”; acesso provisório no Continuar |

**Pendente (próxima iteração):** fila para curador territorial (não só SystemAdmin); anti-sobreposição; votação comunitária.


**Outras fontes (futuro):** shapefile IBGE completo (setores censitários), limites estaduais (MRE/IBGE), OSM apenas como fallback com flag de proveniência.

**Critérios de aceite:**

- [ ] Território criado via onboarding exibe contorno municipal no mapa (não só círculo).
- [ ] Metadado de origem (`ibgeCode`, `malhaVersao`) persistido ou auditável.
- [ ] Funciona offline após primeiro fetch (cache).

---

**Registrado em:** 2026-06-29 (teste local Chrome + BFF/API Docker)
