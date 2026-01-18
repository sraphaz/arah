# DevPortal - Estrutura de Páginas Separadas

**Data**: 2025-01-20  
**Versão**: 1.0  
**Status**: 🟢 PROPOSTA - Estrutura modular para exploração progressiva

---

## 🎯 Objetivo

Criar uma estrutura de páginas separadas que permita ao usuário explorar o conteúdo com calma, mantendo os elementos fixos de navegação (header, sidebar) sempre visíveis.

---

## 📐 Estrutura Proposta

### Arquitetura de Navegação

```
┌─────────────────────────────────────────────────────────┐
│  HEADER (fixo)                                          │
│  - Logo/título                                          │
│  - Idioma, tema                                         │
│  - Link para araponga.app                               │
└─────────────────────────────────────────────────────────┘
┌──────────┬──────────────────────────────────────────────┐
│          │                                              │
│ SIDEBAR  │  CONTEÚDO PRINCIPAL (dinâmico)              │
│ (fixo)   │  - Carrega conteúdo da página/seção ativa   │
│          │  - Permite exploração progressiva            │
│ - Menu   │  - Sem scroll infinito                       │
│   hierárq│                                              │
│   uico   │                                              │
│          │                                              │
│ - Links  │                                              │
│   para   │                                              │
│   páginas│                                              │
└──────────┴──────────────────────────────────────────────┘
```

### Páginas Principais

1. **`/` (index.html)** - Homepage
   - Hero/Introdução
   - Navegação rápida
   - Links para seções principais

2. **`/comecando.html`** - Começando
   - Quickstart
   - Autenticação (JWT)
   - Território & Headers
   - Onboarding

3. **`/fundamentos.html`** - Fundamentos
   - Visão Geral
   - Como o Araponga funciona
   - Territórios
   - Conceitos de Produto
   - Modelo de Domínio

4. **`/api-pratica.html`** - API Prática
   - Fluxos Principais
   - Casos de Uso
   - OpenAPI / Explorer
   - Erros & Convenções

5. **`/funcionalidades.html`** - Funcionalidades
   - Marketplace
   - Payout & Gestão Financeira
   - Eventos
   - Admin & Filas

6. **`/avancado.html`** - Avançado
   - FAQ
   - Capacidades Técnicas
   - Roadmap
   - Contribuir
   - Versões

---

## 🔧 Implementação

### Opção 1: Arquivos HTML Separados (Recomendado)

Criar arquivos HTML separados que compartilham:
- Header fixo
- Sidebar fixa
- CSS/JS comum
- Apenas o conteúdo principal muda

**Vantagens:**
- ✅ URLs amigáveis (`/comecando.html`, `/api-pratica.html`)
- ✅ SEO melhor
- ✅ Compartilhamento direto de links
- ✅ Carregamento mais rápido (menos conteúdo por página)

**Desvantagens:**
- ⚠️ Precisa manter múltiplos arquivos sincronizados
- ⚠️ Header/sidebar duplicado em cada arquivo

### Opção 2: SPA Client-Side (Roteamento JavaScript)

Usar um único HTML com JavaScript que carrega conteúdo dinamicamente.

**Vantagens:**
- ✅ Navegação instantânea (sem reload)
- ✅ Um único arquivo para manter
- ✅ Header/sidebar sempre consistente

**Desvantagens:**
- ⚠️ URLs precisam de hash (`/#comecando`) ou `history.pushState`
- ⚠️ SEO pode ser pior (mas resolvível com SSR no futuro)

---

## 🎨 Design das Páginas

### Layout Consistente

Todas as páginas compartilham:
- **Header fixo** (sticky top)
- **Sidebar fixa** (sticky left)
- **Área de conteúdo** (margin-left para sidebar, max-width para legibilidade)

### Conteúdo Progressivo

Cada página deve:
- Ter uma **introdução curta** (2-3 parágrafos)
- Usar **accordions** para organizar conteúdo relacionado
- Incluir **exemplos práticos** (code blocks)
- Ter **links relacionados** ao final
- Permitir **navegação lateral** (sidebar) para seções dentro da página

---

## 📋 Estrutura de Arquivos Proposta

```
frontend/devportal/
├── index.html           # Homepage
├── comecando.html       # Começando
├── fundamentos.html     # Fundamentos
├── api-pratica.html     # API Prática
├── funcionalidades.html # Funcionalidades
├── avancado.html        # Avançado
├── assets/
│   ├── css/
│   │   └── devportal.css  # CSS compartilhado
│   ├── js/
│   │   └── devportal.js   # JS compartilhado
│   └── images/
└── openapi.json
```

---

## 🔗 Navegação

### Sidebar Fixa

A sidebar deve:
- Mostrar todas as páginas principais sempre visíveis
- Destacar a página ativa
- Ter submenus para seções dentro da página (accordions)

### Breadcrumbs (Opcional)

```
Home > API Prática > Fluxos Principais > Autenticação
```

### Links de Navegação

- **Entre páginas**: Links na sidebar e no footer
- **Dentro da página**: Scroll para seções (#anchors)
- **Relacionados**: Links ao final de cada seção

---

## ✅ Benefícios

1. **Exploração Progressiva**: Usuário pode focar em uma seção por vez
2. **Navegação Fixa**: Header e sidebar sempre acessíveis
3. **Carregamento Rápido**: Menos conteúdo por página
4. **URLs Amigáveis**: Fácil compartilhar links específicos
5. **SEO Melhor**: Cada página tem seu próprio título/descrição
6. **Manutenção**: Conteúdo organizado em arquivos lógicos

---

## 📝 Checklist de Implementação

### Fase 1: Estrutura Base
- [ ] Criar template HTML compartilhado (header + sidebar)
- [ ] Criar páginas separadas (comecando.html, fundamentos.html, etc.)
- [ ] Garantir CSS/JS funcionam em todas as páginas

### Fase 2: Navegação
- [ ] Implementar links entre páginas na sidebar
- [ ] Destacar página ativa na sidebar
- [ ] Adicionar breadcrumbs (opcional)

### Fase 3: Conteúdo
- [ ] Mover conteúdo de index.html para páginas específicas
- [ ] Organizar conteúdo em accordions por seção
- [ ] Adicionar introduções curtas em cada página

### Fase 4: Refinamentos
- [ ] Garantir links internos funcionam (#anchors)
- [ ] Adicionar links relacionados entre páginas
- [ ] Testar navegação em diferentes tamanhos de tela

---

**Status**: Proposta completa - Pronto para implementação
