# Plano de Implementação: Requisitos Desejáveis para Produção

## Resumo

Este documento detalha o plano de implementação para os **requisitos desejáveis (pós-lançamento)** identificados na avaliação completa para produção (`docs/50_PRODUCAO_AVALIACAO_COMPLETA.md`).

---

## 📋 Requisitos Desejáveis Planejados

### 1. Índices de Banco de Dados

**Status**: Planejado  
**Prioridade**: Média  
**Complexidade**: Média  
**Estimativa**: 1-2 dias

#### Objetivo

Adicionar índices faltantes identificados na revisão de código para melhorar performance de queries.

#### Índices a Adicionar

1. **`territory_memberships`**:
   - Índice composto: `(user_id, territory_id)`
   - Uso: Busca de membership por usuário e território
   - Impacto: Alto (queries frequentes)

2. **`community_posts`**:
   - Índice composto: `(territory_id, status, created_at_utc)`
   - Uso: Feed do território ordenado por data
   - Impacto: Alto (queries frequentes)

3. **`moderation_reports`**:
   - Índice composto: `(target_type, target_id, created_at_utc)`
   - Uso: Listagem de reports por target
   - Impacto: Médio (queries menos frequentes)

#### Plano de Implementação

1. **Análise de Queries**:
   - Identificar queries lentas via logs
   - Analisar execution plans
   - Priorizar índices por impacto

2. **Criar Migration**:
   ```csharp
   // Migration: AddPerformanceIndexes
   migrationBuilder.CreateIndex(
       name: "IX_territory_memberships_user_territory",
       table: "territory_memberships",
       columns: new[] { "user_id", "territory_id" },
       unique: true);

   migrationBuilder.CreateIndex(
       name: "IX_community_posts_territory_status_created",
       table: "community_posts",
       columns: new[] { "territory_id", "status", "created_at_utc" });

   migrationBuilder.CreateIndex(
       name: "IX_moderation_reports_target_created",
       table: "moderation_reports",
       columns: new[] { "target_type", "target_id", "created_at_utc" });
   ```

3. **Testar Performance**:
   - Comparar tempos de execução antes/depois
   - Verificar impacto em writes
   - Monitorar espaço em disco

4. **Validar em Staging**:
   - Testar com dados de produção (sanitizados)
   - Verificar impactos negativos
   - Ajustar se necessário

#### Critérios de Sucesso

- ✅ Queries críticas com latência < 100ms (P95)
- ✅ Índices criados sem impacto negativo em writes
- ✅ Migration testada em staging

---

### 2. Métricas Básicas

**Status**: Planejado  
**Prioridade**: Média  
**Complexidade**: Média-Alta  
**Estimativa**: 2-3 dias

#### Objetivo

Implementar métricas básicas para observabilidade em produção.

#### Métricas a Implementar

1. **Métricas de Performance**:
   - Request rate (req/s)
   - Error rate (%)
   - Latência (P50, P95, P99)
   - Throughput (bytes/s)

2. **Métricas de Negócio**:
   - Posts criados
   - Eventos criados
   - Membros cadastrados
   - Territórios criados

3. **Métricas de Sistema**:
   - CPU usage
   - Memory usage
   - Database connections
   - Cache hit rate

#### Opções de Implementação

**Opção 1: Application Insights (Azure)**
- Pros: Integração fácil, dashboards prontos, alertas
- Contras: Custo, dependência de Azure
- Estimativa: 2 dias

**Opção 2: Prometheus + Grafana**
- Pros: Open source, flexível, sem vendor lock-in
- Contras: Mais configuração necessária
- Estimativa: 3 dias

**Opção 3: CloudWatch (AWS)**
- Pros: Integração AWS, fácil configuração
- Contras: Custo, dependência de AWS
- Estimativa: 2 dias

#### Recomendação

**Prometheus + Grafana** (Opção 2) para flexibilidade e sem vendor lock-in.

#### Plano de Implementação

1. **Adicionar Prometheus**:
   ```bash
   dotnet add package prometheus-net.AspNetCore
   ```

2. **Configurar Métricas**:
   ```csharp
   // Program.cs
   app.UseHttpMetrics();
   app.MapMetrics();
   ```

3. **Adicionar Métricas Customizadas**:
   ```csharp
   private static readonly Counter PostsCreated = Metrics
       .CreateCounter("araponga_posts_created_total", "Total posts created");

   private static readonly Histogram RequestDuration = Metrics
       .CreateHistogram("araponga_request_duration_seconds", "Request duration");
   ```

4. **Configurar Grafana**:
   - Dashboard para métricas de performance
   - Dashboard para métricas de negócio
   - Alertas básicos

#### Critérios de Sucesso

- ✅ Métricas coletadas corretamente
- ✅ Dashboards funcionando
- ✅ Alertas configurados

---

### 3. Connection Pooling Explícito

**Status**: Planejado  
**Prioridade**: Baixa  
**Complexidade**: Baixa  
**Estimativa**: 1 dia

#### Objetivo

Configurar connection pooling explicitamente para melhor controle e monitoramento.

#### Plano de Implementação

1. **Configurar Pooling no EF Core**:
   ```csharp
   services.AddDbContext<ArapongaDbContext>(options =>
       options.UseNpgsql(connectionString, npgsqlOptions =>
       {
           npgsqlOptions.EnableRetryOnFailure(
               maxRetryCount: 3,
               maxRetryDelay: TimeSpan.FromSeconds(5),
               errorCodesToAdd: null);
           npgsqlOptions.CommandTimeout(30);
       }));
   ```

2. **Configurar Connection String**:
   ```json
   {
     "ConnectionStrings": {
       "Postgres": "Host=...;Port=5432;Database=...;Username=...;Password=...;Pooling=true;MinPoolSize=5;MaxPoolSize=100;Connection Lifetime=300"
     }
   }
   ```

3. **Monitorar Conexões**:
   - Métricas de conexões ativas
   - Alertas para pool exhaustion
   - Logs de conexões

#### Critérios de Sucesso

- ✅ Pool configurado corretamente
- ✅ Métricas de conexões funcionando
- ✅ Sem connection leaks

---

### 4. Exception Mapping com Exceções Tipadas

**Status**: Planejado  
**Prioridade**: Média  
**Complexidade**: Média  
**Estimativa**: 2-3 dias

#### Objetivo

Criar exceções tipadas e mapeamento adequado para melhor tratamento de erros.

#### Plano de Implementação

1. **Criar Exceções Tipadas**:
   ```csharp
   // Application/Exceptions/DomainException.cs
   public class DomainException : Exception
   {
       public DomainException(string message) : base(message) { }
       public DomainException(string message, Exception innerException) : base(message, innerException) { }
   }

   public class ValidationException : DomainException
   {
       public ValidationException(string message) : base(message) { }
   }

   public class NotFoundException : DomainException
   {
       public NotFoundException(string resource, object id) 
           : base($"{resource} with ID {id} was not found.") { }
   }

   public class UnauthorizedException : DomainException
   {
       public UnauthorizedException(string message) : base(message) { }
   }
   ```

2. **Atualizar Exception Handler**:
   ```csharp
   var statusCode = exception switch
   {
       ValidationException => StatusCodes.Status400BadRequest,
       NotFoundException => StatusCodes.Status404NotFound,
       UnauthorizedException => StatusCodes.Status401Unauthorized,
       ArgumentException => StatusCodes.Status400BadRequest,
       _ => StatusCodes.Status500InternalServerError
   };
   ```

3. **Migração Gradual**:
   - Substituir `throw new Exception(...)` por exceções tipadas
   - Atualizar services para usar exceções tipadas
   - Manter compatibilidade com código existente

#### Critérios de Sucesso

- ✅ Exceções tipadas criadas
- ✅ Exception handler atualizado
- ✅ Migração gradual concluída

---

### 5. Validação Completa com Validators

**Status**: Planejado  
**Prioridade**: Baixa  
**Complexidade**: Baixa-Média  
**Estimativa**: 3-5 dias

#### Objetivo

Criar validators para todos os requests críticos usando FluentValidation.

#### Validators a Criar

1. **Auth**:
   - `SocialLoginRequestValidator`

2. **Territories**:
   - `TerritorySearchRequestValidator`
   - `TerritoryNearbyRequestValidator`
   - `TerritorySuggestionRequestValidator`

3. **Memberships**:
   - `DeclareMembershipRequestValidator`

4. **Feed**:
   - `CreatePostRequestValidator` ✅ (já existe)
   - `CreateCommentRequestValidator`
   - `FeedQueryRequestValidator`

5. **Events**:
   - `CreateEventRequestValidator`
   - `UpdateEventRequestValidator`

6. **Map**:
   - `CreateMapEntityRequestValidator`
   - `MapQueryRequestValidator`

7. **Moderation**:
   - `CreateReportRequestValidator`

8. **Marketplace**:
   - `CreateStoreRequestValidator`
   - `CreateListingRequestValidator`
   - `CreateInquiryRequestValidator`

#### Plano de Implementação

1. **Criar Validators**:
   - Um validator por request crítico
   - Mensagens de erro claras
   - Validações de negócio quando necessário

2. **Registrar Validators**:
   - Já configurado automaticamente via `AddValidatorsFromAssemblyContaining<Program>`

3. **Testar Validators**:
   - Testes unitários para cada validator
   - Testes de integração para validação end-to-end

#### Critérios de Sucesso

- ✅ Validators para todos os requests críticos
- ✅ Mensagens de erro claras
- ✅ Testes implementados

---

### 6. Concorrência Otimista

**Status**: Planejado (Pós-lançamento)  
**Prioridade**: Baixa  
**Complexidade**: Média-Alta  
**Estimativa**: 3-5 dias

#### Objetivo

Implementar concorrência otimista para evitar perda de dados em alta concorrência.

#### Entidades a Atualizar

1. **CommunityPost**
2. **TerritoryEvent**
3. **MapEntity**
4. **TerritoryMembership**

#### Plano de Implementação

1. **Adicionar RowVersion**:
   ```csharp
   public class CommunityPost
   {
       public byte[] RowVersion { get; set; }
   }
   ```

2. **Configurar no DbContext**:
   ```csharp
   entity.Property(e => e.RowVersion)
       .IsRowVersion()
       .ValueGeneratedOnAddOrUpdate();
   ```

3. **Tratar ConcurrencyException**:
   ```csharp
   try
   {
       await _unitOfWork.CommitAsync(cancellationToken);
   }
   catch (DbUpdateConcurrencyException)
   {
       throw new DomainException("The entity was modified by another operation.");
   }
   ```

#### Critérios de Sucesso

- ✅ RowVersion em entidades críticas
- ✅ Tratamento de conflitos implementado
- ✅ Testes de concorrência

---

### 7. Distributed Tracing

**Status**: Planejado (Futuro)  
**Prioridade**: Baixa  
**Complexidade**: Alta  
**Estimativa**: 1-2 semanas

#### Objetivo

Implementar distributed tracing quando houver múltiplos serviços.

#### Quando Implementar

- Quando houver separação de serviços (Auth, Feed, Map, etc.)
- Quando houver comunicação assíncrona entre serviços
- Quando precisar rastrear requests através de múltiplos serviços

#### Opções

- **OpenTelemetry**: Padrão da indústria, vendor-agnostic
- **Jaeger**: Open source, popular
- **Zipkin**: Open source, simples
- **Application Insights**: Azure, fácil integração

#### Recomendação

**OpenTelemetry** para flexibilidade e padrão da indústria.

---

### 8. Redis Cache

**Status**: Planejado (Futuro)  
**Prioridade**: Baixa  
**Complexidade**: Média  
**Estimativa**: 3-5 dias

#### Objetivo

Implementar cache distribuído quando houver múltiplas instâncias da aplicação.

#### Quando Implementar

- Quando houver múltiplas instâncias da aplicação
- Quando cache in-memory não for suficiente
- Quando precisar compartilhar cache entre instâncias

#### Plano de Implementação

1. **Adicionar Redis**:
   ```bash
   dotnet add package Microsoft.Extensions.Caching.StackExchangeRedis
   ```

2. **Configurar Cache**:
   ```csharp
   services.AddStackExchangeRedisCache(options =>
   {
       options.Configuration = connectionString;
       options.InstanceName = "Arah:";
   });
   ```

3. **Migrar de IMemoryCache para IDistributedCache**:
   - Atualizar `TerritoryCacheService`
   - Atualizar `FeatureFlagCacheService`
   - Testar performance

---

## 📊 Priorização

### Alta Prioridade (Pós-lançamento Imediato)

1. **Índices de Banco de Dados** - Melhora performance imediata
2. **Métricas Básicas** - Necessário para monitoramento
3. **Exception Mapping** - Melhora tratamento de erros

### Média Prioridade (3-6 meses)

4. **Validação Completa** - Melhora qualidade de dados
5. **Connection Pooling** - Otimização

### Baixa Prioridade (6-12 meses)

6. **Concorrência Otimista** - Quando houver alta concorrência
7. **Distributed Tracing** - Quando houver múltiplos serviços
8. **Redis Cache** - Quando houver múltiplas instâncias

---

## 📝 Checklist de Implementação

### Índices de Banco
- [ ] Análise de queries lentas
- [ ] Criar migration com índices
- [ ] Testar performance
- [ ] Validar em staging
- [ ] Deploy em produção

### Métricas
- [ ] Escolher plataforma (Prometheus/Grafana)
- [ ] Adicionar pacotes NuGet
- [ ] Configurar métricas básicas
- [ ] Criar dashboards
- [ ] Configurar alertas

### Connection Pooling
- [ ] Configurar pooling explicitamente
- [ ] Adicionar retry policies
- [ ] Monitorar conexões
- [ ] Documentar configuração

### Exception Mapping
- [ ] Criar exceções tipadas
- [ ] Atualizar exception handler
- [ ] Migrar código existente
- [ ] Testar tratamento de erros

### Validação Completa
- [ ] Listar requests críticos
- [ ] Criar validators
- [ ] Testar validators
- [ ] Documentar validações

### Concorrência Otimista
- [ ] Identificar entidades críticas
- [ ] Adicionar RowVersion
- [ ] Configurar no DbContext
- [ ] Tratar ConcurrencyException
- [ ] Testar concorrência

---

**Documento criado em**: 2025-01-XX  
**Próxima revisão**: Após lançamento em produção
