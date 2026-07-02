# Lições Aprendidas - Arah

**Última Atualização**: 2026-07-02  
**Total de Lições**: 4  
**Status**: Documento Vivo

---

> Este documento captura lições aprendidas de revisões técnicas, garantindo que conhecimento adquirido seja permanentemente incorporado às diretrizes do projeto.

**Processo**: Ver `docs/PROCESSO_AUTO_APRENDIZADO_REVISOES.md`

---

## 📚 Lições por Categoria

### 🔴 Críticas

#### LIC-001 - Cores Hardcoded Proibidas
**Data**: 2025-01-20  
**Categoria**: Crítico  
**Revisão Origem**: `docs/REVISAO_ARTE_DESIGN_WIKI.md`

**Contexto**: 
Revisão completa de conformidade de design da Wiki identificou 29 ocorrências de cores hardcoded (`#4dd4a8`, `#7dd3ff`, `#25303a`, etc.) em `frontend/wiki/app/globals.css`. Embora diretrizes mencionassem uso de variáveis CSS, não havia proibição explícita de Tailwind arbitrárias (`dark:bg-[#4dd4a8]`).

**Lição**:
Diretrizes devem ser **explícitas e proibitivas** sobre o que NÃO fazer, não apenas sugestivas sobre o que fazer. Especificamente:
- ❌ PROIBIR cores hex/rgb diretas
- ❌ PROIBIR Tailwind arbitrárias com cores (`[#4dd4a8]`)
- ✅ OBRIGAR variáveis CSS (`var(--accent)`) ou classes Tailwind configuradas (`dark:bg-dark-accent`)

**Ação Tomada**:
1. ✅ Corrigidas 29 ocorrências de cores hardcoded
2. ✅ Adicionadas variáveis CSS no `:root`: `--accent`, `--link`, `--border-dark`
3. ✅ Todas as cores agora via variáveis CSS ou classes Tailwind configuradas

**Diretriz Atualizada**:
- `docs/CURSOR_DESIGN_RULES.md`:
  - Seção 2.1: Regra obrigatória explícita sobre cores hardcoded
  - Exemplos atualizados: Button component usa `dark:bg-dark-accent`
  - Checklist de validação inclui verificação de cores hardcoded
- `.cursorrules`:
  - Nova seção: "Regras Críticas de Design"
  - Proibição explícita documentada

**Prevenção Futura**:
- ✅ Checklist de validação inclui: "Cores: Usa variáveis CSS ou classes Tailwind configuradas (NUNCA hex/rgb diretos ou Tailwind arbitrárias)"
- ✅ Script de verificação: `grep -r "dark:bg-\[#"` pode detectar violações
- ✅ Code review deve verificar conformidade antes de merge

**Métrica de Sucesso**:
- Antes: 29 ocorrências de cores hardcoded
- Depois: 0 ocorrências (100% conformidade)
- Efetividade: Problema eliminado completamente

---

#### LIC-002 - Cores hardcoded reaparecem sem agente/gate dedicado de design
**Data**: 2026-07-02
**Categoria**: Crítico
**Revisão Origem**: `docs/design/AUDITORIA_DESIGN.md`

**Contexto**:
Mesmo após LIC-001, a auditoria autônoma encontrou novas violações: Tailwind arbitrárias
(`bg-[#4dd4a8]`/`[#7dd3ff]`) no Mermaid fullscreen da wiki e `Color(0xFF228B22)`/`Colors.orange`
fora do tema no onboarding Flutter. A regressão ocorreu porque não havia inteligência de design
**contínua e autônoma** revisando cada mudança de frontend, nem gate de CI que barrasse cores fora
de token.

**Lição**:
Diretriz proibitiva (LIC-001) não basta: é preciso um **agente de domínio autônomo** que revise
frontend a cada interação e um **gate automatizado** que falhe o PR ao detectar cor hardcoded. O
mesmo rigor aplicado a backend/domínio deve valer para design.

**Ação Tomada**:
1. ✅ Criado agente `design-ux` (`.agents/domain/design-ux.agent.yaml`) + regra na coreografia → acionamento automático em `frontend/**` e `docs/**/design*`.
2. ✅ Corrigidas as violações (DSG-01 wiki, DSG-02 Flutter) usando classes/tokens configurados.
3. ✅ Auditoria `docs/design/AUDITORIA_DESIGN.md` + backlog `design-quality` no `PHASE_QUEUE`.

**Diretriz Atualizada**:
- `AGENTS.md` e `.cursor/rules/domain-agents-autonomy.mdc`: `design-ux` no catálogo e mapeamento.
- Backlog `design-quality` inclui **DSG-08** (gate CI anti-cor-hardcoded) como prevenção estrutural.

**Prevenção Futura**:
- ✅ Agente `design-ux` publica parecer (local via hook `stop` + CI via `agents.yml`/Agents Orchestrate; gate em `agents-gates.yml` via `run-gates.ps1`).
- ✅ DSG-08: gate `scripts/agents/design-gate-check.ps1` (no `run-gates.ps1`/QA) falha o PR ao detectar `[#...]`, hex inline, cor literal em CSS ou `Color(0x...)`/`Colors.*` fora dos arquivos de token.

**Métrica de Sucesso**:
- Antes: 10 novas ocorrências (5 wiki + 5 Flutter) sem revisão autônoma de design.
- Depois: 0 ocorrências nesses arquivos; agente de design ativo em todo PR de frontend.

---

### 🟡 Importantes

#### LIC-003 - Referências fantasma em manifests de agentes passam despercebidas sem validação de integridade
**Data**: 2026-07-02
**Categoria**: Importante
**Revisão Origem**: `docs/ops/AGENT_STRATEGY_VALIDATION.md`

**Contexto**:
A validação da estratégia de agentes encontrou referências quebradas acumuladas: `flutter.agent.yaml`
consultava domínios `feed-conteudo`/`mapa-lugares` que não existiam, `backend.agent.yaml` consultava
specialist `postgresql` inexistente, e specs/docs traziam contagens hardcoded desatualizadas (22/17 vs
real). O `validate-manifests.ps1` só checava campos obrigatórios e existência de checklist — nunca a
resolução das referências cruzadas.

**Lição**:
Manifests declarativos (agentes ↔ domínios ↔ skills) precisam de **validação de integridade
referencial automatizada**, como chaves estrangeiras: toda referência declarada deve resolver para um
arquivo existente. E contagens/valores dinâmicos nunca devem ser hardcoded em specs ou docs.

**Ação Tomada**:
1. ✅ Criados os agentes de domínio faltantes (`feed-conteudo`, `mapa-lugares`, `comunidade-conexoes`, `identidade-privacidade`) e o specialist `postgresql`.
2. ✅ `validate-manifests.ps1` agora falha se `consult.domain`, `consult.specialists` ou `skills` referenciarem manifests inexistentes.
3. ✅ `agent-operation.spec.yaml` AC-AG-1 sem contagem hardcoded (contagem dinâmica).

**Prevenção Futura**:
- ✅ Gate roda em todo PR (`agents-validate.yml` / `run-gates.ps1`) — referência fantasma quebra o CI.
- Ao citar um agente/skill em manifest ou doc, criá-lo no mesmo PR.

---

### 🟢 Otimizações

#### LIC-004 - Contexto fixo e comunicação ativa entre agentes inflam consumo de API
**Data**: 2026-07-02  
**Categoria**: Otimização  
**Revisão Origem**: análise de consumo Cursor Ultra (API 44% com 21 dias restantes no ciclo)

**Contexto**:
Três ofensores identificados: (1) `.cursorrules` monolítico (~60 KB) e `AGENTS.md` (~15 KB)
injetados em toda requisição; (2) hook `stop` com `followup_message` gerando turno extra de
modelo a cada interação; (3) ~1.478 skills globais do usuário com catálogo inteiro injetado
em todas as conversas de todos os projetos.

**Lição**:
Contexto de agentes deve seguir modelo **pull, não push**: núcleo mínimo always-apply,
regras/skills escopadas por glob ou sob demanda, comunicação entre agentes passiva (arquivo
+ CI) sem turnos extras de modelo. Catálogos grandes de skills usam roteador + índice
pesquisável (tags/categoria), nunca injeção em massa.

**Ação Tomada**:
1. ✅ `.cursorrules` v2.0 (~4 KB) + regras escopadas (`backend-standards`, `frontend-design`, `docs-organization`, `domain-agents-autonomy`).
2. ✅ Hook `stop` passivo — grava `.cursor/domain-review.md` sem `followup_message`.
3. ✅ Skills globais movidas para `skills-backup/`; única skill global `skill-router` + `INDEX.md` com tags.
4. ✅ `AGENTS.md` v2.0 enxuto; detalhes em `docs/ops/AGENT_OPERATION.md` (PR 14).

**Prevenção Futura**:
- Não voltar a `alwaysApply: true` em rules volumosas sem justificativa.
- Novas skills Cursor do repo: apontar para `.skills/*.yaml`, não duplicar corpo.
- Manter pasta global de skills pequena; catálogo grande fica em backup + índice.

---

## 📊 Estatísticas

- **Total de lições críticas**: 2 (LIC-001, LIC-002)
- **Total de lições importantes**: 1 (LIC-003)
- **Total de lições de otimização**: 1 (LIC-004)
- **Diretrizes atualizadas**: 8 (inclui `.cursorrules` v2.0, `AGENTS.md` v2.0, 4 rules em `.cursor/rules/`, `AGENT_OPERATION.md`, `AGENT_QUICKSTART.md`)
- **Componentes corrigidos**: 39 ocorrências (29 em `globals.css` via LIC-001 + 10 wiki/Flutter via LIC-002) e 6 manifests/gates via LIC-003
- **Taxa de resolução**: 100% (39/39 corrigidas)

---

## 📈 Tendências

### Padrões Identificados

1. **Diretrizes Ambíguas** (1 ocorrência)
   - Problema: Diretrizes sugerem mas não proíbem explicitamente
   - Solução: Usar linguagem proibitiva (❌ PROIBIDO, ✅ OBRIGATÓRIO)
   - Status: Resolvido em LIC-001

### Categorias Mais Frequentes

*(Será preenchido conforme mais lições são adicionadas)*

---

## 🔄 Histórico de Lições

| ID | Data | Categoria | Título | Status |
|----|------|-----------|--------|--------|
| LIC-001 | 2025-01-20 | 🔴 Crítico | Cores Hardcoded Proibidas | ✅ Resolvido |
| LIC-002 | 2026-07-02 | 🔴 Crítico | Cores hardcoded exigem agente + gate de design | ✅ Resolvido |
| LIC-003 | 2026-07-02 | 🟡 Importante | Integridade referencial de manifests de agentes | ✅ Resolvido |
| LIC-004 | 2026-07-02 | 🟢 Otimização | Contexto fixo e comunicação ativa inflam API | ✅ Resolvido |

---

**Próximas Ações**:
- Monitorar consumo de API no Cursor após PR 14 (contexto em camadas + skill-router)
- Monitorar conformidade de cores nos próximos PRs
- Criar script automatizado de verificação de conformidade
- Revisar outras diretrizes para garantir linguagem explícita e proibitiva

---

**Última Revisão**: 2026-07-02  
**Próxima Revisão Periódica**: 2026-10-02 (trimestral)
