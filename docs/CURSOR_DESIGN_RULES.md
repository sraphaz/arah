# Regras de Design para Desenvolvimento - Araponga

**Versão**: 1.0  
**Data**: 2025-01-20  
**Aplicação**: Wiki, DevPortal, App Mobile, Site Institucional, todas as interfaces Araponga

---

## 📋 Sumário

Este documento define **regras práticas de design** para desenvolvimento no repositório Araponga. Use como referência para todas as decisões de design durante o desenvolvimento.

**Documento Relacionado**: `DESIGN_SYSTEM_IDENTIDADE_VISUAL.md` (identidade e marca)  
**Análise Detalhada**: `ANALISE_DESIGN_SISTEMATICA_MELHORIAS.md` (análise e propostas)  
**Revisão Completa**: `REVISAO_ARTE_DESIGN_WIKI.md` (análise de conformidade realizada em 2025-01-20)

---

## 1. Princípios Fundamentais

### 1.1 Mobile-First

**Regra Obrigatória**: Todo CSS e layout deve seguir abordagem **mobile-first**.

✅ **Correto (Mobile-First)**:
```css
/* Base: mobile */
.component {
  padding: 1rem;
  font-size: 0.875rem;
}

/* Desktop: ajuste */
@media (min-width: 1024px) {
  .component {
    padding: 2rem;
    font-size: 1rem;
  }
}
```

❌ **Incorreto (Desktop-First)**:
```css
/* Base: desktop */
.component {
  padding: 2rem;
  font-size: 1rem;
}

/* Mobile: override */
@media (max-width: 1023px) {
  .component {
    padding: 1rem;
    font-size: 0.875rem;
  }
}
```

**Breakpoints Padrão**:
- Mobile: `< 768px` (default, sem media query)
- Tablet: `768px - 1023px` (opcional, quando necessário)
- Desktop: `≥ 1024px` (lg breakpoint)

### 1.2 Sobriedade Elevada

**Regra**: Design minimalista, sem elementos desnecessários.

- ✅ Elementos que servem à função
- ✅ Hierarquia clara de informação
- ✅ Espaçamento generoso e respiração
- ❌ Decorações sem propósito
- ❌ Cores saturadas demais
- ❌ Animações chamativas

### 1.3 Consistência

**Regra**: Sempre usar tokens de design (variáveis CSS).

✅ **Correto (Usando Variáveis)**:
```css
.component {
  background: var(--glass-bg);
  color: var(--text);
  padding: var(--space-8);
  border-radius: var(--glass-radius);
}
```

❌ **Incorreto (Valores Hardcoded)**:
```css
.component {
  background: rgba(255, 255, 255, 0.98);
  color: #e8edf2;
  padding: 2rem;
  border-radius: 24px;
}
```

---

## 2. Sistema de Cores

### 2.1 Uso de Variáveis CSS

**Regra Obrigatória**: NUNCA usar cores hex/rgb diretamente. Sempre usar variáveis CSS ou classes Tailwind do config.

**⚠️ PROIBIDO**: 
- ❌ Valores hex diretos: `#4dd4a8`, `#7dd3ff`, `#25303a`, etc.
- ❌ Valores rgb/rgba diretos: `rgb(77, 212, 168)`, `rgba(77, 212, 168, 0.3)`, etc.
- ❌ Tailwind arbitrárias: `dark:bg-[#4dd4a8]`, `text-[#7dd3ff]`, etc.

**Variáveis CSS Disponíveis**:
```css
/* Backgrounds */
--bg, --bg-elevated, --bg-card, --bg-muted
--bg-dark: #0a0e12 (dark mode background)

/* Texto */
--text, --text-muted, --text-subtle

/* Cores de Acento */
--accent: #4dd4a8
--accent-hover: #5ee5b9
--accent-strong: #3bc495
--accent-subtle: rgba(77, 212, 168, 0.15)

/* Links */
--link: #7dd3ff
--link-hover: #9de3ff

/* Bordas Dark Mode */
--border-dark: #25303a

/* Estados */
--warning, --danger, --success
```

**Classes Tailwind Configuradas** (preferir estas em vez de variáveis quando usar Tailwind):
- `dark:bg-dark-accent` (em vez de `dark:bg-[#4dd4a8]`)
- `dark:text-dark-link` (em vez de `dark:text-[#7dd3ff]`)
- `dark:border-dark-border` (em vez de `dark:border-[#25303a]`)

✅ **Correto**:
```css
.button {
  background: var(--accent);
  color: var(--text);
}

.button:hover {
  background: var(--accent-hover);
}
```

❌ **Incorreto**:
```css
/* ❌ NUNCA fazer isso */
.button {
  background: #4dd4a8; /* Cor hardcoded */
  color: #e8edf2;
}

/* ❌ NUNCA usar Tailwind arbitrárias */
.button {
  @apply dark:bg-[#4dd4a8]; /* Tailwind arbitrária proibida */
  @apply text-[#7dd3ff]; /* Tailwind arbitrária proibida */
}
```

✅ **Correto com Tailwind Classes**:
```tsx
/* ✅ Usar classes do Tailwind config */
<button className="bg-forest-600 dark:bg-dark-accent text-white">
  Botão
</button>

<a className="text-forest-600 dark:text-dark-link">
  Link
</a>
```

### 2.2 Contraste WCAG AA

**Regra Obrigatória**: Todo texto deve ter contraste mínimo 4.5:1 (normal) ou 3:1 (grande 18px+).

✅ **Correto**:
```css
.text-primary {
  color: var(--text); /* Contraste 4.5:1 garantido */
}

.text-large {
  font-size: 1.125rem; /* 18px+ pode usar 3:1 */
  color: var(--text-muted);
}
```

**Verificação**: Sempre testar com ferramenta WCAG (WebAIM Contrast Checker)

---

## 3. Tipografia

### 3.1 Fontes do Sistema

**Regra Obrigatória**: Sempre usar Inter (UI/Texto) e JetBrains Mono (Código).

✅ **Correto**:
```css
body {
  font-family: var(--font-inter), system-ui, sans-serif;
}

code, pre {
  font-family: var(--font-mono), 'Menlo', monospace;
}
```

### 3.2 Escala Tipográfica

**Regra**: Usar variáveis CSS para tamanhos.

**Variáveis Disponíveis**:
```css
--font-size-xs: 0.75rem;    /* 12px */
--font-size-sm: 0.875rem;   /* 14px */
--font-size-base: 1rem;     /* 16px */
--font-size-lg: 1.125rem;   /* 18px */
--font-size-xl: 1.25rem;    /* 20px */
--font-size-2xl: 1.5rem;    /* 24px */
--font-size-3xl: 1.875rem;  /* 30px */
--font-size-4xl: 2.25rem;   /* 36px */
--font-size-5xl: 3rem;      /* 48px */
--font-size-6xl: 3.75rem;   /* 60px */
```

✅ **Correto**:
```css
.heading {
  font-size: var(--font-size-3xl);
  line-height: var(--line-height-tight);
  letter-spacing: var(--letter-spacing-tight);
}
```

❌ **Incorreto**:
```css
.heading {
  font-size: 1.875rem;
  line-height: 1.25;
  letter-spacing: -0.025em;
}
```

### 3.3 Line-height e Letter-spacing

**Regra**: Usar variáveis CSS.

**Variáveis**:
```css
--line-height-tight: 1.25;    /* Títulos */
--line-height-relaxed: 1.75;  /* Corpo de texto */
--letter-spacing-tight: -0.025em;
--letter-spacing-normal: 0;
```

---

## 4. Espaçamento

### 4.1 Escala 8px

**Regra Obrigatória**: Todo espaçamento deve seguir escala 8px (0.5rem = 8px).

**Escala Padrão**:
```css
--space-1: 0.25rem;  /* 4px */
--space-2: 0.5rem;   /* 8px */
--space-4: 1rem;     /* 16px */
--space-6: 1.5rem;   /* 24px */
--space-8: 2rem;     /* 32px */
--space-12: 3rem;    /* 48px */
--space-16: 4rem;    /* 64px */
--space-24: 6rem;    /* 96px */
```

✅ **Correto**:
```css
.card {
  padding: var(--space-8); /* 32px */
  margin-bottom: var(--space-16); /* 64px */
}
```

❌ **Incorreto**:
```css
.card {
  padding: 1.25rem; /* 20px - não está na escala */
  margin-bottom: 5rem; /* 80px - não está na escala */
}
```

### 4.2 Mobile-First em Espaçamento

**Regra**: Espaçamento menor em mobile, maior em desktop.

✅ **Correto**:
```css
.section {
  padding: var(--space-8); /* 32px mobile */
}

@media (min-width: 1024px) {
  .section {
    padding: var(--space-16); /* 64px desktop */
  }
}
```

---

## 5. Glass Morphism

### 5.1 Aplicação Padrão

**Regra**: Cards principais devem usar glass morphism.

**Classe CSS**: `.glass-card`

✅ **Correto**:
```tsx
<div className="glass-card">
  {/* Conteúdo */}
</div>
```

**Variáveis Glass**:
```css
--glass-bg: rgba(255, 255, 255, 0.98);
--glass-border: rgba(198, 227, 210, 0.4);
--glass-shadow: 0 4px 24px rgba(...);
--glass-blur: 16px;
--glass-radius: 16px;
```

### 5.2 Customização

**Regra**: Se precisar customizar, usar variáveis CSS, não valores hardcoded.

✅ **Correto**:
```css
.custom-card {
  background: var(--glass-bg);
  border-radius: var(--glass-radius);
  /* Apenas customizar o que for necessário */
}
```

---

## 6. Animações e Transições

### 6.1 Velocidade

**Regra**: Animações devem ser rápidas (< 0.5s) e sutis.

**Variáveis**:
```css
--transition-quick: all 0.2s ease-out;    /* Feedback imediato */
--transition-smooth: all 0.4s ease-out;   /* Transições suaves */
```

✅ **Correto**:
```css
.button {
  transition: var(--transition-quick);
}
```

### 6.2 Hover States

**Regra**: Hovers devem ser sutis e consistentes.

**Padrão Hover**:
```css
.card-hover {
  transition: transform 0.3s ease-out, box-shadow 0.3s ease-out;
}

.card-hover:hover {
  transform: translateY(-2px); /* Sutil, 2px máximo */
  box-shadow: var(--glass-hover-shadow);
}
```

❌ **Evitar**:
```css
.card:hover {
  transform: scale(1.1); /* Muito exagerado */
  transform: translateY(-10px); /* Muito movimento */
}
```

---

## 7. Componentes Reutilizáveis

### 7.1 Uso de Componentes

**Regra**: Sempre usar componentes existentes quando possível.

**Componentes Disponíveis**:
- `.glass-card` - Card padrão com glass morphism
- `.btn-primary`, `.btn-secondary` - Botões
- `.nav-link` - Links de navegação
- `.sidebar-container` - Container de sidebar

✅ **Correto**:
```tsx
<button className="btn-primary">Salvar</button>
```

❌ **Incorreto**:
```tsx
<button className="bg-blue-500 text-white px-4 py-2 rounded">
  Salvar
</button>
```

### 7.2 Criar Novo Componente

**Regra**: Se criar novo componente, seguir padrões existentes e documentar.

**Checklist**:
- [ ] Usa variáveis CSS para cores/espaçamento
- [ ] Mobile-first responsivo
- [ ] Estados hover/focus/active definidos
- [ ] Acessível (WCAG AA)
- [ ] Documentado em comentário

---

## 8. Responsividade

### 8.1 Breakpoints

**Regra**: Usar breakpoints consistentes.

**Breakpoints Padrão**:
```css
/* Mobile: default (< 768px) - sem media query */

/* Tablet: quando necessário */
@media (min-width: 768px) { }

/* Desktop: lg breakpoint */
@media (min-width: 1024px) { }
```

**Tailwind Classes**:
- Mobile: default (sem prefixo)
- Desktop: `lg:` (1024px+)

✅ **Correto**:
```tsx
<div className="text-sm lg:text-base">
  {/* 14px mobile, 16px desktop */}
</div>
```

### 8.2 Ocultar/Mostrar Elementos

**Regra**: Usar classes Tailwind para visibilidade.

✅ **Correto**:
```tsx
{/* Oculto em mobile, visível em desktop */}
<aside className="hidden lg:block">
  Sidebar
</aside>

{/* Visível em mobile, oculto em desktop */}
<nav className="block lg:hidden">
  Mobile Menu
</nav>
```

---

## 9. Acessibilidade

### 9.1 Focus States

**Regra Obrigatória**: Todo elemento interativo deve ter estado de foco visível.

✅ **Correto**:
```css
.button:focus-visible {
  outline: 3px solid var(--accent);
  outline-offset: 4px;
}
```

### 9.2 Contraste

**Regra Obrigatória**: Texto mínimo 4.5:1, elementos interativos mínimo 3:1.

**Verificação**: Sempre testar com ferramenta WCAG antes de commit.

### 9.3 Navegação por Teclado

**Regra**: Todos os elementos interativos devem ser acessíveis via teclado.

✅ **Correto**:
```tsx
<button
  onClick={handleClick}
  onKeyDown={(e) => e.key === 'Enter' && handleClick()}
  aria-label="Descrição acessível"
>
  Ação
</button>
```

---

## 10. Performance

### 10.1 Imagens

**Regra**: Sempre usar `loading="lazy"` e otimizar.

✅ **Correto**:
```tsx
<Image
  src="/image.jpg"
  alt="Descrição"
  width={800}
  height={600}
  loading="lazy"
/>
```

### 10.2 CSS

**Regra**: Evitar seletores complexos e preferir classes utilitárias.

✅ **Correto**:
```css
.card { }
.card-title { }
.card-content { }
```

❌ **Incorreto**:
```css
.container > div > div > .nested > .deep > .card { }
```

---

## 11. Checklist de Validação

Antes de fazer commit de mudanças de design, verificar:

- [ ] **Mobile-first**: CSS começa com mobile, ajusta para desktop
- [ ] **Cores**: Usa variáveis CSS ou classes Tailwind configuradas (NUNCA hex/rgb diretos ou Tailwind arbitrárias como `[#4dd4a8]`)
- [ ] **Espaçamento**: Segue escala 8px (variáveis CSS)
- [ ] **Tipografia**: Usa Inter/JetBrains Mono e variáveis CSS
- [ ] **Contraste**: WCAG AA verificado (4.5:1 texto, 3:1 interativo)
- [ ] **Acessibilidade**: Focus states visíveis, navegação por teclado
- [ ] **Performance**: Imagens otimizadas, CSS eficiente
- [ ] **Consistência**: Usa componentes/classes existentes quando possível

---

## 12. Exemplos Práticos

### 12.1 Card Component (React/TSX)

```tsx
export function Card({ title, children }: CardProps) {
  return (
    <div className="glass-card">
      <h3 className="text-2xl font-semibold mb-4 text-forest-900 dark:text-forest-50">
        {title}
      </h3>
      <div className="text-forest-700 dark:text-forest-200">
        {children}
      </div>
    </div>
  );
}
```

### 12.2 Button Component

```tsx
export function Button({ variant = 'primary', children, ...props }: ButtonProps) {
  const baseClasses = "px-8 py-4 rounded-xl font-semibold transition-all duration-300";
  const variantClasses = {
    primary: "bg-forest-600 dark:bg-dark-accent text-white hover:bg-forest-700 dark:hover:bg-dark-accent-hover",
    secondary: "bg-transparent border-2 border-forest-300 dark:border-dark-border text-forest-700 dark:text-forest-200",
  };
  
  return (
    <button
      className={`${baseClasses} ${variantClasses[variant]}`}
      {...props}
    >
      {children}
    </button>
  );
}
```

### 12.3 Responsive Layout

```tsx
<div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
  {/* 1 coluna mobile, 3 colunas desktop */}
  <div className="glass-card">Card 1</div>
  <div className="glass-card">Card 2</div>
  <div className="glass-card">Card 3</div>
</div>
```

---

## 13. Referências Rápidas

### 13.1 Arquivos de Referência

- **Identidade Visual**: `docs/DESIGN_SYSTEM_IDENTIDADE_VISUAL.md`
- **Análise Detalhada**: `docs/ANALISE_DESIGN_SISTEMATICA_MELHORIAS.md`
- **CSS Tokens**: `frontend/wiki/app/globals.css`
- **Componentes**: `frontend/wiki/components/`

### 13.2 Ferramentas

- **Contraste WCAG**: WebAIM Contrast Checker
- **Acessibilidade**: axe DevTools
- **Tipografia**: Type Scale Calculator

---

## 14. Atualização e Evolução

**Este documento é vivo** e deve evoluir conforme padrões são refinados.

**Quando Atualizar**:
- Novos componentes são criados
- Novos padrões são estabelecidos
- Problemas são identificados e resolvidos
- Ferramentas ou tecnologias mudam

**Versões**:
- **1.0** (2025-01-20): Documento inicial com regras fundamentais
- **1.1** (2025-01-20): Reforço de regras sobre cores hardcoded após revisão completa (ver `REVISAO_ARTE_DESIGN_WIKI.md`)

**Lições da Revisão 2025-01-20**:
- ✅ 29 ocorrências de cores hardcoded corrigidas na Wiki
- ✅ Cores agora 100% via variáveis CSS ou classes Tailwind configuradas
- ✅ Padrão estabelecido: usar `dark:bg-dark-accent` em vez de `dark:bg-[#4dd4a8]`
- ✅ Todos os componentes devem seguir este padrão obrigatoriamente

---

**Use este documento como referência diária durante o desenvolvimento. Em caso de dúvida, siga os princípios fundamentais (Mobile-First, Sobriedade, Consistência) e consulte os documentos relacionados.**
