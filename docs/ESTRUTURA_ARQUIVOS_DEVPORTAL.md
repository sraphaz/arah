# Estrutura de Arquivos - DevPortal Refatorado

## 🎯 Objetivo
Separar conteúdo em arquivos HTML individuais para melhor organização, manutenção e performance.

---

## 📁 Estrutura Proposta

```
frontend/devportal/
├── index.html                    # Shell principal (header, sidebar, footer)
├── pages/                        # Páginas de conteúdo
│   ├── home.html                 # Homepage
│   ├── comecando/
│   │   ├── index.html            # Overview do "Começando"
│   │   ├── quickstart.html
│   │   ├── auth.html
│   │   └── territory-session.html
│   ├── fundamentos/
│   │   ├── index.html            # Overview do "Fundamentos"
│   │   ├── visao-geral.html
│   │   ├── como-funciona.html
│   │   ├── territorios.html
│   │   └── conceitos.html
│   ├── funcionalidades/
│   │   ├── index.html            # Overview do "Funcionalidades"
│   │   ├── marketplace.html
│   │   ├── payout.html
│   │   ├── eventos.html
│   │   └── admin.html
│   ├── api-pratica/
│   │   ├── index.html            # Overview do "API Prática"
│   │   ├── fluxos.html
│   │   └── casos-de-uso.html
│   └── avancado/
│       ├── index.html            # Overview do "Avançado"
│       ├── roadmap.html
│       ├── contribuir.html
│       └── configuracao.html
├── assets/
│   ├── css/
│   ├── js/
│   ├── images/
│   └── icons/                    # Nova pasta para ícones SVG
└── components/                   # Componentes reutilizáveis (opcional)
    ├── hero-section.html
    └── content-card.html
```

---

## 🔄 Migração do Sistema Atual

### Fase 1: Preparação

**1. Criar estrutura de pastas**
```bash
mkdir -p frontend/devportal/pages/{comecando,fundamentos,funcionalidades,api-pratica,avancado}
mkdir -p frontend/devportal/assets/icons
```

**2. Extrair conteúdo dos phase-panels**

Cada phase-panel atual vira um arquivo HTML:

```html
<!-- pages/comecando/index.html -->
<section class="section" id="introducao">
  <h2>Bem-vindo ao Developer Portal</h2>
  <p class="lead-text">
    Este é o portal técnico da plataforma Araponga...
  </p>
</section>

<section class="section" id="quickstart">
  <!-- Conteúdo do quickstart -->
</section>
```

---

## 🔧 Atualização do Router

### Router.js Atualizado

```javascript
_fetchContent: function(route) {
  // Se for homepage
  if (route === 'home' || route === '') {
    return this._fetchHTML('pages/home.html');
  }

  // Parse da rota (ex: "funcionalidades/marketplace")
  const parts = route.split('/');
  const phase = parts[0]; // "funcionalidades"
  const subRoute = parts[1] || 'index'; // "marketplace" ou "index"

  // Caminho do arquivo
  const filePath = `pages/${phase}/${subRoute}.html`;

  return this._fetchHTML(filePath);
},

_fetchHTML: function(filePath) {
  return fetch(filePath)
    .then(response => {
      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }
      return response.text();
    })
    .catch(error => {
      console.error('Erro ao carregar:', filePath, error);
      // Fallback: tenta conteúdo inline se disponível
      return this._getInlineContent(filePath) || '<div class="error">Conteúdo não encontrado</div>';
    });
}
```

---

## 📝 Estrutura de Cada Arquivo HTML

### Template de Página

```html
<!-- pages/funcionalidades/marketplace.html -->
<section class="section-hero" id="marketplace">
  <div class="hero-content">
    <span class="eyebrow">Marketplace</span>
    <h1>Economia Local Territorial</h1>
    <p class="hero-lead">
      Crie lojas, publique produtos e gerencie vendas com payout territorial.
    </p>
  </div>
</section>

<section class="section" id="marketplace-overview">
  <!-- Conteúdo principal -->
</section>

<section class="section" id="marketplace-api">
  <!-- Referência de API -->
</section>
```

**Nota**: Não incluir `<html>`, `<head>`, `<body>` - apenas o conteúdo que será injetado no `#page-content`.

---

## ✅ Vantagens da Separação

### 1. Manutenibilidade
- ✅ Arquivos menores e focados (~200-500 linhas vs 3800)
- ✅ Fácil localizar e editar conteúdo específico
- ✅ Menos conflitos em merge (Git)
- ✅ Histórico de mudanças mais claro

### 2. Performance
- ✅ Carregamento sob demanda (lazy loading)
- ✅ Cache de arquivos individuais
- ✅ Menor payload inicial
- ✅ Melhor para CDN

### 3. SEO
- ✅ URLs dedicadas por página
- ✅ Meta tags específicas por conteúdo
- ✅ Melhor indexação
- ✅ Compartilhamento de links específicos

### 4. Desenvolvimento
- ✅ Colaboração paralela mais fácil
- ✅ Testes unitários por arquivo
- ✅ Reutilização de componentes
- ✅ Build process mais simples

### 5. Escalabilidade
- ✅ Fácil adicionar novas páginas
- ✅ Estrutura clara e previsível
- ✅ Suporta internacionalização (i18n)
- ✅ Preparado para SSG (Static Site Generation)

---

## 🔄 Estratégia de Migração

### Opção 1: Migração Gradual (Recomendada)

**Passo 1**: Criar estrutura de pastas e mover um phase-panel por vez
- Começar com "Começando" (menor)
- Testar router com arquivo externo
- Validar funcionamento

**Passo 2**: Migrar phase-panels restantes
- Um por vez, mantendo fallback inline
- Testar após cada migração

**Passo 3**: Remover phase-panels do index.html
- Limpar HTML principal
- Manter apenas shell (header, sidebar, footer)

### Opção 2: Migração Completa

**Passo 1**: Extrair todo conteúdo de uma vez
- Script para extrair phase-panels
- Criar arquivos automaticamente

**Passo 2**: Atualizar router
- Implementar fetch de arquivos
- Manter fallback para desenvolvimento local

**Passo 3**: Testar e ajustar
- Validar todas as rotas
- Corrigir links internos
- Ajustar navegação

---

## 🛠️ Script de Extração (Opcional)

```javascript
// scripts/extract-phase-panels.js
const fs = require('fs');
const path = require('path');
const { JSDOM } = require('jsdom');

const htmlPath = path.join(__dirname, '../index.html');
const htmlContent = fs.readFileSync(htmlPath, 'utf-8');
const dom = new JSDOM(htmlContent);
const document = dom.window.document;

const phasePanels = document.querySelectorAll('.phase-panel');

phasePanels.forEach(panel => {
  const phase = panel.getAttribute('data-phase-panel');
  const content = panel.innerHTML;
  
  // Criar diretório se não existir
  const dirPath = path.join(__dirname, '../pages', phase);
  if (!fs.existsSync(dirPath)) {
    fs.mkdirSync(dirPath, { recursive: true });
  }
  
  // Salvar como index.html
  const filePath = path.join(dirPath, 'index.html');
  fs.writeFileSync(filePath, content, 'utf-8');
  
  console.log(`✅ Extraído: ${phase} → ${filePath}`);
});
```

---

## 🔗 Atualização de Links

### Links Internos

**Antes** (dentro do mesmo arquivo):
```html
<a href="#marketplace">Marketplace</a>
```

**Depois** (entre arquivos):
```html
<a href="#/funcionalidades/marketplace">Marketplace</a>
```

### Links do Sidebar

**Atualizar** `devportal.js` para usar hash routing:
```javascript
// Mapeamento de seções para rotas
const sectionToRoute = {
  'marketplace': '/funcionalidades/marketplace',
  'payout-gestao-financeira': '/funcionalidades/payout',
  'eventos': '/funcionalidades/eventos',
  // ...
};
```

---

## 📊 Comparação: Antes vs Depois

| Aspecto | Antes | Depois |
|---------|-------|--------|
| **Arquivos HTML** | 1 arquivo (3800 linhas) | 20+ arquivos (~200-500 linhas cada) |
| **Tamanho médio** | 3800 linhas | 300 linhas |
| **Manutenção** | Difícil (buscar em arquivo grande) | Fácil (arquivo específico) |
| **Carregamento** | Tudo de uma vez | Sob demanda |
| **Cache** | Tudo ou nada | Por página |
| **SEO** | 1 URL | 20+ URLs |
| **Colaboração** | Conflitos frequentes | Conflitos raros |
| **Testes** | Testar tudo junto | Testar por página |

---

## 🚀 Implementação no Plano de Ação

### Atualizar Fase 2.1

**Título**: "Separar Conteúdo em Arquivos HTML Individuais"

**Ações**:
1. Criar estrutura de pastas `pages/`
2. Extrair phase-panels para arquivos separados
3. Atualizar router.js para fetch de arquivos
4. Atualizar links internos
5. Testar todas as rotas
6. Remover phase-panels do index.html

**Prioridade**: 🔴 Alta  
**Esforço**: 5-7 dias  
**Impacto**: Muito Alto (melhora manutenibilidade, performance, SEO)

---

## ⚠️ Considerações

### CORS em Desenvolvimento Local

**Problema**: `file://` protocol não permite fetch de arquivos locais

**Soluções**:
1. **Servidor local** (recomendado)
   ```bash
   # Python
   python -m http.server 8000
   
   # Node.js
   npx serve .
   
   # PHP
   php -S localhost:8000
   ```

2. **Fallback inline** (temporário)
   - Router tenta fetch primeiro
   - Se falhar, usa conteúdo inline do index.html
   - Mantém compatibilidade durante desenvolvimento

3. **Build process**
   - Em produção, arquivos são servidos via HTTP
   - CORS não é problema

### Compatibilidade com Deploy Atual

- ✅ Funciona com GitHub Pages
- ✅ Funciona com qualquer servidor estático
- ✅ Não requer backend
- ✅ Mantém SPA (Single Page Application)

---

**Próximo Passo**: Atualizar plano de ação principal para incluir separação de arquivos como prioridade alta.
