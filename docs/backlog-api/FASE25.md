# Fase 25: Hub de Serviços Digitais Base

**Duração**: 3 semanas (21 dias úteis)  
**Prioridade**: 🔴 ALTA (Base para autonomia digital)  
**Depende de**: Fase 1 (Segurança), Fase 9 (UserPreferences)  
**Estimativa Total**: 96-120 horas  
**Status**: ⏳ Pendente

---

## 🎯 Objetivo

Implementar infraestrutura base para **serviços digitais integrados** que permite:
- Usuários conectarem suas próprias contas de serviços digitais (IA, storage, tradução, etc.)
- Rastreamento de consumo consciente de recursos
- Feature flags territorial e por usuário
- Extrato de consumo visível e transparente
- Base para expansão futura de serviços

**Princípios**:
- ✅ **Autonomia**: Usuários usam seus próprios serviços
- ✅ **Transparência**: Consumo visível e rastreável
- ✅ **Controle**: Feature flags territorial + usuário
- ✅ **Extensibilidade**: Arquitetura genérica para múltiplos serviços
- ✅ **Consciência**: Extrato de consumo educa sobre uso

---

## 📋 Contexto e Requisitos

### Estado Atual
- ✅ Sistema de chat implementado
- ✅ Feature flags territoriais funcionando
- ✅ UserPreferences existe (Fase 9)
- ✅ Sistema de notificações implementado
- ❌ Não existe sistema de serviços digitais
- ❌ Não existe rastreamento de consumo de serviços
- ❌ Não existe infraestrutura para integrações externas

### Requisitos Funcionais

#### 1. Modelo de Dados Genérico para Serviços Digitais
- ✅ Categorias de serviços (AI, Storage, Translation, MediaProcessing, etc.)
- ✅ Provedores de serviços (OpenAI, Google, AWS, etc.)
- ✅ Configuração de serviço por usuário (credenciais, quotas)
- ✅ Log de consumo de serviços
- ✅ Agregação de consumo por período

#### 2. Sistema de Rastreamento de Consumo
- ✅ Log de cada uso de serviço
- ✅ Métricas por provedor (tokens, requests, bytes, etc.)
- ✅ Custo estimado por uso
- ✅ Agregação mensal/diária
- ✅ Alertas de quota próxima ao limite

#### 3. Feature Flags para Serviços Digitais
- ✅ Feature flags territoriais (`DigitalServicesEnabled`)
- ✅ Feature flags por categoria (`DigitalServicesAIEnabled`)
- ✅ Feature flags por usuário (armazenado em UserPreferences)
- ✅ Habilitação gradual por território

#### 4. Extrato de Consumo Consciente
- ✅ Dashboard de consumo por usuário
- ✅ Visão por categoria de serviço
- ✅ Visão por provedor
- ✅ Período configurável (mensal, semanal, diário)
- ✅ Custos estimados quando disponível
- ✅ Alertas de limite

#### 5. Segurança e Privacidade
- ✅ Credenciais criptografadas no banco
- ✅ Isolamento de dados entre usuários
- ✅ Auditoria de uso
- ✅ Validação de credenciais antes de uso
- ✅ Revogação segura de credenciais

---

## 📋 Tarefas Detalhadas

### Semana 1: Modelo de Dados e Infraestrutura Base

#### 25.1 Modelo de Domínio - Serviços Digitais
**Estimativa**: 24 horas (3 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar enum `DigitalServiceCategory`:
  - [ ] `ArtificialIntelligence = 1` (IA - chat, análise, geração)
  - [ ] `Storage = 2` (Armazenamento em nuvem)
  - [ ] `Translation = 3` (Tradução de texto)
  - [ ] `MediaProcessing = 4` (Processamento de mídia)
  - [ ] `Analytics = 5` (Análise de dados)
  - [ ] `Communication = 6` (Comunicação - SMS, email)
  - [ ] `Mapping = 7` (Mapas - já existe, pode expandir)
  - [ ] `Custom = 99` (Serviços customizados)
- [ ] Criar enum `ServiceProvider`:
  - [ ] `OpenAI = 1`, `AnthropicClaude = 2`, `GoogleGemini = 3`, `AzureOpenAI = 4`
  - [ ] `AWS_S3 = 10`, `GoogleCloudStorage = 11`, `AzureBlob = 12`
  - [ ] `GoogleTranslate = 20`, `AzureTranslator = 21`, `DeepL = 22`
  - [ ] `Custom = 99`
- [ ] Criar modelo `UserDigitalServiceConfig`:
  - [ ] `Id`, `UserId`, `Category`, `Provider`
  - [ ] `ApiKeyEncrypted` (nullable, criptografado)
  - [ ] `OAuthTokenEncrypted` (nullable, criptografado)
  - [ ] `EndpointUrl` (nullable, para serviços customizados)
  - [ ] `IsDefault` (bool, provedor padrão para categoria)
  - [ ] `IsEnabled` (bool, usuário pode desabilitar)
  - [ ] `CustomSettings` (Dictionary<string, string>?)
  - [ ] `MonthlyLimitUnits` (int?, limite mensal configurado)
  - [ ] `LimitResetDate` (DateTime?, data de reset)
  - [ ] `CreatedAtUtc`, `LastUsedAtUtc`, `UpdatedAtUtc`
- [ ] Criar modelo `DigitalServiceUsageLog`:
  - [ ] `Id`, `UserId`, `TerritoryId?` (opcional)
  - [ ] `UserServiceConfigId`, `Category`, `Provider`
  - [ ] `ContextType` (string?, "chat", "translation", etc.)
  - [ ] `ContextId` (Guid?, ID do contexto)
  - [ ] `UnitsConsumed` (int), `UnitsType` (string?, "tokens", "requests", "bytes")
  - [ ] `EstimatedCost` (decimal?), `ProcessingTime` (TimeSpan?)
  - [ ] `Success` (bool), `ErrorMessage` (string?)
  - [ ] `UsedAtUtc`
- [ ] Criar modelo `DigitalServiceConsumption` (agregação):
  - [ ] `UserId`, `Category`, `Provider?` (nullable = todos)
  - [ ] `PeriodStartUtc`, `PeriodEndUtc`
  - [ ] `TotalUnitsConsumed`, `TotalEstimatedCost`, `TotalRequests`
  - [ ] `SuccessfulRequests`, `FailedRequests`
  - [ ] `ByProvider` (Dictionary<ServiceProvider, DigitalServiceConsumption>?)
- [ ] Criar repositórios
- [ ] Criar migrations

**Arquivos a Criar**:
- `backend/Araponga.Domain/DigitalServices/DigitalServiceCategory.cs`
- `backend/Araponga.Domain/DigitalServices/ServiceProvider.cs`
- `backend/Araponga.Domain/DigitalServices/UserDigitalServiceConfig.cs`
- `backend/Araponga.Domain/DigitalServices/DigitalServiceUsageLog.cs`
- `backend/Araponga.Domain/DigitalServices/DigitalServiceConsumption.cs`
- `backend/Araponga.Application/Interfaces/IDigitalServiceConfigRepository.cs`
- `backend/Araponga.Application/Interfaces/IDigitalServiceUsageRepository.cs`
- `backend/Araponga.Infrastructure/Postgres/PostgresDigitalServiceConfigRepository.cs`
- `backend/Araponga.Infrastructure/Postgres/PostgresDigitalServiceUsageRepository.cs`
- `backend/Araponga.Infrastructure/Postgres/Entities/UserDigitalServiceConfigRecord.cs`
- `backend/Araponga.Infrastructure/Postgres/Entities/DigitalServiceUsageLogRecord.cs`

**Critérios de Sucesso**:
- ✅ Modelos criados
- ✅ Repositórios implementados
- ✅ Migrations criadas
- ✅ Credenciais criptografadas no banco
- ✅ Testes de repositório passando

---

#### 25.2 Sistema de Criptografia de Credenciais
**Estimativa**: 16 horas (2 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `ICredentialEncryptionService`:
  - [ ] `EncryptAsync(string plaintext)` → string criptografada
  - [ ] `DecryptAsync(string encrypted)` → string descriptografada
- [ ] Implementar criptografia usando AES-256-GCM
- [ ] Gerenciar chaves de criptografia (via configuração segura)
- [ ] Rotação de chaves (preparação futura)
- [ ] Integrar com `UserDigitalServiceConfig`:
  - [ ] Criptografar ao salvar
  - [ ] Descriptografar ao usar
- [ ] Testes de segurança

**Arquivos a Criar**:
- `backend/Araponga.Application/Interfaces/ICredentialEncryptionService.cs`
- `backend/Araponga.Infrastructure/Security/CredentialEncryptionService.cs`
- `backend/Araponga.Tests/Infrastructure/CredentialEncryptionServiceTests.cs`

**Critérios de Sucesso**:
- ✅ Credenciais criptografadas no banco
- ✅ Descriptografia funcionando
- ✅ Testes de segurança passando
- ✅ Chaves não expostas

---

### Semana 2: Sistema de Rastreamento e Consumo

#### 25.3 Serviço de Gerenciamento de Serviços Digitais
**Estimativa**: 24 horas (3 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `DigitalServiceManager`:
  - [ ] `AddServiceConfigAsync(Guid userId, DigitalServiceCategory category, ServiceProvider provider, ...)` → adicionar configuração
  - [ ] `ListServiceConfigsAsync(Guid userId, DigitalServiceCategory? category)` → listar configurações
  - [ ] `UpdateServiceConfigAsync(Guid configId, Guid userId, ...)` → atualizar configuração
  - [ ] `RemoveServiceConfigAsync(Guid configId, Guid userId)` → remover configuração
  - [ ] `ValidateCredentialsAsync(UserDigitalServiceConfig config)` → validar credenciais
- [ ] Criar `DigitalServiceUsageTracker`:
  - [ ] `TrackUsageAsync(Guid userId, Guid? territoryId, Guid configId, ...)` → registrar uso
  - [ ] `GetConsumptionAsync(Guid userId, ...)` → obter consumo agregado
  - [ ] `CheckQuotaAsync(Guid userId, Guid configId, int unitsToConsume)` → verificar quota
- [ ] Integrar validação de credenciais:
  - [ ] Validar antes de salvar
  - [ ] Validar periodicamente (background job futuro)
- [ ] Validações:
  - [ ] Usuário pode ter múltiplas configs por categoria
  - [ ] Apenas uma config pode ser padrão por categoria
  - [ ] Credenciais devem ser válidas antes de salvar
- [ ] Testes unitários

**Arquivos a Criar**:
- `backend/Araponga.Application/Services/DigitalServiceManager.cs`
- `backend/Araponga.Application/Services/DigitalServiceUsageTracker.cs`
- `backend/Araponga.Tests/Application/DigitalServiceManagerTests.cs`

**Critérios de Sucesso**:
- ✅ Serviço de gerenciamento funcionando
- ✅ Rastreamento de uso funcionando
- ✅ Validação de credenciais funcionando
- ✅ Testes passando

---

#### 25.4 Sistema de Extrato de Consumo Consciente
**Estimativa**: 16 horas (2 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `DigitalServiceConsumptionService`:
  - [ ] `GetUserConsumptionAsync(Guid userId, DateTime? periodStart, DateTime? periodEnd, ...)` → consumo agregado
  - [ ] `GetConsumptionByCategoryAsync(Guid userId, DigitalServiceCategory category, ...)` → consumo por categoria
  - [ ] `GetConsumptionByProviderAsync(Guid userId, ServiceProvider provider, ...)` → consumo por provedor
  - [ ] `GetConsumptionHistoryAsync(Guid userId, ...)` → histórico de uso
- [ ] Agregações:
  - [ ] Consumo mensal por categoria
  - [ ] Consumo mensal por provedor
  - [ ] Top categorias mais usadas
  - [ ] Top provedores mais usados
  - [ ] Custos estimados totais
- [ ] Alertas:
  - [ ] Verificar quotas ao obter consumo
  - [ ] Identificar quando próximo ao limite (80%, 90%, 100%)
- [ ] Criar `DigitalServiceConsumptionController`:
  - [ ] `GET /api/v1/digital-services/me/consumption` → consumo agregado
  - [ ] `GET /api/v1/digital-services/me/consumption/by-category` → por categoria
  - [ ] `GET /api/v1/digital-services/me/consumption/by-provider` → por provedor
  - [ ] `GET /api/v1/digital-services/me/consumption/history` → histórico
- [ ] Feature flags: `DigitalServicesEnabled`
- [ ] Testes

**Arquivos a Criar**:
- `backend/Araponga.Application/Services/DigitalServiceConsumptionService.cs`
- `backend/Araponga.Api/Controllers/DigitalServiceConsumptionController.cs`
- `backend/Araponga.Api/Contracts/DigitalServices/ConsumptionResponse.cs`
- `backend/Araponga.Api/Contracts/DigitalServices/ConsumptionHistoryResponse.cs`

**Critérios de Sucesso**:
- ✅ Extrato de consumo funcionando
- ✅ Agregações corretas
- ✅ Alertas de quota funcionando
- ✅ API funcionando
- ✅ Testes passando

---

### Semana 3: Feature Flags e Integrações

#### 25.5 Feature Flags e Integração com UserPreferences
**Estimativa**: 16 horas (2 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Adicionar feature flags ao enum `FeatureFlag`:
  - [ ] `DigitalServicesEnabled = 25` (master switch territorial)
  - [ ] `DigitalServicesAIEnabled = 26` (IA habilitada)
  - [ ] `DigitalServicesTranslationEnabled = 27` (tradução habilitada)
  - [ ] `DigitalServicesMediaProcessingEnabled = 28` (processamento de mídia)
- [ ] Adicionar preferências a `UserPreferences`:
  - [ ] `DigitalServicesEnabled` (bool, master switch por usuário)
  - [ ] `DigitalServicesAIConfig` (List<Guid>, IDs de configs de IA)
- [ ] Atualizar `UserPreferencesService`:
  - [ ] Permitir atualizar preferências de serviços digitais
- [ ] Atualizar `UserPreferencesController`:
  - [ ] `PUT /api/v1/users/me/preferences/digital-services` → atualizar preferências
- [ ] Validar feature flags antes de executar serviços:
  - [ ] Verificar flag territorial
  - [ ] Verificar flag por categoria
  - [ ] Verificar preferência do usuário
- [ ] Testes

**Arquivos a Modificar**:
- `backend/Araponga.Application/Models/FeatureFlag.cs`
- `backend/Araponga.Domain/Users/UserPreferences.cs`
- `backend/Araponga.Application/Services/UserPreferencesService.cs`
- `backend/Araponga.Api/Controllers/UserPreferencesController.cs`

**Critérios de Sucesso**:
- ✅ Feature flags implementadas
- ✅ Preferências de usuário funcionando
- ✅ Validação de flags funcionando
- ✅ Testes passando

---

#### 25.6 Controller de Configuração de Serviços
**Estimativa**: 16 horas (2 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `DigitalServiceConfigController`:
  - [ ] `GET /api/v1/digital-services/me/configs` → listar configurações
  - [ ] `POST /api/v1/digital-services/me/configs` → adicionar configuração
  - [ ] `GET /api/v1/digital-services/me/configs/{configId}` → obter configuração
  - [ ] `PUT /api/v1/digital-services/me/configs/{configId}` → atualizar configuração
  - [ ] `DELETE /api/v1/digital-services/me/configs/{configId}` → remover configuração
  - [ ] `POST /api/v1/digital-services/me/configs/{configId}/validate` → validar credenciais
  - [ ] `POST /api/v1/digital-services/me/configs/{configId}/set-default` → definir como padrão
- [ ] Validações (FluentValidation):
  - [ ] `AddServiceConfigRequestValidator`
  - [ ] `UpdateServiceConfigRequestValidator`
- [ ] Feature flags: `DigitalServicesEnabled`
- [ ] Segurança:
  - [ ] Credenciais nunca retornadas na API (apenas confirmação)
  - [ ] Apenas usuário pode gerenciar suas configs
  - [ ] Validação de entrada
- [ ] Testes de integração

**Arquivos a Criar**:
- `backend/Araponga.Api/Controllers/DigitalServiceConfigController.cs`
- `backend/Araponga.Api/Contracts/DigitalServices/AddServiceConfigRequest.cs`
- `backend/Araponga.Api/Contracts/DigitalServices/UpdateServiceConfigRequest.cs`
- `backend/Araponga.Api/Contracts/DigitalServices/ServiceConfigResponse.cs`
- `backend/Araponga.Api/Validators/AddServiceConfigRequestValidator.cs`
- `backend/Araponga.Api/Validators/UpdateServiceConfigRequestValidator.cs`
- `backend/Araponga.Tests/Integration/DigitalServiceConfigIntegrationTests.cs`

**Critérios de Sucesso**:
- ✅ API de configuração funcionando
- ✅ Validações funcionando
- ✅ Segurança implementada
- ✅ Testes passando

---

## 📊 Resumo da Fase 25

| Tarefa | Estimativa | Status | Prioridade |
|--------|------------|--------|------------|
| Modelo de Domínio | 24h | ❌ Pendente | 🔴 Alta |
| Criptografia de Credenciais | 16h | ❌ Pendente | 🔴 Alta |
| Gerenciamento de Serviços | 24h | ❌ Pendente | 🔴 Alta |
| Extrato de Consumo | 16h | ❌ Pendente | 🔴 Alta |
| Feature Flags | 16h | ❌ Pendente | 🔴 Alta |
| Controller de Configuração | 16h | ❌ Pendente | 🔴 Alta |
| **Total** | **112h (21 dias)** | | |

---

## ✅ Critérios de Sucesso da Fase 25

### Funcionalidades
- ✅ Sistema completo de configuração de serviços digitais
- ✅ Rastreamento de consumo funcionando
- ✅ Extrato de consumo visível ao usuário
- ✅ Feature flags territoriais e por usuário funcionando
- ✅ Credenciais criptografadas e seguras

### Qualidade
- ✅ Testes com cobertura adequada
- ✅ Documentação completa
- ✅ Segurança implementada
- ✅ Validações completas

### Integração
- ✅ Integração com UserPreferences (Fase 9) funcionando
- ✅ Feature flags integradas ao sistema existente
- ✅ Base preparada para Fase 26 (Chat com IA)

---

## 🔗 Dependências

- **Fase 1**: Segurança (criptografia, validações)
- **Fase 9**: UserPreferences (preferências de serviços digitais)

---

## 📝 Notas de Implementação

### Arquitetura de Serviços Digitais

**Padrão Adapter**:
- Cada serviço digital terá um adapter (`IDigitalServiceAdapter`)
- Adapters serão implementados nas fases seguintes
- Fase 25 apenas cria a infraestrutura base

**Exemplo de Uso Futuro**:
```csharp
// Fase 26 implementará adapters de IA
var aiAdapter = _adapterFactory.GetAdapter<AIServiceAdapter>(provider);
var result = await aiAdapter.ExecuteAsync(request, userConfig, cancellationToken);
await _usageTracker.TrackUsageAsync(userId, territoryId, configId, ...);
```

### Criptografia de Credenciais

**Abordagem**:
- AES-256-GCM para criptografia simétrica
- Chave de criptografia via variável de ambiente
- Credenciais nunca descriptografadas exceto durante uso
- API nunca retorna credenciais descriptografadas

**Segurança**:
- Chave de criptografia não deve estar no código
- Rotação de chaves (implementação futura)
- Auditoria de acesso a credenciais

### Rastreamento de Consumo

**Granularidade**:
- Cada uso de serviço é registrado
- Agregação em tempo real para extrato
- Histórico preservado para auditoria

**Métricas**:
- Unidades consumidas (tokens, requests, bytes, etc.)
- Custo estimado (quando disponível)
- Tempo de processamento
- Taxa de sucesso/falha

---

**Status**: ⏳ **FASE 25 PENDENTE**  
**Depende de**: Fases 1, 9  
**Crítico para**: Autonomia Digital e Consumo Consciente
