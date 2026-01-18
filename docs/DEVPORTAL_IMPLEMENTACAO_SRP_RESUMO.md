# DevPortal - Implementação SRP - Resumo Executivo

**Data**: 2025-01-20  
**Versão**: 1.0  
**Status**: 🟡 EM IMPLEMENTAÇÃO

---

## 🎯 Objetivo

Implementar a reestruturação completa do DevPortal respeitando **SRP** e **Simplicidade de Contexto**, separando cada diagrama em sua própria seção individual.

---

## 📊 Situação Atual vs. Proposta

### ❌ Situação Atual

- **Seção `#fluxos`** (linha ~1577): Múltiplos diagramas misturados na mesma seção
- **13 diagramas** agrupados em uma única seção com múltiplos `flow-step`
- **Violação de SRP**: Uma seção com múltiplas responsabilidades
- **Contexto confuso**: Múltiplos assuntos competindo por atenção

### ✅ Proposta (SRP)

- **Cada diagrama** em sua própria seção individual com ID único
- **13 seções separadas** dentro do `phase-panel` "api-pratica"
- **Aplicação de SRP**: Uma seção = um diagrama = uma responsabilidade
- **Contexto simples**: Cada seção foca em um único assunto

---

## 🗂️ Estrutura Proposta (Hierarquia)

### Nível 1: Phase Panel
- `phase-panel[data-phase-panel="api-pratica"]`

### Nível 2: Seções Individuais (SRP - Uma Seção = Um Diagrama)

```html
<section id="fluxo-autenticacao" class="section">
  <h2>Autenticação Social → JWT</h2>
  <p class="lead">Fluxo de autenticação usando provedor social...</p>
  <!-- Diagrama + Explicação + Código -->
</section>

<section id="fluxo-descoberta-territorio" class="section">
  <h2>Descoberta de Território</h2>
  <p class="lead">Como descobrir territórios próximos...</p>
  <!-- Diagrama + Explicação + Código -->
</section>

<!-- ... mais 11 seções ... -->
```

### Nível 3: Sidebar Hierárquica (Navegação)

```
API & Fluxos
├── Autenticação e Sessão
│   ├── Autenticação Social → JWT (#fluxo-autenticacao)
│   ├── Descoberta de Território (#fluxo-descoberta-territorio)
│   └── Seleção de Território (#territory-selection)
├── Feed e Publicações
│   ├── Listagem de Feed (#fluxo-feed-listagem)
│   ├── Criação de Post (#fluxo-criacao-post)
│   └── Posts com Mídias (mesmo fluxo)
├── Marketplace
│   └── Checkout (#fluxo-marketplace-checkout)
├── Membership
│   ├── Tornar-se Morador (#fluxo-membership-resident)
│   └── Verificação de Residência (#fluxo-residency-verification)
├── Mapa e Assets
│   ├── Entidades do Mapa (#fluxo-map-entities)
│   └── Assets Territoriais (#fluxo-assets-validation)
├── Eventos
│   └── Criação de Eventos (#fluxo-events-creation)
├── Chat
│   └── Chat com Mídia (#fluxo-chat-media)
├── Moderação
│   └── Fluxo de Moderação (#fluxo-moderation)
└── Notificações
    └── Outbox Pattern (#fluxo-notifications-outbox)
```

---

## 📋 Lista de Diagramas (13 Diagramas)

| # | ID da Seção | Título | Diagrama SVG | Linha Atual (aproximada) |
|---|------------|--------|--------------|-------------------------|
| 1 | `fluxo-autenticacao` | Autenticação Social → JWT | `auth.svg` | ~1581 |
| 2 | `fluxo-descoberta-territorio` | Descoberta de Território | `territory-discovery.svg` | ~1607 |
| 3 | `territory-selection` | Seleção de Território | - (sem diagrama) | ~1625 |
| 4 | `fluxo-feed-listagem` | Listagem de Feed | `feed-listing.svg` | ~1637 |
| 5 | `fluxo-criacao-post` | Criação de Post | `post-creation.svg` | ~1657 |
| 6 | `fluxo-events-creation` | Criação de Eventos | `events-creation.svg` | ~1702 |
| 7 | `fluxo-assets-validation` | Assets Territoriais | `assets-validation.svg` | ~1732 |
| 8 | `fluxo-map-entities` | Entidades do Mapa | `map-entities.svg` | ~1773 |
| 9 | `fluxo-membership-resident` | Tornar-se Morador | `membership-resident.svg` | ~1796 |
| 10 | `fluxo-moderation` | Fluxo de Moderação | `moderation.svg` | ~1819 |
| 11 | `fluxo-residency-verification` | Verificação de Residência | `residency-verification.svg` | ~1859 |
| 12 | `fluxo-notifications-outbox` | Notificações Outbox | `notifications-outbox.svg` | ~1895 |
| 13 | `fluxo-chat-media` | Chat com Mídia | `chat-media.svg` | ~1913 |
| 14 | `fluxo-marketplace-checkout` | Marketplace Checkout | `marketplace-checkout.svg` | ~2167 (outro lugar) |

---

## ✅ Checklist de Implementação

- [ ] **Reorganizar Sidebar** (hierarquia clara por assunto)
- [ ] **Mover `#fluxos` para dentro de `api-pratica`** (linha 762-764)
- [ ] **Separar cada diagrama em seção individual** (13 seções)
- [ ] **Aplicar template padronizado** a cada seção
- [ ] **Validar isolamento de conteúdo** (testes)
- [ ] **Garantir navegação funcional** (links da sidebar)

---

## 🎨 Template Padronizado (SRP)

Cada seção de diagrama seguirá este padrão:

```html
<section id="fluxo-[nome]" class="section">
  <span class="eyebrow">Fluxo</span>
  <h2>Título do Fluxo</h2>
  <p class="lead">Contexto simples e focado (1 parágrafo)</p>
  
  <!-- Diagrama (SE HOUVER) -->
  <div class="diagram-container">
    <img src="./assets/images/diagrams/[nome].svg" alt="Diagrama: [Nome]" />
  </div>
  
  <!-- Explicação Passo a Passo -->
  <div class="flow-steps">
    <!-- Passos relacionados APENAS a este diagrama -->
  </div>
  
  <!-- Código de Exemplo -->
  <div class="code-example">
    <!-- Código relacionado APENAS a este diagrama -->
  </div>
  
  <!-- Referências Relacionadas (LINKS, não conteúdo inline) -->
  <div class="related-content">
    <h3>Conteúdo Relacionado</h3>
    <ul>
      <li><a href="#outra-secao">Link para seção relacionada</a></li>
    </ul>
  </div>
</section>
```

**Validação SRP:**
- ✅ **Responsabilidade única:** Esta seção documenta APENAS o fluxo [nome]
- ✅ **Contexto simples:** Todo conteúdo está relacionado APENAS a [nome]
- ✅ **Sem mistura:** Não há outros diagramas ou assuntos nesta seção
- ✅ **Links externos:** Conteúdo relacionado está em seções separadas (não inline)

---

**Próximo passo:** Implementar a reorganização da sidebar e separação dos diagramas em seções individuais.
