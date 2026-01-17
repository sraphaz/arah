# PR: Adicionar 7 Diagramas de Sequência para Cenários Complexos

## Resumo

Este PR implementa todos os diagramas de sequência recomendados para cenários complexos do Araponga, expandindo a cobertura visual de documentação no Developer Portal de 6 para 13 diagramas. Os novos diagramas cobrem fluxos críticos como Marketplace completo, Sistema de Notificações (Outbox Pattern), Eventos, Assets com validação, Verificação de Residência, Chat com Mídia e Entidades do Mapa.

## Problema Resolvido

Após análise do devportal, foram identificados 7 cenários complexos sem documentação visual adequada:
- Marketplace: fluxo completo de compra (loja → item → carrinho → checkout → payout)
- Notificações: padrão Outbox/Inbox para processamento assíncrono
- Eventos: criação automática de posts e sincronização feed/mapa
- Assets: validação por curadores via work queue
- Residência: verificação geo vs documental
- Chat: envio de mensagens com mídia
- Mapa: sugestão → confirmação → validação de entidades

## Solução Implementada

### 7 Novos Diagramas de Sequência

#### 1. **Marketplace - Fluxo Completo** (`marketplace-checkout.mmd`)
- **6 etapas interconectadas**: Criar loja → Criar item → Adicionar ao carrinho → Checkout → Pagamento → Payout automático
- **Processamento assíncrono**: Background worker para payout
- **Integrações externas**: Payment gateway
- **Cenários cobertos**: Compráveis vs não-compráveis, agrupamento por loja, cálculo de platform fees

#### 2. **Sistema de Notificações (Outbox Pattern)** (`notifications-outbox.mmd`)
- **Padrão arquitetural importante**: Outbox para garantir entrega de eventos
- **7 etapas completas**: Evento → Outbox → Background Worker → Processamento → Notificações → Consulta → Marcação como lida
- **Processamento assíncrono**: Worker processa mensagens pendentes
- **Multi-destinatários**: Criação de notificações para cada usuário relevante

#### 3. **Eventos - Criação e Confirmação** (`events-creation.mmd`)
- **Criação automática de post**: Evento cria post automaticamente no feed
- **Sincronização feed + mapa**: Evento aparece em ambos
- **Marcar interesse/confirmação**: Usuários podem participar
- **Validação de permissões**: Verificação de Resident vs Visitor

#### 4. **Assets - Criação e Validação por Curador** (`assets-validation.mmd`)
- **Work queue pattern**: Assets sugeridos enfileirados para validação
- **Validação por curadores**: Curadores validam/rejeitam via work queue
- **Status transitions**: SUGGESTED → VALIDATED ou REJECTED
- **Geolocalização obrigatória**: Assets sempre têm geo anchors

#### 5. **Verificação de Residência (Geo/Document)** (`residency-verification.mmd`)
- **Dois fluxos distintos**: Verificação por geolocalização vs comprovante documental
- **Validações geográficas**: Cálculo de distância e raio permitido
- **Upload e processamento**: Upload de evidências e criação de work items
- **Associação de evidências**: Evidências associadas a memberships

#### 6. **Chat com Mídia** (`chat-media.mmd`)
- **Upload de mídia**: Upload via `/api/v1/media/upload`
- **Envio de mensagem com mediaId**: Mensagens podem incluir imagens
- **Validações de propriedade**: Mídia deve pertencer ao usuário
- **Conversas e mensagens**: Criação automática de conversas se necessário

#### 7. **Mapa - Entidades (Sugestão → Confirmação → Validação)** (`map-entities.mmd`)
- **Fluxo colaborativo multi-etapa**: Sugestão por visitante/morador → Confirmação por moradores → Validação por curadores
- **Confirmação por moradores**: Múltiplos moradores podem confirmar
- **Validação por curadores**: Curadores validam via work queue
- **Integração com feed**: Entidades aparecem no feed e no mapa

### Geração de SVGs Pré-renderizados

- **Script automatizado**: `scripts/generate-mermaid-using-api.js` atualizado para gerar SVGs
- **Tema dark**: SVGs gerados com `theme=dark&bgColor=0a0e12` para harmonia visual
- **API online**: Uso da API `mermaid.ink` para renderização confiável
- **13 SVGs gerados**: Todos os diagramas (6 existentes + 7 novos) disponíveis como SVG

### Integração no DevPortal

- **Botões de tela cheia**: Todos os diagramas têm botão "⛶ Tela Cheia" funcional
- **Ajustes de cores**: CSS filters aplicados para adaptar SVGs ao tema dark
- **Posicionamento**: Diagramas integrados nas seções correspondentes do devportal
- **Responsividade**: SVGs escalam corretamente em diferentes tamanhos de tela

## Mudanças Implementadas

### Arquivos Criados

#### Diagramas Mermaid (`.mmd`)
- `backend/Araponga.Api/wwwroot/devportal/assets/images/diagrams/marketplace-checkout.mmd`
- `backend/Araponga.Api/wwwroot/devportal/assets/images/diagrams/notifications-outbox.mmd`
- `backend/Araponga.Api/wwwroot/devportal/assets/images/diagrams/events-creation.mmd`
- `backend/Araponga.Api/wwwroot/devportal/assets/images/diagrams/assets-validation.mmd`
- `backend/Araponga.Api/wwwroot/devportal/assets/images/diagrams/residency-verification.mmd`
- `backend/Araponga.Api/wwwroot/devportal/assets/images/diagrams/chat-media.mmd`
- `backend/Araponga.Api/wwwroot/devportal/assets/images/diagrams/map-entities.mmd`

#### SVGs Pré-renderizados (`.svg`)
- `backend/Araponga.Api/wwwroot/devportal/assets/images/diagrams/marketplace-checkout.svg`
- `backend/Araponga.Api/wwwroot/devportal/assets/images/diagrams/notifications-outbox.svg`
- `backend/Araponga.Api/wwwroot/devportal/assets/images/diagrams/events-creation.svg`
- `backend/Araponga.Api/wwwroot/devportal/assets/images/diagrams/assets-validation.svg`
- `backend/Araponga.Api/wwwroot/devportal/assets/images/diagrams/residency-verification.svg`
- `backend/Araponga.Api/wwwroot/devportal/assets/images/diagrams/chat-media.svg`
- `backend/Araponga.Api/wwwroot/devportal/assets/images/diagrams/map-entities.svg`

#### Documentação
- `docs/diagramas-faltantes-analise.md` - Análise completa dos diagramas faltantes

### Arquivos Modificados

#### `backend/Araponga.Api/wwwroot/devportal/index.html`
- ✅ Adicionado diagrama **Marketplace** após passo 16 (Criar inquiry)
- ✅ Adicionado diagrama **Notificações** no passo 11 (Notificações in-app)
- ✅ Adicionado diagrama **Eventos** como passo 5a (Criar evento comunitário)
- ✅ Adicionado diagrama **Assets** após passo 6 (Assets territoriais)
- ✅ Adicionado diagrama **Verificação de Residência** como passo 9a
- ✅ Adicionado diagrama **Chat com Mídia** após passo 11a (Enviar mensagem no chat com imagem)
- ✅ Adicionado diagrama **Mapa Entidades** após passo 8 (Mapa: entidades, pins e validação)

Todos os diagramas seguem o mesmo padrão:
```html
<details class="sequence-diagram-toggle">
  <summary class="sequence-diagram-summary">
    <span>📊 Ver Diagrama de Sequência</span>
  </summary>
  <div class="sequence-diagram-container">
    <img src="./assets/images/diagrams/[nome].svg" class="mermaid-diagram" />
    <button class="diagram-fullscreen-btn" onclick="openDiagramFullscreen(...)">⛶ Tela Cheia</button>
  </div>
</details>
```

#### `backend/Araponga.Api/wwwroot/devportal/assets/css/devportal.css`
- ✅ CSS filters adicionados para ajustar cores dos SVGs ao tema dark:
  ```css
  .sequence-diagram-container img.mermaid-diagram {
    filter: invert(0.85) hue-rotate(180deg) saturate(1.2) brightness(1.1) contrast(1.2);
  }
  ```
- ✅ Aumentado `z-index` do botão de tela cheia para `100`

#### `scripts/generate-mermaid-using-api.js`
- ✅ Adicionados parâmetros `theme=dark&bgColor=0a0e12` na URL da API mermaid.ink
- ✅ Comentários explicativos sobre tema dark e cores customizadas

## Características Técnicas

### Design e UX
- ✨ **Paleta harmoniosa**: SVGs gerados com tema dark alinhado ao devportal
- ✨ **Botões de tela cheia**: Todos os 13 diagramas têm botão funcional
- ✨ **Filtros CSS**: Ajuste automático de cores para tema dark
- ✨ **Padrões consistentes**: Todos os diagramas seguem o mesmo padrão visual

### Qualidade do Código
- ✅ **Sintaxe Mermaid válida**: Todos os diagramas validados
- ✅ **SVGs pré-renderizados**: Evita problemas de parsing no navegador
- ✅ **Nomenclatura consistente**: Arquivos seguem padrão `[nome-fluxo].mmd` e `.svg`
- ✅ **Documentação**: Análise completa dos diagramas faltantes documentada

### Performance
- ⚡ **SVGs estáticos**: Renderização rápida sem JavaScript
- ⚡ **Lazy loading**: Diagramas carregam apenas quando acordeão é expandido
- ⚡ **Cache do navegador**: SVGs podem ser cacheados facilmente

## Validações Realizadas

### Diagramas
- ✅ Todos os 7 diagramas renderizam corretamente como SVG
- ✅ Sintaxe Mermaid validada sem erros
- ✅ Conteúdo técnico correto e completo
- ✅ Fluxos cobrem todos os cenários identificados

### Integração
- ✅ Todos os diagramas aparecem nas seções corretas
- ✅ Botões de tela cheia funcionam para todos os diagramas
- ✅ Filtros CSS aplicam corretamente o tema dark
- ✅ Responsividade mantida em diferentes tamanhos de tela

### Scripts
- ✅ `generate-mermaid-using-api.js` gera SVGs com tema dark
- ✅ Todos os 13 SVGs (6 + 7) gerados com sucesso

## Benefícios

1. **Documentação Completa**: 13 diagramas cobrindo todos os fluxos principais e complexos
2. **Padrões Arquiteturais**: Outbox Pattern, Work Queue Pattern documentados visualmente
3. **Onboarding Rápido**: Novos desenvolvedores entendem fluxos complexos rapidamente
4. **Manutenibilidade**: Diagramas em `.mmd` facilitam atualizações futuras
5. **Confiabilidade**: SVGs pré-renderizados evitam problemas de parsing no navegador

## Estatísticas

- **Diagramas criados**: 7 novos
- **Total de diagramas**: 13 (6 existentes + 7 novos)
- **SVGs gerados**: 13
- **Seções do devportal atualizadas**: 7
- **Linhas de código Mermaid**: ~800+
- **Arquivos criados**: 15 (7 .mmd + 7 .svg + 1 .md)
- **Arquivos modificados**: 3 (index.html, devportal.css, generate-mermaid-using-api.js)

## Próximos Passos (Futuro)

- [ ] Adicionar diagramas para fluxos adicionais se necessário
- [ ] Melhorar interatividade dos diagramas (zoom, pan)
- [ ] Adicionar tooltips explicativos nos diagramas
- [ ] Exportar diagramas como PDF para documentação offline

## Deploy

- **URL de Produção**: `https://devportal.araponga.app/`
- **Localização do Código**: `backend/Araponga.Api/wwwroot/devportal/`
- **Workflow de Deploy**: `.github/workflows/devportal-pages.yml`
- **Impacto**: Apenas devportal (sem impacto em código de produção)

## Referências

- Análise completa: `docs/diagramas-faltantes-analise.md`
- Diagramas existentes: PR #119, #120, #121, #123, #124
- Padrões de design: `docs/design-review-devportal.md`

---

**Status**: ✅ Pronto para review  
**Impacto**: DevPortal apenas (sem impacto em código de produção)  
**Breaking Changes**: Nenhum  
**Dependências Externas**: Nenhuma (SVGs pré-renderizados)
