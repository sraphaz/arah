# Análise profunda: Design especificado × App Flutter implementado

**Data**: 2026-07-15  
**Autor**: Agente cloud (análise pré-alinhamento)  
**Escopo**: `design-system/` (fonte canônica de UI) × `frontend/arah.app/` (implementação)  
**Status**: 🚧 Alinhamento em andamento (Onda A+B iniciada 2026-07-21)  
**Próximo passo**: Onda C — redesign high-fidelity das telas núcleo (ver §8)

---

## Veredito

O app Flutter está **funcionalmente avançado** (API ↔ BFF ↔ jornadas wired), mas **visual e estruturalmente distante** do design-system premium (`design-system/ui_kits/app/` + `handoff/Especificacao-Arah.html`).

| Dimensão | Design (canônico) | App Flutter | Gap |
|----------|-------------------|-------------|-----|
| Informação arquitetura (IA) | Feed · Explorar · Publicar · **Serviços** · Perfil | Feed · Explorar · Publicar · **Notificações** · Perfil | 🔴 estrutural |
| Chrome (top bar) | Território + Mensagens + Notificações | Parcial (`TerritoryIndicatorBar` no feed; sem chrome global) | 🔴 |
| Hub Serviços | Diretório por categoria + “Em breve” | ❌ inexistente | 🔴 |
| Sistema visual | Floresta premium (`#A6D6B9` / `#0B0C0A`) + Sora/Geist | Teal portal (`#4DD4A8` / `#0A0E12`) + Inter | 🔴 |
| Componentes | JourneyShell, RoleBadge, PixPay, cards hairline | Material 3 genérico na maioria das telas | 🟠 |
| Telas núcleo | 17 desenhadas em alta fidelidade | ~23 screens, ~4 com UX “desenhada” | 🟠 |
| Backlog pré-visualizado | 16 telas “Em breve” no hub | Só Assinaturas existe (fora do hub) | 🟡 esperado |
| Docs Flutter `24`–`26` | IAs e tipografia conflitantes | Código segue outra IA ainda | 🟠 fontes múltiplas |

**Conclusão:** alinhar código ao design exige **primeiro** declarar fonte canônica única (UI kit + handoff) e **depois** refatorar IA + chrome + tokens + componentes — não só “polir cards”.

---

## 1. Fontes de verdade (e conflitos)

### 1.1 Hierarquia recomendada (esta análise)

> **APP-DS-01 — Decidido / Accepted (2026-07-21):** fonte canônica formalizada em [ADR-021](../architecture/adrs/ADR-021-design-system-app-canonic.md). Bottom-nav: Feed · Explorar · Publicar · Serviços · Perfil; TopBar: território + Mensagens + Notificações. Guidelines `26` / nav em `24` / wireframe v2 = legado.

| Prioridade | Artefato | Por quê |
|------------|----------|---------|
| **P0 canônico** | `design-system/handoff/Especificacao-Arah.html` | Catálogo 33 telas, nav, jornadas, gaps, DoD frontend |
| **P0 visual** | `design-system/ui_kits/app/` + `colors_and_type.css` (`--premium-*`) | Protótipo interativo high-fidelity |
| **P1 código atual** | `frontend/arah.app/lib/core/theme/app_design_tokens.dart` | Tokens reais em produção; divergem do premium |
| **P2 histórico** | `docs/frontend/flutter/24|25|26_*.md`, `design/app_wireframe_v2.pdf` | Úteis como contexto; **não** usar como alvo sem reconciliar |

### 1.2 Conflitos documentais já existentes

| Tema | Handoff / UI kit | Guidelines `26` | App real |
|------|------------------|-----------------|----------|
| Bottom-nav 4ª aba | **Serviços** | Mapa · Eventos · Alertas | **Notificações** |
| Tipografia | Sora + Geist | Inter | Inter (Google Fonts) |
| Tema default | Dark floresta | Light-first documentado | Dark |
| Primária app | Copa `#81C784` / premium `#A6D6B9` | Verde floresta clássico | Teal `#4DD4A8` (portal) |

> A auditoria `AUDITORIA_DESIGN.md` nota o Flutter como **A-** em tokens — isso mede **disciplina de tokenização**, não fidelidade ao UI kit premium. Esta análise cobre o segundo eixo.

---

## 2. Arquitetura de navegação

### 2.1 Spec (handoff §03)

```
Stages: Login → Onboarding → MainShell
Tabs:   Feed · Explorar · Publicar↑ · Serviços · Perfil
Top:    Território ativo | Mensagens | Notificações (badge)
Stack:  overlays push/pop (mapa, post, chat, ferramentas)
Modal:  Jornadas multi-passo (progresso → revisão → sucesso)
```

Regras: trocar aba limpa overlays; trocar território → Feed; ferramentas do lugar em Explorar; itens do usuário no Perfil.

### 2.2 Implementado

```
Stages: Login → Onboarding → MainShell   ✅
Tabs:   Feed · Explorar · Publicar · Notificações · Perfil   ❌ (4ª aba)
Top:    TerritoryIndicatorBar só em Feed/Explore parcial; sem Mensagens/Notif globais   ❌
Stack:  go_router push para mapa/eventos/chat/etc.   🟡 (funciona, sem chrome unificado)
Modal:  bottom sheets / forms; sem JourneyShell   ❌
```

Evidência: `frontend/arah.app/lib/features/home/presentation/screens/main_shell_screen.dart` (comentário ainda cita “Instagram” + “Notifications placeholder”).

### 2.3 Impacto

- Notificações e Mensagens competem com o hub de produto (Serviços).
- Explorar vira “barra de ícones + seletor”, não “ferramentas do território”.
- Perfil vira dump de `ListTile`s de rotas secundárias (settings sheet), em vez de grade de ferramentas + papel.

---

## 3. Design system visual

### 3.1 Paleta

| Token | Premium UI kit | Flutter `AppDesignTokens` |
|-------|----------------|---------------------------|
| Fundo | `#0B0C0A` | `#0A0E12` (azul-noite) |
| Card | `#191B17` + hairline `rgba(255,255,255,0.07)` | `#141A21` |
| Acento | `#A6D6B9` / `#81C784` (copa) | `#4DD4A8` (teal portal) |
| Alerta | `#E8A06A` | `#F5C842` (warning genérico) |
| Evento | `#86AEEA` | `#7DD3FF` (link) |
| Água / saúde | `#6FC5D6` | — |

Leitura: o app herdou a paleta do **portal web Araponga** (teal/cyan), não a **copa floresta** do UI kit. Mesmo com tokens centralizados, a identidade percebida diverge.

### 3.2 Tipografia

| Spec premium | App |
|--------------|-----|
| Sora display + Geist corpo | Inter via `google_fonts` em `app_theme.dart` |

### 3.3 Componentes

| Componente UI kit | No Flutter |
|-------------------|------------|
| `TopBar` / `BottomNav` (Publicar elevada + glow) | `NavigationBar` Material padrão |
| `Card` hairline 18–20px | `Card` Material / poucos `ArahGlassCard` |
| `RoleBadge` / Avatar com selo morador | Ausente |
| `JourneyShell` / `SuccessView` / `PixPay` | Ausente |
| `Btn` / `Chip` / `SegTab` / `PillSelect` | Mistura Material |
| Banner-convite (visitante) | Ausente no feed |

Uso real de componentes “marca”: concentrado em **Login**, **Feed card**, **Mapa (sheet)**. Demais telas = `ListTile` + `AlertDialog`.

### 3.4 Motion

Spec: 150/250ms, `cubic-bezier(0.16,1,0.3,1)`, press `scale(0.975)`, sem bounce.  
App: `arah_motion.dart` existe; adoção nas features é baixa.

---

## 4. Matriz tela a tela (núcleo — 17 telas)

Legenda de fidelidade visual/UX (não de “API existe”):

- **A** — próximo do UI kit  
- **B** — estrutura ok, visual Material genérico  
- **C** — funcional mínimo / API wrapper  
- **D** — ausente ou IA errada  

| Tela (spec) | Flutter | API/BFF | Fidelidade | Notas |
|-------------|---------|---------|------------|-------|
| Login | `login_screen.dart` | ✅ | **B+** | Melhor tela “marca” (brand + glass); sem atmosfera premium Sora/Geist |
| Onboarding | `onboarding_screen.dart` | ✅ | **B** | Fluxo rico (geo, busca, propor território); mapa **inset** em Card; fora de `ArahScaffold` |
| Feed | `feed_screen.dart` | ✅ | **B** | Cards tipados + skeleton; falta banner visitante, selo papel, fotos hero, chrome top |
| Detalhe post | `post_detail_screen.dart` | ✅ | **B** | Funcional; visual Material |
| Publicar | `create_post_screen.dart` | ✅ | **B−** | Form; tipos só General/Alert (spec Tip/Event no card) |
| Explorar | `explore_screen.dart` | ✅ | **C** | Seletor + 6 IconButtons; não é hub de ferramentas do lugar |
| Mapa | `map_screen.dart` | ✅ | **B+** | Full-bleed OSM + boundary + deep-link — mais próximo do espírito do kit |
| Eventos | `events_screen.dart` + create | ✅ | **B−** | Lista/FAB; cards sem composição de calendário do kit |
| Mensagens | `chat_*_screen.dart` | ✅ | **C** | Bolhas sem alinhamento eu/outro; sem entrada na top bar |
| Notificações | `notifications_screen.dart` | ✅ | **C** | Lista ok; está na **bottom-nav** (spec: top bar) |
| Perfil | `profile_screen.dart` | ✅ | **C** | ListTiles; sem stats/grade/papel “Ver como” (demo) |
| Configurações | sheet no perfil | 🟡 | **C** | Hub de rotas, não tela dedicada de conta/privacidade |
| Mercado | `marketplace_screen.dart` | ✅ | **C** | Busca + ListTile; sem grade, detalhe, sacola visual, PIX journey |
| Minha loja | tab no marketplace | ✅ | **C** | CRUD mínimo; sem dashboard payout/PIX do kit |
| Eleições & votação | `governance_screen.dart` | ✅ | **B−** | Funcional (votar/criar/resultados); visual Material |
| Gestão / curadoria | `moderation_screen.dart` | ✅ | **C** | 3 tabs API; sem UX de fila do kit |
| **Serviços (hub)** | — | — | **D** | **Não existe**; peça central da IA do design |

### 4.1 Telas Flutter extras (fora do catálogo handoff)

Úteis, mas reforçam a IA “lista de APIs”: Membership, Connections, Assets, Alerts, Subscriptions (esta última no handoff como backlog F15, porém já wired no app).

---

## 5. Matriz backlog (16 telas “Em breve” no UI kit)

| Tela | Fase | No app | Tratamento esperado no alinhamento |
|------|------|--------|-------------------------------------|
| Compra coletiva | F17 | ❌ | Entry no hub Serviços + selo Em breve |
| Hospedagem | F18 | ❌ | idem |
| Demandas & ofertas | F19 | ❌ | idem |
| Trocas | F20 | ❌ | idem |
| Entregas | F21 | ❌ | idem |
| Carteira Aratá | F22 | ❌ | idem |
| Babás / Bem-estar | E10 | ❌ | idem |
| Aluguéis | F46 | ❌ | idem |
| Hub digital | F26 | ❌ | idem |
| Assinaturas | F15 | ✅ tela | Mover descoberta para hub; alinhar UX |
| Saúde território | F24 | ❌ | Em breve |
| Métricas | F25 | ❌ | Em breve |
| Banco sementes | F48 | ❌ | Em breve |
| Aprendizado | F45 | ❌ | Em breve |
| Assistente IA | F27 | ❌ | Em breve |
| Conquistas | F42 | ❌ | Em breve |

O UI kit já resolve descoberta de roadmap **sem implementar backend**. O app atual esconde o futuro e espalha o presente.

---

## 6. Jornadas (spec × profundidade no cliente)

| Jornada handoff | Estado no Flutter |
|-----------------|-------------------|
| Entrada & onboarding | ✅ fluxo completo (com busca sem GPS) |
| Confirmar residência (GPS + comprovante + pending) | 🟡 botões em Membership; **sem** upload/pending/journey |
| Feed / publicar / comentar | ✅ básico; gaps mídia rica / report |
| Eventos | ✅ listar/criar/participar |
| Mercado & checkout PIX | 🟡 checkout stub (`Checkout via app`) |
| Minha loja / payout | 🟡 CRUD; sem PIX/taxa |
| Governança | ✅ |
| Curadoria | 🟡 API lista; UX rasa |
| Jornadas multi-passo (reserva, babá, carteira…) | ❌ framework inexistente |

Padrão de permissão da spec — **banner-convite** em vez de esconder — quase não aparece.

---

## 7. Gaps priorizados (IDs novos — eixo app↔design)

Complementam `DSG-*` da `AUDITORIA_DESIGN.md` (tokens/web). Estes focam **fidelidade de produto mobile**.

| ID | Gap | Severidade | Dependência |
|----|-----|------------|-------------|
| **APP-DS-01** | Declarar fonte canônica (UI kit + handoff) e marcar `26_FLUTTER_DESIGN_GUIDELINES` / wireframe v2 como legado | ✅ Accepted ([ADR-021](../architecture/adrs/ADR-021-design-system-app-canonic.md)) | — |
| **APP-DS-02** | Trocar 4ª aba Notificações → **Serviços**; Notif/Mensagens na TopBar | 🔴 | APP-DS-01 |
| **APP-DS-03** | Implementar **TopBar** território-primeiro global no MainShell | 🔴 | APP-DS-02 |
| **APP-DS-04** | Hub **Serviços** (categorias + live/soon) | 🔴 | APP-DS-02 |
| **APP-DS-05** | Migrar tokens Flutter canônicos → paleta **premium floresta** (ou dual-theme documentado) | 🔴 | APP-DS-01 |
| **APP-DS-06** | Tipografia Sora + Geist no Flutter (ou subset aprovado) | 🟠 | APP-DS-05 |
| **APP-DS-07** | Biblioteca de componentes: Card hairline, RoleBadge, Btn, JourneyShell | 🟠 | APP-DS-05 |
| **APP-DS-08** | Redesign Explorar (ferramentas do território, não só ícones) | 🟠 | APP-DS-03 |
| **APP-DS-09** | Redesign Perfil (papel, stats, grade) + banner visitante no Feed | 🟠 | APP-DS-07 |
| **APP-DS-10** | Mercado / Minha loja / Chat / Moderação: sair do padrão ListTile | 🟠 | APP-DS-07 |
| **APP-DS-11** | Jornada residência (comprovante + pending) alinhada ao handoff | 🟠 | backend gaps |
| **APP-DS-12** | Estados loading/empty/error + motion 150/250 em todas as telas | 🟡 | APP-DS-07 |
| **APP-DS-13** | Assinar gate visual (golden/screenshot ou checklist) no CI para regressão de IA | 🟡 | APP-DS-02 |

---

## 8. Plano de alinhamento sugerido (após aprovação)

Não estimar calendário — só ordem técnica e risco.

### Onda A — Fundação (baixo risco de feature, alto impacto de marca)

1. APP-DS-01 decisão canônica + ADR curto em `docs/architecture/adrs/`  
2. APP-DS-05 tokens premium no `AppDesignTokens` (manter aliases se preciso)  
3. APP-DS-06 fontes  
4. APP-DS-07 componentes base

### Onda B — IA (quebra controlada de navegação)

1. APP-DS-02 / 03 / 04 — shell + top bar + hub Serviços  
2. Mover rotas secundárias (mapa, eventos, mercado, governança…) para Explorar/Serviços conforme spec  
3. Deep-links e l10n atualizados

### Onda C — Telas núcleo high-fidelity

Login → Onboarding → Feed → Mapa → Publicar → Perfil → Eventos → Mercado (nessa ordem de exposição)

### Onda D — Profundidade de jornadas

Residência, checkout PIX, curadoria, JourneyShell para fluxos futuros (Em breve pode ser só UI)

---

## 9. O que já é útil para testar (sem esperar o alinhamento)

Independente do gap visual, o app **já permite validar produto**:

- Auth, onboarding (geo + busca), feed, mapa, eventos, governança, membership básico, marketplace stub, chat, alertas, assinaturas, moderação API.

Use `.\scripts\run-local-stack.ps1` + `flutter run` com `BFF_BASE_URL`.  
Referência funcional: `docs/FEATURE_MATRIX_API_BFF_APP.md` e `docs/STABLE_RELEASE_APP_ONBOARDING.md`.

O desalinhamento é de **interface e IA**, não de “app vazio”.

---

## 10. Como validar o design-system localmente

```bash
# Protótipo premium (canônico visual)
open design-system/ui_kits/app/index.html

# Spec imprimível
open design-system/handoff/Especificacao-Arah.html
```

Comparar lado a lado com o Flutter: bottom-nav, top bar, feed card, hub Serviços, tipografia.

---

## 11. Referências

| Artefato | Path |
|----------|------|
| Spec handoff | `design-system/handoff/Especificacao-Arah.html` |
| UI kit | `design-system/ui_kits/app/` |
| Tokens DS | `design-system/colors_and_type.css` |
| DS README | `design-system/README.md` |
| App shell | `frontend/arah.app/lib/features/home/presentation/screens/main_shell_screen.dart` |
| Tokens app | `frontend/arah.app/lib/core/theme/app_design_tokens.dart` |
| Matriz funcional | `docs/FEATURE_MATRIX_API_BFF_APP.md` |
| Auditoria tokens/web | `docs/design/AUDITORIA_DESIGN.md` |
| Guidelines Flutter (legado a reconciliar) | `docs/frontend/flutter/26_FLUTTER_DESIGN_GUIDELINES.md` |

---

### Changelog deste documento

- **1.0** (2026-07-15): primeira análise profunda design×app; IDs APP-DS-01..13; ondas A–D.
