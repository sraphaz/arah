# Níveis de Abstração para Evolução até Microserviços

**Data**: 2026-01-27  
**Status**: 📋 Proposta Estratégica  
**Objetivo**: Definir níveis de abstração necessários para evolução Monolito → APIs Modulares → Microserviços, otimizando uso de recursos gratuitos/baratos

---

## 🎯 Objetivo

Propor níveis de abstração que:
1. **Facilitem migração gradual** (Monolito → APIs Modulares → Microserviços)
2. **Otimizem custos** usando recursos gratuitos/baratos em cada fase
3. **Permitam evolução** sem reescrita de código
4. **Mantenham flexibilidade** para trocar provedores

---

## 📊 Evolução Arquitetural

### Fase 1: Monolito Atual (Estado Atual)

```
┌─────────────────────────────────┐
│      Araponga.Api (Única)       │
│  ┌───────────────────────────┐  │
│  │  Módulos (Domain/App)     │  │
│  │  Infrastructure.Shared    │  │
│  │  Modules.*.Infrastructure │  │
│  └───────────────────────────┘  │
│  ┌───────────────────────────┐  │
│  │  PostgreSQL (1 instância) │  │
│  │  Redis (opcional)          │  │
│  │  Local Storage             │  │
│  └───────────────────────────┘  │
└─────────────────────────────────┘
```

**Características**:
- ✅ Uma única API
- ✅ Um banco de dados compartilhado
- ✅ Comunicação in-process
- ✅ Recursos compartilhados

**Custos**:
- Database: PostgreSQL local ou Supabase free tier (500MB) = **$0**
- Storage: LocalFileStorage = **$0**
- Cache: IMemoryCache ou Redis Cloud (30MB) = **$0**
- Email: SMTP Gmail = **$0**
- **Total: $0/mês**

---

### Fase 2: APIs Modulares (Próximo Passo)

```
┌─────────────────────────────────────────┐
│      Araponga.Api.Host (Gateway)       │
└───────────────┬─────────────────────────┘
                │
    ┌───────────┴───────────┬──────────────┐
    │                       │              │
┌───▼───┐            ┌──────▼───┐   ┌──────▼───┐
│ Feed  │            │Marketplace│   │ Events  │
│ :5001 │            │   :5002   │   │  :5003  │
└───┬───┘            └──────┬───┘   └──────┬───┘
    │                      │              │
    │  ┌───────────────────┴──────────────┘
    │  │
┌───▼───▼──────────────────────────────────┐
│  PostgreSQL (1 instância, schemas)      │
│  Redis (compartilhado)                   │
│  Azure Blob Storage (compartilhado)       │
└──────────────────────────────────────────┘
```

**Características**:
- ✅ Múltiplas APIs (uma por módulo)
- ✅ Banco de dados compartilhado (schemas separados)
- ✅ Comunicação via HTTP/Eventos
- ✅ Recursos compartilhados

**Custos**:
- Database: Supabase free tier (500MB) = **$0**
- Storage: Azure Blob Storage free tier (5GB) = **$0**
- Cache: Redis Cloud free tier (30MB) = **$0**
- Email: AWS SES free tier (62K/mês) = **$0**
- Event Bus: InMemory ou AWS SQS free tier (1M/mês) = **$0**
- **Total: $0/mês**

---

### Fase 3: Microserviços (Futuro)

```
┌─────────────────────────────────────────┐
│      API Gateway / Service Mesh         │
└───────────────┬─────────────────────────┘
                │
    ┌───────────┴───────────┬──────────────┐
    │                       │              │
┌───▼───┐            ┌──────▼───┐   ┌──────▼───┐
│ Feed  │            │Marketplace│   │ Events  │
│Service│            │  Service  │   │ Service │
└───┬───┘            └──────┬───┘   └──────┬───┘
    │                      │              │
┌───▼───┐            ┌──────▼───┐   ┌──────▼───┐
│Feed DB│            │Market DB│   │Events DB │
│(Neon) │            │ (Neon)  │   │ (Neon)   │
└───────┘            └─────────┘   └──────────┘
    │                      │              │
    └──────────────────────┴──────────────┘
                    │
        ┌───────────▼───────────┐
        │  Shared Services      │
        │  - Redis (compartilhado)│
        │  - Blob Storage        │
        │  - Event Bus (SQS)     │
        └────────────────────────┘
```

**Características**:
- ✅ Microserviços independentes
- ✅ Bancos de dados separados
- ✅ Comunicação via HTTP/Eventos/Mensageria
- ✅ Escalabilidade independente

**Custos**:
- Database: Neon free tier (512MB) × 3 módulos = **$0** (ou $19/mês × 3 = $57)
- Storage: Backblaze B2 (10GB free) = **$0**
- Cache: Redis Cloud (30MB free) = **$0**
- Email: AWS SES free tier (62K/mês) = **$0**
- Event Bus: AWS SQS free tier (1M/mês) = **$0**
- **Total: $0/mês (free tiers) ou ~$60/mês (paid)**

---

## 🔧 Níveis de Abstração Necessários

### Nível 1: Abstrações de Infraestrutura (Já Implementadas)

**Objetivo**: Permitir troca de provedores sem alterar código de aplicação.

#### 1.1 Cache (`IDistributedCacheService`)
- ✅ **Status**: Implementado
- ✅ **Implementações**: IMemoryCache, Redis
- ✅ **Adequado para**: Todas as fases

**Evolução**:
- **Fase 1**: IMemoryCache (gratuito)
- **Fase 2**: Redis Cloud free tier (30MB) ou IMemoryCache por API
- **Fase 3**: Redis Cloud compartilhado ou Redis por serviço

---

#### 1.2 Storage (`IFileStorage`)
- ✅ **Status**: Implementado
- ✅ **Implementações**: LocalFileStorage, S3FileStorage
- ⚠️ **Falta**: Azure Blob Storage, Backblaze B2

**Evolução**:
- **Fase 1**: LocalFileStorage (gratuito)
- **Fase 2**: Azure Blob Storage free tier (5GB) - **IMPLEMENTAR**
- **Fase 3**: Backblaze B2 (10GB free) ou Azure Blob Storage

**Ação**: Adicionar `AzureBlobStorage` e `BackblazeB2Storage`

---

#### 1.3 Email (`IEmailSender`)
- ✅ **Status**: Implementado
- ✅ **Implementações**: SmtpEmailSender
- ⚠️ **Falta**: SendGrid, Mailgun, AWS SES

**Evolução**:
- **Fase 1**: SMTP Gmail (gratuito, 500/dia)
- **Fase 2**: AWS SES free tier (62K/mês) - **IMPLEMENTAR**
- **Fase 3**: AWS SES pago ($0.10/1K)

**Ação**: Adicionar `AwsSesEmailSender` e `SendGridEmailSender`

---

#### 1.4 Event Bus (`IEventBus`)
- ✅ **Status**: Implementado
- ✅ **Implementações**: InMemoryEventBus
- ⚠️ **Falta**: AWS SQS, Azure Service Bus, RabbitMQ

**Evolução**:
- **Fase 1**: InMemoryEventBus (gratuito)
- **Fase 2**: InMemoryEventBus por API ou AWS SQS free tier (1M/mês) - **IMPLEMENTAR**
- **Fase 3**: AWS SQS compartilhado ou por serviço

**Ação**: Adicionar `AwsSqsEventBus` e `RabbitMqEventBus`

---

### Nível 2: Abstrações de Persistência (Parcialmente Implementadas)

**Objetivo**: Permitir troca de banco de dados e suportar múltiplos DbContexts.

#### 2.1 Database Provider (`IDatabaseProvider`)
- ⚠️ **Status**: Não implementado
- ⚠️ **Necessário para**: Facilitar migração para microserviços

**Proposta**:
```csharp
public interface IDatabaseProvider
{
    string ProviderName { get; }
    DbContext CreateDbContext(string connectionString);
    Task<bool> HealthCheckAsync(CancellationToken cancellationToken);
    Task MigrateAsync(CancellationToken cancellationToken);
}
```

**Implementações**:
- `PostgresDatabaseProvider` (já usado)
- `SqliteDatabaseProvider` (desenvolvimento/testes)
- `NeonDatabaseProvider` (microserviços - serverless PostgreSQL)

**Evolução**:
- **Fase 1**: PostgreSQL local ou Supabase
- **Fase 2**: PostgreSQL com schemas separados
- **Fase 3**: Neon serverless (512MB free por serviço)

**Ação**: Criar `IDatabaseProvider` e implementações

---

#### 2.2 Unit of Work Distribuído (`IDistributedUnitOfWork`)
- ⚠️ **Status**: Não implementado
- ⚠️ **Necessário para**: Transações entre múltiplos DbContexts/microserviços

**Proposta**:
```csharp
public interface IDistributedUnitOfWork
{
    Task BeginTransactionAsync(CancellationToken cancellationToken);
    Task CommitAsync(CancellationToken cancellationToken);
    Task RollbackAsync(CancellationToken cancellationToken);
    void RegisterContext(DbContext context);
}
```

**Implementações**:
- `LocalUnitOfWork` (Fase 1: transações locais)
- `SagaUnitOfWork` (Fase 2-3: Saga Pattern para transações distribuídas)

**Evolução**:
- **Fase 1**: `IUnitOfWork` local (já implementado)
- **Fase 2**: `IDistributedUnitOfWork` com Saga Pattern
- **Fase 3**: Saga Pattern completo entre microserviços

**Ação**: Criar `IDistributedUnitOfWork` e implementação Saga

---

### Nível 3: Abstrações de Comunicação (Parcialmente Implementadas)

**Objetivo**: Permitir comunicação entre módulos/microserviços.

#### 3.1 Service Discovery (`IServiceDiscovery`)
- ⚠️ **Status**: Não implementado
- ⚠️ **Necessário para**: APIs Modulares e Microserviços

**Proposta**:
```csharp
public interface IServiceDiscovery
{
    Task<ServiceEndpoint> ResolveAsync(string serviceName, CancellationToken cancellationToken);
    Task RegisterAsync(string serviceName, ServiceEndpoint endpoint, CancellationToken cancellationToken);
    Task UnregisterAsync(string serviceName, CancellationToken cancellationToken);
}
```

**Implementações**:
- `InMemoryServiceDiscovery` (Fase 2: desenvolvimento)
- `ConsulServiceDiscovery` (Fase 3: produção)
- `KubernetesServiceDiscovery` (Fase 3: Kubernetes)

**Evolução**:
- **Fase 1**: Não necessário (in-process)
- **Fase 2**: `InMemoryServiceDiscovery` ou configuração estática
- **Fase 3**: Consul ou Kubernetes Service Discovery

**Ação**: Criar `IServiceDiscovery` e implementações

---

#### 3.2 HTTP Client Factory (`IModuleHttpClient`)
- ⚠️ **Status**: Não implementado
- ⚠️ **Necessário para**: Comunicação HTTP entre APIs/microserviços

**Proposta**:
```csharp
public interface IModuleHttpClient
{
    Task<TResponse> GetAsync<TResponse>(string module, string endpoint, CancellationToken cancellationToken);
    Task<TResponse> PostAsync<TRequest, TResponse>(string module, string endpoint, TRequest request, CancellationToken cancellationToken);
    Task<TResponse> PutAsync<TRequest, TResponse>(string module, string endpoint, TRequest request, CancellationToken cancellationToken);
    Task DeleteAsync(string module, string endpoint, CancellationToken cancellationToken);
}
```

**Implementações**:
- `InProcessModuleHttpClient` (Fase 1: in-process)
- `HttpModuleHttpClient` (Fase 2-3: HTTP real)
- `ResilientModuleHttpClient` (Fase 3: com retry/circuit breaker)

**Evolução**:
- **Fase 1**: In-process (não necessário)
- **Fase 2**: HTTP real entre APIs
- **Fase 3**: HTTP com resiliência (retry, circuit breaker)

**Ação**: Criar `IModuleHttpClient` e implementações

---

#### 3.3 Event Bus Distribuído (`IDistributedEventBus`)
- ⚠️ **Status**: Parcial (InMemoryEventBus existe, mas não é distribuído)
- ⚠️ **Necessário para**: Eventos entre APIs/microserviços

**Proposta**:
```csharp
public interface IDistributedEventBus : IEventBus
{
    Task SubscribeAsync<TEvent>(string subscriptionName, CancellationToken cancellationToken)
        where TEvent : IAppEvent;
    Task UnsubscribeAsync<TEvent>(string subscriptionName, CancellationToken cancellationToken)
        where TEvent : IAppEvent;
}
```

**Implementações**:
- `InMemoryEventBus` (Fase 1: atual)
- `AwsSqsEventBus` (Fase 2-3: AWS SQS)
- `RabbitMqEventBus` (Fase 2-3: RabbitMQ)
- `AzureServiceBusEventBus` (Fase 3: Azure)

**Evolução**:
- **Fase 1**: InMemoryEventBus (gratuito)
- **Fase 2**: AWS SQS free tier (1M/mês) - **IMPLEMENTAR**
- **Fase 3**: AWS SQS pago ou RabbitMQ

**Ação**: Criar `IDistributedEventBus` e implementações

---

### Nível 4: Abstrações de Configuração (Parcialmente Implementadas)

**Objetivo**: Facilitar configuração de múltiplos provedores e ambientes.

#### 4.1 Infrastructure Factory (`IInfrastructureFactory`)
- ⚠️ **Status**: Não implementado
- ⚠️ **Necessário para**: Centralizar criação de serviços de infraestrutura

**Proposta**:
```csharp
public interface IInfrastructureFactory
{
    IFileStorage CreateFileStorage(IConfiguration configuration);
    IEmailSender CreateEmailSender(IConfiguration configuration, IServiceProvider serviceProvider);
    IDistributedCacheService CreateCacheService(IConfiguration configuration, IServiceProvider serviceProvider);
    IDistributedEventBus CreateEventBus(IConfiguration configuration, IServiceProvider serviceProvider);
    IDatabaseProvider CreateDatabaseProvider(IConfiguration configuration);
}
```

**Implementação**:
- `InfrastructureFactory` (factory pattern centralizado)

**Benefícios**:
- ✅ Configuração centralizada
- ✅ Fácil trocar provedores
- ✅ Suporte a múltiplos ambientes

**Ação**: Criar `IInfrastructureFactory` e implementação

---

#### 4.2 Configuration Provider (`IConfigurationProvider`)
- ⚠️ **Status**: Não implementado
- ⚠️ **Necessário para**: Configuração dinâmica por módulo/ambiente

**Proposta**:
```csharp
public interface IConfigurationProvider
{
    T GetConfiguration<T>(string key) where T : class;
    string GetConnectionString(string name);
    bool IsFeatureEnabled(string feature);
    string GetProvider(string serviceType);
}
```

**Implementações**:
- `AppSettingsConfigurationProvider` (atual)
- `EnvironmentConfigurationProvider` (variáveis de ambiente)
- `ConsulConfigurationProvider` (Fase 3: Consul)

**Ação**: Criar `IConfigurationProvider` e implementações

---

## 📋 Mapeamento: Fase → Recursos Gratuitos → Abstrações

### Fase 1: Monolito (Atual)

| Serviço | Recurso Gratuito | Abstração | Status |
|---------|------------------|-----------|--------|
| **Database** | PostgreSQL local / Supabase (500MB) | `IUnitOfWork` | ✅ Implementado |
| **Storage** | LocalFileStorage | `IFileStorage` | ✅ Implementado |
| **Cache** | IMemoryCache | `IDistributedCacheService` | ✅ Implementado |
| **Email** | SMTP Gmail (500/dia) | `IEmailSender` | ✅ Implementado |
| **Event Bus** | InMemoryEventBus | `IEventBus` | ✅ Implementado |

**Custo Total**: **$0/mês**

**Ações**:
- ✅ Manter abstrações atuais
- ⏳ Adicionar SQLite para desenvolvimento/testes

---

### Fase 2: APIs Modulares (Próximo)

| Serviço | Recurso Gratuito | Abstração | Status |
|---------|------------------|-----------|--------|
| **Database** | Supabase (500MB) / PostgreSQL schemas | `IDatabaseProvider` | ⏳ Implementar |
| **Storage** | Azure Blob Storage (5GB) | `IFileStorage` | ⏳ Adicionar AzureBlob |
| **Cache** | Redis Cloud (30MB) | `IDistributedCacheService` | ✅ Implementado |
| **Email** | AWS SES (62K/mês) | `IEmailSender` | ⏳ Adicionar AwsSes |
| **Event Bus** | AWS SQS (1M/mês) | `IDistributedEventBus` | ⏳ Implementar |
| **Service Discovery** | Configuração estática | `IServiceDiscovery` | ⏳ Implementar |
| **HTTP Client** | HTTP padrão | `IModuleHttpClient` | ⏳ Implementar |

**Custo Total**: **$0/mês** (free tiers)

**Ações**:
1. ⏳ Implementar `IDatabaseProvider` (Postgres, SQLite)
2. ⏳ Adicionar `AzureBlobStorage`
3. ⏳ Adicionar `AwsSesEmailSender`
4. ⏳ Implementar `IDistributedEventBus` com AWS SQS
5. ⏳ Implementar `IServiceDiscovery` (InMemory)
6. ⏳ Implementar `IModuleHttpClient`
7. ⏳ Criar `IInfrastructureFactory`

---

### Fase 3: Microserviços (Futuro)

| Serviço | Recurso Gratuito/Barato | Abstração | Status |
|---------|------------------------|-----------|--------|
| **Database** | Neon (512MB free) ou Supabase (500MB) | `IDatabaseProvider` | ⏳ Adicionar Neon |
| **Storage** | Backblaze B2 (10GB free) | `IFileStorage` | ⏳ Adicionar Backblaze |
| **Cache** | Redis Cloud (30MB free) | `IDistributedCacheService` | ✅ Implementado |
| **Email** | AWS SES (62K/mês free) | `IEmailSender` | ⏳ Adicionar AwsSes |
| **Event Bus** | AWS SQS (1M/mês free) | `IDistributedEventBus` | ⏳ Implementar |
| **Service Discovery** | Consul / Kubernetes | `IServiceDiscovery` | ⏳ Adicionar Consul |
| **HTTP Client** | HTTP com resiliência | `IModuleHttpClient` | ⏳ Adicionar resiliência |
| **Unit of Work** | Saga Pattern | `IDistributedUnitOfWork` | ⏳ Implementar |

**Custo Total**: **$0/mês** (free tiers) ou **~$60/mês** (paid)

**Ações**:
1. ⏳ Adicionar `NeonDatabaseProvider`
2. ⏳ Adicionar `BackblazeB2Storage`
3. ⏳ Implementar `IDistributedUnitOfWork` com Saga Pattern
4. ⏳ Adicionar `ConsulServiceDiscovery`
5. ⏳ Adicionar resiliência ao `IModuleHttpClient` (retry, circuit breaker)

---

## 🎯 Proposta de Implementação por Prioridade

### Prioridade 1: Essenciais para Fase 2 (APIs Modulares)

**Objetivo**: Permitir migração para APIs modulares usando recursos gratuitos.

#### 1.1 Abstrações de Comunicação (2 semanas)

**Implementar**:
- ✅ `IDistributedEventBus` com AWS SQS
- ✅ `IModuleHttpClient` básico
- ✅ `IServiceDiscovery` InMemory

**Benefícios**:
- Comunicação entre APIs modulares
- Eventos distribuídos
- Zero custo (AWS SQS free tier)

---

#### 1.2 Abstrações de Storage (1 semana)

**Implementar**:
- ✅ `AzureBlobStorage` (5GB free)
- ✅ Atualizar `InfrastructureFactory`

**Benefícios**:
- Storage compartilhado entre APIs
- Zero custo (Azure Blob free tier)
- Escalável

---

#### 1.3 Abstrações de Email (1 semana)

**Implementar**:
- ✅ `AwsSesEmailSender` (62K/mês free)
- ✅ Atualizar `InfrastructureFactory`

**Benefícios**:
- Email escalável
- Zero custo (AWS SES free tier)
- Melhor deliverability

---

### Prioridade 2: Essenciais para Fase 3 (Microserviços)

**Objetivo**: Facilitar migração para microserviços.

#### 2.1 Abstrações de Database (2 semanas)

**Implementar**:
- ✅ `IDatabaseProvider`
- ✅ `SqliteDatabaseProvider` (desenvolvimento)
- ✅ `NeonDatabaseProvider` (microserviços)

**Benefícios**:
- Troca fácil de banco de dados
- Suporte a serverless (Neon)
- Zero custo (Neon free tier: 512MB)

---

#### 2.2 Abstrações de Transação (2 semanas)

**Implementar**:
- ✅ `IDistributedUnitOfWork`
- ✅ `SagaUnitOfWork` (Saga Pattern)

**Benefícios**:
- Transações distribuídas
- Consistência eventual
- Preparado para microserviços

---

#### 2.3 Service Discovery Avançado (1 semana)

**Implementar**:
- ✅ `ConsulServiceDiscovery`
- ✅ `KubernetesServiceDiscovery`

**Benefícios**:
- Descoberta automática de serviços
- Load balancing
- Health checks

---

### Prioridade 3: Otimizações e Melhorias

**Objetivo**: Melhorar resiliência e performance.

#### 3.1 HTTP Client Resiliente (1 semana)

**Implementar**:
- ✅ `ResilientModuleHttpClient` (retry, circuit breaker)
- ✅ Integração com Polly

**Benefícios**:
- Resiliência a falhas
- Melhor experiência do usuário
- Preparado para produção

---

#### 3.2 Storage Adicional (1 semana)

**Implementar**:
- ✅ `BackblazeB2Storage` (10GB free, mais barato)

**Benefícios**:
- Custo menor ($0.005/GB vs $0.0184/GB Azure)
- 10GB free tier

---

## 📊 Resumo: Níveis de Abstração por Fase

### Fase 1: Monolito (Atual)

**Abstrações Necessárias**:
- ✅ `IDistributedCacheService` - ✅ Implementado
- ✅ `IFileStorage` - ✅ Implementado
- ✅ `IEmailSender` - ✅ Implementado
- ✅ `IEventBus` - ✅ Implementado
- ✅ `IUnitOfWork` - ✅ Implementado

**Status**: ✅ **Adequado** - Faltam apenas implementações gratuitas adicionais

---

### Fase 2: APIs Modulares

**Abstrações Necessárias**:
- ✅ `IDistributedCacheService` - ✅ Implementado
- ✅ `IFileStorage` - ⚠️ Adicionar AzureBlob
- ✅ `IEmailSender` - ⚠️ Adicionar AwsSes
- ✅ `IDistributedEventBus` - ⏳ **IMPLEMENTAR**
- ✅ `IModuleHttpClient` - ⏳ **IMPLEMENTAR**
- ✅ `IServiceDiscovery` - ⏳ **IMPLEMENTAR**
- ✅ `IDatabaseProvider` - ⏳ **IMPLEMENTAR**

**Status**: ⚠️ **Parcial** - Faltam abstrações de comunicação

---

### Fase 3: Microserviços

**Abstrações Necessárias**:
- ✅ `IDistributedCacheService` - ✅ Implementado
- ✅ `IFileStorage` - ⚠️ Adicionar Backblaze
- ✅ `IEmailSender` - ⚠️ Adicionar AwsSes
- ✅ `IDistributedEventBus` - ⏳ **IMPLEMENTAR**
- ✅ `IModuleHttpClient` - ⚠️ Adicionar resiliência
- ✅ `IServiceDiscovery` - ⚠️ Adicionar Consul/K8s
- ✅ `IDatabaseProvider` - ⚠️ Adicionar Neon
- ✅ `IDistributedUnitOfWork` - ⏳ **IMPLEMENTAR**

**Status**: ⚠️ **Parcial** - Faltam abstrações de transação e service discovery

---

## 💰 Otimização de Custos por Fase

### Fase 1: Monolito
- **Custo**: $0/mês (100% gratuito)
- **Recursos**: Local, PostgreSQL local, SMTP Gmail

### Fase 2: APIs Modulares
- **Custo**: $0/mês (free tiers)
- **Recursos**: Supabase (500MB), Azure Blob (5GB), AWS SES (62K/mês), AWS SQS (1M/mês)

### Fase 3: Microserviços
- **Custo**: $0/mês (free tiers) ou ~$60/mês (paid)
- **Recursos**: Neon (512MB × 3), Backblaze B2 (10GB), AWS SES, AWS SQS

---

## 🚀 Plano de Implementação Recomendado

### Sprint 1-2: Preparação para APIs Modulares (4 semanas)

**Semana 1-2: Comunicação entre APIs**
- [ ] Implementar `IDistributedEventBus` com AWS SQS
- [ ] Implementar `IModuleHttpClient` básico
- [ ] Implementar `IServiceDiscovery` InMemory

**Semana 3: Storage e Email**
- [ ] Adicionar `AzureBlobStorage`
- [ ] Adicionar `AwsSesEmailSender`

**Semana 4: Factory e Configuração**
- [ ] Criar `IInfrastructureFactory`
- [ ] Criar `IConfigurationProvider`
- [ ] Documentar configurações

---

### Sprint 3-4: Preparação para Microserviços (4 semanas)

**Semana 5-6: Database e Transações**
- [ ] Implementar `IDatabaseProvider`
- [ ] Adicionar `SqliteDatabaseProvider`
- [ ] Adicionar `NeonDatabaseProvider`
- [ ] Implementar `IDistributedUnitOfWork` com Saga

**Semana 7: Service Discovery**
- [ ] Adicionar `ConsulServiceDiscovery`
- [ ] Adicionar `KubernetesServiceDiscovery`

**Semana 8: Resiliência**
- [ ] Adicionar resiliência ao `IModuleHttpClient`
- [ ] Adicionar `BackblazeB2Storage`

---

## ✅ Checklist de Implementação

### Abstrações Essenciais (Fase 2)
- [ ] `IDistributedEventBus` - ⏳ **PRIORIDADE ALTA**
- [ ] `IModuleHttpClient` - ⏳ **PRIORIDADE ALTA**
- [ ] `IServiceDiscovery` - ⏳ **PRIORIDADE ALTA**
- [ ] `IDatabaseProvider` - ⏳ **PRIORIDADE ALTA**
- [ ] `IInfrastructureFactory` - ⏳ **PRIORIDADE MÉDIA**

### Implementações Gratuitas (Fase 2)
- [ ] `AzureBlobStorage` - ⏳ **PRIORIDADE ALTA**
- [ ] `AwsSesEmailSender` - ⏳ **PRIORIDADE ALTA**
- [ ] `AwsSqsEventBus` - ⏳ **PRIORIDADE ALTA**

### Abstrações Avançadas (Fase 3)
- [ ] `IDistributedUnitOfWork` - ⏳ **PRIORIDADE MÉDIA**
- [ ] `NeonDatabaseProvider` - ⏳ **PRIORIDADE MÉDIA**
- [ ] `ConsulServiceDiscovery` - ⏳ **PRIORIDADE BAIXA**
- [ ] `ResilientModuleHttpClient` - ⏳ **PRIORIDADE BAIXA**
- [ ] `BackblazeB2Storage` - ⏳ **PRIORIDADE BAIXA**

---

## 📚 Referências

- **Plano de Modularização**: `PLANO_MODULARIZACAO_DESACOPLAMENTO_REAL.md`
- **Abstrações Gratuitas**: `ABSTRACOES_SERVICOS_GRATUITOS_BARATOS.md`
- **APIs Modulares**: `AVALIACAO_ARQUITETURA_APIS_MODULARES.md`

---

**Última Atualização**: 2026-01-27  
**Status**: 📋 Proposta Completa - Pronto para Implementação
