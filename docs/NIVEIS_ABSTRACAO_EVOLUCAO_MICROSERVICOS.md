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

## 📊 Comparação de Limitações entre Fases

### Tabela Comparativa: Capacidade e Limitações

| Aspecto | Fase 1: Monolito | Fase 2: APIs Modulares | Fase 3: Microserviços |
|---------|------------------|------------------------|----------------------|
| **Usuários Simultâneos** | ~50-100 | ~200-500 | ~500-1.000 (free) / ~5.000-10.000 (paid) |
| **Usuários Totais** | ~500-1.000 | ~2.000-5.000 | ~10.000-20.000 (free) / ~50.000-100.000 (paid) |
| **Requisições/segundo** | ~10-20 req/s | ~50-100 req/s | ~200-500 req/s (free) / ~1.000-2.000 req/s (paid) |
| **Escalabilidade Horizontal** | ❌ Não | ✅ Parcial (por API) | ✅ Completa (por serviço) |
| **Gargalo Principal** | Banco único + Hardware | Banco compartilhado (500MB) | Recursos compartilhados (SES, SQS) |
| **Ponto Único de Falha** | 🔴 Sim (tudo) | 🟡 Sim (banco) | 🟢 Não (distribuído) |
| **Complexidade Operacional** | 🟢 Baixa | 🟡 Média | 🔴 Alta |
| **Latência** | 🟢 Baixa (in-process) | 🟡 Média (HTTP) | 🔴 Alta (rede distribuída) |
| **Custo Free Tier** | $0/mês | $0/mês | $0/mês |
| **Custo Paid Tier** | ~$10-20/mês (VPS) | ~$25-40/mês | ~$60/mês |
| **Limite de Banco** | Hardware local | 500MB (compartilhado) | 512MB × N (separado) |
| **Limite de Storage** | Disco local | 5GB (Azure Blob) | 10GB (Backblaze B2) |
| **Limite de Email** | 500/dia (Gmail) | 62K/mês (AWS SES) | 62K/mês (AWS SES) |
| **Limite de Eventos** | IMemoryCache | 1M/mês (AWS SQS) | 1M/mês (AWS SQS) |

### Decisão: Quando Migrar?

**Ficar na Fase 1 (Monolito)** quando:
- ✅ Menos de 500 usuários ativos
- ✅ Requisições < 20 req/s
- ✅ Orçamento limitado ($0/mês)
- ✅ Equipe pequena (1-2 desenvolvedores)
- ✅ Não precisa de alta disponibilidade

**Migrar para Fase 2 (APIs Modulares)** quando:
- ⚠️ 500-2.000 usuários ativos
- ⚠️ Requisições 20-100 req/s
- ⚠️ Precisa escalar horizontalmente
- ⚠️ Quer separar responsabilidades
- ⚠️ Orçamento ainda limitado ($0/mês free tier)

**Migrar para Fase 3 (Microserviços)** quando:
- 🔴 Mais de 5.000 usuários ativos
- 🔴 Requisições > 100 req/s
- 🔴 Precisa escalar serviços independentemente
- 🔴 Orçamento disponível ($60+/mês)
- 🔴 Equipe maior (5+ desenvolvedores)
- 🔴 Precisa de alta disponibilidade

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

## 👥 Limitações de Usuários por Instância

### Fase 1: Monolito (Atual)

**Arquitetura**:
- Uma única instância da API
- Um único banco de dados PostgreSQL
- Todos os módulos executando no mesmo processo
- Recursos compartilhados (CPU, RAM, disco)

**Limitações Principais**:

1. **Escalabilidade Vertical Apenas**
   - ⚠️ Limitada pelos recursos da máquina (CPU, RAM, disco)
   - ⚠️ Não pode adicionar mais instâncias (tudo roda em um processo)
   - ⚠️ Upgrade requer melhor hardware, não mais servidores

2. **Banco de Dados Compartilhado**
   - ⚠️ Todos os módulos competem pelos mesmos recursos do banco
   - ⚠️ Queries de um módulo podem impactar outros
   - ⚠️ Gargalo único: se o banco falhar, tudo falha
   - ⚠️ Limite de conexões simultâneas compartilhado

3. **Sem Escalabilidade Horizontal**
   - ❌ Não pode ter múltiplas instâncias da API
   - ❌ Load balancing não é possível
   - ❌ Alta disponibilidade limitada (single point of failure)

4. **Recursos Locais Limitados**
   - ⚠️ Storage limitado pelo disco local
   - ⚠️ Cache limitado pela RAM disponível
   - ⚠️ Processamento limitado pela CPU

**Capacidade Estimada (Free Tier / Local)**:

| Métrica | Capacidade | Observações |
|---------|-----------|-------------|
| **Usuários Simultâneos** | ~50-100 | Depende do hardware (CPU/RAM) |
| **Usuários Totais** | ~500-1.000 | Com uso moderado (não todos ativos) |
| **Requisições/segundo** | ~10-20 req/s | Limitado pela CPU e I/O |
| **Armazenamento** | Limitado pelo disco | Sem limite definido, mas limitado fisicamente |
| **Throughput de Banco** | ~100-200 transações/s | PostgreSQL local |

**Fatores Limitantes Críticos**:

1. **PostgreSQL Local**
   - Performance limitada pelo hardware
   - Sem replicação ou alta disponibilidade
   - Conexões limitadas pela configuração
   - Backup manual

2. **IMemoryCache**
   - Limitado pela RAM disponível
   - Perdido em restart
   - Não compartilhado entre instâncias (se houvesse)

3. **LocalFileStorage**
   - Limitado pelo espaço em disco
   - Sem redundância
   - Backup manual necessário

4. **SMTP Gmail**
   - ⚠️ **500 emails/dia** (limitação crítica)
   - Bloqueio de conta se exceder
   - Não adequado para produção

**Estratégia de Escala**:
- ❌ **Não escalável horizontalmente** (arquitetura não permite)
- ✅ **Apenas escalabilidade vertical** (mais CPU/RAM/SSD)
- ⚠️ **Limite prático**: ~1.000 usuários ativos antes de precisar upgrade significativo
- ⚠️ **Custo de upgrade**: Requer servidor dedicado ou VPS pago

**Cenários de Limitação**:
- **Pico de tráfego**: Sistema fica lento ou indisponível
- **Crescimento de dados**: Banco fica lento, queries demoram
- **Muitos arquivos**: Disco enche, sistema para
- **Muitos emails**: Gmail bloqueia após 500/dia

---

### Fase 2: APIs Modulares (Próximo)

**Arquitetura**:
- Múltiplas APIs (uma por módulo)
- Banco de dados compartilhado (schemas separados)
- Comunicação via HTTP/Eventos
- Recursos compartilhados (banco, storage, cache)

**Limitações Principais**:

1. **Banco Compartilhado (Gargalo)**
   - ⚠️ Ainda é um ponto único de falha
   - ⚠️ Todas as APIs competem pelos mesmos recursos
   - ⚠️ Limite de 500MB (Supabase free tier)
   - ⚠️ Conexões compartilhadas entre todas as APIs

2. **Free Tiers Limitados**
   - ⚠️ Cada recurso tem limites específicos
   - ⚠️ Limites são compartilhados entre todas as APIs
   - ⚠️ Exceder limites requer upgrade para paid tiers

3. **Escalabilidade Parcial**
   - ✅ Cada API pode ter múltiplas instâncias
   - ✅ Load balancing por API
   - ⚠️ Banco ainda limita escalabilidade geral
   - ⚠️ Storage compartilhado pode ser gargalo

4. **Overhead de Comunicação**
   - ⚠️ Latência de rede entre APIs
   - ⚠️ Serialização/deserialização de dados
   - ⚠️ Possibilidade de falhas de rede

**Capacidade Estimada (Free Tier)**:

| Métrica | Capacidade | Observações |
|---------|-----------|-------------|
| **Usuários Simultâneos** | ~200-500 | Distribuído entre APIs |
| **Usuários Totais** | ~2.000-5.000 | Com uso moderado |
| **Requisições/segundo** | ~50-100 req/s | Distribuídas entre APIs |
| **Armazenamento** | 5GB (Azure Blob) | Compartilhado entre todas as APIs |
| **Throughput de Banco** | ~200-500 transações/s | Limitado pelo Supabase free tier |

**Fatores Limitantes por Recurso**:

| Recurso | Limite Free Tier | Impacto na Capacidade | Gargalo? |
|---------|------------------|----------------------|----------|
| **Supabase DB** | 500MB | ~2.000-5.000 usuários ativos | 🔴 SIM |
| **Azure Blob** | 5GB | ~10.000-20.000 arquivos | 🟡 MÉDIO |
| **AWS SES** | 62K/mês | ~2.000 emails/dia | 🟡 MÉDIO |
| **AWS SQS** | 1M/mês | ~33K eventos/dia | 🟢 BAIXO |
| **Redis Cache** | 30MB | Cache limitado para sessões | 🟡 MÉDIO |

**Análise Detalhada dos Limites**:

1. **Supabase (500MB) - Gargalo Principal**
   - **Cálculo**: ~100 bytes/usuário (dados básicos) = ~5.000 usuários
   - **Com posts/comentários**: ~50-100 bytes/usuário adicional = ~2.000-3.000 usuários ativos
   - **Impacto**: Quando exceder, precisa upgrade ($25/mês para 8GB)
   - **Mitigação**: Otimização de queries, índices, limpeza de dados antigos

2. **Azure Blob (5GB)**
   - **Cálculo**: ~250KB/arquivo médio = ~20.000 arquivos
   - **Impacto**: Quando exceder, custo de $0.0184/GB adicional
   - **Mitigação**: Compressão, CDN, limpeza de arquivos não usados

3. **AWS SES (62K/mês)**
   - **Cálculo**: ~2.000 emails/dia = ~60K/mês
   - **Impacto**: Quando exceder, custo de $0.10/1.000 emails
   - **Mitigação**: Email batching, templates eficientes

4. **AWS SQS (1M/mês)**
   - **Cálculo**: ~33K eventos/dia = ~1M/mês
   - **Impacto**: Quando exceder, custo de $0.40/1M mensagens
   - **Mitigação**: Event batching, filtros de eventos

5. **Redis Cache (30MB)**
   - **Cálculo**: ~1KB/sessão = ~30.000 sessões simultâneas
   - **Impacto**: Quando exceder, cache eviction (LRU)
   - **Mitigação**: Cache apenas dados críticos, TTL curto

**Estratégia de Escala**:
- ✅ **Escalabilidade horizontal por API** (pode ter múltiplas instâncias de cada API)
- ✅ **Load balancing** entre instâncias da mesma API
- ⚠️ **Banco ainda é gargalo** (escalabilidade vertical apenas)
- ⚠️ **Storage compartilhado** pode ser gargalo se muitas APIs escrevem simultaneamente
- ⚠️ **Requer upgrade para paid tiers** para crescer além dos limites

**Cenários de Limitação**:
- **Crescimento de dados**: Banco excede 500MB, precisa upgrade
- **Muitas requisições**: Banco fica lento, todas as APIs são afetadas
- **Muitos arquivos**: Storage excede 5GB, custos adicionais
- **Pico de emails**: AWS SES excede 62K/mês, custos adicionais

**Recomendações**:
- Monitorar uso de cada recurso
- Implementar alertas quando próximo dos limites
- Planejar upgrade antes de atingir limites
- Otimizar queries e índices no banco
- Implementar cache agressivo para reduzir carga no banco

---

### Fase 3: Microserviços (Futuro)

**Arquitetura**:
- Microserviços independentes (um por módulo)
- Bancos de dados separados (um por serviço)
- Comunicação via HTTP/Eventos/Mensageria
- Escalabilidade independente por serviço

**Limitações Principais**:

1. **Free Tiers Múltiplos (Mas Somados)**
   - ✅ Cada serviço tem seu próprio banco (512MB × N)
   - ⚠️ Limites são somados, não multiplicados
   - ⚠️ Alguns recursos ainda compartilhados (SES, SQS, Redis)

2. **Complexidade Operacional**
   - ⚠️ Mais serviços para monitorar e gerenciar
   - ⚠️ Deploy mais complexo
   - ⚠️ Debugging distribuído mais difícil
   - ⚠️ Requer orquestração (Kubernetes, Docker Swarm)

3. **Latência de Rede**
   - ⚠️ Comunicação entre serviços adiciona latência
   - ⚠️ Múltiplas chamadas HTTP podem acumular latência
   - ⚠️ Timeout e retry aumentam complexidade

4. **Consistência Distribuída**
   - ⚠️ Transações distribuídas são complexas
   - ⚠️ Consistência eventual (não imediata)
   - ⚠️ Saga Pattern necessário para transações complexas

**Capacidade Estimada (Free Tier)**:

| Métrica | Capacidade | Observações |
|---------|-----------|-------------|
| **Usuários Simultâneos** | ~500-1.000 | Distribuído entre serviços |
| **Usuários Totais** | ~10.000-20.000 | Com uso moderado |
| **Requisições/segundo** | ~200-500 req/s | Distribuídas entre serviços |
| **Armazenamento** | 10GB (Backblaze B2) | Compartilhado, mas mais generoso |
| **Throughput de Banco** | ~500-1.000 transações/s | Distribuído entre bancos |

**Fatores Limitantes por Recurso (Free Tier)**:

| Recurso | Limite Free Tier | Total Disponível | Impacto na Capacidade | Gargalo? |
|---------|------------------|------------------|----------------------|----------|
| **Neon DB (×3)** | 512MB × 3 = 1.5GB | 1.5GB total | ~10.000-20.000 usuários ativos | 🟡 MÉDIO |
| **Backblaze B2** | 10GB | 10GB | ~50.000-100.000 arquivos | 🟢 BAIXO |
| **AWS SES** | 62K/mês | 62K/mês (compartilhado) | ~2.000 emails/dia | 🟡 MÉDIO |
| **AWS SQS** | 1M/mês | 1M/mês (compartilhado) | ~33K eventos/dia | 🟢 BAIXO |
| **Redis Cache** | 30MB | 30MB (compartilhado) | Cache limitado | 🟡 MÉDIO |

**Análise Detalhada dos Limites**:

1. **Neon (512MB × N serviços)**
   - **Cálculo**: 3 serviços × 512MB = 1.5GB total
   - **Por serviço**: ~100 bytes/usuário = ~5.000 usuários/serviço
   - **Total**: ~15.000-20.000 usuários distribuídos
   - **Vantagem**: Gargalo distribuído (não um único banco)
   - **Impacto**: Quando exceder, upgrade por serviço ($19/mês cada para 10GB)
   - **Mitigação**: Escalar apenas serviços que precisam

2. **Backblaze B2 (10GB)**
   - **Cálculo**: ~100KB/arquivo médio = ~100.000 arquivos
   - **Vantagem**: 2x mais espaço que Azure Blob
   - **Impacto**: Quando exceder, custo de $0.005/GB (mais barato que Azure)
   - **Mitigação**: Compressão, CDN, limpeza

3. **AWS SES (62K/mês) - Compartilhado**
   - **Mesmo limite** que Fase 2 (compartilhado entre todos os serviços)
   - **Impacto**: Quando exceder, custo de $0.10/1.000 emails
   - **Mitigação**: Email batching, templates, priorização

4. **AWS SQS (1M/mês) - Compartilhado**
   - **Mesmo limite** que Fase 2 (compartilhado entre todos os serviços)
   - **Impacto**: Quando exceder, custo de $0.40/1M mensagens
   - **Mitigação**: Event batching, filtros, priorização

5. **Redis Cache (30MB) - Compartilhado**
   - **Mesmo limite** que Fase 2 (compartilhado entre todos os serviços)
   - **Impacto**: Quando exceder, cache eviction (LRU)
   - **Mitigação**: Cache apenas dados críticos, TTL curto, cache local por serviço

**Estratégia de Escala**:
- ✅ **Escalabilidade horizontal completa** (cada serviço escala independentemente)
- ✅ **Bancos separados** eliminam gargalo único do banco
- ✅ **Auto-scaling por serviço** conforme demanda específica
- ✅ **Alta disponibilidade** (falha em um serviço não derruba tudo)
- ✅ **Otimização independente** (pode otimizar cada serviço separadamente)
- ⚠️ **Requer orquestração** (Kubernetes, Docker Swarm) para produção
- ⚠️ **Monitoramento distribuído** necessário

**Capacidade com Paid Tiers (~$60/mês)**:

| Métrica | Capacidade | Observações |
|---------|-----------|-------------|
| **Usuários Simultâneos** | ~5.000-10.000 | Com auto-scaling |
| **Usuários Totais** | ~50.000-100.000 | Com uso moderado |
| **Requisições/segundo** | ~1.000-2.000 req/s | Com load balancing |
| **Armazenamento** | Ilimitado | Com custos incrementais |
| **Throughput de Banco** | ~5.000-10.000 transações/s | Distribuído entre bancos |

**Cenários de Limitação**:
- **Crescimento de dados**: Bancos individuais excedem 512MB, upgrade seletivo
- **Serviço específico com alta demanda**: Escala apenas esse serviço
- **Muitos arquivos**: Storage excede 10GB, custos incrementais baixos
- **Pico de emails**: AWS SES excede 62K/mês, custos incrementais

**Vantagens sobre Fase 2**:
- ✅ **Gargalo distribuído**: Não há um único banco limitando tudo
- ✅ **Escala seletiva**: Escala apenas serviços que precisam
- ✅ **Resiliência**: Falha em um serviço não afeta outros
- ✅ **Otimização independente**: Pode otimizar cada serviço separadamente

**Desvantagens**:
- ⚠️ **Complexidade**: Mais difícil de operar e debugar
- ⚠️ **Latência**: Comunicação entre serviços adiciona latência
- ⚠️ **Custos**: Mais serviços = mais custos (mesmo com free tiers)
- ⚠️ **Operação**: Requer DevOps mais sofisticado

**Recomendações**:
- Implementar observabilidade distribuída (tracing, logging centralizado)
- Usar service mesh para comunicação (Istio, Linkerd)
- Implementar circuit breakers e retry policies
- Monitorar cada serviço independentemente
- Planejar upgrades seletivos (apenas serviços que precisam)

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
