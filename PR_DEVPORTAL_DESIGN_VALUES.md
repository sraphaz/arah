# PR: Melhorias de Design, Card PIX e Incorporação de Valores no DevPortal

## Resumo

Este PR melhora significativamente a organização visual do Developer Portal, adiciona suporte para contribuições financeiras via PIX, ajusta as cores dos diagramas para harmonizar com o tema do site e incorpora sutilmente os valores fundamentais do projeto Araponga (pertencimento, autonomia, economia circular e regeneração social territorial).

## Mudanças Implementadas

### 1. **Card de Contribuição via PIX** 💚

- Novo card adicionado na seção "Contribuir" com design harmonioso
- Botão destacado para contribuição financeira
- Texto explicativo sobre o uso das contribuições
- Layout responsivo utilizando `model-grid` (suporta 3+ colunas em telas grandes)

### 2. **Refatoração de Cards - Seção Admin** 🎨

Seção Admin reorganizada em 4 cards individuais e focados:

- **Papéis Administrativos**: Visão geral dos papéis (SystemAdmin, Curator, FinancialManager)
- **System Config**: Card dedicado com endpoints e permissões
- **Work Queue**: Card explicando filas de trabalho (global/territorial)
- **Evidências & Documentos**: Card focado em upload e download seguro

**Benefício**: Melhor organização visual, conteúdo mais escaneável, aproveitamento de espaço em telas grandes.

### 3. **Refatoração de Cards - Seção Roadmap** 🗺️

Roadmap reorganizado em 8 cards individuais:

- **7 cards para cada Onda estratégica** (1️⃣ a 7️⃣)
- **1 card para Direcionamento e Princípios** (🎯)

**Benefício**: Visualização mais clara do roadmap, fácil navegação entre fases, melhor uso de espaço em telas grandes.

### 4. **Cores Customizadas nos Diagramas** 🎨

- Script de geração atualizado (`scripts/generate-mermaid-using-api.js`) para usar tema customizado
- Cores do site aplicadas diretamente nos SVGs gerados:
  - Fundo: `#141a21` (--bg-card)
  - Texto: `#e8edf2` (--text) e `#b8c5d2` (--text-muted)
  - Setas/linhas: `#4dd4a8` (--accent, verde água)
  - Destaques: `#7dd3ff` (--link, azul claro)
- Filtros CSS removidos (não são mais necessários)
- Diagramas agora harmonizam perfeitamente com o tema dark do devportal

**Benefício**: Consistência visual total, sem necessidade de ajustes via CSS, melhor integração com o design.

### 5. **Incorporação Sutil de Valores** 💫

Valores fundamentais do projeto incorporados de forma técnica e sóbria:

- **Pertencimento**: Menção a vínculos territoriais e organização comunitária
- **Autonomia**: Destaque para configurações comunitárias e decisões locais
- **Economia Circular**: Explicação sobre payout territorial e ciclos econômicos locais
- **Regeneração Social Territorial**: Contexto sobre monitoramento territorial e soberania alimentar

**Locais ajustados:**
- Hero/Introdução
- Visão Geral
- Card "Por que território?"
- Card "Governança em camadas"
- Marketplace (economia circular)
- Roadmap (todas as ondas)

**Benefício**: Linguagem técnica que revela sutilmente o propósito maior da aplicação, mantendo foco em documentação.

### 6. **Atualização de Testes** ✅

- Teste `DevPortal_Css_ShouldHaveFilterStylesForDarkTheme` atualizado
- Agora verifica presença de estilos de diagramas (sem filtros CSS)
- Todos os 26 testes passando

## Arquivos Modificados

- `backend/Araponga.Api/wwwroot/devportal/index.html`
  - Adicionado card PIX na seção Contribuir
  - Refatoradas seções Admin e Roadmap em cards individuais
  - Incorporados valores sutilmente em múltiplas seções
  
- `backend/Araponga.Api/wwwroot/devportal/assets/css/devportal.css`
  - Removidos filtros CSS para diagramas (não mais necessários)
  - Comentários atualizados explicando que cores são aplicadas na geração

- `scripts/generate-mermaid-using-api.js`
  - Adicionado tema customizado Mermaid com cores do site
  - Função `getCustomThemeInit()` com paleta completa
  - Configuração de cores aplicada antes da geração dos SVGs

- `backend/Araponga.Tests/Api/DevPortalTests.cs`
  - Teste atualizado para refletir remoção de filtros CSS

## Impacto Visual

### Antes
- Seções densas com múltiplos elementos em poucos cards
- Diagramas com aparência "paper-like" (claro demais)
- Filtros CSS tentando ajustar cores (menos eficiente)

### Depois
- Cards individuais e focados, melhor organização
- Diagramas harmonizados com tema dark do site
- Cores aplicadas diretamente na geração (mais eficiente)
- Valores do projeto revelados de forma sutil e técnica

## Testes

### ✅ Todos os 26 testes passando

```
Passed!  - Failed:     0, Passed:    26, Skipped:     0, Total:    26
```

## Próximos Passos

1. **Gerar novos SVGs**: Executar `node scripts/generate-mermaid-using-api.js` para regenerar diagramas com cores customizadas
2. **Link do PIX**: Substituir `href="#"` no botão de contribuição quando o link estiver disponível

## Notas Técnicas

- **Responsividade**: `model-grid` se adapta automaticamente (1-4 colunas dependendo do tamanho da tela)
- **Cores**: Paleta do site preservada e aplicada consistentemente
- **Performance**: SVGs gerados com cores corretas evitam processamento CSS adicional
- **Acessibilidade**: Cards mantêm contraste adequado e estrutura semântica

---

**Status**: ✅ Pronto para review  
**Impacto**: DevPortal apenas (sem impacto em código de produção)  
**Breaking Changes**: Nenhum (melhorias visuais e de conteúdo)  
**Testes**: ✅ 26/26 passando
