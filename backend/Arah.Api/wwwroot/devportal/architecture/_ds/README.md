# Arah — Design System

**Arah** (também **Araponga**, sua identidade na web) é uma plataforma digital comunitária **orientada ao território**: tecnologia a serviço da vida local, da convivência e da autonomia das comunidades. Não é mais uma rede social — é uma *extensão digital do território vivo*. O nome vem do **araponga**, o "pássaro-ferreiro" das florestas brasileiras, cujo canto metálico e ressonante simboliza comunicação clara e presença no lugar.

> **Território como referência. Comunidade como prioridade. Tecnologia como ferramenta — não como fim.**

Este design system foi construído a partir do repositório **`sraphaz/arah`**. Ele captura a marca em dois produtos reais e dá a um agente de design tudo o que é preciso para produzir interfaces, mocks e materiais bem-marcados da Arah.

---

## Sources

Tudo aqui foi extraído do repositório oficial. Explore-os para fazer um trabalho ainda melhor projetando para a Arah:

- **Repositório:** https://github.com/sraphaz/arah
- **Portal web (voz da marca):** `frontend/portal/` — site Next.js. Paleta floresta, glass cards, fontes Geist + Sora. → `tailwind.config.ts`, `app/globals.css`, `app/fonts.ts`, `components/sections/*`, `content/landing.md`
- **App mobile (o produto):** `frontend/arah.app/` — Flutter, Material 3, tema verde escuro por padrão. → `lib/core/theme/app_design_tokens.dart`, `app_theme.dart`, `core/config/constants.dart`, `lib/features/*`, `docs/DESIGN_SYSTEM.md`
- **Copy / tom de voz:** `frontend/portal/content/landing.md`, `README.md` do repositório, `lib/l10n/app_pt.arb`

> O leitor **não** precisa ter acesso ao repositório — todo token e regra necessários estão reproduzidos aqui. Os links ficam guardados caso você tenha.

---

## Dois produtos, uma natureza

| | **Portal web** ("Araponga") | **App mobile** ("Arah / Ará") |
|---|---|---|
| Stack | Next.js + Tailwind | Flutter + Material 3 |
| Papel | Marketing / manifesto | O produto em si |
| Modo padrão | **Claro**, sobre textura de floresta | **Escuro** (`#121212`) |
| Assinatura | **Glass cards** sobre imagens da natureza | Cards Material, feed, mapa, bottom-nav |
| Tipo | **Sora** (display) + **Geist** (corpo) | Escala Material 3 + verde da marca |
| Primária | Floresta `#377B57` | Copa `#81C784` (escuro) / `#1B5E20` (claro) |

O fio que conecta os dois é **verde-floresta quente, calmo e enraizado** — e o compromisso território-primeiro.

> **Sobre este DS:** o foco do trabalho foi o **app mobile, reinterpretado em uma versão premium, moderna e minimalista** (ver `ui_kits/app/`). A versão premium adota as fontes da marca (Sora + Geist) em vez do Roboto padrão do Material, e refina espaçamento, contraste e hierarquia — mantendo todas as features e diretrizes do app original (tema escuro, verde copa, território-primeiro, bottom-nav de 5 abas).

---

## Features do app (verificadas no código)

O app implementa, hoje:

- **Autenticação** — login com Google (social) e e-mail/senha; cadastro.
- **Onboarding** — localização → mapa → territórios próximos (`suggested-territories`) → "Continuar como visitante".
- **MainShell** com bottom-nav de **5 abas**: Início/Feed · Explorar · Publicar · Notificações · Perfil.
- **Feed territorial** — posts da região, pull-to-refresh, paginação, barra indicadora do território, cards de post (tipos **General** e **Alert**; visibilidade **Public** / **ResidentsOnly**).
- **Explorar** — lista de territórios + seletor; entrar em um território (`{id}/enter`).
- **Mapa** — tiles OpenStreetMap, contorno do território (polígono/círculo), pins (entity / post / event / asset / alert / media).
- **Publicar** — título, conteúdo, tipo, visibilidade.
- **Notificações** — lista in-app, marcar como lida, tipos (alert / post / event / connection).
- **Perfil** — `me/profile`, edição em bottom sheet, interesses, preferências de notificação.
- **Eventos** — eventos do território, "tenho interesse" / "confirmar presença".

---

## CONTENT FUNDAMENTALS

**Idioma:** Português do Brasil é o padrão (`pt`), com inglês (`en`) como secundário. **Sempre escreva em pt-BR.**

**Pessoa & tom.** A marca fala com **"você"** (informal, direto, acolhedor) e usa **"nós/nosso"** para reforçar pertencimento — *"cuidar do **nosso** território juntos"*. O tom é **comunitário, caloroso e prático**, nunca corporativo ou tecnocrático. Convoca à ação concreta e local.

**Casing.** Frases em *sentence case* — nada de Title Case em botões ou títulos. Rótulos de seção/eyebrow aparecem em **MAIÚSCULAS com tracking** (ex.: `TERRITÓRIO-PRIMEIRO`, `PRÓXIMOS A VOCÊ`). Botões usam verbos no infinitivo: *Entrar, Publicar, Continuar, Salvar, Sair*.

**Vocabulário próprio** (use estes termos): **território**, **comunidade**, **morador** vs **visitante**, **vila**, **costão**, **mutirão**, **conselho de moradores**, **feed da região**, **pesca artesanal**, **caiçara**. O território nunca é "feed" genérico — é *"o feed de Camburi"*, *"a vila"*, *"a região"*.

**Exemplos reais de copy** (do `app_pt.arb` e do app):
- Onboarding: *"Para ver o feed e participar da comunidade, escolha um território próximo a você."*
- Privacidade: *"Sua localização é privada e não fica visível para outros usuários."*
- Visitante: *"Ao continuar, você entrará como visitante neste território e poderá ver o feed da região."*
- Feed vazio: *"Nenhum post nesta região"* / *"Seja o primeiro a publicar aqui."*
- Erro: *"Erro ao carregar."* / *"Tentar de novo"*

**Emoji.** Uso **raríssimo e orgânico** — só quando um morador escreveria assim (ex.: 🐟 num post sobre peixe fresco). Nunca em UI de sistema, botões ou rótulos.

**Vibe.** Sossego de vila litorânea: prático, solidário, pé no chão. Mensagens curtas, sem jargão de produto. A tecnologia desaparece; o território aparece.

---

## VISUAL FOUNDATIONS

**Paleta.** Verde-floresta é tudo. A escala vai de `#E2F1E8` (claro) a `#173525` (profundo), com `#377B57` como primária do portal. No app (escuro), a copa clareada é o acento sobre superfícies quase-pretas. Acentos semânticos são poucos e naturais. Nada de roxo, nada de gradiente arco-íris.

> **Duas paletas de app, documentadas em `colors_and_type.css`:** os tokens **canônicos do Flutter** (fiéis ao repositório: primária `#81C784`, surface `#121212`, alerta `#C0492F`, evento `#2A6FDB`) e a **evolução premium do UI kit** (`--premium-*`: base mais profunda `#0B0C0A`, copa `#A6D6B9`, alerta âmbar `#E8A06A`, evento `#86AEEA`, água `#6FC5D6`, com degradês de profundidade). Use a premium ao construir mocks de alta fidelidade; a canônica ao tocar no código real.

**Tipografia.** **Sora** (display, 600–700) para títulos, nomes e números — geométrica, contemporânea, com leve personalidade. **Geist** (400–600) para corpo, rótulos e UI — neutra e altamente legível. Títulos com tracking negativo (`-0.2` a `-1px`); eyebrows em maiúsculas com `+1.2px`.

**Fundos.** No app: superfície escura sólida (`#0E0F0D`/`#121212`) com um leve *radial glow* verde no topo. No portal: textura aquarela de floresta sob glass cards. **Sem** gradientes chamativos; **sem** imagens de banco corporativas. Mapas usam **tiles OSM reais** (no app premium, estilizados em escuro com leve tom verde).

**Cards.** Superfície `#191B17`, **hairline** de `rgba(255,255,255,0.07)`, raio **18–20px**. Sem sombra pesada no escuro — a separação vem do contraste de superfície e da borda fina. No portal, o card é **glass**: `rgba(255,255,255,0.88)`, `backdrop-filter: blur(16px)`, raio **32px**, sombra suave `0 20px 50px rgba(20,40,28,0.15)`.

**Cantos.** Escala: `8` (sm), `12` (md), `16` (lg), `18–20` (cards), `999` (pills). Generoso mas não infantil.

**Espaçamento.** Base **4 / 8 / 16 / 24 / 32** (constantes do app). Alvo de toque mínimo **44px**. Densidade confortável, respiro generoso — minimalismo vem do espaço, não da falta de conteúdo.

**Sombras / elevação.** Discretas. No escuro, evite drop-shadows; use bordas e tons de superfície. Exceção: a ação central **Publicar** ganha um glow verde (`0 6px 18px rgba(129,199,132,0.35)`), e superfícies **glass** flutuantes (sheets, legendas de mapa) usam `0 10px 30px rgba(0,0,0,0.4)` + blur.

**Bordas & transparência.** Hairlines brancas a baixíssima opacidade (`0.07`–`0.12`). Blur reservado para *overlays* (bottom sheets, barra de legenda do mapa, toasts) — nunca decorativo.

**Estados.** *Hover/press*: leve `scale(0.975)` + mudança de preenchimento (chips/segmented trocam de fundo transparente para `tint` verde a ~14% e o ícone passa a `FILL 1`). Acentos selecionados usam o verde sólido com texto escuro (`#0E1F12`). *Press* nunca muda o layout, só a escala/cor.

**Movimento.** Calmo e curto: `150ms` (rápido), `250ms` (normal), easing `cubic-bezier(0.16,1,0.3,1)`. Entradas com fade + leve translate-up (`12–20px`). **Sem** bounce, sem loops decorativos. Toasts deslizam de baixo.

**Imagery.** Quente, natural, à luz do dia — folhas, mata, mar, vila caiçara. Nunca frio, monocromático ou stock corporativo.

**Layout fixo.** Top bar território-primeiro sempre presente (o território ativo é o âncora da experiência). Bottom-nav fixa com a ação **Publicar** elevada ao centro.

---

## ICONOGRAPHY

O app é **Flutter + Material 3**, então o sistema nativo de ícones é **Material Icons**. Para web/HTML, este DS usa **Material Symbols Rounded** (variante arredondada, que combina com a suavidade da marca) via Google Fonts CDN — a correspondência fiel e gratuita do conjunto Material.

- **Estilo:** Rounded, peso 400–500, eixo `FILL` alternando entre **0** (inativo/outline) e **1** (ativo/selecionado). Tamanhos padrão: 18 / 20 / 24 px; constantes do app: `iconSizeSm/Md/Lg`.
- **Ícones-chave por feature:** `forest`/`eco` (território/feed), `explore` (explorar), `add` (publicar), `notifications` (avisos), `person` (perfil), `map`/`place`/`my_location` (mapa/localização), `event` (eventos), `warning` (alerta), `group` (conexões), `favorite`/`mode_comment`/`ios_share` (ações de post), `storefront`/`article`/`photo_camera` (pins).
- **SVGs / PNGs:** não há sprite ou icon-font próprio no repositório além do Material; o logo da Araponga é um **PNG transparingente** (`assets/arah-logo.png`) e a imagem-marca é uma foto (`assets/app-icon-leaf.png`).
- **Emoji como ícone:** não. **Unicode como ícone:** não. Use sempre Material Symbols.

> Ao montar slides ou mocks, **copie os ícones via Material Symbols** (link CDN no `<head>`) — nunca desenhe SVGs à mão nem use emoji no lugar de ícones.

**Substituição sinalizada:** o app real usa Material Icons (empacotado no Flutter). Na web reproduzimos com **Material Symbols Rounded** (CDN). É a correspondência oficial do mesmo conjunto; nenhuma fonte de ícone foi inventada.

---

## Index — manifesto da pasta

**Raiz:**
- `README.md` — este arquivo.
- `colors_and_type.css` — todos os tokens (cores, fontes, escala de tipo, espaçamento, raios, sombras, motion) como CSS vars + `@font-face`. **Importe este arquivo** ao construir qualquer artefato.
- `SKILL.md` — instruções para uso como Agent Skill.

**Pastas:**
- `fonts/` — `Geist-VariableFont_wght.ttf`, `Sora-VariableFont_wght.ttf`.
- `assets/` — `arah-logo.png` (logo araponga, transparente), `app-icon-leaf.png`, `cover-rainforest.png`, `app-banner.png`, `texture-bukeh.jpg`.
- `preview/` — cards do Design System (cores, tipo, espaçamento, componentes, brand). Conteúdo de referência rápida.
- `ui_kits/app/` — **UI Kit do app mobile premium** (interativo). Ver `ui_kits/app/README.md`.
- `site/` — **Site institucional do Arah** (movimento de soberania digital, modelo território-primeiro, app embarcado ao vivo, funcionalidades, roadmap de 48 fases, chamado às comunidades, contribuidores). Abra `site/index.html`.
- `handoff/` — **Especificação de handoff de desenvolvimento** (frontend): papéis, navegação, catálogo de telas, todas as jornadas passo-a-passo com endpoints, estados de UI, cache/offline, gaps backend-first, design system e definição de pronto. Abra `handoff/Especificacao-Arah.html`.
- `wiki/` — **Documentação viva** (hub navegável com sidebar + busca): visão, manifesto, modelo território-primeiro, glossário, papéis, domínio, funcionalidades, arquitetura, roadmap e contribuir. Abra `wiki/index.html`.
- `screenshots/` — capturas de apoio.

**UI Kits disponíveis:**
- `ui_kits/app/` — **App mobile Arah** (premium, minimalista, território-primeiro). Cobre o produto atual e pré-visualiza **todo o backlog** (48 fases):
  - **Núcleo:** Login · Onboarding (geolocalização) · Feed (+ detalhe com comentários, fotos) · Publicar (+ mídia) · Explorar · Mapa (entidades + pins) · Notificações · Perfil · Configurações.
  - **Hub de Serviços** por categoria: Mercado · Minha loja (pagamento PIX) · Compra coletiva · Hospedagem · Demandas & ofertas · Trocas · Entregas · Carteira (Aratá) · Babás · Bem-estar · Aluguéis · Hub digital · Eleições & votação · Gestão/curadoria · Assinaturas · Saúde do território · Métricas · Banco de sementes · Aprendizado · Assistente IA · Conquistas.
  - **Papéis** (visitante / morador / curador) com seletor "Ver como".
  - **Jornadas navegáveis** (fluxos multi-passo com progresso + sucesso) para reserva, babá, bem-estar, checkout, carteira, assinatura, residência, demanda/troca/semente, aluguel, entrega, curso e serviço digital.
  - Conteúdo **por território** (trocar de território muda feed, mapa, eventos, saúde).
  - Abra `index.html`.

**Site:**
- `site/index.html` — site institucional completo (light editorial + momentos escuros cinematográficos), com o app premium embarcado ao vivo. Linguagem do movimento de soberania digital, modelo território-primeiro, funcionalidades, transparência/código aberto, roadmap e chamado às comunidades. Estilos em `site/site.css`, interações em `site/site.js`.

**Handoff:**
- `handoff/Especificacao-Arah.html` — especificação de desenvolvimento (frontend) imprimível, com TOC fixa: visão & princípios, papéis & permissões, arquitetura de navegação, catálogo de 33 telas, **todas as jornadas passo-a-passo com endpoints**, estados de UI, cache/offline, **gaps backend-first** priorizados, design system, acessibilidade/i18n e definição de pronto.

**Wiki:**
- `wiki/index.html` — **documentação viva** do Arah (hub navegável com sidebar + busca), 13 páginas: visão geral, manifesto & princípios, modelo território-primeiro, glossário, papéis & permissões, modelo de domínio, feed & mapa, mercado & economia, governança & curadoria, comunicação, arquitetura, roadmap (48 fases) e contribuir. Linguagem harmoniosa do movimento, fiel à documentação do repositório. Estilos em `wiki/wiki.css`, navegação em `wiki/wiki.js`.
