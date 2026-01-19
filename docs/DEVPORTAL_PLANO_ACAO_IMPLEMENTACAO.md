# DevPortal - Plano de Ação de Implementação URGENTE

**Data**: 2025-01-20  
**Status**: 🔴 URGENTE  
**Confirmação de Estrutura**: Cenários Práticos em API Prática ✅

---

## ✅ Confirmação de Estrutura Final

### **FUNCIONALIDADES**
1. **Operações** (submenu): Métodos/endpoints com diagramas em páginas próprias

### **API PRÁTICA**
2. **Cenários Práticos** (submenu): 6 páginas dedicadas
3. **Guia de Produção** (submenu): Passos em páginas dedicadas
4. **Autenticação**: Página dedicada (já existe `#auth`)
5. **Contexto e Headers**: Página dedicada (já existe - linha 2409)
6. **OpenAPI**: Página dedicada (já existe `#openapi`)
7. **Erros & Convenções**: Página dedicada (já existe `#erros`)
8. **Casos de Uso Comuns**: Página dedicada (atualmente card - linha 895, 2309)
9. **Pontos de Atenção**: Página dedicada (atualmente card - linha 904, 2318)

### **RECURSOS**
10. **Configure seu Ambiente**: Ex-Quickstart (mover `#quickstart`)
11. **Onboarding Funcional**: Submenu (`#onboarding-analistas`)
12. **Onboarding Dev**: Submenu (`#onboarding-developers`)

---

## 🚀 Implementação Imediata - Ordem de Prioridade

### **ETAPA 1: Estrutura Base de Submenus (CRÍTICO)**

Criar CSS/JS para suportar submenus aninhados no sidebar:

```css
/* Estrutura de submenu */
.sidebar-submenu {
  margin-left: 1rem;
}

.sidebar-submenu-toggle {
  display: flex;
  align-items: center;
  width: 100%;
  padding: 0.5rem 1rem;
  /* ... */
}

.sidebar-submenu-items {
  list-style: none;
  margin: 0;
  padding: 0;
}

.sidebar-submenu-item {
  padding-left: 2rem;
}
```

### **ETAPA 2: Reorganizar Sidebar - API Prática**

**ANTES**:
```html
<ul class="sidebar-items" data-section-items="api">
  <li><a href="#modelo-dominio">Modelo de Domínio</a></li>
  <li><a href="#fluxos">Fluxos Principais</a></li>
  <li><a href="#casos-de-uso">Casos de Uso</a></li>
  <li><a href="#openapi">OpenAPI</a></li>
  <li><a href="#erros">Erros</a></li>
</ul>
```

**DEPOIS**:
```html
<ul class="sidebar-items" data-section-items="api">
  <li><a href="#modelo-dominio">Modelo de Domínio</a></li>
  <li><a href="#fluxos">Fluxos Principais</a></li>
  
  <!-- Submenu: Cenários Práticos -->
  <li class="sidebar-submenu">
    <button class="sidebar-submenu-toggle">Cenários Práticos</button>
    <ul class="sidebar-submenu-items">
      <li><a href="#cenario-onboarding-usuario">Onboarding Usuário</a></li>
      <li><a href="#cenario-publicar-midias">Publicar conteúdo com mídias</a></li>
      <li><a href="#cenario-assets">Assets territoriais</a></li>
      <li><a href="#cenario-chat">Chat territorial e grupos</a></li>
      <li><a href="#cenario-marketplace">Marketplace e economia local</a></li>
      <li><a href="#cenario-eventos">Eventos comunitários</a></li>
    </ul>
  </li>
  
  <!-- Submenu: Guia de Produção -->
  <li class="sidebar-submenu">
    <button class="sidebar-submenu-toggle">Guia de Produção</button>
    <ul class="sidebar-submenu-items">
      <li><a href="#guia-producao-passo-1">Passo 1: Entendendo o Fluxo</a></li>
      <li><a href="#guia-producao-passo-2">Passo 2: Configurar Payout</a></li>
      <!-- ... mais passos ... -->
    </ul>
  </li>
  
  <li><a href="#auth">Autenticação</a></li>
  <li><a href="#contexto-headers">Contexto e Headers</a></li>
  <li><a href="#openapi">OpenAPI</a></li>
  <li><a href="#erros">Erros & Convenções</a></li>
  <li><a href="#casos-uso-comuns">Casos de Uso Comuns</a></li>
  <li><a href="#pontos-atencao">Pontos de Atenção</a></li>
</ul>
```

### **ETAPA 3: Reorganizar Sidebar - Funcionalidades**

**DEPOIS**:
```html
<ul class="sidebar-items" data-section-items="features">
  <!-- Submenu: Operações -->
  <li class="sidebar-submenu">
    <button class="sidebar-submenu-toggle">Operações</button>
    <ul class="sidebar-submenu-items">
      <!-- Criar páginas dedicadas para cada método com diagrama -->
      <li><a href="#operacao-auth">Autenticação Social</a></li>
      <li><a href="#operacao-territory-discovery">Descoberta de Território</a></li>
      <li><a href="#operacao-marketplace-checkout">Marketplace Checkout</a></li>
      <!-- ... mais operações ... -->
    </ul>
  </li>
  
  <!-- Manter Marketplace, Eventos, etc como estão? OU mover para Operações? -->
  <!-- Confirmar com usuário -->
</ul>
```

### **ETAPA 4: Criar Páginas Dedicadas - Cenários Práticos**

Cada card de `#casos-de-uso` (linhas 1996-2078) vira uma seção/página dedicada:

1. `#cenario-onboarding-usuario` - Extrair conteúdo do card "🚀 Onboarding completo"
2. `#cenario-publicar-midias` - Extrair conteúdo do card "📝 Publicar conteúdo com mídias"
3. `#cenario-assets` - Extrair conteúdo do card "🗺️ Assets territoriais"
4. `#cenario-chat` - Extrair conteúdo do card "💬 Chat territorial"
5. `#cenario-marketplace` - Extrair conteúdo do card "🏪 Marketplace"
6. `#cenario-eventos` - Extrair conteúdo do card "📅 Eventos comunitários"

### **ETAPA 5: Criar Páginas Dedicadas - Casos de Uso Comuns e Pontos de Atenção**

**ANTES**: Cards dentro de `#payout-gestao-financeira` (linhas 895, 904, 2309, 2318)

**DEPOIS**: 
- `#casos-uso-comuns` - Nova seção dedicada (extrair cards)
- `#pontos-atencao` - Nova seção dedicada (extrair cards)

### **ETAPA 6: Reorganizar Recursos**

**ANTES**:
```html
<ul class="sidebar-items" data-section-items="recursos">
  <li><a href="#capacidades-tecnicas">Capacidades Técnicas</a></li>
  <li><a href="#versoes">Versões</a></li>
  <li><a href="#roadmap">Roadmap</a></li>
  <li><a href="#contribuir">Contribuir</a></li>
</ul>
```

**DEPOIS**:
```html
<ul class="sidebar-items" data-section-items="recursos">
  <li><a href="#configure-ambiente">Configure seu Ambiente</a></li>
  
  <!-- Submenu: Onboarding Funcional -->
  <li class="sidebar-submenu">
    <button class="sidebar-submenu-toggle">Onboarding Funcional</button>
    <ul class="sidebar-submenu-items">
      <!-- Dividir #onboarding-analistas em partes -->
      <li><a href="#onboarding-funcional-parte-1">Parte 1: Introdução</a></li>
      <li><a href="#onboarding-funcional-parte-2">Parte 2: Configuração</a></li>
      <!-- ... mais partes ... -->
    </ul>
  </li>
  
  <!-- Submenu: Onboarding Dev -->
  <li class="sidebar-submenu">
    <button class="sidebar-submenu-toggle">Onboarding Dev</button>
    <ul class="sidebar-submenu-items">
      <!-- Dividir #onboarding-developers em partes -->
      <li><a href="#onboarding-dev-parte-1">Parte 1: Ambiente</a></li>
      <!-- ... mais partes ... -->
    </ul>
  </li>
  
  <li><a href="#capacidades-tecnicas">Capacidades Técnicas</a></li>
  <li><a href="#versoes">Versões</a></li>
  <li><a href="#roadmap">Roadmap</a></li>
  <li><a href="#contribuir">Contribuir</a></li>
</ul>
```

---

## 📝 Notas Importantes

### **IDs a Criar/Reutilizar**

- ✅ `#auth` - Já existe (linha 2338)
- ✅ `#openapi` - Já existe (linha 2442)
- ✅ `#erros` - Já existe (linha 2460)
- ✅ Contexto e Headers - Já existe (linha 2409) - **preciso verificar ID exato**
- ✅ `#quickstart` - Já existe (linha 2490) - **renomear para `#configure-ambiente`**
- ✅ `#onboarding-analistas` - Já existe (linha 2539)
- ✅ `#onboarding-developers` - Já existe (linha 2768)
- ➕ `#casos-uso-comuns` - **CRIAR** (extrair de cards)
- ➕ `#pontos-atencao` - **CRIAR** (extrair de cards)
- ➕ `#cenario-*` - **CRIAR 6 páginas** (extrair de cards de `#casos-de-uso`)
- ➕ `#operacao-*` - **CRIAR páginas** para cada método com diagrama
- ➕ `#guia-producao-passo-*` - **CRIAR páginas** para cada passo

---

## ⚠️ Decisões Pendentes

1. **Operações em Funcionalidades**: Manter Marketplace/Eventos/Payout em Funcionalidades ou mover tudo para "Operações"?
2. **Divisão de Onboarding**: Como dividir `#onboarding-analistas` e `#onboarding-developers` em partes? (por seção, por passo, etc?)
3. **Divisão de Guia de Produção**: Como dividir em passos? (identificar seções existentes)

---

**Status**: Pronto para implementação após confirmação de detalhes  
**Próximo passo**: Implementar Estrutura Base de Submenus (ETAPA 1)
