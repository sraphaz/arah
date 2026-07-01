# Plano de Implementação da Reestruturação do DevPortal

**Data**: 2025-01-20  
**Status**: Em Progresso  
**Arquivo**: `frontend/devportal/index.html` (~2795 linhas)

---

## 📋 Estratégia de Implementação

Devido ao tamanho do arquivo (~2795 linhas), a reestruturação será feita em **etapas incrementais**:

### **Etapa 1: Estrutura Base** ✅ (Em Progresso)
- Adicionar Phase Tabs no início do `<main>`
- Criar Phase Panels vazios para cada tab
- Validar CSS e JavaScript já existentes

### **Etapa 2: Tab 1 - Começando** (Pendente)
- Mover `#quickstart` para accordion
- Mover `#auth` para accordion
- Mover `#territory-session` para accordion
- Mover `#onboarding-analistas` para accordion
- Mover `#onboarding-developers` para accordion

### **Etapa 3: Tab 2 - Fundamentos** (Pendente)
- Mover `#visao-geral` (expandido por padrão)
- Mover `#como-funciona` para accordion
- Mover `#territorios` para accordion
- Mover `#conceitos` para accordion
- Mover `#modelo-dominio` para accordion (com cards como expandible details)

### **Etapa 4: Tab 3 - API Prática** (Pendente)
- Mover `#fluxos` para accordion (com fluxos como expandible details)
- Mover `#casos-de-uso` para accordion (com casos como expandible details)
- Mover `#openapi` para side panel button
- Mover `#erros` para accordion

### **Etapa 5: Tab 4 - Funcionalidades** (Pendente)
- Mover `#marketplace` para accordion
- Mover `#payout-gestao-financeira` para accordion
- Mover `#eventos` para accordion
- Mover `#admin` para accordion

### **Etapa 6: Tab 5 - Avançado** (Pendente)
- Mover `#capacidades-tecnicas` para accordion
- Mover `#versoes` para accordion
- Mover `#roadmap` para accordion
- Mover `#contribuir` para accordion

### **Etapa 7: Validação** (Pendente)
- Testar navegação entre tabs
- Testar accordions (abrir/fechar)
- Testar expandible details
- Testar side panels
- Validar links do sidebar

---

## 🗺️ Mapeamento de Seções para Tabs

| Seção ID | Título | Tab | Tipo |
|----------|--------|-----|------|
| `#quickstart` | Quickstart | Tab 1 | Accordion |
| `#auth` | Autenticação | Tab 1 | Accordion |
| `#territory-session` | Território & Headers | Tab 1 | Accordion |
| `#onboarding-analistas` | Onboarding Analistas | Tab 1 | Accordion |
| `#onboarding-developers` | Onboarding Developers | Tab 1 | Accordion |
| `#visao-geral` | Visão Geral | Tab 2 | Expandido |
| `#como-funciona` | Como o Arah funciona | Tab 2 | Accordion |
| `#territorios` | Territórios | Tab 2 | Accordion |
| `#conceitos` | Conceitos de produto | Tab 2 | Accordion |
| `#modelo-dominio` | Modelo de domínio | Tab 2 | Accordion + Details |
| `#fluxos` | Fluxos principais | Tab 3 | Accordion + Details |
| `#casos-de-uso` | Casos de uso | Tab 3 | Accordion + Details |
| `#openapi` | OpenAPI / Explorer | Tab 3 | Side Panel |
| `#erros` | Erros & convenções | Tab 3 | Accordion |
| `#marketplace` | Marketplace | Tab 4 | Accordion |
| `#payout-gestao-financeira` | Payout & Gestão Financeira | Tab 4 | Accordion |
| `#eventos` | Eventos | Tab 4 | Accordion |
| `#admin` | Admin & filas | Tab 4 | Accordion |
| `#capacidades-tecnicas` | Capacidades técnicas | Tab 5 | Accordion |
| `#versoes` | Versões & compatibilidade | Tab 5 | Accordion |
| `#roadmap` | Roadmap | Tab 5 | Accordion |
| `#contribuir` | Contribuir | Tab 5 | Accordion |

---

## 🛠️ Estrutura HTML Proposta

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

  <!-- Phase Panels -->
  <div class="phase-panels">
    <!-- Tab 1: Começando -->
    <div class="phase-panel active" data-phase-panel="comecando">
      <!-- Accordions aqui -->
    </div>
    
    <!-- Tab 2: Fundamentos -->
    <div class="phase-panel" data-phase-panel="fundamentos">
      <!-- Conteúdo aqui -->
    </div>
    
    <!-- ... outras tabs ... -->
  </div>
</main>
```

---

## 📝 Checklist de Implementação

### Estrutura Base
- [ ] Adicionar `.phase-navigation` após `<main role="main">`
- [ ] Adicionar `.phase-panels` container
- [ ] Criar 5 `.phase-panel` vazios
- [ ] Validar CSS e JavaScript já funcionam

### Tab 1: Começando
- [ ] Mover `#quickstart` para accordion
- [ ] Mover `#auth` para accordion
- [ ] Mover `#territory-session` para accordion
- [ ] Mover `#onboarding-analistas` para accordion
- [ ] Mover `#onboarding-developers` para accordion

### Tab 2: Fundamentos
- [ ] Mover `#visao-geral` (expandido)
- [ ] Mover `#como-funciona` para accordion
- [ ] Mover `#territorios` para accordion
- [ ] Mover `#conceitos` para accordion
- [ ] Mover `#modelo-dominio` para accordion

### Tab 3: API Prática
- [ ] Mover `#fluxos` para accordion
- [ ] Mover `#casos-de-uso` para accordion
- [ ] Configurar `#openapi` como side panel
- [ ] Mover `#erros` para accordion

### Tab 4: Funcionalidades
- [ ] Mover `#marketplace` para accordion
- [ ] Mover `#payout-gestao-financeira` para accordion
- [ ] Mover `#eventos` para accordion
- [ ] Mover `#admin` para accordion

### Tab 5: Avançado
- [ ] Mover `#capacidades-tecnicas` para accordion
- [ ] Mover `#versoes` para accordion
- [ ] Mover `#roadmap` para accordion
- [ ] Mover `#contribuir` para accordion

### Validação Final
- [ ] Testar navegação entre tabs
- [ ] Testar accordions
- [ ] Testar expandible details
- [ ] Testar side panels
- [ ] Validar links do sidebar
- [ ] Testar em diferentes navegadores

---

## ⚠️ Notas Importantes

1. **CSS e JavaScript já estão prontos** - Não precisa criar novos arquivos
2. **Backup recomendado** - Fazer backup antes de grandes mudanças
3. **Testes incrementais** - Testar após cada etapa
4. **Sidebar links** - Podem precisar atualização para funcionar com tabs
5. **IDs das seções** - Manter os mesmos IDs para compatibilidade

---

**Última Atualização**: 2025-01-20
