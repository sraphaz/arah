# DevPortal - Reestruturação de Conteúdo

**Data**: 2025-01-20  
**Versão**: 1.0  
**Status**: 🟡 EM IMPLEMENTAÇÃO - Reestruturação completa do conteúdo

---

## 🎯 Objetivo

Reformular a apresentação do conteúdo do DevPortal criando:
- ✅ Itens e subitens de navegação hierárquicos por assunto
- ✅ Identificação de padrões de conteúdo
- ✅ **Uma página dedicada por diagrama de sequência** (SRP)
- ✅ **Contexto simples e focado** em cada página (Simplicidade)
- ✅ Padronização rigorosa de exposição de conteúdo
- ✅ Excelência em padrão e orientação gráfica

## ⚠️ Princípios Obrigatórios

**VER `docs/DEVPORTAL_PRINCIPIOS_ESTRUTURA.md` para princípios completos.**

### Princípio de Responsabilidade Única (SRP)
- ✅ **Uma página = Um propósito único**
- ✅ **Um diagrama = Uma página dedicada**
- ❌ **NUNCA** misturar múltiplos assuntos na mesma página

### Simplicidade de Contexto
- ✅ **Contexto claro desde o título**
- ✅ **Foco em um único objetivo**
- ❌ **NUNCA** múltiplos contextos competindo por atenção

---

## 📊 Padrões de Conteúdo Identificados

### 1. **Diagramas de Sequência** (17 diagramas)
Fluxos técnicos documentados visualmente:

1. `auth` - Autenticação social → JWT
2. `territory-discovery` - Descoberta de território
3. `feed-listing` - Listagem de feed territorial
4. `post-creation` - Criação de post + âncoras + mídias
5. `marketplace-checkout` - Checkout do marketplace
6. `membership-resident` - Tornar-se morador
7. `residency-verification` - Verificação de residência
8. `map-entities` - Entidades do mapa
9. `events-creation` - Criação de eventos
10. `assets-validation` - Validação de assets territoriais
11. `chat-media` - Chat com mídia
12. `moderation` - Fluxo de moderação
13. `notifications-outbox` - Notificações via outbox

**Padrão de Apresentação:**
- Título descritivo
- Contexto de negócio (por que esse fluxo existe)
- Diagrama de sequência (imagem SVG)
- Explicação passo a passo
- Exemplos de código (cURL)
- Referências relacionadas

### 2. **Guias Práticos**
Instruções passo a passo para tarefas específicas:

- Quickstart
- Autenticação (JWT)
- Seleção de Território
- Primeira Requisição

**Padrão de Apresentação:**
- Objetivo claro
- Pré-requisitos
- Passos numerados
- Exemplos de código
- Troubleshooting comum

### 3. **Referências Técnicas**
Documentação de endpoints, modelos e especificações:

- Endpoints da API
- Modelos de dados
- Códigos de erro
- OpenAPI / Swagger

**Padrão de Apresentação:**
- Tabela estruturada (método, path, descrição)
- Parâmetros documentados
- Exemplos de request/response
- Links para diagramas relacionados

### 4. **Conceitos de Produto**
Explicações de conceitos e valores:

- Visão Geral
- Como Funciona
- Territórios
- Conceitos de Produto
- Modelo de Domínio

**Padrão de Apresentação:**
- Definição clara
- Contexto de uso
- Relacionamentos
- Exemplos práticos

### 5. **Casos de Uso**
Jornadas práticas conectando objetivo → pré-requisitos → endpoints:

- Marketplace
- Eventos
- Feed Comunitário
- Moderação

**Padrão de Apresentação:**
- Objetivo do caso de uso
- Pré-requisitos listados
- Fluxo de passos
- Endpoints usados
- Diagrama de sequência relacionado (se houver)

---

## 🗂️ Nova Estrutura Hierárquica

### Nível 1: Categorias Principais (Tabs)

1. **Começando** (`/comecando`)
2. **Fundamentos** (`/fundamentos`)
3. **Fluxos & Diagramas** (`/fluxos`) ⭐ **NOVO**
4. **API Prática** (`/api-pratica`)
5. **Funcionalidades** (`/funcionalidades`)
6. **Avancado** (`/avancado`)

### Nível 2: Seções (Sidebar Accordions)

#### **Começando**
- Introdução
- Quickstart
- Autenticação
- Território & Sessão

#### **Fundamentos**
- Visão Geral
- Como Funciona
- Territórios
- Conceitos de Produto
- Modelo de Domínio

#### **Fluxos & Diagramas** ⭐ **NOVO**
- 🔐 Autenticação e Sessão
  - Autenticação Social → JWT
  - Descoberta de Território
  - Seleção de Território
- 📝 Feed e Publicações
  - Listagem de Feed
  - Criação de Post
  - Posts com Mídias
- 🛒 Marketplace
  - Checkout
- 👥 Membership
  - Tornar-se Morador
  - Verificação de Residência
- 🗺️ Mapa e Assets
  - Entidades do Mapa
  - Assets Territoriais
- 🎉 Eventos
  - Criação de Eventos
- 💬 Chat
  - Chat com Mídia
- 🛡️ Moderação
  - Fluxo de Moderação
- 🔔 Notificações
  - Outbox Pattern

#### **API Prática**
- Casos de Uso
- Endpoints de Referência
- OpenAPI / Explorer
- Erros & Convenções

#### **Funcionalidades**
- Marketplace
- Payout & Gestão Financeira
- Eventos
- Admin & Filas

#### **Avancado**
- FAQ
- Capacidades Técnicas
- Roadmap
- Contribuir
- Versões

### Nível 3: Páginas Individuais (SRP - Uma Página = Um Propósito)

**⚠️ REGRA CRÍTICA: Cada diagrama de sequência terá sua PRÓPRIA página isolada.**

**Estrutura:**
```
/fluxos/
  ├── autenticacao.html              → SRP: Apenas fluxo de autenticação
  ├── descoberta-territorio.html     → SRP: Apenas descoberta de território
  ├── feed-listagem.html             → SRP: Apenas listagem de feed
  ├── criacao-post.html              → SRP: Apenas criação de post
  ├── marketplace-checkout.html      → SRP: Apenas checkout do marketplace
  ├── membership-morador.html        → SRP: Apenas tornar-se morador
  ├── verificacao-residencia.html    → SRP: Apenas verificação de residência
  ├── entidades-mapa.html            → SRP: Apenas entidades do mapa
  ├── assets-territoriais.html       → SRP: Apenas assets territoriais
  ├── criacao-eventos.html           → SRP: Apenas criação de eventos
  ├── chat-midia.html                → SRP: Apenas chat com mídia
  ├── moderation.html                → SRP: Apenas fluxo de moderação
  └── notifications-outbox.html      → SRP: Apenas outbox pattern
```

**❌ NUNCA criar:**
- `/fluxos/todos.html` → Violaria SRP (múltiplos diagramas)
- `/fluxos/autenticacao-e-territorio.html` → Violaria SRP (dois assuntos)

**Cada página de diagrama seguirá o padrão (SRP aplicado):**

```html
<!-- Título e Contexto -->
<h1>Autenticação Social → JWT</h1>
<p class="lead">Fluxo de autenticação usando provedor social...</p>

<!-- Diagrama de Sequência -->
<div class="diagram-container">
  <img src="./assets/images/diagrams/auth.svg" alt="Diagrama: Autenticação" />
</div>

<!-- Explicação Passo a Passo -->
<div class="flow-steps">
  <div class="flow-step">
    <h3>1. Cliente envia credenciais sociais</h3>
    <p>O cliente envia provider, externalId, displayName e CPF...</p>
    <pre><code>POST /api/v1/auth/social</code></pre>
  </div>
  <!-- ... mais passos ... -->
</div>

<!-- Código de Exemplo -->
<div class="code-example">
  <h3>Exemplo Completo</h3>
  <pre><code>curl -X POST ...</code></pre>
</div>

<!-- Referências Relacionadas (LINKS, não conteúdo inline) -->
<div class="related-content">
  <h3>Conteúdo Relacionado</h3>
  <ul>
    <li><a href="/comecando/autenticacao">Guia de Autenticação</a></li>
    <li><a href="/api-pratica/endpoints#auth">Endpoints de Referência</a></li>
  </ul>
</div>
```

**Validação SRP e Simplicidade:**
- ✅ **Responsabilidade única:** Esta página documenta APENAS o fluxo de autenticação social → JWT
- ✅ **Contexto simples:** Todo conteúdo está relacionado APENAS ao fluxo de autenticação
- ✅ **Sem mistura:** Não há outros diagramas ou assuntos nesta página
- ✅ **Links externos:** Conteúdo relacionado está em páginas separadas (não inline)

---

## 📐 Padrão Visual de Exposição de Conteúdo

### Hierarquia Visual

1. **Título (H1)** - `font-size: 2rem`, `font-weight: 600`, `line-height: 1.2`
2. **Lead Text** - `font-size: 1.125rem`, `line-height: 1.75`, `color: var(--text-muted)`
3. **Diagrama** - Largura máxima: `100%`, `border-radius: 0.5rem`, `box-shadow: subtle`
4. **Seções (H2)** - `font-size: 1.5rem`, `font-weight: 600`, `margin-top: 3rem`
5. **Subseções (H3)** - `font-size: 1.25rem`, `font-weight: 500`, `margin-top: 2rem`
6. **Conteúdo (P)** - `font-size: 1rem`, `line-height: 1.75`, `max-width: 70ch`

### Espaçamento Padrão

- Entre seções: `3rem` (48px)
- Entre subseções: `2rem` (32px)
- Entre parágrafos: `1rem` (16px)
- Padding interno de cards: `1.5rem` (24px)
- Gap em grids: `1.5rem` (24px)

### Cores e Contraste

- Texto principal: `var(--text)` (WCAG AA)
- Texto secundário: `var(--text-muted)` (60% opacity)
- Links: `var(--accent)` com hover `var(--accent-strong)`
- Borders: `var(--border-subtle)` (20% opacity)
- Backgrounds: `var(--bg)` e `var(--bg-elevated)`

---

## ✅ Checklist de Implementação

- [ ] Criar estrutura de pastas `/fluxos/` para diagramas
- [ ] Criar template HTML padronizado para páginas de diagrama
- [ ] Mover diagramas para páginas individuais
- [ ] Reorganizar sidebar com nova hierarquia
- [ ] Aplicar padrão visual consistente
- [ ] Criar testes para garantir isolamento de conteúdo
- [ ] Validar navegação hierárquica
- [ ] Documentar padrões de conteúdo

---

## 📚 Referências

- `docs/60_API_LÓGICA_NEGÓCIO.md` - Lógica de negócio completa
- `docs/DEVPORTAL_ESTRUTURA_PAGINAS_SEPARADAS.md` - Estrutura de páginas
- `frontend/devportal/assets/images/diagrams/` - Diagramas SVG
