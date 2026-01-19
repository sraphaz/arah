# DevPortal - Reestruturação em Arquivos Separados

**Data**: 2025-01-20  
**Versão**: 1.0  
**Status**: 🟢 PROPOSTA - Estrutura proposta para facilitar manutenção

---

## 🎯 Objetivo

Dividir o conteúdo do DevPortal em arquivos separados (uma página por seção) e criar um sistema de roteamento real, facilitando a manutenção e organização do código.

---

## 📁 Estrutura de Arquivos Proposta

```
frontend/devportal/
├── index.html              # Container base (header, sidebar, footer)
├── assets/
│   ├── js/
│   │   ├── router.js       # Roteador que carrega páginas dinamicamente
│   │   ├── devportal.js    # Scripts gerais (mantém)
│   │   └── toc.js          # TOC dinâmico (mantém)
│   └── css/                # Estilos (mantém)
└── pages/                  # NOVO - Páginas separadas
    ├── comecando.html      # Página inicial / Quickstart
    ├── fundamentos/
    │   ├── visao-geral.html
    │   ├── como-funciona.html
    │   ├── territorios.html
    │   ├── conceitos.html
    │   └── modelo-dominio.html
    ├── api-pratica/
    │   ├── fluxos.html
    │   ├── casos-de-uso.html
    │   ├── auth.html
    │   ├── territory-session.html
    │   ├── openapi.html
    │   ├── erros.html
    │   └── quickstart.html
    ├── funcionalidades/
    │   ├── marketplace.html
    │   ├── eventos.html
    │   ├── payout-gestao-financeira.html
    │   └── admin.html
    └── avancado/
        ├── capacidades-tecnicas.html
        ├── versoes.html
        ├── roadmap.html
        └── contribuir.html
```

---

## 🔄 Sistema de Roteamento

### Estrutura do `index.html` (Container Base)

O `index.html` será um shell que contém apenas:
- `<head>` (metas, CSS)
- `<header>` (fixo)
- `<nav>` (sidebar fixa)
- `<main id="page-content">` (conteúdo dinâmico)
- `<footer>` (fixo)
- Scripts (router.js, devportal.js, etc.)

### Funcionamento do Router

O `router.js` carregará páginas dinamicamente via `fetch()`:

```javascript
// Exemplo simplificado
const router = {
  routes: {
    '/comecando': './pages/comecando.html',
    '/fundamentos/visao-geral': './pages/fundamentos/visao-geral.html',
    // ...
  },
  
  async loadPage(path) {
    const html = await fetch(this.routes[path]).then(r => r.text());
    document.getElementById('page-content').innerHTML = html;
  }
};
```

### URLs Amigáveis

- `#/comecando` → `pages/comecando.html`
- `#/fundamentos/visao-geral` → `pages/fundamentos/visao-geral.html`
- `#/api-pratica/fluxos` → `pages/api-pratica/fluxos.html`

---

## ✅ Benefícios

1. **Manutenção mais fácil**: Cada página em seu próprio arquivo
2. **Navegação real**: URLs amigáveis e compartilháveis
3. **Carregamento otimizado**: Carrega apenas o conteúdo necessário
4. **Organização clara**: Estrutura de pastas reflete a hierarquia
5. **Reutilização**: Header, sidebar e footer compartilhados
6. **Escalabilidade**: Fácil adicionar novas páginas

---

## 🛠️ Implementação

### Passo 1: Criar Estrutura de Pastas
```bash
mkdir -p pages/fundamentos pages/api-pratica pages/funcionalidades pages/avancado
```

### Passo 2: Extrair Conteúdo
- Mover cada seção do `index.html` atual para seu próprio arquivo
- Manter apenas estrutura base no `index.html`

### Passo 3: Atualizar Router
- Implementar carregamento dinâmico via `fetch()`
- Atualizar links da sidebar para usar rotas (`#/fundamentos/visao-geral`)

### Passo 4: Testar
- Verificar que todas as páginas carregam corretamente
- Validar que a navegação funciona
- Testar em diferentes navegadores

---

## 📝 Exemplo de Estrutura de Página

### `pages/fundamentos/visao-geral.html`

```html
<section class="section" id="visao-geral">
  <span class="eyebrow" data-i18n="overview.eyebrow">Visão geral</span>
  <div>
    <h2 data-i18n="overview.title">API orientada a território...</h2>
    <p data-i18n="overview.lead">...</p>
  </div>
  <!-- resto do conteúdo -->
</section>
```

### `index.html` (simplificado)

```html
<!doctype html>
<html lang="pt-BR">
<head>
  <!-- metas, CSS -->
</head>
<body>
  <header>...</header>
  <nav class="sidebar-container">...</nav>
  <main id="page-content">
    <!-- Conteúdo carregado dinamicamente aqui -->
  </main>
  <footer>...</footer>
  <script src="./assets/js/router.js"></script>
  <script src="./assets/js/devportal.js"></script>
</body>
</html>
```

---

## 🚀 Próximos Passos

1. ✅ Criar estrutura de pastas
2. ⏳ Extrair conteúdo em arquivos separados
3. ⏳ Implementar router com `fetch()`
4. ⏳ Atualizar links da sidebar
5. ⏳ Testar navegação

---

## 📚 Referências

- Sistema de roteamento similar: Vue Router, React Router (inspiração para conceito)
- Carregamento dinâmico: `fetch()` API nativa do navegador
- Hash routing: Compatível com servidor estático (sem backend)
