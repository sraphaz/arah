# Lições Aprendidas - Arah

**Última Atualização**: 2026-07-02  
**Total de Lições**: 2  
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

*(Nenhuma lição importante ainda documentada)*

---

### 🟢 Otimizações

*(Nenhuma lição de otimização ainda documentada)*

---

## 📊 Estatísticas

- **Total de lições críticas**: 1
- **Total de lições importantes**: 0
- **Total de lições de otimização**: 0
- **Diretrizes atualizadas**: 2 (`CURSOR_DESIGN_RULES.md`, `.cursorrules`)
- **Componentes corrigidos**: 29 ocorrências em `globals.css`
- **Taxa de resolução**: 100% (29/29 corrigidas)

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

---

**Próximas Ações**:
- Monitorar conformidade de cores nos próximos PRs
- Criar script automatizado de verificação de conformidade
- Revisar outras diretrizes para garantir linguagem explícita e proibitiva

---

**Última Revisão**: 2025-01-20  
**Próxima Revisão Periódica**: 2025-04-20 (trimestral)
