# Fase 27: Chat com IA e Consumo Consciente

**Duração**: 2 semanas (14 dias úteis)  
**Prioridade**: 🟡 ALTA (Valor diferenciado e autonomia)  
**Depende de**: Fase 26 (Serviços Digitais Base), Chat (existe)  
**Estimativa Total**: 64-80 horas  
**Status**: ⏳ Pendente  
**Nota**: Renumerada de Fase 26 para Fase 27 (Onda 6: Autonomia Digital).

---

## 🎯 Objetivo

Implementar **IA integrada ao chat** que permite:
- Usuários usarem IA em conversas do chat
- Seleção de diferentes provedores de IA (OpenAI, Claude, Gemini, etc.)
- Rastreamento de consumo por conversa
- Feature flags territorial e por usuário
- Consumo consciente com extrato visível
- Integração harmoniosa com sistema de chat existente

**Princípios**:
- ✅ **Autonomia**: Usuários usam suas próprias contas de IA
- ✅ **Transparência**: Consumo rastreado e visível
- ✅ **Controle**: Feature flags permitem controle granular
- ✅ **Harmonia**: Integração suave com chat existente
- ✅ **Consciência**: Extrato educa sobre uso de recursos

---

## 📋 Contexto e Requisitos

### Estado Atual
- ✅ Sistema de chat implementado (canais, grupos, DM)
- ✅ Fase 26 (Serviços Digitais Base) fornece infraestrutura
- ✅ UserPreferences existe (Fase 9)
- ✅ Feature flags territoriais funcionando
- ❌ IA não está integrada ao chat
- ❌ Não existe seleção de provedores de IA
- ❌ Não existe rastreamento de consumo por conversa

### Requisitos Funcionais

#### 1. Integração de IA no Chat
- ✅ Endpoint para enviar mensagem com IA
- ✅ Resposta de IA na conversa
- ✅ Seleção de provedor de IA antes/ durante conversa
- ✅ Histórico de mensagens com IA
- ✅ Indicação visual de mensagens com IA

#### 2. Adapters de Provedores de IA
- ✅ OpenAI (GPT-3.5, GPT-4)
- ✅ Anthropic Claude (Claude 2, Claude 3)
- ✅ Google Gemini (Gemini Pro)
- ✅ Azure OpenAI (compatibilidade OpenAI)
- ✅ Arquitetura extensível para novos provedores

#### 3. Rastreamento de Consumo por Conversa
- ✅ Cada mensagem de IA registra consumo
- ✅ Tokens input/output registrados
- ✅ Custo estimado por mensagem
- ✅ Consumo agregado por conversa
- ✅ Consumo visível no extrato

#### 4. Feature Flags e Controle
- ✅ Feature flag territorial (`ChatAIEnabled`)
- ✅ Feature flag por categoria (`DigitalServicesAIEnabled`)
- ✅ Preferência do usuário (`DigitalServicesEnabled`)
- ✅ Habilitação gradual por território

#### 5. Quotas e Limites
- ✅ Quota mensal configurável por usuário
- ✅ Alertas quando próximo ao limite (80%, 90%, 100%)
- ✅ Bloqueio quando quota esgotada
- ✅ Mensagem informativa sobre quota

---

## 📋 Tarefas Detalhadas

### Semana 1: Adapters de IA e Integração com Chat

#### 27.1 Adapters de Provedores de IA
**Estimativa**: 24 horas (3 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar interface `IAIServiceAdapter`:
  - [ ] `ExecuteAsync<TRequest, TResponse>(TRequest request, UserDigitalServiceConfig config, CancellationToken)` → executar chamada de IA
  - [ ] `EstimateCostAsync(TRequest request, UserDigitalServiceConfig config)` → estimar custo
  - [ ] `GetQuotaInfoAsync(UserDigitalServiceConfig config)` → informações de quota
- [ ] Implementar `OpenAIServiceAdapter`:
  - [ ] Integração com API OpenAI (GPT-3.5, GPT-4)
  - [ ] Rastreamento de tokens input/output
  - [ ] Estimativa de custo
  - [ ] Tratamento de erros
- [ ] Implementar `AnthropicClaudeServiceAdapter`:
  - [ ] Integração com API Anthropic (Claude 2, Claude 3)
  - [ ] Rastreamento de tokens
  - [ ] Estimativa de custo
- [ ] Implementar `GoogleGeminiServiceAdapter`:
  - [ ] Integração com API Google Gemini
  - [ ] Rastreamento de tokens
  - [ ] Estimativa de custo
- [ ] Implementar `AzureOpenAIServiceAdapter`:
  - [ ] Compatibilidade com OpenAI via Azure
  - [ ] Rastreamento de tokens
- [ ] Criar `AIServiceAdapterFactory`:
  - [ ] `GetAdapterAsync(ServiceProvider provider)` → obter adapter
  - [ ] Cache de adapters
- [ ] Testes unitários

**Arquivos a Criar**:
- `backend/Araponga.Application/Interfaces/IAIServiceAdapter.cs`
- `backend/Araponga.Infrastructure/DigitalServices/AI/OpenAIServiceAdapter.cs`
- `backend/Araponga.Infrastructure/DigitalServices/AI/AnthropicClaudeServiceAdapter.cs`
- `backend/Araponga.Infrastructure/DigitalServices/AI/GoogleGeminiServiceAdapter.cs`
- `backend/Araponga.Infrastructure/DigitalServices/AI/AzureOpenAIServiceAdapter.cs`
- `backend/Araponga.Application/Services/AIServiceAdapterFactory.cs`
- `backend/Araponga.Tests/Infrastructure/AIServiceAdapterTests.cs`

**Critérios de Sucesso**:
- ✅ Adapters implementados
- ✅ Integração com APIs funcionando
- ✅ Rastreamento de tokens funcionando
- ✅ Estimativa de custo funcionando
- ✅ Testes passando

---

#### 27.2 Serviço de IA no Chat
**Estimativa**: 16 horas (2 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `ChatAIService`:
  - [ ] `SendMessageWithAIAsync(Guid conversationId, Guid userId, string message, ServiceProvider? provider, ...)` → enviar mensagem com IA
  - [ ] `GetAvailableProvidersAsync(Guid userId, Guid territoryId)` → listar provedores disponíveis
  - [ ] `GetConversationAIConsumptionAsync(Guid conversationId, Guid userId)` → consumo por conversa
  - [ ] `CheckAIAvailabilityAsync(Guid userId, Guid territoryId)` → verificar disponibilidade
- [ ] Integrar com `ChatService` existente:
  - [ ] Mensagens de IA aparecem no histórico da conversa
  - [ ] Indicação visual de mensagem com IA
  - [ ] Contexto de conversa incluído (histórico)
- [ ] Validações:
  - [ ] Feature flags (territorial + usuário)
  - [ ] Quota disponível
  - [ ] Provedor configurado pelo usuário
  - [ ] Conversa existe e usuário tem acesso
- [ ] Tratamento de erros:
  - [ ] Fallback quando provedor indisponível
  - [ ] Mensagem de erro amigável
  - [ ] Log de erros para debugging
- [ ] Rastreamento de consumo:
  - [ ] Registrar uso em `DigitalServiceUsageLog`
  - [ ] Associar com `ConversationId`
  - [ ] ContextType = "chat_ai"
- [ ] Testes unitários

**Arquivos a Criar**:
- `backend/Araponga.Application/Services/ChatAIService.cs`
- `backend/Araponga.Tests/Application/ChatAIServiceTests.cs`

**Arquivos a Modificar**:
- `backend/Araponga.Application/Services/ChatService.cs` (integração opcional)

**Critérios de Sucesso**:
- ✅ Serviço de IA funcionando
- ✅ Integração com chat funcionando
- ✅ Rastreamento de consumo funcionando
- ✅ Validações funcionando
- ✅ Testes passando

---

### Semana 2: API, Quotas e Extrato

#### 27.3 Controller de IA no Chat
**Estimativa**: 12 horas (1.5 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `ChatAIController`:
  - [ ] `POST /api/v1/chat/conversations/{conversationId}/ai-message` → enviar mensagem com IA
  - [ ] `GET /api/v1/chat/conversations/{conversationId}/ai-consumption` → consumo de IA na conversa
  - [ ] `GET /api/v1/chat/ai/providers` → listar provedores disponíveis
- [ ] Atualizar `ChatController` (opcional):
  - [ ] Adicionar campo `HasAIAvailable` nas respostas de conversa
- [ ] Validações (FluentValidation):
  - [ ] `SendAIMessageRequestValidator`
- [ ] Feature flags: `ChatAIEnabled`, `DigitalServicesAIEnabled`
- [ ] Rate limiting:
  - [ ] Limitar chamadas de IA (ex: 30 req/min)
- [ ] Testes de integração

**Arquivos a Criar**:
- `backend/Araponga.Api/Controllers/ChatAIController.cs`
- `backend/Araponga.Api/Contracts/Chat/SendAIMessageRequest.cs`
- `backend/Araponga.Api/Contracts/Chat/AIMessageResponse.cs`
- `backend/Araponga.Api/Contracts/Chat/ConversationAIConsumptionResponse.cs`
- `backend/Araponga.Api/Validators/SendAIMessageRequestValidator.cs`
- `backend/Araponga.Tests/Integration/ChatAIIntegrationTests.cs`

**Arquivos a Modificar**:
- `backend/Araponga.Api/Controllers/ChatController.cs` (opcional)

**Critérios de Sucesso**:
- ✅ API funcionando
- ✅ Validações funcionando
- ✅ Rate limiting funcionando
- ✅ Testes passando

---

#### 27.4 Sistema de Quotas e Alertas
**Estimativa**: 12 horas (1.5 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `AIConsumptionQuotaService`:
  - [ ] `CheckQuotaBeforeRequestAsync(Guid userId, Guid configId, int estimatedTokens)` → verificar quota
  - [ ] `GetRemainingQuotaAsync(Guid userId, Guid configId)` → quota restante
  - [ ] `GetQuotaStatusAsync(Guid userId, Guid configId)` → status da quota (OK, WARNING, EXHAUSTED)
- [ ] Integrar verificações de quota:
  - [ ] Verificar antes de executar chamada de IA
  - [ ] Bloquear se quota esgotada
  - [ ] Mensagem informativa sobre quota
- [ ] Alertas de quota:
  - [ ] Notificação quando quota em 80% (`digital_service.quota.warning`)
  - [ ] Notificação quando quota em 90% (`digital_service.quota.critical`)
  - [ ] Notificação quando quota esgotada (`digital_service.quota.exhausted`)
- [ ] Integrar com sistema de notificações:
  - [ ] Criar tipos de notificação (Fase 26 preparou)
  - [ ] Enviar notificações via outbox
- [ ] Dashboard de quota:
  - [ ] Mostrar quota restante no extrato de consumo
  - [ ] Indicadores visuais (verde/amarelo/vermelho)
- [ ] Testes

**Arquivos a Criar**:
- `backend/Araponga.Application/Services/AIConsumptionQuotaService.cs`
- `backend/Araponga.Tests/Application/AIConsumptionQuotaServiceTests.cs`

**Arquivos a Modificar**:
- `backend/Araponga.Application/Services/DigitalServiceUsageTracker.cs` (integração)

**Critérios de Sucesso**:
- ✅ Verificação de quota funcionando
- ✅ Bloqueio quando quota esgotada
- ✅ Alertas funcionando
- ✅ Notificações funcionando
- ✅ Testes passando

---

#### 27.5 Extrato de Consumo de IA
**Estimativa**: 12 horas (1.5 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `AIConsumptionReportService`:
  - [ ] `GetConsumptionByConversationAsync(Guid conversationId, Guid userId)` → consumo por conversa
  - [ ] `GetConsumptionSummaryAsync(Guid userId, DateTime? periodStart, DateTime? periodEnd)` → resumo de consumo
  - [ ] `GetTopConversationsByConsumptionAsync(Guid userId, int limit)` → conversas mais consumidoras
- [ ] Extrato de consumo:
  - [ ] Total de tokens consumidos (input + output)
  - [ ] Custo estimado total
  - [ ] Número de mensagens com IA
  - [ ] Provedor mais usado
  - [ ] Conversa mais consumidora
- [ ] Integrar com `DigitalServiceConsumptionController` (Fase 26):
  - [ ] Endpoints de consumo já existem
  - [ ] Adicionar filtros específicos para IA
- [ ] Dashboard visual (preparação para frontend):
  - [ ] Estatísticas de consumo de IA
  - [ ] Gráficos de uso ao longo do tempo
  - [ ] Comparação entre provedores
- [ ] Testes

**Arquivos a Criar**:
- `backend/Araponga.Application/Services/AIConsumptionReportService.cs`
- `backend/Araponga.Api/Contracts/DigitalServices/AIConsumptionSummaryResponse.cs`
- `backend/Araponga.Tests/Application/AIConsumptionReportServiceTests.cs`

**Arquivos a Modificar**:
- `backend/Araponga.Api/Controllers/DigitalServiceConsumptionController.cs` (adicionar filtros de IA)

**Critérios de Sucesso**:
- ✅ Relatórios de consumo funcionando
- ✅ Extrato de IA visível
- ✅ Estatísticas corretas
- ✅ Testes passando

---

## 📊 Resumo da Fase 27

| Tarefa | Estimativa | Status | Prioridade |
|--------|------------|--------|------------|
| Adapters de IA | 24h | ❌ Pendente | 🔴 Alta |
| Serviço de IA no Chat | 16h | ❌ Pendente | 🔴 Alta |
| Controller de IA | 12h | ❌ Pendente | 🔴 Alta |
| Sistema de Quotas | 12h | ❌ Pendente | 🔴 Alta |
| Extrato de Consumo | 12h | ❌ Pendente | 🔴 Alta |
| **Total** | **76h (14 dias)** | | |

---

## ✅ Critérios de Sucesso da Fase 27

### Funcionalidades
- ✅ IA integrada ao chat funcionando
- ✅ Múltiplos provedores de IA suportados
- ✅ Seleção de provedor funcionando
- ✅ Rastreamento de consumo por conversa
- ✅ Quotas e limites funcionando
- ✅ Extrato de consumo de IA visível

### Qualidade
- ✅ Testes com cobertura adequada
- ✅ Documentação completa
- ✅ Tratamento de erros robusto
- ✅ Rate limiting implementado
- Considerar **Testcontainers + PostgreSQL** para testes de integração (chat IA, consumo por conversa) com banco real (estratégia na Fase 43; [TESTCONTAINERS_POSTGRES_IMPACTO](../../TESTCONTAINERS_POSTGRES_IMPACTO.md)).

### Integração
- ✅ Integração harmoniosa com ChatService existente
- ✅ Uso da infraestrutura da Fase 26
- ✅ Feature flags funcionando
- ✅ Notificações de quota funcionando

---

## 🔗 Dependências

- **Fase 26**: Serviços Digitais Base (infraestrutura, rastreamento, extrato)
- **Chat**: Sistema de chat existente (conversas, mensagens)

---

## 📝 Notas de Implementação

### Integração com Chat Existente

**Abordagem**:
- IA não altera estrutura existente de `ChatMessage`
- Mensagens de IA são mensagens normais com flag `IsAIGenerated`
- Histórico de conversa inclui mensagens de IA
- Contexto para IA inclui últimas N mensagens

**Estrutura de Mensagem com IA**:
```csharp
ChatMessage {
    Id, ConversationId, UserId, Content, CreatedAtUtc,
    IsAIGenerated = true,
    AIProvider = ServiceProvider.OpenAI,
    AITokensInput = 100,
    AITokensOutput = 50,
    AIEstimatedCost = 0.002m
}
```

### Contexto de Conversa para IA

**Histórico**:
- Incluir últimas 10-20 mensagens como contexto
- Excluir mensagens de IA muito antigas (configurável)
- Manter contexto dentro de limite de tokens

**Personalização**:
- Usuário pode configurar tamanho do contexto
- Usuário pode configurar comportamento da IA (futuro)

### Provedores de IA

**Ordem de Prioridade**:
1. Provedor selecionado pelo usuário
2. Provedor padrão do usuário
3. Fallback para outro provedor disponível

**Tratamento de Erros**:
- Se provedor falhar, tentar fallback
- Se todos falharem, retornar erro amigável
- Log de erros para debugging

---

**Status**: ⏳ **FASE 27 PENDENTE**  
**Depende de**: Fase 26, Chat  
**Crítico para**: Valor Diferenciado e Autonomia Digital
