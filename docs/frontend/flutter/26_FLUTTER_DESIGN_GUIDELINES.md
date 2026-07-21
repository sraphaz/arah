# Diretrizes de Design do App Flutter Arah

> **LEGADO / DEPRECATED (IA e visual)** — 2026-07-21  
> Este documento **não** é mais a fonte canônica de UI do app.  
> Fonte oficial: `design-system/handoff/Especificacao-Arah.html`, `design-system/ui_kits/app/` e tokens `--premium-*` em `design-system/colors_and_type.css`.  
> Decisão: [ADR-021](../../architecture/adrs/ADR-021-design-system-app-canonic.md).  
> Manter apenas como histórico; não usar como alvo de implementação.

**Versão**: 1.0  
**Data**: 2025-01-20  
**Status**: ⚠️ Legado (histórico) — ver ADR-021  
**Tipo**: Design System e Guia de Estilo Completo

---

## 📋 Índice

1. [Visão Geral e Filosofia](#visão-geral-e-filosofia)
2. [Identidade Visual e Valores](#identidade-visual-e-valores)
3. [Estratégia de Conversão e Engajamento](#estratégia-de-conversão-e-engajamento)
4. [Sistema de Cores](#sistema-de-cores)
5. [Tipografia](#tipografia)
6. [Espaçamento e Layout](#espaçamento-e-layout)
7. [Formas e Bordas](#formas-e-bordas)
8. [Transições e Animações](#transições-e-animações)
9. [Efeitos Visuais e Profundidade](#efeitos-visuais-e-profundidade)
10. [Componentes e Padrões](#componentes-e-padrões)
11. [Hierarquia Visual e Navegação](#hierarquia-visual-e-navegação)
12. [Estados e Feedback](#estados-e-feedback)
13. [Microinterações e Impressões](#microinterações-e-impressões)
14. [Acessibilidade](#acessibilidade)
15. [Responsividade e Adaptação](#responsividade-e-adaptação)
16. [Dark Mode](#dark-mode)
17. [Imagens e Mídia](#imagens-e-mídia)
18. [Iconografia](#iconografia)
19. [Guidelines Específicas por Feature](#guidelines-específicas-por-feature)
20. [Checklist de Implementação](#checklist-de-implementação)

---

## 🎯 Visão Geral e Filosofia

### Propósito do Design System

O design system do Arah Flutter App deve **guiar, harmonizar e acelerar** o desenvolvimento, garantindo:

- ✅ **Experiência fluida e natural** (baixa fricção)
- ✅ **Conversão eficiente** (estratégias de captura de atenção aplicadas ao contexto territorial)
- ✅ **Identidade visual única** (preservando a estética estabelecida)
- ✅ **Consistência** (padrões claros em todas as telas)
- ✅ **Acessibilidade** (inclusivo e responsivo)
- ✅ **Performance** (design que não compromete performance)

### Filosofia de Design

O design do Arah é **território-primeiro, comunidade-primeiro**. Ele deve:

1. **Respeitar o contexto local**: Cada território é único; o design deve se adaptar sem perder a identidade
2. **Priorizar a relevância**: Informações territoriais devem se destacar naturalmente
3. **Facilitar conexão real**: Design que aproxima pessoas do lugar onde vivem
4. **Reduzir fricção cognitiva**: Interface limpa, hierarquia clara, ações intuitivas
5. **Cuidar sem manipular**: Engajamento autêntico, não dependência

---

## 🎨 Identidade Visual e Valores

### Valores Visuais Fundamentais

#### 1. **Território como Referência**
- Elementos visuais conectados ao lugar (mapa, geolocalização, pins)
- Cores inspiradas na natureza (verde floresta, tons terrosos)
- Texturas sutis que remetem ao natural (sem saturação excessiva)

#### 2. **Baixa Excitação e Foco**
- Paleta suave e harmoniosa (não gritante)
- Hierarquia clara (não sobrecarregada)
- Espaço em branco generoso
- Silêncio funcional (design que não compete com conteúdo)

#### 3. **Silêncio Funcional com Hierarquia Clara**
- Informações prioritárias se destacam naturalmente
- Elementos secundários discretos
- Backgrounds neutros que não competem com conteúdo
- Transparências e blur para criar camadas visuais

#### 4. **Ação Consciente e Explícita**
- CTAs claros e bem posicionados
- Estados visuais distintos (hover, active, disabled)
- Feedback imediato para todas as ações
- Confirmações para ações destrutivas

#### 5. **Evolução Lenta e Íntegra**
- Mudanças incrementais e testadas
- Consistência ao longo do tempo
- Preservação de padrões estabelecidos
- Retrocompatibilidade visual

---

## 🚀 Estratégia de Conversão e Engajamento

### Princípios de Captura de Atenção (Inspirados em Apps Top)

#### 1. **Fluidez e Baixa Fricção** (Instagram/TikTok)
- **Scroll infinito suave**: Feed territorial sem fricção, transições imperceptíveis
- **Gestos intuitivos**: Swipe, pull-to-refresh, tap duplo para interação rápida
- **Carregamento progressivo**: Skeleton loaders, imagens com fade-in suave
- **Navegação por gestos**: Bottom navigation, swipe entre tabs

**Aplicação no Arah**:
- Feed territorial com scroll fluido (velocidade 60fps)
- Pull-to-refresh suave para atualizar conteúdo local
- Swipe em cards de post para ações rápidas (favoritar, compartilhar)
- Bottom navigation fixa para acesso rápido a Feed, Mapa, Eventos, Perfil

#### 2. **Hierarquia Visual Clara** (Twitter/X)
- **Tipografia escalada**: Tamanhos distintos para importância
- **Espaçamento consistente**: Grid de 8px para alinhamento perfeito
- **Contraste inteligente**: Cores primárias para CTAs, neutros para secundários
- **Cards bem definidos**: Bordas sutis, sombras suaves, espaçamento interno generoso

**Aplicação no Arah**:
- Posts com hierarquia clara: autor em destaque, conteúdo legível, ações discretas
- Cards de território com informações prioritárias (nome, localização, membros)
- Eventos com data/hora em destaque, localização clara
- Marketplace com preços destacados, imagens em destaque

#### 3. **Microinterações e Feedback** (TikTok/Instagram)
- **Animações sutis**: Feedback visual para todas as ações
- **Haptic feedback**: Vibração discreta em ações importantes
- **Estados visuais claros**: Loading, success, error, disabled
- **Confirmações discretas**: Snackbars, toasts, badges

**Aplicação no Arah**:
- Like/curtida com animação de escala suave (0.9x → 1.0x → 0.95x → 1.0x)
- Badge de notificação com bounce sutil quando nova
- Pull-to-refresh com indicador de carregamento
- Confirmação de ações com snackbar discreto

#### 4. **Contextualização Territorial** (Arah Exclusivo)
- **Mapa integrado**: Visualização espacial imediata
- **Pins visuais**: Destaque para conteúdo georreferenciado
- **Proximidade visual**: Conteúdo próximo aparece primeiro
- **Conexão com lugar**: Elementos que remetem ao território (cores naturais, imagens locais)

**Aplicação no Arah**:
- Feed e mapa sincronizados (seleção em um reflete no outro)
- Pins coloridos por tipo (post, evento, alerta)
- Badge de "próximo a você" para conteúdo local
- Cores que remetem ao território (verde, terroso, azul céu)

---

## 🎨 Sistema de Cores

### Paleta Principal (Preservando Identidade Atual)

#### Verde Floresta (Primary)
A cor primária representa **natureza, território, comunidade viva**.

```dart
// Flutter Colors
class ArapongaColors {
  // Verde Floresta (Primary)
  static const Color forest50 = Color(0xFFF1F8F4);   // Background suave
  static const Color forest100 = Color(0xFFE2F1E8);  // Background hover
  static const Color forest200 = Color(0xFFC6E3D2);  // Border suave
  static const Color forest300 = Color(0xFF9FCEB4);  // Disabled
  static const Color forest400 = Color(0xFF6FB28C);  // Secondary CTA
  static const Color forest500 = Color(0xFF4F956F);  // Primary CTA
  static const Color forest600 = Color(0xFF377B57);  // Primary hover
  static const Color forest700 = Color(0xFF2B6246);  // Primary pressed
  static const Color forest800 = Color(0xFF214D37);  // Text em light mode
  static const Color forest900 = Color(0xFF173525);  // Text emphasis
}
```

**Uso**:
- Primary CTA (botões principais, links importantes)
- Seleção ativa (tabs, filtros, checkboxes)
- Indicadores de sucesso (badges, confirmações)
- Destaque de elementos territoriais (pins no mapa, badges de morador)

#### Azul Céu (Secondary)
Representa **confiança, informação, conexão**.

```dart
// Azul Céu (Secondary)
static const Color sky50 = Color(0xFFF0F9FF);   // Background info
static const Color sky100 = Color(0xFFE0F2FE);  // Background hover
static const Color sky200 = Color(0xFFBAE6FD);  // Border info
static const Color sky300 = Color(0xFF7DD3FC);  // Secondary info
static const Color sky400 = Color(0xFF38BDF8);  // Info CTA
static const Color sky500 = Color(0xFF0EA5E9);  // Info hover
static const Color sky600 = Color(0xFF0284C7);  // Info pressed
static const Color sky700 = Color(0xFF0369A1);  // Text info
static const Color sky800 = Color(0xFF075985);  // Text emphasis
static const Color sky900 = Color(0xFF0C4A6E);  // Text dark
```

**Uso**:
- Links e navegação
- Informações e alertas informativos
- Conteúdo de suporte (help, docs)
- Mapa e elementos geográficos

#### Tons Terrosos (Tertiary)
Representam **solo, lugar, autenticidade**.

```dart
// Tons Terrosos (Tertiary)
static const Color earth50 = Color(0xFFFAF7F4);   // Background warm
static const Color earth100 = Color(0xFFF5EFE8);  // Background hover
static const Color earth200 = Color(0xFFE8DCC9);  // Border warm
static const Color earth300 = Color(0xFFD4C4A8);  // Disabled warm
static const Color earth400 = Color(0xFFB8A082);  // Secondary warm
static const Color earth500 = Color(0xFF9C8461);  // Tertiary CTA
static const Color earth600 = Color(0xFF7A6649);  // Tertiary hover
static const Color earth700 = Color(0xFF5D4C38);  // Text warm
static const Color earth800 = Color(0xFF463A2C);  // Text emphasis
static const Color earth900 = Color(0xFF2F2620);  // Text dark
```

**Uso**:
- Elementos de marketplace
- Conteúdo de economia local
- Destaques de autenticidade
- Fundos aquecidos para storytelling

### Paleta Semântica

#### Sucesso
```dart
static const Color success50 = Color(0xFFF0FDF4);   // Background success
static const Color success100 = Color(0xFFDCFCE7);  // Background hover
static const Color success500 = Color(0xFF22C55E);  // Success CTA
static const Color success600 = Color(0xFF16A34A);  // Success hover
static const Color success700 = Color(0xFF15803D);  // Success text
```

**Uso**: Confirmações, ações bem-sucedidas, badges de verificação

#### Aviso
```dart
static const Color warning50 = Color(0xFFFEFCE8);   // Background warning
static const Color warning100 = Color(0xFFFEF9C3);  // Background hover
static const Color warning500 = Color(0xFFEAB308);  // Warning CTA
static const Color warning600 = Color(0xFFCA8A04);  // Warning hover
static const Color warning700 = Color(0xFFA16207);  // Warning text
```

**Uso**: Alertas não críticos, avisos, pendências

#### Erro/Danger
```dart
static const Color error50 = Color(0xFFFEF2F2);     // Background error
static const Color error100 = Color(0xFFFEE2E2);    // Background hover
static const Color error500 = Color(0xFFEF4444);    // Error CTA
static const Color error600 = Color(0xFFDC2626);    // Error hover
static const Color error700 = Color(0xFFB91C1C);    // Error text
```

**Uso**: Erros, ações destrutivas, reportes críticos

#### Informação
```dart
// Usar sky500, sky600, sky700 (já definidos acima)
```

**Uso**: Informações neutras, dicas, help text

### Paleta Neutra (Base para Light/Dark Mode)

#### Light Mode
```dart
static const Color gray50 = Color(0xFFFAFAFA);      // Background base
static const Color gray100 = Color(0xFFF5F5F5);     // Background elevated
static const Color gray200 = Color(0xFFE5E5E5);     // Border subtle
static const Color gray300 = Color(0xFFD4D4D4);     // Border
static const Color gray400 = Color(0xFFA3A3A3);     // Text disabled
static const Color gray500 = Color(0xFF737373);     // Text muted
static const Color gray600 = Color(0xFF525252);     // Text secondary
static const Color gray700 = Color(0xFF404040);     // Text primary
static const Color gray800 = Color(0xFF262626);     // Text emphasis
static const Color gray900 = Color(0xFF171717);     // Text dark
```

#### Dark Mode
```dart
static const Color dark50 = Color(0xFF171717);      // Background base
static const Color dark100 = Color(0xFF1F1F1F);     // Background elevated
static const Color dark200 = Color(0xFF262626);     // Border subtle
static const Color dark300 = Color(0xFF404040);     // Border
static const Color dark400 = Color(0xFF525252);     // Text disabled
static const Color dark500 = Color(0xFF737373);     // Text muted
static const Color dark600 = Color(0xFFA3A3A3);     // Text secondary
static const Color dark700 = Color(0xFFD4D4D4);     // Text primary
static const Color dark800 = Color(0xFFE5E5E5);     // Text emphasis
static const Color dark900 = Color(0xFFFAFAFA);     // Text light
```

### Diretrizes de Uso de Cores

#### Hierarquia de Cores
1. **Primary (Verde Floresta)**: Ações principais, CTAs, elementos territoriais
2. **Secondary (Azul Céu)**: Links, navegação, informações
3. **Tertiary (Tons Terrosos)**: Marketplace, economia local
4. **Semântica**: Sucesso, aviso, erro, informação (uso contextual)

#### Regras de Contraste
- **Text sobre Background**: Mínimo 4.5:1 (WCAG AA), preferencial 7:1 (WCAG AAA)
- **Text sobre CTA**: Mínimo 4.5:1
- **Ícones sobre Background**: Mínimo 3:1
- **Bordas sobre Background**: Mínimo 3:1

#### Cores Proibidas (Evitar)
- ❌ Cores vibrantes sem contexto (rosa, roxo, amarelo saturado)
- ❌ Gradientes excessivos (usar apenas em backgrounds sutis)
- ❌ Cores que não respeitam contraste mínimo
- ❌ Paletas que não seguem a identidade (cores de redes sociais concorrentes)

---

## ✍️ Tipografia

### Família de Fontes

#### Fonte Primária: Inter (ou System Fallback)
```dart
class ArapongaTypography {
  static const String primaryFont = 'Inter';
  static const String systemFallback = '-apple-system, BlinkMacSystemFont, "SF Pro Display", "Segoe UI", "Roboto", "Helvetica Neue", Arial, sans-serif';
}
```

**Características**:
- Sem serifa (moderna, limpa, legível)
- Altura de linha generosa (1.5-1.7 para corpo)
- Largura variável (suporte a weight 400-700)
- Oldstyle numbers (para dados numéricos)
- Kerning e ligatures otimizados

#### Fonte Monospace (Código, Dados)
```dart
static const String monoFont = 'SF Mono, Menlo, Monaco, "Courier New", monospace';
```

**Uso**: Códigos, IDs, timestamps, dados técnicos

### Escala Tipográfica

```dart
class ArapongaTextStyles {
  // Display (Hero, Títulos Grandes)
  static const TextStyle displayLarge = TextStyle(
    fontSize: 72,
    fontWeight: FontWeight.bold,
    letterSpacing: -2,
    height: 1.1,
  );
  
  static const TextStyle displayMedium = TextStyle(
    fontSize: 56,
    fontWeight: FontWeight.bold,
    letterSpacing: -1.5,
    height: 1.2,
  );
  
  static const TextStyle displaySmall = TextStyle(
    fontSize: 44,
    fontWeight: FontWeight.bold,
    letterSpacing: -1,
    height: 1.2,
  );
  
  // Headings (Seções, Cards)
  static const TextStyle heading1 = TextStyle(
    fontSize: 32,
    fontWeight: FontWeight.bold,
    letterSpacing: -0.5,
    height: 1.3,
  );
  
  static const TextStyle heading2 = TextStyle(
    fontSize: 24,
    fontWeight: FontWeight.bold,
    letterSpacing: -0.3,
    height: 1.4,
  );
  
  static const TextStyle heading3 = TextStyle(
    fontSize: 20,
    fontWeight: FontWeight.w600,
    letterSpacing: -0.2,
    height: 1.4,
  );
  
  static const TextStyle heading4 = TextStyle(
    fontSize: 18,
    fontWeight: FontWeight.w600,
    letterSpacing: -0.1,
    height: 1.4,
  );
  
  // Body (Conteúdo Principal)
  static const TextStyle bodyLarge = TextStyle(
    fontSize: 18,
    fontWeight: FontWeight.normal,
    letterSpacing: 0,
    height: 1.6,
  );
  
  static const TextStyle bodyMedium = TextStyle(
    fontSize: 16,
    fontWeight: FontWeight.normal,
    letterSpacing: 0,
    height: 1.6,
  );
  
  static const TextStyle bodySmall = TextStyle(
    fontSize: 14,
    fontWeight: FontWeight.normal,
    letterSpacing: 0,
    height: 1.5,
  );
  
  // Labels (Formulários, Captions)
  static const TextStyle labelLarge = TextStyle(
    fontSize: 14,
    fontWeight: FontWeight.w600,
    letterSpacing: 0.1,
    height: 1.4,
  );
  
  static const TextStyle labelMedium = TextStyle(
    fontSize: 12,
    fontWeight: FontWeight.w600,
    letterSpacing: 0.2,
    height: 1.4,
  );
  
  static const TextStyle labelSmall = TextStyle(
    fontSize: 11,
    fontWeight: FontWeight.w600,
    letterSpacing: 0.3,
    height: 1.3,
  );
  
  // Captions (Metadados, Timestamps)
  static const TextStyle caption = TextStyle(
    fontSize: 12,
    fontWeight: FontWeight.normal,
    letterSpacing: 0.2,
    height: 1.4,
  );
  
  static const TextStyle overline = TextStyle(
    fontSize: 10,
    fontWeight: FontWeight.w700,
    letterSpacing: 0.5,
    height: 1.2,
    textTransform: TextTransform.uppercase,
  );
}
```

### Diretrizes de Uso

#### Hierarquia Tipográfica
1. **Display**: Apenas para hero sections, onboarding, tela inicial
2. **Headings**: Seções, cards principais, títulos de telas
3. **Body**: Conteúdo principal (posts, descrições, textos)
4. **Labels**: Formulários, botões, campos de input
5. **Captions**: Timestamps, metadados, informações secundárias

#### Regras de Legibilidade
- **Tamanho mínimo**: 14px para body, 12px para captions
- **Line height**: 1.5-1.7 para body, 1.3-1.4 para headings
- **Letter spacing**: Negativo para headings grandes (-0.5 a -2), positivo para labels (0.1-0.3)
- **Weight**: Regular (400) para body, Semibold (600) para headings, Bold (700) para emphasis

---

## 📐 Espaçamento e Layout

### Grid System (8px Base)

```dart
class ArapongaSpacing {
  static const double xs = 4;    // 0.5 × 8px
  static const double sm = 8;    // 1 × 8px
  static const double md = 16;   // 2 × 8px
  static const double lg = 24;   // 3 × 8px
  static const double xl = 32;   // 4 × 8px
  static const double xxl = 48;  // 6 × 8px
  static const double xxxl = 64; // 8 × 8px
}
```

**Uso**:
- `xs`: Espaçamento interno de elementos pequenos (ícones, badges)
- `sm`: Espaçamento entre elementos relacionados (label + input)
- `md`: Espaçamento padrão entre elementos (cards, seções)
- `lg`: Espaçamento entre seções principais
- `xl`: Espaçamento entre blocos grandes (hero, footer)
- `xxl`: Espaçamento em telas grandes, seções principais

### Padding e Margins

```dart
class ArapongaPadding {
  // Padding interno de cards
  static const EdgeInsets cardPadding = EdgeInsets.all(16);
  static const EdgeInsets cardPaddingLarge = EdgeInsets.all(24);
  
  // Padding de telas
  static const EdgeInsets screenPadding = EdgeInsets.symmetric(horizontal: 16);
  static const EdgeInsets screenPaddingLarge = EdgeInsets.symmetric(horizontal: 24);
  
  // Padding de inputs
  static const EdgeInsets inputPadding = EdgeInsets.symmetric(horizontal: 16, vertical: 12);
  
  // Padding de botões
  static const EdgeInsets buttonPadding = EdgeInsets.symmetric(horizontal: 24, vertical: 12);
  static const EdgeInsets buttonPaddingSmall = EdgeInsets.symmetric(horizontal: 16, vertical: 8);
}
```

### Breakpoints (Responsividade)

```dart
class ArapongaBreakpoints {
  static const double mobile = 360;      // Mobile pequeno
  static const double mobileLarge = 480; // Mobile grande
  static const double tablet = 768;      // Tablet
  static const double tabletLarge = 1024; // Tablet grande
  static const double desktop = 1280;    // Desktop
}
```

**Layout Strategy**:
- **Mobile (< 768px)**: Single column, bottom navigation
- **Tablet (768px - 1024px)**: Two columns quando possível, drawer lateral
- **Desktop (> 1024px)**: Multi-column, sidebar permanente

---

## 🔲 Formas e Bordas

### Border Radius

```dart
class ArapongaRadius {
  static const double none = 0;
  static const double xs = 4;   // Inputs, badges pequenos
  static const double sm = 8;   // Botões, cards pequenos
  static const double md = 12;  // Cards padrão
  static const double lg = 16;  // Cards grandes, modais
  static const double xl = 24;  // Hero sections, splash screens
  static const double full = 9999; // Círculos, pills
}
```

**Diretrizes**:
- **xs (4px)**: Inputs, campos de formulário
- **sm (8px)**: Botões, chips, badges
- **md (12px)**: Cards padrão, avatares
- **lg (16px)**: Modais, drawers, cards grandes
- **xl (24px)**: Telas de onboarding, hero sections
- **full**: Avatares circulares, pills de filtro

### Border Width

```dart
class ArapongaBorders {
  static const double none = 0;
  static const double thin = 0.5;  // Dividers sutis
  static const double base = 1;    // Bordas padrão
  static const double thick = 2;   // Destaque, foco
}
```

**Uso**:
- **thin (0.5px)**: Dividers entre seções, separadores de lista
- **base (1px)**: Bordas de cards, inputs, botões
- **thick (2px)**: Estado de foco, seleção ativa

---

## 🎬 Transições e Animações

### Princípios de Animação

#### 1. **Natural e Orgânica**
- Animações baseadas em física (easing curves naturais)
- Durações que respeitam percepção humana (não muito rápidas, não muito lentas)
- Movimento que faz sentido (elementos aparecem de onde faz sentido)

#### 2. **Funcional, Não Decorativa**
- Cada animação serve a um propósito (feedback, orientação, hierarquia)
- Não distrai do conteúdo
- Melhora compreensão, não apenas entretenimento

#### 3. **Respeitosa**
- Durações curtas (200-400ms para microinterações, 300-600ms para transições)
- Respeita preferências de acessibilidade (reduz motion quando solicitado)
- Não causa náusea ou desconforto

### Durações Padrão

```dart
class ArapongaDurations {
  static const Duration instant = Duration(milliseconds: 0);
  static const Duration fast = Duration(milliseconds: 150);   // Microinterações
  static const Duration normal = Duration(milliseconds: 300); // Transições padrão
  static const Duration slow = Duration(milliseconds: 500);   // Transições complexas
  static const Duration slower = Duration(milliseconds: 800); // Animações de entrada
}
```

### Easing Curves

```dart
class ArapongaCurves {
  // Easing padrão (suave e natural)
  static const Curve standard = Curves.easeInOutCubic; // cubic-bezier(0.4, 0, 0.2, 1)
  
  // Entrada (elemento aparece)
  static const Curve enter = Curves.easeOutCubic; // cubic-bezier(0, 0, 0.2, 1)
  
  // Saída (elemento desaparece)
  static const Curve exit = Curves.easeInCubic; // cubic-bezier(0.4, 0, 1, 1)
  
  // Bounce suave (para feedback positivo)
  static const Curve bounce = Curves.easeOutBack; // cubic-bezier(0.34, 1.56, 0.64, 1)
  
  // Spring (para elementos que precisam de elasticidade)
  static const Curve spring = Curves.easeOutElastic;
}
```

### Animações Padrão

#### Fade In/Out
```dart
// Aparecer/desaparecer suave
AnimatedOpacity(
  opacity: isVisible ? 1.0 : 0.0,
  duration: ArapongaDurations.normal,
  curve: ArapongaCurves.standard,
)
```

**Uso**: Mensagens, toasts, overlays, modais

#### Scale (Feedback de Toque)
```dart
// Escala suave ao tocar
AnimatedScale(
  scale: isPressed ? 0.95 : 1.0,
  duration: ArapongaDurations.fast,
  curve: ArapongaCurves.standard,
)
```

**Uso**: Botões, cards clicáveis, ícones interativos

#### Slide (Transições de Tela)
```dart
// Deslizar para cima/baixo
SlideTransition(
  position: Tween<Offset>(
    begin: const Offset(0, 0.1),
    end: Offset.zero,
  ).animate(CurvedAnimation(
    parent: controller,
    curve: ArapongaCurves.enter,
  )),
  child: child,
)
```

**Uso**: Navegação entre telas, modais, bottom sheets

#### Rotate (Loading, Refresh)
```dart
// Rotação contínua para loading
RotationTransition(
  turns: Tween(begin: 0.0, end: 1.0).animate(controller),
  child: icon,
)
```

**Uso**: Indicadores de carregamento, pull-to-refresh

### Microinterações Específicas

#### Like/Curitiva (Instagram-style)
```dart
// Animação de like com bounce
AnimatedSequence([
  ScaleTransition(scale: 0.9),  // Comprimir
  ScaleTransition(scale: 1.1),  // Expandir
  ScaleTransition(scale: 1.0),  // Normalizar
])
```

**Duração**: 300ms  
**Curve**: Bounce suave

#### Pull-to-Refresh
```dart
// Rotação + fade do indicador
AnimatedRotation(
  rotation: pullDistance / 100,
  child: AnimatedOpacity(
    opacity: pullDistance > 50 ? 1.0 : pullDistance / 50,
    child: refreshIcon,
  ),
)
```

**Duração**: Instantânea (seguindo gesto)

#### Badge de Notificação
```dart
// Bounce ao aparecer nova notificação
AnimatedSequence([
  ScaleTransition(scale: 0.0),
  ScaleTransition(scale: 1.2), // Overshoot
  ScaleTransition(scale: 1.0), // Normalizar
])
```

**Duração**: 400ms  
**Curve**: Bounce

---

## 💎 Efeitos Visuais e Profundidade

### Sombras (Elevation System)

```dart
class ArapongaShadows {
  // Sem sombra (nível 0)
  static const List<BoxShadow> none = [];
  
  // Sombra sutil (nível 1)
  static const List<BoxShadow> xs = [
    BoxShadow(
      color: Color(0x0A000000), // 4% opacidade
      blurRadius: 4,
      offset: Offset(0, 2),
    ),
  ];
  
  // Sombra leve (nível 2)
  static const List<BoxShadow> sm = [
    BoxShadow(
      color: Color(0x14000000), // 8% opacidade
      blurRadius: 8,
      offset: Offset(0, 4),
    ),
  ];
  
  // Sombra padrão (nível 3)
  static const List<BoxShadow> md = [
    BoxShadow(
      color: Color(0x1A000000), // 10% opacidade
      blurRadius: 16,
      offset: Offset(0, 8),
    ),
  ];
  
  // Sombra elevada (nível 4)
  static const List<BoxShadow> lg = [
    BoxShadow(
      color: Color(0x24000000), // 14% opacidade
      blurRadius: 24,
      offset: Offset(0, 12),
    ),
  ];
  
  // Sombra destacada (nível 5)
  static const List<BoxShadow> xl = [
    BoxShadow(
      color: Color(0x33000000), // 20% opacidade
      blurRadius: 32,
      offset: Offset(0, 16),
    ),
  ];
}
```

**Uso**:
- **xs**: Dividers, separadores sutis
- **sm**: Cards hover, inputs focados
- **md**: Cards padrão, botões elevados
- **lg**: Modais, bottom sheets
- **xl**: Dialogs, overlays completos

### Glass Morphism (Preservando Identidade Visual)

```dart
class ArapongaGlass {
  // Glass card padrão
  static BoxDecoration glassCard({
    Color? backgroundColor,
    double? blur,
  }) {
    return BoxDecoration(
      color: backgroundColor ?? Colors.white.withOpacity(0.88),
      borderRadius: BorderRadius.circular(ArapongaRadius.lg),
      border: Border.all(
        color: Colors.white.withOpacity(0.7),
        width: ArapongaBorders.base,
      ),
      boxShadow: ArapongaShadows.md,
    );
  }
}
```

**Aplicar blur**:
```dart
BackdropFilter(
  filter: ImageFilter.blur(sigmaX: 16, sigmaY: 16),
  child: Container(
    decoration: ArapongaGlass.glassCard(),
  ),
)
```

**Uso**: Cards principais, modais, overlays

### Gradientes Sutis

```dart
class ArapongaGradients {
  // Background suave (inspirado em devportal)
  static const LinearGradient backgroundSubtle = LinearGradient(
    begin: Alignment.topLeft,
    end: Alignment.bottomRight,
    colors: [
      Color(0xFFF1F8F4), // forest50
      Color(0xFFF0F9FF), // sky50
    ],
  );
  
  // Overlay escuro para modais
  static const LinearGradient overlayDark = LinearGradient(
    begin: Alignment.topCenter,
    end: Alignment.bottomCenter,
    colors: [
      Color(0x80000000), // 50% preto
      Color(0xCC000000), // 80% preto
    ],
  );
}
```

**Uso**: Apenas backgrounds sutis, nunca em elementos interativos

---

## 🧩 Componentes e Padrões

### Botões

#### Primary Button
```dart
ElevatedButton(
  style: ElevatedButton.styleFrom(
    backgroundColor: ArapongaColors.forest500,
    foregroundColor: Colors.white,
    padding: ArapongaPadding.buttonPadding,
    shape: RoundedRectangleBorder(
      borderRadius: BorderRadius.circular(ArapongaRadius.sm),
    ),
    elevation: 0,
    shadowColor: Colors.transparent,
  ),
  onPressed: () {},
  child: Text('Ação Principal', style: ArapongaTextStyles.labelLarge),
)
```

**Estados**:
- **Normal**: `forest500`, sem elevação
- **Hover**: `forest600`, sombra `sm`
- **Pressed**: `forest700`, escala 0.98
- **Disabled**: `forest300`, opacity 0.5

#### Secondary Button
```dart
OutlinedButton(
  style: OutlinedButton.styleFrom(
    foregroundColor: ArapongaColors.forest600,
    side: BorderSide(color: ArapongaColors.forest500, width: 1),
    padding: ArapongaPadding.buttonPadding,
    shape: RoundedRectangleBorder(
      borderRadius: BorderRadius.circular(ArapongaRadius.sm),
    ),
  ),
  onPressed: () {},
  child: Text('Ação Secundária', style: ArapongaTextStyles.labelLarge),
)
```

#### Text Button
```dart
TextButton(
  style: TextButton.styleFrom(
    foregroundColor: ArapongaColors.sky600,
    padding: ArapongaPadding.buttonPaddingSmall,
  ),
  onPressed: () {},
  child: Text('Link', style: ArapongaTextStyles.labelMedium),
)
```

### Cards

#### Card Padrão
```dart
Container(
  decoration: BoxDecoration(
    color: Colors.white, // Light mode
    borderRadius: BorderRadius.circular(ArapongaRadius.md),
    border: Border.all(
      color: ArapongaColors.gray200,
      width: ArapongaBorders.base,
    ),
    boxShadow: ArapongaShadows.sm,
  ),
  padding: ArapongaPadding.cardPadding,
  child: content,
)
```

**Estados**:
- **Normal**: Sombra `sm`, borda sutil
- **Hover**: Sombra `md`, elevação +2px, transição suave
- **Pressed**: Sombra `xs`, elevação -1px

#### Glass Card (Preservando Identidade)
```dart
Container(
  decoration: ArapongaGlass.glassCard(),
  child: BackdropFilter(
    filter: ImageFilter.blur(sigmaX: 16, sigmaY: 16),
    child: Padding(
      padding: ArapongaPadding.cardPaddingLarge,
      child: content,
    ),
  ),
)
```

### Inputs

#### Text Field
```dart
TextField(
  style: ArapongaTextStyles.bodyMedium,
  decoration: InputDecoration(
    labelText: 'Label',
    hintText: 'Placeholder',
    labelStyle: ArapongaTextStyles.labelMedium.copyWith(
      color: ArapongaColors.gray600,
    ),
    hintStyle: ArapongaTextStyles.bodyMedium.copyWith(
      color: ArapongaColors.gray400,
    ),
    filled: true,
    fillColor: ArapongaColors.gray50,
    border: OutlineInputBorder(
      borderRadius: BorderRadius.circular(ArapongaRadius.xs),
      borderSide: BorderSide(
        color: ArapongaColors.gray300,
        width: ArapongaBorders.base,
      ),
    ),
    enabledBorder: OutlineInputBorder(
      borderRadius: BorderRadius.circular(ArapongaRadius.xs),
      borderSide: BorderSide(
        color: ArapongaColors.gray300,
        width: ArapongaBorders.base,
      ),
    ),
    focusedBorder: OutlineInputBorder(
      borderRadius: BorderRadius.circular(ArapongaRadius.xs),
      borderSide: BorderSide(
        color: ArapongaColors.forest500,
        width: ArapongaBorders.thick,
      ),
    ),
    errorBorder: OutlineInputBorder(
      borderRadius: BorderRadius.circular(ArapongaRadius.xs),
      borderSide: BorderSide(
        color: ArapongaColors.error500,
        width: ArapongaBorders.base,
      ),
    ),
    contentPadding: ArapongaPadding.inputPadding,
  ),
)
```

### Badges

#### Badge Padrão
```dart
Container(
  padding: EdgeInsets.symmetric(
    horizontal: ArapongaSpacing.sm,
    vertical: ArapongaSpacing.xs,
  ),
  decoration: BoxDecoration(
    color: ArapongaColors.forest100,
    borderRadius: BorderRadius.circular(ArapongaRadius.full),
  ),
  child: Text(
    'Badge',
    style: ArapongaTextStyles.labelSmall.copyWith(
      color: ArapongaColors.forest700,
    ),
  ),
)
```

**Variações**:
- **Success**: `success100` background, `success700` text
- **Warning**: `warning100` background, `warning700` text
- **Error**: `error100` background, `error700` text
- **Info**: `sky100` background, `sky700` text

---

## 🧭 Hierarquia Visual e Navegação

### Princípios de Hierarquia

1. **Conteúdo Primeiro**: Conteúdo territorial deve ser o foco
2. **Navegação Discreta**: Navegação existe para apoiar, não competir
3. **Ações Claras**: CTAs devem ser óbvios, mas não gritantes
4. **Informação Progressiva**: Mostrar o essencial, expandir quando necessário

### Bottom Navigation (Mobile-First)

```dart
BottomNavigationBar(
  type: BottomNavigationBarType.fixed,
  backgroundColor: Colors.white,
  selectedItemColor: ArapongaColors.forest500,
  unselectedItemColor: ArapongaColors.gray400,
  selectedLabelStyle: ArapongaTextStyles.labelSmall,
  unselectedLabelStyle: ArapongaTextStyles.labelSmall,
  elevation: 8,
  items: [
    BottomNavigationBarItem(
      icon: Icon(Icons.home_outlined),
      activeIcon: Icon(Icons.home),
      label: 'Feed',
    ),
    BottomNavigationBarItem(
      icon: Icon(Icons.map_outlined),
      activeIcon: Icon(Icons.map),
      label: 'Mapa',
    ),
    BottomNavigationBarItem(
      icon: Icon(Icons.event_outlined),
      activeIcon: Icon(Icons.event),
      label: 'Eventos',
    ),
    BottomNavigationBarItem(
      icon: Icon(Icons.notifications_outlined),
      activeIcon: Icon(Icons.notifications),
      label: 'Alertas',
    ),
    BottomNavigationBarItem(
      icon: Icon(Icons.person_outline),
      activeIcon: Icon(Icons.person),
      label: 'Perfil',
    ),
  ],
)
```

**Características**:
- Fixa na parte inferior
- 5 itens principais (Feed, Mapa, Eventos, Alertas, Perfil)
- Ícones outline quando inativo, filled quando ativo
- Badge de notificação sobre ícone de Alertas

### App Bar (Top Navigation)

```dart
AppBar(
  backgroundColor: Colors.white,
  elevation: 0,
  shadowColor: Colors.transparent,
  title: Text(
    'Título da Tela',
    style: ArapongaTextStyles.heading3,
  ),
  actions: [
    IconButton(
      icon: Icon(Icons.search),
      color: ArapongaColors.gray700,
      onPressed: () {},
    ),
    // Outros actions
  ],
)
```

**Características**:
- Background branco (light mode) ou dark (dark mode)
- Sem elevação (flat)
- Título em `heading3`
- Ações com ícones discretos

### Drawer (Navegação Lateral)

```dart
Drawer(
  child: ListView(
    padding: EdgeInsets.zero,
    children: [
      DrawerHeader(
        decoration: BoxDecoration(
          color: ArapongaColors.forest500,
        ),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            CircleAvatar(
              radius: 30,
              backgroundColor: Colors.white,
            ),
            SizedBox(height: ArapongaSpacing.md),
            Text(
              'Nome do Usuário',
              style: ArapongaTextStyles.heading4.copyWith(
                color: Colors.white,
              ),
            ),
            Text(
              '@username',
              style: ArapongaTextStyles.bodySmall.copyWith(
                color: Colors.white.withOpacity(0.8),
              ),
            ),
          ],
        ),
      ),
      ListTile(
        leading: Icon(Icons.territory),
        title: Text('Meus Territórios'),
        onTap: () {},
      ),
      // Outros itens
    ],
  ),
)
```

---

## 💬 Estados e Feedback

### Loading States

#### Skeleton Loader
```dart
Shimmer(
  gradient: LinearGradient(
    colors: [
      ArapongaColors.gray200,
      ArapongaColors.gray100,
      ArapongaColors.gray200,
    ],
    stops: [0.0, 0.5, 1.0],
  ),
  child: Container(
    height: 100,
    decoration: BoxDecoration(
      color: ArapongaColors.gray200,
      borderRadius: BorderRadius.circular(ArapongaRadius.md),
    ),
  ),
)
```

**Uso**: Feed, cards, listas durante carregamento

#### Circular Progress
```dart
CircularProgressIndicator(
  valueColor: AlwaysStoppedAnimation<Color>(ArapongaColors.forest500),
  strokeWidth: 3,
)
```

**Uso**: Botões com loading, ações que requerem espera

#### Linear Progress
```dart
LinearProgressIndicator(
  valueColor: AlwaysStoppedAnimation<Color>(ArapongaColors.forest500),
  backgroundColor: ArapongaColors.gray200,
  minHeight: 4,
)
```

**Uso**: Upload de arquivos, progresso de formulário

### Empty States

```dart
Column(
  mainAxisAlignment: MainAxisAlignment.center,
  children: [
    Icon(
      Icons.inbox_outlined,
      size: 64,
      color: ArapongaColors.gray400,
    ),
    SizedBox(height: ArapongaSpacing.lg),
    Text(
      'Nenhum conteúdo ainda',
      style: ArapongaTextStyles.heading3.copyWith(
        color: ArapongaColors.gray700,
      ),
    ),
    SizedBox(height: ArapongaSpacing.sm),
    Text(
      'Quando houver conteúdo do território, ele aparecerá aqui.',
      style: ArapongaTextStyles.bodyMedium.copyWith(
        color: ArapongaColors.gray500,
      ),
      textAlign: TextAlign.center,
    ),
    SizedBox(height: ArapongaSpacing.xl),
    ElevatedButton(
      onPressed: () {},
      child: Text('Criar Primeiro Post'),
    ),
  ],
)
```

**Características**:
- Ícone grande e discreto
- Título claro
- Descrição útil
- CTA quando aplicável

### Error States

#### Snackbar (Erro Simples)
```dart
ScaffoldMessenger.of(context).showSnackBar(
  SnackBar(
    content: Text('Erro ao carregar conteúdo'),
    backgroundColor: ArapongaColors.error500,
    behavior: SnackBarBehavior.floating,
    shape: RoundedRectangleBorder(
      borderRadius: BorderRadius.circular(ArapongaRadius.sm),
    ),
    action: SnackBarAction(
      label: 'Tentar Novamente',
      textColor: Colors.white,
      onPressed: () {},
    ),
  ),
)
```

#### Dialog (Erro Crítico)
```dart
showDialog(
  context: context,
  builder: (context) => AlertDialog(
    title: Text('Erro'),
    content: Text('Não foi possível completar a ação. Tente novamente.'),
    actions: [
      TextButton(
        onPressed: () => Navigator.pop(context),
        child: Text('Fechar'),
      ),
      ElevatedButton(
        onPressed: () {},
        child: Text('Tentar Novamente'),
      ),
    ],
  ),
)
```

### Success States

#### Toast (Sucesso Simples)
```dart
ScaffoldMessenger.of(context).showSnackBar(
  SnackBar(
    content: Row(
      children: [
        Icon(Icons.check_circle, color: Colors.white),
        SizedBox(width: ArapongaSpacing.sm),
        Text('Ação realizada com sucesso'),
      ],
    ),
    backgroundColor: ArapongaColors.success500,
    behavior: SnackBarBehavior.floating,
    duration: Duration(seconds: 2),
    shape: RoundedRectangleBorder(
      borderRadius: BorderRadius.circular(ArapongaRadius.sm),
    ),
  ),
)
```

---

## ✨ Microinterações e Impressões

### Haptic Feedback

```dart
// Sucesso (light impact)
HapticFeedback.lightImpact();

// Erro (medium impact)
HapticFeedback.mediumImpact();

// Confirmação importante (heavy impact)
HapticFeedback.heavyImpact();
```

**Uso**:
- **Light**: Likes, favoritos, toggle switches
- **Medium**: Ações importantes (criar post, confirmar)
- **Heavy**: Erros críticos, confirmações destrutivas

### Gestos Nativos

#### Pull-to-Refresh
```dart
RefreshIndicator(
  onRefresh: () async {
    // Atualizar conteúdo
  },
  color: ArapongaColors.forest500,
  child: ListView(
    children: [...],
  ),
)
```

#### Swipe Actions
```dart
Slidable(
  startActionPane: ActionPane(
    motion: DrawerMotion(),
    children: [
      SlidableAction(
        onPressed: (_) {},
        backgroundColor: ArapongaColors.error500,
        icon: Icons.delete,
        label: 'Deletar',
      ),
    ],
  ),
  child: Card(...),
)
```

**Uso**: Cards de post, itens de lista, ações rápidas

#### Long Press (Context Menu)
```dart
GestureDetector(
  onLongPress: () {
    showModalBottomSheet(
      context: context,
      builder: (context) => ContextMenu(...),
    );
  },
  child: Card(...),
)
```

**Uso**: Posts, comentários, opções adicionais

### Transições de Tela

#### Fade Transition
```dart
PageRouteBuilder(
  pageBuilder: (context, animation, secondaryAnimation) => NextScreen(),
  transitionsBuilder: (context, animation, secondaryAnimation, child) {
    return FadeTransition(
      opacity: animation,
      child: child,
    );
  },
  transitionDuration: ArapongaDurations.normal,
)
```

**Uso**: Navegação entre telas relacionadas

#### Slide Transition
```dart
PageRouteBuilder(
  pageBuilder: (context, animation, secondaryAnimation) => NextScreen(),
  transitionsBuilder: (context, animation, secondaryAnimation, child) {
    const begin = Offset(1.0, 0.0); // Da direita
    const end = Offset.zero;
    final tween = Tween(begin: begin, end: end);
    final offsetAnimation = animation.drive(tween);
    
    return SlideTransition(
      position: offsetAnimation,
      child: child,
    );
  },
  transitionDuration: ArapongaDurations.normal,
)
```

**Uso**: Navegação hierárquica (detalhes, configurações)

---

## ♿ Acessibilidade

### Contraste

- **Text sobre Background**: Mínimo 4.5:1 (WCAG AA), preferencial 7:1 (WCAG AAA)
- **Large Text (18px+)**: Mínimo 3:1 (WCAG AA)
- **UI Components (botões, inputs)**: Mínimo 3:1 (WCAG AA)

### Tamanhos de Toque

- **Área mínima de toque**: 44×44px (iOS) / 48×48dp (Material)
- **Espaçamento entre elementos clicáveis**: Mínimo 8px

### Semântica

```dart
Semantics(
  label: 'Botão de criar post',
  hint: 'Duplo toque para criar novo post',
  button: true,
  child: FloatingActionButton(...),
)
```

### Redução de Movimento

```dart
// Respeitar preferência de acessibilidade
MediaQuery.of(context).disableAnimations
```

**Aplicação**: Desabilitar animações quando usuário prefere reduzir movimento

### Tamanhos de Fonte Escaláveis

```dart
// Usar escalas relativas
Text(
  'Conteúdo',
  style: ArapongaTextStyles.bodyMedium.copyWith(
    fontSize: MediaQuery.of(context).textScaleFactor * 16,
  ),
)
```

---

## 📱 Responsividade e Adaptação

### Layout Adaptativo

```dart
LayoutBuilder(
  builder: (context, constraints) {
    if (constraints.maxWidth < ArapongaBreakpoints.tablet) {
      // Mobile layout
      return MobileLayout();
    } else if (constraints.maxWidth < ArapongaBreakpoints.desktop) {
      // Tablet layout
      return TabletLayout();
    } else {
      // Desktop layout
      return DesktopLayout();
    }
  },
)
```

### Grid Adaptativo

```dart
GridView.builder(
  gridDelegate: SliverGridDelegateWithAdaptiveCrossAxisCount(
    crossAxisCount: MediaQuery.of(context).size.width < 600 ? 1 : 2,
    mainAxisSpacing: ArapongaSpacing.md,
    crossAxisSpacing: ArapongaSpacing.md,
    childAspectRatio: 1.2,
  ),
  itemBuilder: (context, index) => Card(...),
)
```

### Padding Adaptativo

```dart
Padding(
  padding: EdgeInsets.symmetric(
    horizontal: MediaQuery.of(context).size.width < 600 
      ? ArapongaSpacing.md 
      : ArapongaSpacing.xl,
  ),
  child: content,
)
```

---

## 🌙 Dark Mode

### Paleta Dark Mode

```dart
class ArapongaDarkColors {
  // Backgrounds
  static const Color background = Color(0xFF0F1419); // dark100
  static const Color surface = Color(0xFF141A21);   // dark200
  static const Color elevated = Color(0xFF1A2129);  // dark300
  
  // Text
  static const Color textPrimary = Color(0xFFE8EDF2);   // dark700
  static const Color textSecondary = Color(0xFFB8C5D2); // dark600
  static const Color textMuted = Color(0xFF8A97A4);     // dark500
  
  // Borders
  static const Color border = Color(0xFF25303A);      // dark400
  static const Color borderSubtle = Color(0xFF1E2830); // dark300
  
  // Accents (mantém cores primárias, ajusta luminosidade)
  static const Color accent = Color(0xFF4DD4A8);     // forest500 (mais luminoso)
  static const Color accentHover = Color(0xFF5EE5B9); // forest400
}
```

### Tema Dark Mode

```dart
ThemeData(
  brightness: Brightness.dark,
  primaryColor: ArapongaDarkColors.accent,
  scaffoldBackgroundColor: ArapongaDarkColors.background,
  cardColor: ArapongaDarkColors.surface,
  textTheme: TextTheme(
    bodyLarge: ArapongaTextStyles.bodyLarge.copyWith(
      color: ArapongaDarkColors.textPrimary,
    ),
  ),
)
```

### Transição Suave Dark/Light

```dart
AnimatedTheme(
  data: isDarkMode ? darkTheme : lightTheme,
  duration: ArapongaDurations.normal,
  child: MaterialApp(...),
)
```

---

## 🖼️ Imagens e Mídia

### Imagens de Perfil (Avatar)

```dart
CircleAvatar(
  radius: 24,
  backgroundColor: ArapongaColors.gray200,
  backgroundImage: imageUrl != null ? NetworkImage(imageUrl) : null,
  child: imageUrl == null 
    ? Text(
        initials,
        style: ArapongaTextStyles.labelLarge,
      )
    : null,
)
```

**Tamanhos**:
- **Small**: 24px (comentários, listas)
- **Medium**: 40px (posts, cards)
- **Large**: 64px (perfil, detalhes)

### Imagens em Posts

```dart
CachedNetworkImage(
  imageUrl: imageUrl,
  fit: BoxFit.cover,
  placeholder: (context, url) => Shimmer(...),
  errorWidget: (context, url, error) => Container(
    color: ArapongaColors.gray200,
    child: Icon(Icons.broken_image, color: ArapongaColors.gray400),
  ),
)
```

**Aspect Ratios**:
- **Post**: 16:9 ou 4:3 (landscape)
- **Story/Highlight**: 9:16 (portrait)
- **Perfil**: 1:1 (square)

### Galeria de Imagens

```dart
GridView.builder(
  gridDelegate: SliverGridDelegateWithFixedCrossAxisCount(
    crossAxisCount: 3,
    mainAxisSpacing: 2,
    crossAxisSpacing: 2,
  ),
  itemBuilder: (context, index) => GestureDetector(
    onTap: () => showImageViewer(images, index),
    child: CachedNetworkImage(...),
  ),
)
```

---

## 🎯 Iconografia

### Tamanhos de Ícones

```dart
class ArapongaIconSizes {
  static const double xs = 16;
  static const double sm = 20;
  static const double md = 24;
  static const double lg = 32;
  static const double xl = 48;
}
```

**Uso**:
- **xs**: Badges, chips pequenos
- **sm**: Listas, cards compactos
- **md**: Botões, navegação (padrão)
- **lg**: Headers, destaques
- **xl**: Empty states, ilustrações

### Estilo de Ícones

- **Material Icons**: Principal (consistente com Material Design)
- **Outline quando inativo**: `Icons.home_outlined`
- **Filled quando ativo**: `Icons.home`
- **Cores semânticas**: Usar cores da paleta, não apenas preto/cinza

### Ícones Customizados (Quando Necessário)

- Manter estilo consistente (stroke width, corner radius)
- Tamanho mínimo: 24px para detalhes visíveis
- Exportar em múltiplas densidades (@1x, @2x, @3x)

---

## 📋 Guidelines Específicas por Feature

### Feed Territorial

**Hierarquia Visual**:
1. **Post Card** (elevação `md`, padding `lg`)
   - Header: Avatar + Nome + Timestamp (linha única)
   - Conteúdo: Texto (body medium), imagens (16:9)
   - Ações: Like, Comentário, Compartilhar (ícones discretos)
   - Geolocalização: Badge "📍 Próximo" quando aplicável

**Estados**:
- **Loading**: Skeleton com 3 cards
- **Empty**: Ilustração + "Seja o primeiro a postar"
- **Error**: Retry button + mensagem

**Interações**:
- Tap no card → Abre detalhes
- Swipe left → Favoritar
- Swipe right → Compartilhar
- Long press → Menu contextual

### Mapa Territorial

**Hierarquia Visual**:
1. **Mapa** (full screen, Google Maps)
2. **Pins Coloridos**:
   - Post: Verde (`forest500`)
   - Evento: Azul (`sky500`)
   - Alerta: Laranja (`warning500`)
   - Asset: Terroso (`earth500`)
3. **Bottom Sheet** (seleção de pin):
   - Preview do conteúdo
   - Ação "Ver Detalhes"

**Interações**:
- Tap no pin → Abre bottom sheet
- Drag do mapa → Atualiza pins visíveis
- Zoom → Agrupa pins próximos (clustering)

### Eventos

**Hierarquia Visual**:
1. **Card de Evento**:
   - Imagem de capa (16:9)
   - Badge de data/hora (destaque)
   - Título (heading3)
   - Localização (ícone + endereço)
   - Botão "Participar" (primary)

**Estados**:
- **Próximo**: Badge verde "Em breve"
- **Acontecendo**: Badge azul "Ao vivo"
- **Passado**: Opacidade reduzida, badge "Finalizado"

### Marketplace

**Hierarquia Visual**:
1. **Card de Item**:
   - Imagem do produto (1:1)
   - Título (heading4)
   - Preço (heading3, `forest600`)
   - Loja (caption)
   - Botão "Contatar" (secondary)

**Filtros**:
- Chips horizontais scrolláveis
- Active: `forest500` background, white text
- Inactive: `gray200` background, `gray700` text

---

## ✅ Checklist de Implementação

### Por Componente

#### ✅ Botão
- [ ] Estilos definidos (primary, secondary, text)
- [ ] Estados implementados (hover, pressed, disabled)
- [ ] Haptic feedback configurado
- [ ] Animações de toque (scale)
- [ ] Contraste verificado (WCAG AA)
- [ ] Tamanho mínimo de toque (44×44px)

#### ✅ Card
- [ ] Elevação correta (shadow system)
- [ ] Padding consistente
- [ ] Border radius aplicado
- [ ] Estados de hover/press
- [ ] Transições suaves

#### ✅ Input
- [ ] Estados definidos (normal, focused, error, disabled)
- [ ] Label e placeholder estilizados
- [ ] Feedback de validação
- [ ] Contraste de texto verificado

#### ✅ Navegação
- [ ] Bottom navigation (mobile)
- [ ] Drawer (tablet/desktop)
- [ ] App bar consistente
- [ ] Breadcrumbs quando necessário
- [ ] Deep linking configurado

### Por Tela

#### ✅ Feed
- [ ] Skeleton loader
- [ ] Pull-to-refresh
- [ ] Paginação infinita
- [ ] Empty state
- [ ] Error state com retry
- [ ] Transições de navegação

#### ✅ Mapa
- [ ] Integração com Google Maps
- [ ] Pins coloridos por tipo
- [ ] Clustering de pins
- [ ] Bottom sheet de detalhes
- [ ] Geolocalização do usuário

#### ✅ Perfil
- [ ] Avatar editável
- [ ] Bio editável
- [ ] Lista de posts
- [ ] Estatísticas
- [ ] Configurações acessíveis

---

## 🎓 Referências e Inspiração

### Apps Top de Captura de Atenção (Adaptadas)
- **Instagram**: Scroll infinito, gestos intuitivos, microinterações
- **TikTok**: Fluidez, feedback imediato, transições suaves
- **Twitter/X**: Hierarquia clara, tipografia escalada, espaçamento generoso

### Princípios Aplicados no Arah
- **Baixa fricção**: Minimizar passos para ações comuns
- **Fluidez**: Animações 60fps, transições suaves
- **Contexto territorial**: Conteúdo local em destaque
- **Autenticidade**: Design que não manipula, apenas facilita

---

## 📝 Notas Finais

### Preservação da Identidade
- ✅ Paleta verde floresta mantida
- ✅ Glass morphism preservado
- ✅ Watermark sutil do logo
- ✅ Tons terrosos para economia local
- ✅ Azul céu para informações

### Evolução Contínua
- Este documento deve evoluir com feedback dos usuários
- Mudanças incrementais, não revoluções
- Testes A/B para grandes mudanças
- Validação com comunidade antes de implementar

---

**Status**: 🎨 Diretrizes Oficiais de Design  
**Versão**: 1.0  
**Última Atualização**: 2025-01-20

**Documentação relacionada**:
- [Planejamento do Frontend Flutter](./24_FLUTTER_FRONTEND_PLAN.md)
- [Roadmap de Implementação](./25_FLUTTER_IMPLEMENTATION_ROADMAP.md)
- [Contribuindo - Design e Coerência](../CONTRIBUTING.md)
