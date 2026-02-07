# PR: Integração de Diagramas de Sequência no DevPortal

## Resumo

Este PR integra diagramas de sequência interativos no Developer Portal usando Mermaid.js. Os diagramas são exibidos de forma harmoniosa através de acordeões expansíveis, permitindo que desenvolvedores visualizem os principais fluxos de negócio da aplicação de forma clara e interativa.

## Problema Resolvido

- Falta de visualização clara dos fluxos de negócio no DevPortal
- Documentação textual apenas não permite compreensão completa das interações entre componentes
- Necessidade de visualizar sequências de chamadas entre controllers, services e repositories

## Solução Implementada

### 1. Integração do Mermaid.js
- Adicionado Mermaid.js v10 via CDN
- Configuração personalizada com tema dark alinhado à paleta do devportal
- Renderização lazy (apenas quando acordeão é expandido)

### 2. Componentes de Acordeão Expansíveis
- Implementação usando `<details>` nativo do HTML5
- Estilos CSS harmoniosos seguindo padrões do devportal
- Animações suaves de transição
- Feedback visual claro com ícones animados

### 3. Diagramas de Sequência Integrados
Diagramas adicionados para os seguintes fluxos principais:

1. **Autenticação social → JWT**
   - Fluxo completo de login/cadastro
   - Tratamento de 2FA
   - Emissão de tokens

2. **Descoberta de território e Membership**
   - Busca de territórios próximos
   - Criação de membership como Visitor
   - Validações e criação de settings

3. **Feed territorial (listagem)**
   - Listagem paginada de posts
   - Filtragem por visibilidade e bloqueios
   - Carga de eventos e contagens

4. **Criação de post**
   - Validações de permissões
   - Verificação de feature flags e sanções
   - Criação de geo anchors
   - Publicação de eventos

5. **Membership: visitor → resident**
   - Solicitação de residência
   - Upload de documentos
   - Criação de work items

6. **Moderação e governança**
   - Criação de reports
   - Processamento de casos de moderação
   - Aplicação de sanções

## Mudanças Implementadas

**Nota de Deploy**: O Developer Portal está disponível em `https://devportal.Arah.app/` e é servido pelo GitHub Pages através do workflow `.github/workflows/devportal-pages.yml`. O workflow copia automaticamente o conteúdo de `backend/Arah.Api/wwwroot/devportal/` para o GitHub Pages durante o deploy.

### Arquivos Modificados

#### `backend/Arah.Api/wwwroot/devportal/index.html`
- ✅ Adicionado script Mermaid.js via CDN
- ✅ Adicionados 6 diagramas de sequência em acordeões expansíveis
- ✅ JavaScript para inicialização e renderização lazy do Mermaid
- ✅ Configuração personalizada do tema Mermaid com paleta do devportal

**Seções modificadas:**
- Seção "Fluxos principais" (`#fluxos`):
  - Fluxo 1: Autenticação social → JWT
  - Fluxo 2: Descoberta de território
  - Fluxo 4: Feed territorial
  - Fluxo 5: Criar post + âncoras geográficas + mídias
  - Fluxo 9: Membership: visitor → resident
  - Fluxo 10: Moderação & segurança

#### `backend/Arah.Api/wwwroot/devportal/assets/css/devportal.css`
- ✅ Estilos para `.sequence-diagram-toggle` (acordeão)
- ✅ Estilos para `.sequence-diagram-summary` (botão de expansão)
- ✅ Estilos para `.sequence-diagram-container` (container do diagrama)
- ✅ Animação `slideDown` para revelação suave
- ✅ Responsividade para dispositivos móveis
- ✅ Ajustes para impressão

**Novos estilos:**
```css
.sequence-diagram-toggle {
  /* Acordeão expansível harmonioso */
}

.sequence-diagram-summary {
  /* Botão de expansão com ícone animado */
}

.sequence-diagram-container {
  /* Container do diagrama com animação */
}
```

## Características Técnicas

### Design e UX
- ✨ **Paleta de cores harmoniosa**: Cores do devportal aplicadas aos diagramas
- ✨ **Animações suaves**: Transições CSS para expansão/colapso
- ✨ **Feedback visual**: Ícone que rotaciona ao expandir
- ✨ **Responsividade**: Funciona perfeitamente em mobile

### Performance
- ⚡ **Lazy loading**: Diagramas renderizados apenas quando necessário
- ⚡ **Renderização sob demanda**: Cada diagrama renderiza uma única vez
- ⚡ **Otimização**: Verificação de carregamento do Mermaid antes de inicializar

### Acessibilidade
- ♿ **Semântica HTML5**: Uso de `<details>` nativo
- ♿ **Navegação por teclado**: Suporte completo a navegação por teclado
- ♿ **Feedback visual**: Estados hover e focus claramente definidos

## Configuração do Mermaid

### Tema Personalizado
```javascript
theme: 'dark',
themeVariables: {
  primaryColor: '#4dd4a8',
  background: '#0a0e12',
  textColor: '#e8edf2',
  actorBkg: '#1a2129',
  signalColor: '#7dd3ff',
  // ... mais variáveis personalizadas
}
```

### Sequências
- Margens otimizadas para melhor legibilidade
- UseMaxWidth habilitado para responsividade
- MirrorActors habilitado para melhor visualização

## Exemplo de Uso

No DevPortal, cada fluxo principal agora possui um acordeão "📊 Ver Diagrama de Sequência":

```html
<details class="sequence-diagram-toggle">
  <summary class="sequence-diagram-summary">
    <span>📊 Ver Diagrama de Sequência</span>
  </summary>
  <div class="sequence-diagram-container">
    <div class="mermaid" data-diagram="auth">
      sequenceDiagram
        participant Cliente
        participant AuthController
        ...
    </div>
  </div>
</details>
```

## Benefícios

1. **Clareza Visual**: Desenvolvedores podem ver visualmente como os componentes interagem
2. **Documentação Viva**: Diagramas sempre atualizados junto com o código
3. **Onboarding Rápido**: Novos desenvolvedores entendem os fluxos rapidamente
4. **Manutenção**: Fácil adicionar novos diagramas seguindo o padrão estabelecido

## Testes e Validação

### Validações Realizadas
- ✅ HTML válido (sem erros de linter)
- ✅ CSS sem conflitos
- ✅ JavaScript funcional
- ✅ Renderização correta dos diagramas
- ✅ Responsividade em diferentes tamanhos de tela
- ✅ Acessibilidade (navegação por teclado)

### Compatibilidade
- ✅ Chrome/Edge (últimas versões)
- ✅ Firefox (últimas versões)
- ✅ Safari (últimas versões)
- ✅ Mobile browsers

## Próximos Passos (Futuro)

- [ ] Adicionar mais diagramas para outros fluxos (Chat, Marketplace, Eventos)
- [ ] Exportar diagramas como imagem/SVG
- [ ] Adicionar zoom interativo em diagramas grandes
- [ ] Integrar diagramas C4 de componentes no mesmo padrão

## Notas Técnicas

- Mermaid.js é carregado via CDN (jsdelivr)
- Renderização acontece apenas quando acordeão é expandido (lazy loading)
- Diagramas são renderizados uma única vez e armazenados em cache
- Tratamento de erros implementado com fallback visual

## Deploy

- **URL de Produção**: `https://devportal.Arah.app/`
- **Localização do Código**: `backend/Arah.Api/wwwroot/devportal/`
- **Workflow de Deploy**: `.github/workflows/devportal-pages.yml`
- **Processo**: O workflow do GitHub Actions copia automaticamente o conteúdo de `wwwroot/devportal/` para o GitHub Pages quando há push na branch `main` ou `master`

## Referências

- [Mermaid.js Documentation](https://mermaid.js.org/)
- [HTML5 Details Element](https://developer.mozilla.org/en-US/docs/Web/HTML/Element/details)
- Padrões de design do DevPortal (paleta de cores e tipografia)

---

**Status**: ✅ Pronto para review
**Impacto**: DevPortal apenas (sem impacto em código de produção)
**Breaking Changes**: Nenhum
**Dependências Externas**: Mermaid.js v10 via CDN
