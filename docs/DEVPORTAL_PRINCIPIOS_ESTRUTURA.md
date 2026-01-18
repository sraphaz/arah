# DevPortal - Princípios de Estrutura

**Data**: 2025-01-20  
**Versão**: 1.0  
**Status**: ✅ DIRETRIZES OBRIGATÓRIAS

---

## 🎯 Princípios Fundamentais

### 1. Princípio de Responsabilidade Única (SRP)

**Cada página deve ter UMA única responsabilidade clara.**

- ✅ **Uma página = Um assunto específico**
- ✅ **Um diagrama = Uma página dedicada**
- ✅ **Um guia = Uma página focada**
- ❌ **NUNCA** misturar múltiplos assuntos na mesma página
- ❌ **NUNCA** misturar diagramas diferentes na mesma página

**Exemplos:**

✅ **CORRETO:**
- `/fluxos/autenticacao.html` → Apenas autenticação (diagrama + explicação)
- `/comecando/quickstart.html` → Apenas quickstart (guia prático)
- `/referencias/jwt.html` → Apenas referência JWT (especificação técnica)

❌ **INCORRETO:**
- `/fluxos/todos.html` → Múltiplos diagramas na mesma página
- `/comecando/tudo.html` → Quickstart + Autenticação + Território misturados

### 2. Simplicidade de Contexto

**Cada página deve ter contexto simples e focado.**

- ✅ **Contexto claro desde o título**
- ✅ **Foco em um único objetivo**
- ✅ **Informações relacionadas, mas separadas**
- ❌ **NUNCA** múltiplos contextos competindo por atenção
- ❌ **NUNCA** informações irrelevantes para o contexto atual

**Exemplos:**

✅ **CORRETO:**
```
Página: "Autenticação Social → JWT"
Contexto: Como obter token JWT via autenticação social
Conteúdo: Diagrama de autenticação + explicação + código
```

❌ **INCORRETO:**
```
Página: "Autenticação e Outros Assuntos"
Contexto: Mistura autenticação + feed + marketplace
Conteúdo: Múltiplos diagramas e explicações diferentes
```

---

## 📐 Estrutura por Tipo de Conteúdo

### Tipo 1: Diagrama de Sequência

**Responsabilidade Única:** Documentar UM fluxo específico

**Estrutura Padrão:**
```html
<!-- Título único e específico -->
<h1>Autenticação Social → JWT</h1>

<!-- Contexto claro (1 parágrafo) -->
<p class="lead">Fluxo de autenticação usando provedor social...</p>

<!-- Diagrama (1 único) -->
<div class="diagram-container">
  <img src="./diagrams/auth.svg" alt="Diagrama: Autenticação" />
</div>

<!-- Explicação passo a passo (relacionada ao diagrama) -->
<div class="flow-steps">...</div>

<!-- Código de exemplo (relacionado ao diagrama) -->
<div class="code-example">...</div>

<!-- Referências (links externos, não conteúdo) -->
<div class="related-links">...</div>
```

**Critérios:**
- ✅ Um único diagrama por página
- ✅ Contexto focado no fluxo do diagrama
- ✅ Não misturar com outros diagramas
- ✅ Links para páginas relacionadas (não conteúdo inline)

### Tipo 2: Guia Prático

**Responsabilidade Única:** Ensinar UM processo específico

**Estrutura Padrão:**
```html
<!-- Objetivo único e claro -->
<h1>Quickstart</h1>
<p class="lead">Comece a usar a API em 5 minutos...</p>

<!-- Objetivos (relacionados ao guia) -->
<div class="objectives">...</div>

<!-- Passos numerados (focados no guia) -->
<div class="steps">
  <div class="step">1. ...</div>
  <div class="step">2. ...</div>
</div>

<!-- Código de exemplo (relacionado ao guia) -->
<div class="code-example">...</div>

<!-- Próximos passos (links, não conteúdo) -->
<div class="next-steps">...</div>
```

**Critérios:**
- ✅ Um único processo por página
- ✅ Contexto focado no objetivo do guia
- ✅ Não misturar múltiplos processos
- ✅ Links para diagramas relacionados (não diagramas inline)

### Tipo 3: Referência Técnica

**Responsabilidade Única:** Documentar UM endpoint/especificação

**Estrutura Padrão:**
```html
<!-- Endpoint único e específico -->
<h1>POST /api/v1/auth/social</h1>
<p class="lead">Autenticação usando provedor social...</p>

<!-- Especificação (focada no endpoint) -->
<div class="spec">
  <table>
    <tr><th>Método</th><td>POST</td></tr>
    <tr><th>Path</th><td>/api/v1/auth/social</td></tr>
  </table>
</div>

<!-- Parâmetros (relacionados ao endpoint) -->
<div class="parameters">...</div>

<!-- Exemplos (relacionados ao endpoint) -->
<div class="examples">...</div>

<!-- Referências (links para diagramas/guias) -->
<div class="related-links">...</div>
```

**Critérios:**
- ✅ Um único endpoint/especificação por página
- ✅ Contexto focado na documentação técnica
- ✅ Não misturar múltiplos endpoints
- ✅ Links para diagramas relacionados (não diagramas inline)

### Tipo 4: Conceito de Produto

**Responsabilidade Única:** Explicar UM conceito específico

**Estrutura Padrão:**
```html
<!-- Conceito único e claro -->
<h1>Territórios</h1>
<p class="lead">Unidade primária de organização...</p>

<!-- Definição (focada no conceito) -->
<div class="definition">...</div>

<!-- Contexto de uso (relacionado ao conceito) -->
<div class="context">...</div>

<!-- Exemplos (relacionados ao conceito) -->
<div class="examples">...</div>

<!-- Referências (links para diagramas/fluxos) -->
<div class="related-links">...</div>
```

**Critérios:**
- ✅ Um único conceito por página
- ✅ Contexto focado na explicação do conceito
- ✅ Não misturar múltiplos conceitos
- ✅ Links para diagramas relacionados (não diagramas inline)

---

## 🔍 Checklist de Validação

Antes de criar uma página, verificar:

- [ ] **Responsabilidade Única:**
  - [ ] A página tem um único propósito claro?
  - [ ] O título reflete exatamente o conteúdo?
  - [ ] Não há múltiplos assuntos competindo?

- [ ] **Simplicidade de Contexto:**
  - [ ] O contexto é claro desde o início?
  - [ ] Todo conteúdo está relacionado ao propósito único?
  - [ ] Não há informações irrelevantes?

- [ ] **Separação:**
  - [ ] Diagramas diferentes têm páginas separadas?
  - [ ] Guias diferentes têm páginas separadas?
  - [ ] Referências diferentes têm páginas separadas?
  - [ ] Links para conteúdo relacionado (não conteúdo inline)?

---

## 📚 Exemplos Práticos

### ✅ Estrutura CORRETA

```
/fluxos/
  ├── autenticacao.html          → Apenas diagrama de autenticação
  ├── descoberta-territorio.html → Apenas diagrama de descoberta
  └── feed-listagem.html         → Apenas diagrama de listagem

/comecando/
  ├── quickstart.html            → Apenas guia quickstart
  ├── autenticacao.html          → Apenas guia de autenticação
  └── territorio-sessao.html     → Apenas guia de território

/referencias/
  ├── jwt.html                   → Apenas especificação JWT
  ├── endpoints-auth.html        → Apenas endpoints de auth
  └── erros.html                 → Apenas códigos de erro
```

### ❌ Estrutura INCORRETA

```
/fluxos/
  └── todos.html                 → ❌ Múltiplos diagramas misturados

/comecando/
  └── tudo.html                  → ❌ Quickstart + Auth + Território misturados

/referencias/
  └── completo.html              → ❌ Todas as referências em uma página
```

---

## 🎨 Aplicação na Implementação

**Ao criar uma nova página:**

1. **Defina a responsabilidade única** em uma frase
   - Ex: "Esta página documenta APENAS o fluxo de autenticação social → JWT"

2. **Valide o contexto simples**
   - Ex: "Todo conteúdo nesta página está relacionado APENAS ao fluxo de autenticação"

3. **Separe conteúdo relacionado**
   - Ex: "Se há outro diagrama, ele vai em outra página. Aqui apenas links."

4. **Mantenha foco**
   - Ex: "Se o conteúdo não é sobre autenticação, não deve estar aqui"

---

**Esses princípios são OBRIGATÓRIOS para toda a estrutura do DevPortal.**
