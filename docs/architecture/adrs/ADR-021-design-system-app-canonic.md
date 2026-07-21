# ADR-021: Design system do app como fonte canônica de UI

**Status**: Accepted  
**Data**: 2026-07-21  
**Autor**: Solutions Architect (agente / humano)  
**Spec-Id**: APP-DS-01  
**LikeC4 view**: —

---

## Contexto

A análise `docs/design/ANALISE_DESIGN_VS_APP_FLUTTER.md` (APP-DS-01) mostrou múltiplas fontes de IA/visual em conflito: handoff + UI kit premium (`design-system/`), guidelines Flutter (`docs/frontend/flutter/26_*.md`, planos de nav em `24_*.md`) e o wireframe PDF legado. O código Flutter segue ainda outra combinação (tokens portal, bottom-nav com Notificações). Sem uma fonte canônica única, alinhamento visual e de navegação fica ambíguo e regressivo.

## Decisão

1. **Fonte canônica visual/IA do app mobile** (ordem de autoridade):
   - `design-system/handoff/Especificacao-Arah.html` — catálogo de telas, nav, jornadas, DoD frontend
   - `design-system/ui_kits/app/` — protótipo interativo high-fidelity
   - `design-system/colors_and_type.css` — tokens `--premium-*` (paleta floresta, tipografia)

2. **O código Flutter** (`frontend/arah.app/`) **deve convergir** para essa fonte; divergências são gaps a fechar (APP-DS-02+), não “outra verdade”.

3. **Legado (histórico apenas — não alvo de implementação)**:
   - `docs/frontend/flutter/26_FLUTTER_DESIGN_GUIDELINES.md`
   - Seções de navegação / IA em `docs/frontend/flutter/24_FLUTTER_FRONTEND_PLAN.md`
   - `design/app_wireframe_v2.pdf`

4. **Bottom-nav canônica**: Feed · Explorar · Publicar · Serviços · Perfil

5. **TopBar canônica**: território (indicador) + Mensagens + Notificações

## Consequências

**Positivo**
- Uma hierarquia clara para PRs de UI, reviews e ondas APP-DS-*
- Docs Flutter e wireframe deixam de competir com o handoff
- IA e chrome (Serviços na 4ª aba; Notif/Mensagens na TopBar) ficam decididos

**Negativo / custo**
- Guidelines `26` e plano `24` (nav) passam a banner de legado; leitores devem ser redirecionados ao `design-system/`
- Refatoração de shell, tokens e tipografia no Flutter torna-se trabalho obrigatório subsequente (não opcional)

## Alternativas consideradas

1. **Manter guidelines `26` + código atual como canônicos** — rejeitado: distantes do UI kit premium e da spec de handoff já investida.
2. **Dual-canon (portal teal + premium floresta sem prioridade)** — rejeitado: perpetua conflito; dual-theme só após migração documentada a partir do premium (ver APP-DS-05).
3. **Wireframe PDF como alvo** — rejeitado: desatualizado frente ao handoff HTML e ao UI kit.

## Diagrama

_Opcional: mapa handoff → shell Flutter em onda B (APP-DS-02/03/04)._

## Referências

- [ADR Registry](../ADR-REGISTRY.yaml)
- [10_ARCHITECTURE_DECISIONS.md](../10_ARCHITECTURE_DECISIONS.md)
- [ANALISE_DESIGN_VS_APP_FLUTTER.md](../../design/ANALISE_DESIGN_VS_APP_FLUTTER.md)
- `design-system/handoff/Especificacao-Arah.html`
- `design-system/ui_kits/app/`
- `design-system/colors_and_type.css`
