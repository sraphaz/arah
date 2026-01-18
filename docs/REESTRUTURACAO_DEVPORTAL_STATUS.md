# Status da Reestruturação do DevPortal

**Data**: 2025-01-20  
**Branch**: `feat/devportal-restruturacao-navegacao`  
**Status**: 🚧 Em Progresso

---

## ✅ Preparação Concluída

1. ✅ CSS para tabs/panels/accordions implementado
2. ✅ JavaScript para interatividade implementado
3. ✅ Mapeamento de conteúdo documentado
4. ✅ Plano de implementação criado
5. ✅ Onboarding técnico criado e mapeado

---

## 🚧 Implementação em Progresso

### **Etapa 1: Estrutura Base de Tabs/Panels** (Em Progresso)

**Ação necessária**: Adicionar Phase Tabs e Panels logo após `<main role="main">` (linha 171)

**Estrutura HTML a ser adicionada:**
```html
<main role="main">
  <!-- Phase Navigation Tabs -->
  <div class="phase-navigation" role="tablist">
    <button class="phase-tab active" data-phase="comecando">🚀 Começando</button>
    <button class="phase-tab" data-phase="fundamentos">📚 Fundamentos</button>
    <button class="phase-tab" data-phase="api-pratica">🔧 API Prática</button>
    <button class="phase-tab" data-phase="funcionalidades">⚙️ Funcionalidades</button>
    <button class="phase-tab" data-phase="avancado">🎓 Avançado</button>
  </div>

  <!-- Phase Panels Container -->
  <div class="phase-panels">
    <!-- Panels serão adicionados aqui -->
  </div>
</main>
```

---

## ⏳ Próximas Etapas

### **Etapa 2: Tab 1 - Começando**

Mover estas seções para dentro de `.phase-panel[data-phase-panel="comecando"]`:

- `#quickstart` (linha ~1763)
- `#auth` (linha ~1482)
- `#territory-session` (linha ~1552)
- `#onboarding-analistas` (linha ~1812)
- `#onboarding-developers` (linha ~2041)

**Conversão**: Cada seção deve virar um `.section-accordion` com `.section-accordion-header` e `.section-accordion-content`.

### **Etapa 3: Tab 2 - Fundamentos**

Mover estas seções para dentro de `.phase-panel[data-phase-panel="fundamentos"]`:

- `#visao-geral` (linha ~172) - **expandido por padrão**
- `#como-funciona` (linha ~201)
- `#territorios` (linha ~242)
- `#conceitos` (linha ~270)
- `#modelo-dominio` (linha ~322)

### **Etapa 4: Tab 3 - API Prática**

Mover estas seções para dentro de `.phase-panel[data-phase-panel="api-pratica"]`:

- `#fluxos` (linha ~551)
- `#casos-de-uso` (linha ~931)
- `#openapi` (linha ~1715) - **side panel button**
- `#erros` (linha ~1733)

### **Etapa 5: Tab 4 - Funcionalidades**

Mover estas seções para dentro de `.phase-panel[data-phase-panel="funcionalidades"]`:

- `#marketplace` (linha ~1027)
- `#payout-gestao-financeira` (linha ~1148)
- `#eventos` (linha ~1420)
- `#admin` (linha ~1586)

### **Etapa 6: Tab 5 - Avançado**

Mover estas seções para dentro de `.phase-panel[data-phase-panel="avancado"]`:

- `#capacidades-tecnicas` (linha ~2347)
- `#versoes` (linha ~2664)
- `#roadmap` (linha ~2475)
- `#contribuir` (linha ~2583)

---

## 📊 Mapeamento de Seções

| ID | Título | Tab | Tipo | Linha |
|----|--------|-----|------|-------|
| `#quickstart` | Quickstart | 1 | Accordion | ~1763 |
| `#auth` | Autenticação | 1 | Accordion | ~1482 |
| `#territory-session` | Território & Headers | 1 | Accordion | ~1552 |
| `#onboarding-analistas` | Onboarding Analistas | 1 | Accordion | ~1812 |
| `#onboarding-developers` | Onboarding Developers | 1 | Accordion | ~2041 |
| `#visao-geral` | Visão Geral | 2 | Expandido | ~172 |
| `#como-funciona` | Como o Araponga funciona | 2 | Accordion | ~201 |
| `#territorios` | Territórios | 2 | Accordion | ~242 |
| `#conceitos` | Conceitos de produto | 2 | Accordion | ~270 |
| `#modelo-dominio` | Modelo de domínio | 2 | Accordion | ~322 |
| `#fluxos` | Fluxos principais | 3 | Accordion | ~551 |
| `#casos-de-uso` | Casos de uso | 3 | Accordion | ~931 |
| `#openapi` | OpenAPI / Explorer | 3 | Side Panel | ~1715 |
| `#erros` | Erros & convenções | 3 | Accordion | ~1733 |
| `#marketplace` | Marketplace | 4 | Accordion | ~1027 |
| `#payout-gestao-financeira` | Payout & Gestão Financeira | 4 | Accordion | ~1148 |
| `#eventos` | Eventos | 4 | Accordion | ~1420 |
| `#admin` | Admin & filas | 4 | Accordion | ~1586 |
| `#capacidades-tecnicas` | Capacidades técnicas | 5 | Accordion | ~2347 |
| `#versoes` | Versões & compatibilidade | 5 | Accordion | ~2664 |
| `#roadmap` | Roadmap | 5 | Accordion | ~2475 |
| `#contribuir` | Contribuir | 5 | Accordion | ~2583 |

**Total**: 22 seções para reorganizar

---

## ⚠️ Complexidade da Tarefa

- **Arquivo HTML**: ~2795 linhas
- **Seções a reorganizar**: 22
- **Reorganização**: Movimentação de grandes blocos de HTML
- **Estrutura**: Conversão de `<section>` para `.section-accordion`

**Estratégia**: Implementação incremental, testando após cada etapa.

---

## 🎯 Próximo Passo Imediato

**Adicionar estrutura base de tabs/panels** logo após `<main role="main">` (linha 171).

---

**Última Atualização**: 2025-01-20
