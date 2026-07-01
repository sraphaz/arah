# Guia de Acessibilidade - Arah Flutter App

**Versão**: 1.0  
**Data**: 2025-01-20  
**Status**: ♿ Guia Completo de Acessibilidade  
**Tipo**: Documentação Técnica de Acessibilidade

---

## 📋 Índice

1. [Visão Geral](#visão-geral)
2. [Padrões e Conformidade](#padrões-e-conformidade)
3. [Semântica e Labels](#semântica-e-labels)
4. [Navegação e Foco](#navegação-e-foco)
5. [Contraste e Cores](#contraste-e-cores)
6. [Tipografia e Legibilidade](#tipografia-e-legibilidade)
7. [Tamanhos de Toque](#tamanhos-de-toque)
8. [Screen Readers](#screen-readers)
9. [Redução de Movimento](#redução-de-movimento)
10. [Testes de Acessibilidade](#testes-de-acessibilidade)
11. [Checklist WCAG AA](#checklist-wcag-aa)
12. [Boas Práticas](#boas-práticas)

---

## 🎯 Visão Geral

### Objetivo

Este documento especifica as **diretrizes completas de acessibilidade** para o app Flutter Arah, garantindo que o aplicativo seja acessível para todos os usuários, incluindo pessoas com deficiências visuais, auditivas, motoras e cognitivas.

### Princípios Fundamentais

1. **Acessibilidade Universal**: O app deve ser acessível para todos os usuários
2. **Conformidade WCAG AA**: Seguir padrões WCAG 2.1 Level AA como mínimo
3. **Inclusão**: Design inclusivo desde o início, não como adição posterior
4. **Testes Contínuos**: Testar acessibilidade durante todo o desenvolvimento
5. **Feedback do Usuário**: Ouvir e incorporar feedback de usuários com deficiências

### Conformidade

- **WCAG 2.1 Level AA**: Mínimo obrigatório
- **WCAG 2.1 Level AAA**: Onde possível
- **Material Design Accessibility**: Seguir diretrizes do Material Design 3
- **Platform Guidelines**: iOS (VoiceOver) e Android (TalkBack)

---

## 📏 Padrões e Conformidade

### WCAG 2.1 Level AA

**Princípios WCAG**:
1. **Perceptível**: Informações e UI devem ser apresentáveis de forma que os usuários possam percebê-las
2. **Operável**: Componentes de UI e navegação devem ser operáveis
3. **Compreensível**: Informações e operação da UI devem ser compreensíveis
4. **Robusto**: O conteúdo deve ser robusto o suficiente para ser interpretado por assistivas

**Critérios AA Obrigatórios**:
- ✅ Contraste de cor mínimo 4.5:1 (texto normal) ou 3:1 (texto grande)
- ✅ Funcionalidade disponível via teclado (navegação sem mouse)
- ✅ Labels e nomes semânticos para todos os componentes
- ✅ Ordem de foco lógica e previsível
- ✅ Mensagens de erro claras e descritivas
- ✅ Navegação consistente
- ✅ Tamanho mínimo de toque 44x44 pontos (iOS) ou 48x48 dp (Android)

---

## 🏷️ Semântica e Labels

### Semântica Flutter

**Semantics Widget**: Widget para fornecer informações semânticas aos assistentes de tela

```dart
// Exemplo: Botão com semântica completa
Semantics(
  label: 'Criar novo post',
  hint: 'Double tap to create a new post',
  button: true,
  enabled: true,
  child: FloatingActionButton(
    onPressed: () => _createPost(),
    child: Icon(Icons.add),
  ),
)

// Exemplo: Campo de texto com label e hint
Semantics(
  label: 'Título do post',
  hint: 'Enter post title (required)',
  textField: true,
  required: true,
  child: TextField(
    decoration: InputDecoration(
      labelText: 'Título do post',
      hintText: 'Digite o título...',
      helperText: 'Máximo 200 caracteres',
    ),
  ),
)
```

### Labels Semânticos

**Regras**:
- **Labels devem ser descritivos**: "Criar post" ao invés de "Botão"
- **Labels devem ser únicos**: Cada elemento deve ter um label único
- **Labels devem ser concisos**: Evitar textos longos desnecessários

**Exemplos**:
```dart
// ✅ BOM
Semantics(
  label: 'Curtir post de João',
  button: true,
  child: IconButton(
    icon: Icon(Icons.favorite),
    onPressed: () => _likePost(),
  ),
)

// ❌ RUIM
Semantics(
  label: 'Botão',
  child: IconButton(icon: Icon(Icons.favorite), onPressed: () {}),
)
```

### Hints Semânticos

**Uso**: Fornecer informações adicionais sobre ações disponíveis

```dart
Semantics(
  label: 'Post de João',
  hint: 'Double tap to view post details, swipe right to like',
  child: PostCard(post: post),
)
```

### States Semânticos

**Estados importantes**:
- `enabled` / `disabled`
- `selected` / `unselected`
- `checked` / `unchecked`
- `expanded` / `collapsed`
- `loading`
- `error`

```dart
Semantics(
  label: 'Salvar post',
  enabled: !isLoading,
  button: true,
  child: ElevatedButton(
    onPressed: isLoading ? null : _savePost,
    child: isLoading ? CircularProgressIndicator() : Text('Salvar'),
  ),
)
```

---

## 🎯 Navegação e Foco

### Ordem de Foco

**Regra**: A ordem de foco deve ser lógica e previsível (geralmente da esquerda para direita, de cima para baixo)

```dart
// Usar FocusNode para controlar ordem de foco
class _LoginScreenState extends State<LoginScreen> {
  final _emailFocusNode = FocusNode();
  final _passwordFocusNode = FocusNode();
  final _loginButtonFocusNode = FocusNode();
  
  @override
  void dispose() {
    _emailFocusNode.dispose();
    _passwordFocusNode.dispose();
    _loginButtonFocusNode.dispose();
    super.dispose();
  }
  
  @override
  Widget build(BuildContext context) {
    return Column(
      children: [
        TextField(
          focusNode: _emailFocusNode,
          onSubmitted: (_) => _passwordFocusNode.requestFocus(),
          // ...
        ),
        TextField(
          focusNode: _passwordFocusNode,
          onSubmitted: (_) => _loginButtonFocusNode.requestFocus(),
          // ...
        ),
        ElevatedButton(
          focusNode: _loginButtonFocusNode,
          onPressed: _login,
          // ...
        ),
      ],
    );
  }
}
```

### Skip Links

**Uso**: Permitir pular conteúdo repetitivo (ex: navegação principal)

```dart
Semantics(
  label: 'Skip to main content',
  button: true,
  child: InkWell(
    onTap: () => _scrollToMainContent(),
    child: Padding(
      padding: EdgeInsets.all(8),
      child: Text('Pular para conteúdo principal'),
    ),
  ),
)
```

### Foco Visível

**Regra**: O foco deve ser claramente visível (alcançar 3:1 de contraste)

```dart
// Material Design 3 já inclui foco visível automático
// Para customizar:
Focus(
  onFocusChange: (hasFocus) {
    if (hasFocus) {
      // Indicador visual de foco customizado
    }
  },
  child: CustomButton(),
)
```

---

## 🎨 Contraste e Cores

### Contraste Mínimo (WCAG AA)

**Texto Normal (< 18pt)**:
- Contraste mínimo: **4.5:1**
- Exemplo: `neutral900` (#171717) sobre `neutral50` (#FAFAFA) = 15.8:1 ✅

**Texto Grande (≥ 18pt ou ≥ 14pt bold)**:
- Contraste mínimo: **3:1**
- Exemplo: `neutral800` (#262626) sobre `neutral100` (#F5F5F5) = 11.4:1 ✅

**Componentes Não Textuais**:
- Contraste mínimo: **3:1**
- Exemplo: Bordas, ícones, gráficos

### Verificação de Contraste

**Ferramentas**:
- **WebAIM Contrast Checker**: [https://webaim.org/resources/contrastchecker/](https://webaim.org/resources/contrastchecker/)
- **Material Design Color Tool**: [https://material.io/resources/color/](https://material.io/resources/color/)

**Código**:
```dart
// Calcular contraste programaticamente
double calculateContrast(Color foreground, Color background) {
  final fgLuminance = foreground.computeLuminance();
  final bgLuminance = background.computeLuminance();
  
  final lighter = fgLuminance > bgLuminance ? fgLuminance : bgLuminance;
  final darker = fgLuminance < bgLuminance ? fgLuminance : bgLuminance;
  
  return (lighter + 0.05) / (darker + 0.05);
}

// Verificar se contraste atende WCAG AA
bool meetsWCAGAA(Color foreground, Color background, {bool isLargeText = false}) {
  final contrast = calculateContrast(foreground, background);
  return isLargeText ? contrast >= 3.0 : contrast >= 4.5;
}
```

### Não Depender Apenas de Cor

**Regra**: Não usar apenas cor para transmitir informações importantes

**Exemplos**:
```dart
// ✅ BOM: Usar ícone + cor
Row(
  children: [
    Icon(Icons.error, color: Colors.red),
    Text('Erro ao salvar'),
  ],
)

// ❌ RUIM: Apenas cor
Container(
  color: Colors.red,
  child: Text('Erro'),
)
```

---

## 📖 Tipografia e Legibilidade

### Tamanho de Fonte Mínimo

**Regra**: Texto mínimo de 14sp (Android) ou 14pt (iOS)

**Material Design 3**:
- BodySmall: 14sp ✅
- BodyMedium: 16sp ✅
- BodyLarge: 18sp ✅

### Escalabilidade

**Regra**: Textos devem ser escaláveis até 200% sem perda de funcionalidade

```dart
// Respeitar configurações de acessibilidade do sistema
MediaQuery(
  data: MediaQuery.of(context).copyWith(
    textScaler: MediaQuery.of(context).textScaler.clamp(
      minScaleFactor: 1.0,
      maxScaleFactor: 2.0, // Até 200%
    ),
  ),
  child: Text('Texto escalável'),
)
```

### Espaçamento de Linha

**Regra**: Line height mínimo 1.5 para legibilidade

**Material Design 3**:
- BodyMedium: 16sp / 24px line height = 1.5 ✅

### Família de Fonte

**Regra**: Usar fontes legíveis (sans-serif recomendado)

**Arah**:
- Fonte primária: **Inter** (sans-serif) ✅
- Fallback: System sans-serif

---

## 👆 Tamanhos de Toque

### Tamanho Mínimo

**iOS**: 44x44 pontos
**Android**: 48x48 dp (density-independent pixels)

**Flutter**: Usar `SizeBox` ou `Container` para garantir tamanho mínimo

```dart
// Botão com tamanho mínimo garantido
SizedBox(
  width: 48,
  height: 48,
  child: IconButton(
    icon: Icon(Icons.favorite),
    onPressed: () => _likePost(),
  ),
)

// Ou usar Material Design padrão (já garante tamanho mínimo)
IconButton(
  icon: Icon(Icons.favorite),
  onPressed: () => _likePost(),
  constraints: BoxConstraints(
    minWidth: 48,
    minHeight: 48,
  ),
)
```

### Espaçamento entre Elementos

**Regra**: Espaçamento mínimo de 8dp entre elementos clicáveis

```dart
Row(
  children: [
    IconButton(icon: Icon(Icons.like), onPressed: () {}),
    SizedBox(width: 8), // Espaçamento mínimo
    IconButton(icon: Icon(Icons.comment), onPressed: () {}),
  ],
)
```

---

## 🔊 Screen Readers

### TalkBack (Android)

**Ativação**: Configurações → Acessibilidade → TalkBack

**Comandos**:
- **Swipe Right**: Próximo elemento
- **Swipe Left**: Elemento anterior
- **Double Tap**: Ativar elemento focado
- **Swipe Right-Right**: Ler página inteira

### VoiceOver (iOS)

**Ativação**: Configurações → Acessibilidade → VoiceOver

**Comandos**:
- **Swipe Right**: Próximo elemento
- **Swipe Left**: Elemento anterior
- **Double Tap**: Ativar elemento focado
- **Three Finger Swipe Down**: Ler página inteira

### Testando com Screen Readers

**Android**:
```bash
adb shell settings put secure enabled_accessibility_services com.google.android.marvin.talkback/com.google.android.marvin.talkback.TalkBackService
```

**iOS**: Simulador → Device → VoiceOver (ou Cmd+F5)

### Labels para Screen Readers

```dart
// Exemplo: Post card acessível
Semantics(
  label: 'Post de ${post.authorName}',
  hint: 'Publicado há ${post.timeAgo}',
  value: '${post.likesCount} curtidas, ${post.commentsCount} comentários',
  child: PostCard(post: post),
)

// Exemplo: Botão de ação acessível
Semantics(
  label: 'Curtir post',
  hint: post.isLiked ? 'Post já curtido. Double tap to unlike' : 'Double tap to like',
  button: true,
  selected: post.isLiked,
  child: IconButton(
    icon: Icon(post.isLiked ? Icons.favorite : Icons.favorite_border),
    onPressed: () => _toggleLike(),
  ),
)
```

---

## 🎬 Redução de Movimento

### Preferências do Sistema

**Respeitar**: `prefers-reduced-motion` do sistema

```dart
// Verificar preferência do sistema
final prefersReducedMotion = MediaQuery.of(context).prefersReducedMotion;

// Aplicar animação reduzida se preferido
AnimatedContainer(
  duration: prefersReducedMotion 
    ? Duration.zero  // Sem animação
    : Duration(milliseconds: 300),  // Animação normal
  // ...
)
```

### Animações Opcionais

**Regra**: Permitir desabilitar animações não essenciais

```dart
// Configuração de acessibilidade
final accessibilitySettings = ref.watch(accessibilitySettingsProvider);

AnimatedOpacity(
  duration: accessibilitySettings.enableAnimations
    ? Duration(milliseconds: 300)
    : Duration.zero,
  opacity: isVisible ? 1.0 : 0.0,
  child: Content(),
)
```

---

## ✅ Testes de Acessibilidade

### Semantics Tests

```dart
testWidgets('should have correct semantics for accessibility', (WidgetTester tester) async {
  await tester.pumpWidget(
    MaterialApp(
      home: FeedScreen(territoryId: 'test-id'),
    ),
  );
  
  // Verificar semântica de elementos principais
  expect(
    find.bySemanticsLabel('Feed do território'),
    findsOneWidget,
    reason: 'Screen should have semantic label',
  );
  
  expect(
    find.bySemanticsLabel('Criar novo post'),
    findsOneWidget,
    reason: 'FAB should have semantic label',
  );
});
```

### Contraste Tests

```dart
test('text colors should meet WCAG AA contrast requirements', () {
  // Verificar contraste de texto
  expect(
    meetsWCAGAA(ArapongaColors.neutral900, ArapongaColors.neutral50),
    true,
    reason: 'Body text should meet 4.5:1 contrast',
  );
  
  // Verificar contraste de texto grande
  expect(
    meetsWCAGAA(ArapongaColors.neutral800, ArapongaColors.neutral100, isLargeText: true),
    true,
    reason: 'Large text should meet 3:1 contrast',
  );
});
```

### Tamanho de Toque Tests

```dart
testWidgets('interactive elements should meet minimum touch target size', (WidgetTester tester) async {
  await tester.pumpWidget(
    MaterialApp(
      home: FeedScreen(territoryId: 'test-id'),
    ),
  );
  
  // Verificar tamanho de botões
  final fab = tester.getSize(find.byType(FloatingActionButton));
  expect(fab.width, greaterThanOrEqualTo(48));
  expect(fab.height, greaterThanOrEqualTo(48));
  
  // Verificar tamanho de ícones clicáveis
  final iconButtons = find.byType(IconButton);
  for (final iconButton in iconButtons.evaluate()) {
    final size = tester.getSize(iconButton);
    expect(size.width, greaterThanOrEqualTo(48));
    expect(size.height, greaterThanOrEqualTo(48));
  }
});
```

---

## 📋 Checklist WCAG AA

### Perceptível

- [ ] **1.1.1 Conteúdo não textual**: Imagens têm alt text ou são decorativas
- [ ] **1.3.1 Info e relacionamentos**: Estrutura semântica correta
- [ ] **1.4.3 Contraste (mínimo)**: Texto normal 4.5:1, texto grande 3:1
- [ ] **1.4.4 Redimensionamento de texto**: Escalável até 200%
- [ ] **1.4.10 Reflow**: Conteúdo reflui sem perda de funcionalidade
- [ ] **1.4.11 Contraste não textual**: Componentes 3:1
- [ ] **1.4.12 Espaçamento de texto**: Line height 1.5+
- [ ] **1.4.13 Conteúdo ao passar o mouse**: Não desaparece ao mover mouse

### Operável

- [ ] **2.1.1 Teclado**: Toda funcionalidade disponível via teclado
- [ ] **2.1.2 Sem armadilhas de teclado**: Foco pode sair de componentes
- [ ] **2.4.1 Bypass blocks**: Skip links disponíveis
- [ ] **2.4.2 Título de página**: Tela tem título descritivo
- [ ] **2.4.3 Ordem de foco**: Ordem lógica e previsível
- [ ] **2.4.4 Propósito do link**: Labels descritivos
- [ ] **2.4.7 Foco visível**: Indicador de foco claro
- [ ] **2.5.1 Gestos de ponteiro**: Gestos podem ser cancelados
- [ ] **2.5.2 Cancelar ponteiro**: Cancelamento de ações
- [ ] **2.5.5 Tamanho do alvo**: Mínimo 44x44 (iOS) ou 48x48 (Android)

### Compreensível

- [ ] **3.1.1 Idioma da página**: Idioma declarado
- [ ] **3.2.3 Navegação consistente**: Navegação consistente entre telas
- [ ] **3.2.4 Identificação consistente**: Componentes consistentes
- [ ] **3.3.1 Identificação de erro**: Erros identificados
- [ ] **3.3.2 Labels ou instruções**: Labels claros
- [ ] **3.3.3 Sugestões de erro**: Sugestões fornecidas
- [ ] **3.3.4 Prevenção de erros**: Confirmação para ações importantes

### Robusto

- [ ] **4.1.2 Nome, função, valor**: Informações semânticas completas
- [ ] **4.1.3 Mensagens de status**: Status comunicados via semântica

---

## ✅ Boas Práticas

### 1. Semântica Sempre

- Sempre usar `Semantics` widget para elementos interativos
- Fornecer labels descritivos e únicos
- Incluir hints quando necessário

### 2. Contraste Adequado

- Verificar contraste de todas as cores de texto
- Usar ferramentas de verificação de contraste
- Não depender apenas de cor

### 3. Tamanhos Adequados

- Garantir tamanho mínimo de toque (44x44 ou 48x48)
- Espaçamento adequado entre elementos
- Texto escalável até 200%

### 4. Navegação Lógica

- Ordem de foco lógica e previsível
- Skip links para conteúdo repetitivo
- Navegação consistente

### 5. Testes Contínuos

- Testar com screen readers regularmente
- Verificar contraste programaticamente
- Testar tamanhos de toque

### 6. Feedback do Usuário

- Ouvir feedback de usuários com deficiências
- Incorporar melhorias baseadas em feedback
- Documentar problemas e soluções

---

**Versão**: 1.0  
**Última Atualização**: 2025-01-20  
**Referências**: 
- [WCAG 2.1 Guidelines](https://www.w3.org/WAI/WCAG21/quickref/)
- [Flutter Accessibility](https://docs.flutter.dev/accessibility-and-localization/accessibility)
- [Material Design Accessibility](https://material.io/design/usability/accessibility.html)
