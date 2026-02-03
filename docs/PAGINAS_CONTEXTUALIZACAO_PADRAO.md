# Padrão de Páginas de Contextualização - DevPortal

## 🎯 Objetivo
Criar páginas de contextualização que introduzam cada temática antes de mergulhar nos detalhes técnicos.

---

## 📋 Estrutura Padrão

Cada categoria/temática deve ter:

1. **Página de Contextualização** (`index.html`) - Visão geral da temática
2. **Páginas Específicas** (`[topico].html`) - Detalhes técnicos

---

## 🏗️ Template: Página de Contextualização

### Exemplo: `pages/funcionalidades/index.html`

```html
<!-- 1. HERO - Apresentação da Temática -->
<section class="page-hero">
  <div class="hero-content">
    <span class="eyebrow">Funcionalidades</span>
    <h1>Funcionalidades da Plataforma Araponga</h1>
    <p class="hero-lead">
      Conjunto de funcionalidades que permitem comunidades gerenciarem economia local,
      eventos territoriais, comunicação e governança através de ferramentas técnicas
      que respeitam autonomia e soberania territorial.
    </p>
  </div>
  <div class="hero-visual">
    <svg class="hero-illustration" viewBox="0 0 400 300">
      <!-- Ilustração SVG simples representando funcionalidades -->
    </svg>
  </div>
</section>

<!-- 2. VISÃO GERAL - Por que existe? -->
<section class="section section-overview">
  <h2>Por que essas funcionalidades?</h2>
  <p class="lead-text">
    As funcionalidades do Araponga foram projetadas para fortalecer autonomia territorial,
    economia circular local e organização comunitária. Cada funcionalidade mantém recursos
    e decisões no território, respeitando soberania local e evitando dependência externa.
  </p>
  
  <div class="content-grid">
    <div class="content-card">
      <div class="card-header">
        <svg class="icon icon-store" width="24" height="24">...</svg>
        <h3>Economia Local</h3>
      </div>
      <p>
        Marketplace e payout territorial garantem que recursos financeiros permaneçam
        na comunidade, fortalecendo ciclos econômicos locais.
      </p>
    </div>
    
    <div class="content-card">
      <div class="card-header">
        <svg class="icon icon-calendar" width="24" height="24">...</svg>
        <h3>Organização Territorial</h3>
      </div>
      <p>
        Eventos e feed organizam comunicação e atividades locais, mantendo contexto
        geográfico como primeira classe.
      </p>
    </div>
    
    <div class="content-card">
      <div class="card-header">
        <svg class="icon icon-shield" width="24" height="24">...</svg>
        <h3>Governança Comunitária</h3>
      </div>
      <p>
        Moderação e filas permitem decisões comunitárias transparentes e auditáveis,
        com autonomia territorial.
      </p>
    </div>
  </div>
</section>

<!-- 3. NAVEGAÇÃO - O que você vai encontrar -->
<section class="section section-navigation">
  <h2>Explore as Funcionalidades</h2>
  <p class="lead-text">
    Escolha uma funcionalidade abaixo para ver documentação completa, exemplos de API
    e guias práticos de implementação.
  </p>
  
  <div class="content-grid">
    <a href="#/funcionalidades/marketplace" class="content-card card-link">
      <div class="card-header">
        <svg class="icon icon-store">...</svg>
        <h3>Marketplace</h3>
      </div>
      <p class="card-summary">
        Sistema completo para criar lojas, publicar produtos/serviços e gerenciar vendas
        com payout territorial.
      </p>
      <div class="card-meta">
        <span>4 endpoints principais</span>
        <span>•</span>
        <span>Guia passo-a-passo</span>
      </div>
    </a>
    
    <a href="#/funcionalidades/payout" class="content-card card-link">
      <div class="card-header">
        <svg class="icon icon-currency">...</svg>
        <h3>Payout & Gestão Financeira</h3>
      </div>
      <p class="card-summary">
        Sistema de payout automático que mantém recursos financeiros na comunidade,
        com configuração territorial.
      </p>
      <div class="card-meta">
        <span>Configuração avançada</span>
        <span>•</span>
        <span>Monitoramento</span>
      </div>
    </a>
    
    <a href="#/funcionalidades/eventos" class="content-card card-link">
      <div class="card-header">
        <svg class="icon icon-calendar">...</svg>
        <h3>Eventos</h3>
      </div>
      <p class="card-summary">
        Crie eventos territoriais com data/hora, localização e mídias. Aparecem no feed
        e no mapa.
      </p>
      <div class="card-meta">
        <span>Geolocalização</span>
        <span>•</span>
        <span>Interesse e confirmação</span>
      </div>
    </a>
    
    <a href="#/funcionalidades/admin" class="content-card card-link">
      <div class="card-header">
        <svg class="icon icon-settings">...</svg>
        <h3>Admin & Filas</h3>
      </div>
      <p class="card-summary">
        Sistema de filas para processar tarefas que requerem revisão humana, com
        configuração avançada de limites.
      </p>
      <div class="card-meta">
        <span>WorkQueue</span>
        <span>•</span>
        <span>Configuração territorial</span>
      </div>
    </a>
  </div>
</section>

<!-- 4. PRÓXIMOS PASSOS (Opcional) -->
<section class="section section-next-steps">
  <h2>Próximos Passos</h2>
  <div class="content-grid">
    <div class="content-card">
      <h3>Novo na plataforma?</h3>
      <p>Comece pelo <a href="#/comecando">guia de início rápido</a> para configurar
      seu ambiente e fazer sua primeira requisição.</p>
    </div>
    <div class="content-card">
      <h3>Quer entender os conceitos?</h3>
      <p>Explore os <a href="#/fundamentos">fundamentos</a> para entender território,
      memberships e governança.</p>
    </div>
  </div>
</section>
```

---

## 🎨 Página Específica com Contexto

### Exemplo: `pages/funcionalidades/marketplace.html`

```html
<!-- 1. BREADCRUMB - Navegação hierárquica -->
<nav class="breadcrumb">
  <a href="#/funcionalidades">Funcionalidades</a>
  <span>/</span>
  <span>Marketplace</span>
</nav>

<!-- 2. HERO - Contexto específico -->
<section class="page-hero">
  <div class="hero-content">
    <span class="eyebrow">Marketplace</span>
    <h1>Economia Local Territorial</h1>
    <p class="hero-lead">
      Sistema completo para criar lojas, publicar produtos/serviços e gerenciar vendas,
      com payout territorial que mantém recursos financeiros na comunidade, fortalecendo
      ciclos econômicos locais.
    </p>
    <div class="hero-actions">
      <a href="#quickstart" class="button button-primary">
        <svg class="icon icon-rocket">...</svg>
        Quickstart
      </a>
      <a href="#api-reference" class="button button-secondary">
        <svg class="icon icon-code">...</svg>
        Ver Referência de API
      </a>
    </div>
  </div>
  <div class="hero-visual">
    <svg class="hero-illustration" viewBox="0 0 400 300">
      <!-- Ilustração específica: loja + produtos + território -->
    </svg>
  </div>
</section>

<!-- 3. TL;DR - Resumo executivo -->
<section class="section section-tldr">
  <div class="info-panel info-panel--tip">
    <div class="info-panel-header">
      <svg class="icon icon-lightbulb">...</svg>
      <strong>Resumo</strong>
    </div>
    <p>
      O marketplace permite moradores validados criarem lojas, publicarem produtos/serviços (items),
      gerenciarem carrinho de compras e receberem inquiries de interessados. Tudo ancorado no território,
      com sistema de payout automático que mantém recursos financeiros na comunidade.
    </p>
  </div>
</section>

<!-- 4. CONCEITOS - O que é? Por que existe? -->
<section class="section" id="marketplace-conceitos">
  <h2>O que é o Marketplace?</h2>
  <p class="lead-text">
    O marketplace é o sistema de economia local do Araponga, permitindo que moradores
    validados criem lojas e publiquem produtos ou serviços dentro de um território.
  </p>
  
  <div class="content-grid">
    <div class="content-card">
      <h3>Lojas</h3>
      <p>
        Cada morador validado pode criar uma loja no território, definindo informações
        de contato e visibilidade.
      </p>
    </div>
    <div class="content-card">
      <h3>Items (Produtos/Serviços)</h3>
      <p>
        Lojas podem publicar items com preço fixo, negociável ou gratuito, incluindo
        até 10 mídias (imagens, vídeos, áudios).
      </p>
    </div>
    <div class="content-card">
      <h3>Payout Territorial</h3>
      <p>
        Sistema automático que processa vendas e mantém recursos financeiros na comunidade,
        fortalecendo economia circular local.
      </p>
    </div>
  </div>
</section>

<!-- 5. COMO USAR - Guia prático (progressive disclosure) -->
<section class="section" id="marketplace-quickstart">
  <h2>Como Usar</h2>
  <p class="lead-text">
    Siga estes passos para começar a usar o marketplace em seu território.
  </p>
  
  <div class="flow-steps">
    <div class="flow-step">
      <div class="step-number">1</div>
      <div class="step-content">
        <h3>Criar Loja</h3>
        <p>Moradores validados criam loja via <code>POST /api/v1/stores</code></p>
        <details class="step-details">
          <summary>Ver exemplo de código</summary>
          <pre class="code-example"><code>curl -X POST ...</code></pre>
        </details>
      </div>
    </div>
    <!-- Mais passos -->
  </div>
</section>

<!-- 6. REFERÊNCIA DE API - Detalhes técnicos (progressive disclosure) -->
<section class="section" id="marketplace-api">
  <h2>Referência de API</h2>
  <p class="lead-text">
    Documentação completa dos endpoints, modelos e exemplos de uso.
  </p>
  
  <!-- Conteúdo técnico detalhado aqui -->
</section>
```

---

## 🎯 Princípios de Contextualização

### 1. Hierarquia de Informação

```
Nível 1: CONTEXTO (Por que existe?)
  ↓
Nível 2: CONCEITO (O que é?)
  ↓
Nível 3: COMO USAR (Guia prático)
  ↓
Nível 4: REFERÊNCIA (Detalhes técnicos)
```

### 2. Progressive Disclosure

- **Sempre visível**: Contexto, conceito básico, resumo
- **Expansível**: Exemplos de código, detalhes técnicos
- **Navegável**: Links para seções específicas

### 3. Elementos Obrigatórios

Cada página de contextualização deve ter:

1. ✅ **Hero Section** - Título, descrição, ações principais
2. ✅ **Visão Geral** - Por que existe? Qual o propósito?
3. ✅ **Navegação** - Links para sub-seções (se aplicável)
4. ✅ **TL;DR** - Resumo executivo em destaque
5. ✅ **Próximos Passos** - Onde ir depois?

---

## 📐 CSS para Contextualização

```css
/* Hero de página completa */
.page-hero {
  display: grid;
  grid-template-columns: 1fr;
  gap: 3rem;
  padding: 4rem 0;
  border-bottom: 2px solid var(--border-subtle);
  margin-bottom: 3rem;
  background: linear-gradient(180deg, 
    var(--bg) 0%, 
    var(--bg-muted) 100%);
}

@media (min-width: 1024px) {
  .page-hero {
    grid-template-columns: 1.2fr 1fr;
    align-items: center;
  }
}

.page-hero .hero-content h1 {
  font-size: clamp(2rem, 4vw, 3rem);
  line-height: 1.2;
  margin: 1rem 0 1.5rem;
  font-weight: 700;
  color: var(--text);
}

.hero-lead {
  font-size: var(--font-size-xl);
  line-height: var(--line-height-relaxed);
  color: var(--text-muted);
  margin-bottom: 2rem;
  max-width: 65ch;
}

.hero-actions {
  display: flex;
  flex-wrap: wrap;
  gap: 1rem;
  margin-top: 2rem;
}

.hero-illustration {
  width: 100%;
  height: auto;
  max-width: 500px;
  opacity: 0.8;
  filter: brightness(0.95);
}

/* Seção de visão geral */
.section-overview {
  background: var(--bg-muted);
  border-radius: var(--radius-lg);
  padding: clamp(2rem, 3vw, 3rem);
  margin: 3rem 0;
  border: 1px solid var(--border-subtle);
}

.section-overview h2 {
  margin-top: 0;
  font-size: clamp(1.5rem, 3vw, 2rem);
}

/* Cards de navegação */
.card-link {
  text-decoration: none;
  color: inherit;
  display: block;
  transition: var(--transition-base);
  cursor: pointer;
}

.card-link:hover {
  transform: translateY(-4px);
  box-shadow: var(--elevation-3);
  border-color: var(--accent-subtle);
}

.card-link:focus-visible {
  outline: 2px solid var(--accent);
  outline-offset: 4px;
}

.card-summary {
  font-size: var(--font-size-base);
  line-height: var(--line-height-relaxed);
  color: var(--text-muted);
  margin: 0.75rem 0;
}

.card-meta {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  margin-top: 1rem;
  padding-top: 1rem;
  border-top: 1px solid var(--border-subtle);
  font-size: var(--font-size-sm);
  color: var(--text-subtle);
}

/* Breadcrumb */
.breadcrumb {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  margin-bottom: 1.5rem;
  font-size: var(--font-size-sm);
  color: var(--text-subtle);
}

.breadcrumb a {
  color: var(--accent);
  text-decoration: none;
  transition: var(--transition-base);
}

.breadcrumb a:hover {
  color: var(--accent-hover);
  text-decoration: underline;
}

.breadcrumb span:last-child {
  color: var(--text);
  font-weight: 500;
}

/* TL;DR Section */
.section-tldr {
  margin: 2rem 0;
}

/* Flow Steps */
.flow-steps {
  display: flex;
  flex-direction: column;
  gap: 2rem;
  margin: 2rem 0;
}

.flow-step {
  display: flex;
  gap: 1.5rem;
  padding: 1.5rem;
  background: var(--bg-muted);
  border-radius: var(--radius-md);
  border-left: 4px solid var(--accent);
}

.step-number {
  width: 2.5rem;
  height: 2.5rem;
  border-radius: 50%;
  background: var(--accent);
  color: white;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 700;
  flex-shrink: 0;
}

.step-content {
  flex: 1;
}

.step-details {
  margin-top: 1rem;
}

.step-details summary {
  cursor: pointer;
  color: var(--accent);
  font-weight: 500;
  padding: 0.5rem 0;
}
```

---

## ✅ Checklist de Contextualização

Para cada página de categoria (`index.html`):

- [ ] Hero section com título e descrição clara
- [ ] Seção "Por que existe?" explicando propósito
- [ ] Cards de navegação para sub-seções
- [ ] Links para próximos passos
- [ ] Ilustração ou diagrama conceitual (opcional mas recomendado)

Para cada página específica (`[topico].html`):

- [ ] Breadcrumb mostrando hierarquia
- [ ] Hero section contextualizando o tópico
- [ ] TL;DR em destaque (resumo executivo)
- [ ] Seção "O que é?" antes de "Como usar?"
- [ ] Progressive disclosure (detalhes em expansíveis)
- [ ] Links para seções relacionadas

---

## 📊 Exemplo Completo: Marketplace

Ver `docs/EXEMPLO_MARKETPLACE_CONTEXTUALIZADO.md` para exemplo completo de como
uma página deve ser estruturada com contextualização adequada.

---

**Próximo Passo**: Implementar páginas de contextualização como parte da Fase 2.
