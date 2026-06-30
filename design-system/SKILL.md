---
name: arah-design
description: Use this skill to generate well-branded interfaces and assets for Arah (Araponga) — a território-first, comunidade-first community platform — for production or throwaway prototypes/mocks. Contains essential design guidelines, colors, type, fonts, assets, and a premium mobile UI kit.
user-invocable: true
---

Read the `README.md` file within this skill, and explore the other available files.

If creating visual artifacts (slides, mocks, throwaway prototypes, etc), copy assets out and create static HTML files for the user to view. If working on production code, you can copy assets and read the rules here to become an expert in designing with this brand.

If the user invokes this skill without any other guidance, ask them what they want to build or design, ask some questions, and act as an expert designer who outputs HTML artifacts _or_ production code, depending on the need.

## Quick orientation
- **Arah / Araponga** = território-primeiro, comunidade-primeiro. Brazilian Portuguese (pt-BR) first. Warm forest-green, calm, rooted. Never corporate, never purple gradients.
- **Two surfaces:** dark-default mobile app (Flutter/Material 3, canopy green `#81C784`; the premium UI kit refines this to base `#0B0C0A` + canopy `#A6D6B9`) and a light web portal/site (Geist + Sora, glass cards over forest imagery).
- **Tokens:** `colors_and_type.css` — import it. Fonts in `fonts/` (Geist, Sora). Logo + imagery in `assets/`.
- **Icons:** Material Symbols Rounded (CDN). `FILL 0` inactive, `FILL 1` active. Never emoji-as-icon.
- **Voice:** "você" + "nós/nosso", sentence case, infinitive verbs on buttons, vocabulary like *território, morador, visitante, vila, mutirão, conselho de moradores, feed da região*.
- **Reference UI:** `ui_kits/app/` — premium minimalist recreation of the mobile app. Covers the current product **and** previews the full 48-phase backlog: feed/posts, map, events, **marketplace + my store (PIX)**, **compra coletiva, hospedagem, demandas, trocas, entregas, carteira (Aratá)**, **babás, bem-estar, aluguéis, hub digital**, **eleições/governança, curadoria**, assinaturas, saúde do território, métricas, sementes, aprendizado, IA, conquistas. Roles (visitante/morador/curador) + multi-step **navigable journeys**. Lift components from its `*.jsx` files.
- **Site:** `site/index.html` — institutional site (movement, model, live app embed, roadmap, call to communities). Styles in `site/site.css`.
- **Handoff:** `handoff/Especificacao-Arah.html` — full frontend dev spec (roles, navigation, screen catalog, journeys with endpoints, UI states, cache/offline, backend-first gaps).
- **Cards:** `preview/` holds quick-reference specimens for colors, type, spacing, components, brand.

Stay faithful to what exists — copy the design, don't reinvent it. When a pattern isn't in the system, omit it or leave it blank with a disclaimer rather than inventing.
