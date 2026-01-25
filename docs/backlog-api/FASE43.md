# Fase 43: Arquitetura Modular e Deploy Dual (Monolito/Distribuído)

**Duração**: 5 semanas (35 dias úteis)  
**Prioridade**: 🟡 MÉDIA  
**Bloqueia**: Escalabilidade horizontal e deploy flexível  
**Estimativa Total**: 180 horas  
**Status**: ⏳ Pendente  
**Nota**: Renumerada de Fase 19 para Fase 43 (Onda 9: Gamificação e Diferenciação). Fase 19 agora é Demandas e Ofertas.

---

## 🎯 Objetivo

Criar arquitetura modular que permita **duas versões** da aplicação:
1. **Versão Monolito**: Aplicação única, todos os módulos no mesmo processo (simples, ideal para início)
2. **Versão Distribuída**: Módulos separados em serviços independentes (escalável, ideal para crescimento)

Ambas as versões devem compartilhar o máximo de código possível (Domain, Application, Infrastructure compartilhados).

---

## 📋 Contexto e Requisitos

### Problema Atual
A aplicação é um **monolito acoplado** onde:
- Todos os módulos estão no mesmo processo
- Todos compartilham o mesmo banco de dados
- Event Bus é em memória (não funciona entre instâncias)
- Não é possível escalar módulos independentemente
- Não é possível desativar módulos sem desabilitar toda a aplicação

### Requisitos Funcionais
- ✅ Código compartilhado maximizado (Domain, Application, Infrastructure)
- ✅ Configuração flexível (escolher modelo via configuração)
- ✅ Migração gradual (módulo por módulo)
- ✅ Mesma API em ambos os modelos
- ✅ Mesma funcionalidade em ambos os modelos
- ✅ Desativação de módulos sem consumir recursos

---

## 🏗️ Arquitetura Proposta

### Visão Geral

```
┌─────────────────────────────────────────────────────┐
│                   Versão Monolito                    │
│  ┌──────────────────────────────────────────────┐  │
│  │  Araponga.Api (Todos os módulos)              │  │
│  │  ├── Core Module                              │  │
│  │  ├── Feed Module                              │  │
│  │  ├── Marketplace Module                       │  │
│  │  ├── Chat Module                              │  │
│  │  └── ... (todos os módulos)                   │  │
│  └──────────────────────────────────────────────┘  │
│                        │                             │
│                        ▼                             │
│              ┌─────────────────┐                     │
│              │   PostgreSQL    │                     │
│              │  (compartilhado) │                    │
│              └─────────────────┘                     │
└─────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────┐
│                Versão Distribuída                    │
│  ┌─────────┐  ┌─────────┐  ┌─────────┐            │
│  │  API    │  │  API    │  │  API    │            │
│  │ Gateway │  │Gateway  │  │Gateway  │            │
│  └────┬────┘  └────┬────┘  └────┬────┘            │
│       │            │             │                   │
│  ┌────▼────┐  ┌────▼────┐  ┌────▼────┐            │
│  │  Core   │  │  Feed   │  │Marketplace│          │
│  │ Service │  │ Service │  │  Service │            │
│  └────┬────┘  └────┬────┘  └────┬────┘            │
│       │            │             │                   │
│       └────────────┴─────────────┘                   │
│                   │                                   │
│                   ▼                                   │
│       ┌──────────────────────────┐                   │
│       │   Message Broker         │                   │
│       │   (RabbitMQ/Azure SB)    │                   │
│       └──────────────────────────┘                   │
│                   │                                   │
│       ┌───────────┴───────────┐                     │
│       │                       │                       │
│  ┌────▼────┐          ┌──────▼─────┐               │
│  │ Core DB │          │Marketplace │                │
│  │         │          │    DB      │                │
│  └─────────┘          └────────────┘                │
└─────────────────────────────────────────────────────┘
```

---

## 🚩 Módulos Funcionais Identificados

| Módulo | Controllers | Services | Repositories | Depende de |
|--------|-----------|----------|--------------|------------|
| **Core** | Auth, Territories, Memberships | AuthService, TerritoryService, MembershipService | User, Territory, Membership | Nenhuma (base) |
| **Feed** | FeedController | FeedService, PostCreationService, PostInteractionService | FeedRepository | Core, Map (read-only), Notifications |
| **Marketplace** | Stores, Items, Cart, PlatformFees | StoreService, StoreItemService, CartService, PaymentService | Store, StoreItem, Cart, Checkout | Core, Notifications |
| **Chat** | ChatController, TerritoryChatController | ChatService | ChatConversation, ChatMessage | Core |
| **Events** | EventsController | EventsService | TerritoryEvent, EventParticipation | Core, Map (read-only), Notifications |
| **Map** | MapController | MapService | MapEntity, MapEntityRelation | Core, Feed (read-only) |
| **Moderation** | ModerationController | ModerationCaseService, ReportService | Report, Sanction, WorkItem | Core, Feed (read-only) |
| **Notifications** | NotificationsController | - | NotificationInbox, Outbox | Core (todos usam) |
| **Alerts** | AlertsController | - | HealthAlert | Core |
| **Assets** | AssetsController | TerritoryAssetService | TerritoryAsset | Core |
| **Admin** | Admin*Controllers | SystemConfigService, WorkQueueService | SystemConfig, WorkItem | Core |

---

## 📋 Tarefas Detalhadas

### Semana 1: Fundação Modular (Modular Monolith)

#### 43.1 Interface de Módulo e Registry
**Estimativa**: 12 horas (1.5 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `IModule` interface
- [ ] Criar `ModuleBase` abstract class
- [ ] Criar `ModuleRegistry` para gerenciar módulos
- [ ] Implementar registro condicional de módulos
- [ ] Implementar desativação de módulos via configuração
- [ ] Documentar interface de módulo

**Arquivos a Criar**:
- `backend/Araponga.Application/Modules/IModule.cs`
- `backend/Araponga.Application/Modules/ModuleBase.cs`
- `backend/Araponga.Application/Modules/ModuleRegistry.cs`

**Critérios de Sucesso**:
- ✅ Interface de módulo criada
- ✅ Registry funcionando
- ✅ Módulos podem ser desativados via configuração
- ✅ Documentação completa

---

#### 43.2 Organização de Módulos por Funcionalidade
**Estimativa**: 16 horas (2 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar módulo `CoreModule`
- [ ] Criar módulo `FeedModule`
- [ ] Criar módulo `MarketplaceModule`
- [ ] Criar módulo `ChatModule`
- [ ] Criar módulo `EventsModule`
- [ ] Criar módulo `MapModule`
- [ ] Criar módulo `ModerationModule`
- [ ] Criar módulo `AlertsModule`
- [ ] Criar módulo `AssetsModule`
- [ ] Criar módulo `NotificationsModule`
- [ ] Criar módulo `AdminModule`
- [ ] Organizar controllers por módulo (opcional: mover para pastas de módulo)

**Arquivos a Criar**:
- `backend/Araponga.Api/Modules/CoreModule.cs`
- `backend/Araponga.Api/Modules/FeedModule.cs`
- `backend/Araponga.Api/Modules/MarketplaceModule.cs`
- `backend/Araponga.Api/Modules/ChatModule.cs`
- `backend/Araponga.Api/Modules/EventsModule.cs`
- `backend/Araponga.Api/Modules/MapModule.cs`
- `backend/Araponga.Api/Modules/ModerationModule.cs`
- `backend/Araponga.Api/Modules/AlertsModule.cs`
- `backend/Araponga.Api/Modules/AssetsModule.cs`
- `backend/Araponga.Api/Modules/NotificationsModule.cs`
- `backend/Araponga.Api/Modules/AdminModule.cs`

**Critérios de Sucesso**:
- ✅ Módulos criados para todas as funcionalidades
- ✅ Cada módulo registra seus serviços e controllers
- ✅ Módulos podem ser desativados independentemente
- ✅ Documentação completa

---

#### 43.3 Configuração e Desativação de Módulos
**Estimativa**: 8 horas (1 dia)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar configuração `Modules` em `appsettings.json`
- [ ] Atualizar `Program.cs` para usar `ModuleRegistry`
- [ ] Implementar validação de dependências entre módulos
- [ ] Implementar desativação em cascata (se módulo base desativado)
- [ ] Criar health checks por módulo
- [ ] Documentar configuração

**Arquivos a Modificar**:
- `backend/Araponga.Api/Program.cs`
- `backend/Araponga.Api/appsettings.json`

**Arquivos a Criar**:
- `backend/Araponga.Api/appsettings.Development.json` (exemplo)
- `backend/Araponga.Api/appsettings.Production.json` (exemplo)

**Critérios de Sucesso**:
- ✅ Configuração de módulos funcionando
- ✅ Validação de dependências implementada
- ✅ Health checks por módulo funcionando
- ✅ Documentação completa

---

### Semana 2: Abstração de Event Bus e Message Broker

#### 43.4 Abstração de Event Bus
**Estimativa**: 12 horas (1.5 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Verificar `IEventBus` interface existente
- [ ] Criar `MessageBrokerEventBus` para distribuído
- [ ] Manter `InMemoryEventBus` para monolito
- [ ] Criar factory para escolher implementação
- [ ] Migrar eventos para usar abstração
- [ ] Testar ambas as implementações

**Arquivos a Criar**:
- `backend/Araponga.Infrastructure/Messaging/IMessageBroker.cs`
- `backend/Araponga.Infrastructure/Messaging/MessageBrokerEventBus.cs`
- `backend/Araponga.Application/Events/EventBusFactory.cs`

**Arquivos a Modificar**:
- `backend/Araponga.Infrastructure/Eventing/InMemoryEventBus.cs` (se necessário)
- `backend/Araponga.Api/Extensions/ServiceCollectionExtensions.cs`

**Critérios de Sucesso**:
- ✅ Event Bus funciona em monolito (InMemory)
- ✅ Event Bus funciona em distribuído (Message Broker)
- ✅ Escolha via configuração
- ✅ Testes passando

---

#### 43.5 Message Broker (RabbitMQ)
**Estimativa**: 20 horas (2.5 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Escolher Message Broker (RabbitMQ recomendado)
- [ ] Implementar `RabbitMQMessageBroker`
- [ ] Implementar retry e dead letter queue
- [ ] Implementar circuit breaker
- [ ] Implementar mensagens serializadas (JSON)
- [ ] Testes de integração
- [ ] Documentar configuração

**Arquivos a Criar**:
- `backend/Araponga.Infrastructure/Messaging/RabbitMQMessageBroker.cs`
- `backend/Araponga.Infrastructure/Messaging/MessageBrokerOptions.cs`
- `backend/Araponga.Infrastructure/Messaging/RetryPolicy.cs`
- `backend/Araponga.Infrastructure/Messaging/CircuitBreaker.cs`

**Arquivos a Modificar**:
- `backend/Araponga.Api/appsettings.json` (configuração)

**Critérios de Sucesso**:
- ✅ Message Broker funcionando
- ✅ Retry e dead letter queue funcionando
- ✅ Circuit breaker funcionando
- ✅ Testes de integração passando
- ✅ Documentação completa

---

### Semana 3: API Gateway e Service Discovery

#### 43.6 API Gateway (YARP)
**Estimativa**: 20 horas (2.5 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Escolher API Gateway (YARP recomendado - .NET native)
- [ ] Criar projeto `Araponga.Gateway`
- [ ] Configurar roteamento por serviço
- [ ] Implementar load balancing
- [ ] Implementar circuit breaker no gateway
- [ ] Implementar rate limiting no gateway
- [ ] Documentar configuração

**Arquivos a Criar**:
- `backend/Araponga.Gateway/Araponga.Gateway.csproj`
- `backend/Araponga.Gateway/Program.cs`
- `backend/Araponga.Gateway/Configuration/GatewayConfig.cs`
- `backend/Araponga.Gateway/Middleware/GatewayMiddleware.cs`
- `backend/Araponga.Gateway/appsettings.json`

**Critérios de Sucesso**:
- ✅ API Gateway funcionando
- ✅ Roteamento por serviço funcionando
- ✅ Load balancing funcionando
- ✅ Circuit breaker implementado
- ✅ Documentação completa

---

#### 43.7 Service Discovery
**Estimativa**: 12 horas (1.5 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Escolher Service Discovery (Consul ou Kubernetes DNS)
- [ ] Implementar `IServiceDiscovery` interface
- [ ] Implementar `ConsulServiceDiscovery` (ou Kubernetes)
- [ ] Integrar com API Gateway
- [ ] Implementar health checks para service discovery
- [ ] Documentar configuração

**Arquivos a Criar**:
- `backend/Araponga.Application/Interfaces/IServiceDiscovery.cs`
- `backend/Araponga.Infrastructure/ServiceDiscovery/ConsulServiceDiscovery.cs`
- `backend/Araponga.Infrastructure/ServiceDiscovery/ServiceDiscoveryOptions.cs`

**Critérios de Sucesso**:
- ✅ Service Discovery funcionando
- ✅ Integração com API Gateway funcionando
- ✅ Health checks funcionando
- ✅ Documentação completa

---

### Semana 4: Separar Primeiro Serviço (Notifications)

#### 43.8 Serviço Notifications
**Estimativa**: 24 horas (3 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar projeto `Araponga.Api.Notifications`
- [ ] Mover `NotificationsController` para novo projeto
- [ ] Criar banco de dados separado (ou schema separado)
- [ ] Implementar API REST para Notifications
- [ ] Criar `INotificationsApiClient` interface
- [ ] Implementar `NotificationsApiClient` para outros serviços usarem
- [ ] Atualizar outros módulos para usar API de Notifications
- [ ] Testes end-to-end
- [ ] Documentar serviço

**Arquivos a Criar**:
- `backend/Araponga.Api.Notifications/Araponga.Api.Notifications.csproj`
- `backend/Araponga.Api.Notifications/Program.cs`
- `backend/Araponga.Api.Notifications/Controllers/NotificationsController.cs`
- `backend/Araponga.Api.Notifications/appsettings.json`
- `backend/Araponga.Application/Interfaces/INotificationsApiClient.cs`
- `backend/Araponga.Infrastructure/Clients/NotificationsApiClient.cs`

**Arquivos a Modificar**:
- `backend/Araponga.Api/Modules/NotificationsModule.cs` (remover do monolito)

**Critérios de Sucesso**:
- ✅ Notifications como serviço independente
- ✅ Outros módulos usam API de Notifications
- ✅ Funciona em monolito (via módulo) e distribuído (via API)
- ✅ Testes end-to-end passando
- ✅ Documentação completa

---

#### 43.9 Migração de Dependências
**Estimativa**: 12 horas (1.5 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Atualizar `FeedService` para usar `INotificationsApiClient`
- [ ] Atualizar `MarketplaceService` para usar `INotificationsApiClient`
- [ ] Atualizar `EventsService` para usar `INotificationsApiClient`
- [ ] Atualizar `ModerationService` para usar `INotificationsApiClient`
- [ ] Criar factory para escolher implementação (local vs API)
- [ ] Testes de integração
- [ ] Documentar migração

**Arquivos a Modificar**:
- `backend/Araponga.Application/Services/FeedService.cs`
- `backend/Araponga.Application/Services/PaymentService.cs`
- `backend/Araponga.Application/Services/EventsService.cs`
- `backend/Araponga.Application/Services/ReportService.cs`

**Critérios de Sucesso**:
- ✅ Dependências migradas
- ✅ Factory funcionando
- ✅ Testes passando
- ✅ Documentação completa

---

### Semana 5: Documentação e Deploy Dual

#### 43.10 Documentação de Arquitetura Dual
**Estimativa**: 16 horas (2 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `docs/ARCHITECTURE_DUAL.md`
  - [ ] Arquitetura monolito vs distribuída
  - [ ] Diagramas de ambas as arquiteturas
  - [ ] Fluxo de comunicação em ambos os modelos
  - [ ] Decisões arquiteturais
- [ ] Criar `docs/DEPLOY_MONOLITH.md`
  - [ ] Guia completo de deploy monolito
  - [ ] Requisitos de sistema
  - [ ] Configuração passo a passo
  - [ ] Docker compose para monolito
- [ ] Criar `docs/DEPLOY_DISTRIBUTED.md`
  - [ ] Guia completo de deploy distribuído
  - [ ] Requisitos de sistema
  - [ ] Configuração passo a passo
  - [ ] Docker compose para distribuído
  - [ ] Kubernetes manifests
- [ ] Criar `docs/MIGRATION_GUIDE.md`
  - [ ] Como migrar de monolito para distribuído
  - [ ] Passo a passo de migração
  - [ ] Rollback procedures
  - [ ] Troubleshooting

**Arquivos a Criar**:
- `docs/ARCHITECTURE_DUAL.md`
- `docs/DEPLOY_MONOLITH.md`
- `docs/DEPLOY_DISTRIBUTED.md`
- `docs/MIGRATION_GUIDE.md`

**Critérios de Sucesso**:
- ✅ Documentação completa de ambas as arquiteturas
- ✅ Guias de deploy criados
- ✅ Guia de migração completo
- ✅ Diagramas incluídos

---

#### 43.11 Docker Compose e Kubernetes
**Estimativa**: 16 horas (2 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `docker-compose.monolith.yml`
  - [ ] Serviço API (monolito)
  - [ ] PostgreSQL
  - [ ] Redis (opcional)
  - [ ] Health checks
- [ ] Criar `docker-compose.distributed.yml`
  - [ ] API Gateway
  - [ ] Core Service
  - [ ] Feed Service
  - [ ] Marketplace Service
  - [ ] Notifications Service
  - [ ] RabbitMQ
  - [ ] PostgreSQL (compartilhado ou separado)
  - [ ] Redis
  - [ ] Service Discovery (Consul ou Kubernetes)
- [ ] Criar Kubernetes manifests
  - [ ] Deployments para cada serviço
  - [ ] Services (ClusterIP, LoadBalancer)
  - [ ] ConfigMaps e Secrets
  - [ ] Ingress para API Gateway
  - [ ] Health checks
- [ ] Criar scripts de deploy
  - [ ] `scripts/deploy-monolith.sh`
  - [ ] `scripts/deploy-distributed.sh`
- [ ] Documentar deploy

**Arquivos a Criar**:
- `docker-compose.monolith.yml`
- `docker-compose.distributed.yml`
- `k8s/manifests/core-deployment.yaml`
- `k8s/manifests/feed-deployment.yaml`
- `k8s/manifests/marketplace-deployment.yaml`
- `k8s/manifests/notifications-deployment.yaml`
- `k8s/manifests/gateway-deployment.yaml`
- `k8s/manifests/postgres-deployment.yaml`
- `k8s/manifests/rabbitmq-deployment.yaml`
- `scripts/deploy-monolith.sh`
- `scripts/deploy-distributed.sh`

**Critérios de Sucesso**:
- ✅ Docker compose para monolito funcionando
- ✅ Docker compose para distribuído funcionando
- ✅ Kubernetes manifests criados
- ✅ Scripts de deploy funcionando
- ✅ Documentação completa

---

#### 43.12 Testes de Integração e Carga
**Estimativa**: 12 horas (1.5 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar testes de integração para monolito
  - [ ] Testar desativação de módulos
  - [ ] Testar health checks por módulo
  - [ ] Testar comunicação entre módulos
- [ ] Criar testes de integração para distribuído
  - [ ] Testar comunicação via API Gateway
  - [ ] Testar comunicação via Message Broker
  - [ ] Testar service discovery
  - [ ] Testar circuit breaker
- [ ] Criar testes de carga
  - [ ] Teste de carga em monolito
  - [ ] Teste de carga em distribuído
  - [ ] Comparar performance
- [ ] Documentar resultados

**Arquivos a Criar**:
- `backend/Araponga.Tests/Integration/MonolithIntegrationTests.cs`
- `backend/Araponga.Tests/Integration/DistributedIntegrationTests.cs`
- `backend/Araponga.Tests/Performance/MonolithLoadTests.cs`
- `backend/Araponga.Tests/Performance/DistributedLoadTests.cs`

**Critérios de Sucesso**:
- ✅ Testes de integração passando
- ✅ Testes de carga realizados
- ✅ Performance comparada
- ✅ Documentação completa

---

## 📊 Resumo da Fase 43

| Tarefa | Estimativa | Status | Prioridade |
|--------|------------|--------|------------|
| Interface de Módulo e Registry | 12h | ❌ Pendente | 🟢 Média |
| Organização de Módulos | 16h | ❌ Pendente | 🟢 Média |
| Configuração de Módulos | 8h | ❌ Pendente | 🟢 Média |
| Abstração de Event Bus | 12h | ❌ Pendente | 🟢 Média |
| Message Broker (RabbitMQ) | 20h | ❌ Pendente | 🟢 Média |
| API Gateway (YARP) | 20h | ❌ Pendente | 🟢 Média |
| Service Discovery | 12h | ❌ Pendente | 🟢 Média |
| Serviço Notifications | 24h | ❌ Pendente | 🟢 Média |
| Migração de Dependências | 12h | ❌ Pendente | 🟢 Média |
| Documentação de Arquitetura | 16h | ❌ Pendente | 🟢 Média |
| Docker Compose e Kubernetes | 16h | ❌ Pendente | 🟢 Média |
| Testes de Integração e Carga | 12h | ❌ Pendente | 🟢 Média |
| **Total** | **180h (35 dias)** | | |

---

## ✅ Critérios de Sucesso da Fase 43

### Funcionalidades
- ✅ Versão monolito funcionando
- ✅ Versão distribuída funcionando
- ✅ Mesma API em ambos os modelos
- ✅ Mesma funcionalidade em ambos os modelos
- ✅ Módulos podem ser desativados independentemente
- ✅ Migração entre modelos possível

### Qualidade
- ✅ Código compartilhado maximizado (Domain, Application, Infrastructure)
- ✅ Duplicação mínima
- ✅ Testes passando em ambos os modelos
- ✅ Performance comparável

### Documentação
- ✅ Arquitetura documentada (monolito e distribuído)
- ✅ Guias de deploy criados
- ✅ Guia de migração completo
- ✅ Docker compose para ambos os modelos
- ✅ Kubernetes manifests criados

### Operação
- ✅ Deploy monolito funcionando
- ✅ Deploy distribuído funcionando
- ✅ Service discovery funcionando
- ✅ Message Broker funcionando
- ✅ API Gateway funcionando

---

## 🔗 Dependências

- **Fase 4**: Observabilidade completa (para monitorar ambos os modelos)
- **Fase 7**: Sistema de Payout (Notifications depende de eventos de pagamento)

---

## 📝 Notas de Implementação

### Estrutura de Módulos

**Exemplo de Módulo**:
```csharp
public class FeedModule : ModuleBase
{
    public override string Name => "Feed";
    public override string Version => "1.0.0";
    public override ModuleType Type => ModuleType.Both; // Monolith, Distributed, Both
    
    public override void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        if (!IsEnabled) return;
        
        services.AddScoped<FeedService>();
        services.AddScoped<PostCreationService>();
        services.AddScoped<PostInteractionService>();
        services.AddScoped<PostFilterService>();
    }
    
    public override void RegisterControllers(IMvcBuilder mvcBuilder)
    {
        if (!IsEnabled) return;
        
        // Em monolito: registrar controllers diretamente
        if (Type == ModuleType.Monolith || Type == ModuleType.Both)
        {
            mvcBuilder.AddApplicationPart(typeof(FeedController).Assembly);
        }
    }
}
```

### Configuração (Monolito)

```json
{
  "Deployment": {
    "Model": "Monolith"
  },
  "Modules": {
    "Core": {
      "Enabled": true,
      "Required": true
    },
    "Feed": {
      "Enabled": true
    },
    "Marketplace": {
      "Enabled": true
    },
    "Chat": {
      "Enabled": false  // Desativado
    }
  },
  "EventBus": {
    "Provider": "InMemory"
  }
}
```

### Configuração (Distribuído)

```json
{
  "Deployment": {
    "Model": "Distributed"
  },
  "Services": {
    "Core": {
      "BaseUrl": "http://core-service:5000"
    },
    "Notifications": {
      "BaseUrl": "http://notifications-service:5001"
    }
  },
  "MessageBroker": {
    "Provider": "RabbitMQ",
    "ConnectionString": "amqp://guest:guest@rabbitmq:5672"
  }
}
```

---

**Status**: ⏳ **FASE 43 PENDENTE**  
**Prioridade**: 🟡 **FUTURO (Escalabilidade futura)**
