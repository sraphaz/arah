# Exemplo: Marketplace com Contextualização Adequada

Este documento mostra como a página do Marketplace deve ser estruturada com contextualização adequada.

---

## ❌ ANTES: Estrutura Atual (Problema)

```html
<!-- Começa abruptamente com endpoint -->
<section class="section" id="marketplace">
  <span class="eyebrow">Marketplace</span>
  <h2>Economia local territorial</h2>
  <p class="lead">
    O marketplace permite criar lojas e publicar produtos/serviços (items), gerenciar carrinho de compras
    e receber inquiries de interessados. Tudo ancorado no território. O sistema de payout territorial
    garante que recursos financeiros permaneçam na comunidade...
  </p>
  
  <!-- Já vai direto para código -->
  <div class="flow-step">
    <h4>12. Criar ou atualizar loja</h4>
    <pre class="code-block"><code>curl -X POST ...</code></pre>
  </div>
</section>
```

**Problemas**:
- ❌ Não explica "por que" existe marketplace
- ❌ Não contextualiza o conceito antes dos detalhes
- ❌ Usuário não entende propósito antes de usar
- ❌ Falta visão geral do que vai encontrar
- ❌ Navegação confusa (não sabe por onde começar)

---

## ✅ DEPOIS: Estrutura com Contextualização

### Página de Contextualização: `pages/funcionalidades/index.html`

```html
<!-- HERO: Apresenta a categoria -->
<section class="page-hero">
  <div class="hero-content">
    <span class="eyebrow">Funcionalidades</span>
    <h1>Funcionalidades da Plataforma</h1>
    <p class="hero-lead">
      Conjunto de funcionalidades que permitem comunidades gerenciarem economia local,
      eventos territoriais, comunicação e governança através de ferramentas técnicas
      que respeitam autonomia e soberania territorial.
    </p>
  </div>
</section>

<!-- VISÃO GERAL: Por que existem? -->
<section class="section section-overview">
  <h2>Por que essas funcionalidades?</h2>
  <p class="lead-text">
    As funcionalidades do Araponga foram projetadas para fortalecer autonomia territorial,
    economia circular local e organização comunitária. Cada funcionalidade mantém recursos
    e decisões no território, respeitando soberania local.
  </p>
  
  <div class="content-grid">
    <div class="content-card">
      <div class="card-header">
        <svg class="icon icon-store">...</svg>
        <h3>Economia Local</h3>
      </div>
      <p>Marketplace e payout mantêm recursos na comunidade.</p>
    </div>
    <!-- Mais cards -->
  </div>
</section>

<!-- NAVEGAÇÃO: O que você vai encontrar -->
<section class="section section-navigation">
  <h2>Explore as Funcionalidades</h2>
  <p class="lead-text">
    Escolha uma funcionalidade para ver documentação completa.
  </p>
  
  <div class="content-grid">
    <a href="#/funcionalidades/marketplace" class="content-card card-link">
      <div class="card-header">
        <svg class="icon icon-store">...</svg>
        <h3>Marketplace</h3>
      </div>
      <p class="card-summary">
        Crie lojas, publique produtos e gerencie vendas com payout territorial.
      </p>
      <div class="card-meta">
        <span>4 endpoints</span>
        <span>•</span>
        <span>Guia completo</span>
      </div>
    </a>
    <!-- Mais cards de navegação -->
  </div>
</section>
```

### Página Específica: `pages/funcionalidades/marketplace.html`

```html
<!-- BREADCRUMB: Onde estou? -->
<nav class="breadcrumb">
  <a href="#/funcionalidades">Funcionalidades</a>
  <span>/</span>
  <span>Marketplace</span>
</nav>

<!-- HERO: Contexto específico -->
<section class="page-hero">
  <div class="hero-content">
    <span class="eyebrow">Marketplace</span>
    <h1>Economia Local Territorial</h1>
    <p class="hero-lead">
      Sistema completo para criar lojas, publicar produtos/serviços e gerenciar vendas,
      com payout territorial que mantém recursos financeiros na comunidade.
    </p>
    <div class="hero-actions">
      <a href="#quickstart" class="button button-primary">
        <svg class="icon icon-rocket">...</svg>
        Quickstart
      </a>
      <a href="#api-reference" class="button button-secondary">
        <svg class="icon icon-code">...</svg>
        Ver API
      </a>
    </div>
  </div>
  <div class="hero-visual">
    <!-- Ilustração: loja + produtos + território -->
  </div>
</section>

<!-- TL;DR: Resumo executivo -->
<section class="section section-tldr">
  <div class="info-panel info-panel--tip">
    <div class="info-panel-header">
      <svg class="icon icon-lightbulb">...</svg>
      <strong>Resumo</strong>
    </div>
    <p>
      O marketplace permite moradores validados criarem lojas, publicarem produtos/serviços,
      gerenciarem carrinho e receberem inquiries. Tudo ancorado no território, com payout
      automático que mantém recursos na comunidade.
    </p>
  </div>
</section>

<!-- CONCEITOS: O que é? Por que existe? -->
<section class="section" id="marketplace-conceitos">
  <h2>O que é o Marketplace?</h2>
  <p class="lead-text">
    O marketplace é o sistema de economia local do Araponga, permitindo que moradores
    validados criem lojas e publiquem produtos ou serviços dentro de um território.
  </p>
  
  <div class="content-grid">
    <div class="content-card">
      <h3>Propósito</h3>
      <p>
        Fortalecer economia circular local mantendo recursos financeiros na comunidade,
        permitindo que territórios construam autonomia através de suas próprias
        capacidades produtivas.
      </p>
    </div>
    
    <div class="content-card">
      <h3>Componentes Principais</h3>
      <ul>
        <li><strong>Lojas:</strong> Criadas por moradores validados</li>
        <li><strong>Items:</strong> Produtos ou serviços com preço e mídias</li>
        <li><strong>Carrinho:</strong> Gerenciamento de compras</li>
        <li><strong>Payout:</strong> Sistema automático territorial</li>
      </ul>
    </div>
    
    <div class="content-card">
      <h3>Fluxo Básico</h3>
      <ol>
        <li>Morador cria loja</li>
        <li>Publica items (produtos/serviços)</li>
        <li>Interessados fazem inquiries</li>
        <li>Vendas processadas com payout automático</li>
      </ol>
    </div>
  </div>
</section>

<!-- COMO USAR: Guia prático (progressive disclosure) -->
<section class="section" id="marketplace-quickstart">
  <h2>Como Usar</h2>
  <p class="lead-text">
    Siga estes passos para começar a usar o marketplace. Detalhes técnicos completos
    estão disponíveis na <a href="#api-reference">referência de API</a>.
  </p>
  
  <div class="flow-steps">
    <div class="flow-step">
      <div class="step-number">1</div>
      <div class="step-content">
        <h3>Criar Loja</h3>
        <p>
          Moradores validados podem criar uma loja no território. A loja define
          informações de contato e visibilidade.
        </p>
        <details class="step-details">
          <summary>
            <svg class="icon icon-code">...</svg>
            Ver exemplo de código
          </summary>
          <div class="details-content">
            <pre class="code-example"><code>curl -X POST /api/v1/stores \
  -H "Authorization: Bearer &lt;token&gt;" \
  -d '{...}'</code></pre>
          </div>
        </details>
      </div>
    </div>
    
    <div class="flow-step">
      <div class="step-number">2</div>
      <div class="step-content">
        <h3>Publicar Item</h3>
        <p>
          Lojas podem criar items (produtos ou serviços) com preço, categoria e mídias.
          Items podem incluir até 10 mídias (imagens, vídeos, áudios).
        </p>
        <details class="step-details">
          <summary>
            <svg class="icon icon-code">...</svg>
            Ver exemplo de código
          </summary>
          <!-- Código aqui -->
        </details>
      </div>
    </div>
    
    <!-- Mais passos -->
  </div>
</section>

<!-- REFERÊNCIA DE API: Detalhes técnicos -->
<section class="section" id="marketplace-api">
  <h2>Referência de API</h2>
  <p class="lead-text">
    Documentação completa dos endpoints, modelos de dados e exemplos avançados.
  </p>
  
  <!-- Conteúdo técnico detalhado aqui -->
  <!-- Progressive disclosure: informações básicas visíveis, avançadas em expansíveis -->
</section>
```

---

## 📊 Comparação: Antes vs Depois

| Aspecto | Antes | Depois |
|---------|-------|--------|
| **Introdução** | ❌ Ausente | ✅ Hero section contextualizando |
| **Propósito** | ❌ Não explicado | ✅ Seção "Por que existe?" |
| **Conceitos** | ❌ Misturado com código | ✅ Seção dedicada antes de código |
| **Navegação** | ❌ Confusa | ✅ Cards de navegação claros |
| **Resumo** | ❌ Não há | ✅ TL;DR em destaque |
| **Hierarquia** | ❌ Tudo no mesmo nível | ✅ Progressivo (contexto → conceito → código) |
| **Orientação** | ❌ Usuário perdido | ✅ Breadcrumbs e ações claras |

---

## 🎯 Benefícios da Contextualização

1. **Compreensão**: Usuário entende "por que" antes de "como"
2. **Orientação**: Sabe o que vai encontrar e por onde começar
3. **Confiança**: Entende propósito e valor antes de investir tempo
4. **Navegação**: Fácil encontrar o que precisa
5. **Primeira Impressão**: Profissional e bem estruturado

---

**Este padrão deve ser aplicado a todas as páginas de categoria e páginas específicas.**
