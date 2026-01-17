# Fase 4: Observabilidade e Monitoramento

**Duração**: 2 semanas (14 dias úteis)  
**Prioridade**: 🟡 ALTA  
**Bloqueia**: Operação eficiente em produção  
**Estimativa Total**: 80 horas  
**Status**: ✅ 100% Completo

---

## 🎯 Objetivo

Observabilidade completa com métricas, logs e tracing.

---

## 📋 Tarefas Detalhadas

### Semana 7: Logging e Métricas

#### 7.1 Logs Centralizados
**Estimativa**: 24 horas (3 dias)  
**Status**: ✅ 100% Completo

**Tarefas**:
- [x] Escolher plataforma (Seq, Application Insights, ou ELK)
- [x] Configurar Serilog sink para plataforma escolhida
- [x] Adicionar enrichers (MachineName, ThreadId, etc.)
- [x] Configurar níveis de log por ambiente
- [x] Adicionar structured logging em pontos críticos
- [x] Melhorar Correlation ID middleware
- [x] Testar logs centralizados
- [x] Documentar configuração

**Arquivos a Modificar**:
- `backend/Araponga.Api/Program.cs` (Serilog configuration)
- `backend/Araponga.Api/Middleware/CorrelationIdMiddleware.cs`

**Critérios de Sucesso**:
- ✅ Logs centralizados funcionando
- ✅ Enrichers configurados
- ✅ Níveis de log por ambiente
- ✅ Structured logging implementado
- ✅ Correlation ID em todos os logs
- ✅ Documentação completa

---

#### 7.2 Métricas Básicas
**Estimativa**: 32 horas (4 dias)  
**Status**: ✅ 100% Completo

**Tarefas**:
- [x] Escolher plataforma (Prometheus/Grafana ou Application Insights)
- [x] Adicionar pacote de métricas (prometheus-net.AspNetCore)
- [x] Configurar métricas HTTP (request rate, error rate, latência)
- [x] Adicionar métricas de negócio (posts criados, eventos, etc.)
- [x] Adicionar métricas de sistema (CPU, memória, conexões)
- [x] Criar dashboards básicos
- [x] Configurar alertas básicos
- [x] Documentar métricas

**Arquivos a Criar**:
- `backend/Araponga.Application/Metrics/ArapongaMetrics.cs`
- `backend/Araponga.Api/Metrics/` (novo diretório)
- `docs/METRICS.md`

**Arquivos a Modificar**:
- `backend/Araponga.Api/Program.cs`
- Services principais (instrumentar)

**Critérios de Sucesso**:
- ✅ Endpoint /metrics exposto
- ✅ Métricas HTTP automáticas
- ✅ Métricas de negócio coletadas
- ✅ Dashboards criados
- ✅ Alertas configurados
- ✅ Documentação completa

---

### Semana 8: Tracing e Monitoramento Avançado

#### 8.1 Distributed Tracing
**Estimativa**: 24 horas (3 dias)  
**Status**: ✅ 100% Completo

**Tarefas**:
- [x] Adicionar OpenTelemetry
- [x] Configurar tracing para HTTP requests
- [x] Configurar tracing para database queries
- [x] Configurar tracing para eventos
- [x] Integrar com Jaeger ou Application Insights
- [x] Testar distributed tracing
- [x] Documentar configuração

**Arquivos a Criar**:
- `backend/Araponga.Api/Tracing/` (novo diretório)

**Arquivos a Modificar**:
- `backend/Araponga.Api/Program.cs`

**Critérios de Sucesso**:
- ✅ OpenTelemetry configurado
- ✅ Tracing de HTTP requests funcionando
- ✅ Tracing de database queries funcionando
- ✅ Tracing de eventos funcionando
- ✅ Visualização em Jaeger/Application Insights
- ✅ Documentação completa

---

#### 8.2 Monitoramento Avançado
**Estimativa**: 16 horas (2 dias)  
**Status**: ✅ 100% Completo

**Tarefas**:
- [x] Criar dashboard de performance
- [x] Criar dashboard de negócio
- [x] Criar dashboard de sistema
- [x] Configurar alertas críticos
- [x] Configurar alertas de negócio
- [x] Configurar alertas de sistema
- [x] Documentar dashboards e alertas

**Arquivos a Criar**:
- `docs/MONITORING.md`
- Dashboards (Grafana ou Application Insights)

**Critérios de Sucesso**:
- ✅ Dashboards criados
- ✅ Alertas configurados
- ✅ Documentação completa

---

#### 8.3 Runbook e Troubleshooting
**Estimativa**: 16 horas (2 dias)  
**Status**: ✅ 100% Completo

**Tarefas**:
- [x] Criar runbook de operações
- [x] Documentar troubleshooting comum
- [x] Documentar procedimentos de emergência
- [x] Documentar rollback procedures
- [x] Documentar escalação
- [x] Criar playbook de incidentes

**Arquivos a Criar**:
- `docs/RUNBOOK.md`
- `docs/TROUBLESHOOTING.md`
- `docs/INCIDENT_PLAYBOOK.md`

**Critérios de Sucesso**:
- ✅ Runbook completo
- ✅ Troubleshooting documentado
- ✅ Procedimentos de emergência documentados
- ✅ Playbook de incidentes criado

---

## 📊 Resumo da Fase 4

| Tarefa | Estimativa | Status | Prioridade |
|--------|------------|--------|------------|
| Logs Centralizados | 24h | ✅ 100% Completo | 🟡 Alta |
| Métricas Básicas | 32h | ✅ 100% Completo | 🟡 Alta |
| Distributed Tracing | 24h | ✅ 100% Completo | 🟡 Alta |
| Monitoramento Avançado | 16h | ✅ 100% Completo | 🟡 Alta |
| Runbook e Troubleshooting | 16h | ✅ 100% Completo | 🟡 Alta |
| **Total** | **80h (14 dias)** | ✅ **100% Completo** | |

---

#### 4.X Configuração de Mapas e Geo-localização (Complementar)
**Estimativa**: 16 horas (2 dias)  
**Status**: ⏳ Pendente  
**Prioridade**: 🟢 Baixa

**Contexto**: Raio de busca, limites de distância e configuração de provedores de mapas atualmente fixos no código. Esta tarefa permite configuração por território para ajustes baseados em densidade territorial e integração com diferentes provedores.

**Tarefas**:
- [ ] Criar modelo de domínio `MapConfig`:
  - [ ] `Id`, `TerritoryId` (nullable para config global)
  - [ ] `SearchRadiusMeters` (int, raio de busca em metros)
  - [ ] `MaxDistanceMeters` (int, distância máxima para "territórios próximos")
  - [ ] `MapProvider` (enum: Google, Mapbox, OpenStreetMap, etc.)
  - [ ] `ProviderSettings` (JSON, configurações específicas do provider)
  - [ ] `DefaultZoom` (int, nível de zoom padrão)
  - [ ] `Bounds` (JSON, limites de área opcionais)
  - [ ] `Enabled` (bool)
  - [ ] `CreatedAtUtc`, `UpdatedAtUtc`
- [ ] Criar `IMapConfigRepository` e implementações (Postgres, InMemory)
- [ ] Criar `MapConfigService`:
  - [ ] `GetConfigAsync(Guid? territoryId, CancellationToken)` → busca config territorial ou global
  - [ ] `CreateOrUpdateConfigAsync(MapConfig, CancellationToken)`
- [ ] Atualizar serviços de mapa:
  - [ ] Usar `MapConfig` ao buscar territórios próximos
  - [ ] Aplicar raio de busca configurado
  - [ ] Usar provedor de mapas configurado
- [ ] Criar `MapConfigController`:
  - [ ] `GET /api/v1/territories/{territoryId}/map-config` (Curator)
  - [ ] `PUT /api/v1/territories/{territoryId}/map-config` (Curator)
  - [ ] `GET /api/v1/admin/map-config` (global, SystemAdmin)
  - [ ] `PUT /api/v1/admin/map-config` (global, SystemAdmin)
- [ ] Interface administrativa (DevPortal):
  - [ ] Seção para configuração de mapas
  - [ ] Explicação de raio de busca e limites
- [ ] Testes de integração
- [ ] Documentação

**Arquivos a Criar**:
- `backend/Araponga.Domain/Map/MapConfig.cs`
- `backend/Araponga.Application/Interfaces/Map/IMapConfigRepository.cs`
- `backend/Araponga.Application/Services/Map/MapConfigService.cs`
- `backend/Araponga.Api/Controllers/MapConfigController.cs`
- `backend/Araponga.Infrastructure/Postgres/PostgresMapConfigRepository.cs`
- `backend/Araponga.Infrastructure/InMemory/InMemoryMapConfigRepository.cs`
- `backend/Araponga.Tests/Api/MapConfigIntegrationTests.cs`

**Arquivos a Modificar**:
- `backend/Araponga.Application/Services/TerritoryService.cs` (ou serviço de mapas equivalente)
- `backend/Araponga.Infrastructure/InMemory/InMemoryDataStore.cs`
- `backend/Araponga.Api/Extensions/ServiceCollectionExtensions.cs`
- `backend/Araponga.Api/wwwroot/devportal/index.html`

**Critérios de Sucesso**:
- ✅ Configuração de mapas por território
- ✅ Raio de busca configurável
- ✅ Suporte a múltiplos provedores de mapas
- ✅ Interface administrativa disponível
- ✅ Testes passando
- ✅ Documentação atualizada

**Referência**: Consulte `FASE10_CONFIG_FLEXIBILIZACAO_AVALIACAO.md` para contexto completo.

---

## ✅ Critérios de Sucesso da Fase 4

- ✅ Logs centralizados funcionando
- ✅ Enrichers configurados
- ✅ Structured logging implementado
- ✅ Métricas de performance coletadas
- ✅ Métricas de negócio coletadas
- ✅ Dashboards criados
- ✅ Alertas configurados
- ✅ OpenTelemetry configurado
- ✅ Tracing de HTTP requests funcionando
- ✅ Tracing de database queries funcionando
- ✅ Runbook completo
- ✅ Troubleshooting documentado

---

## 🔗 Dependências

- **Fase 1**: Health Checks completos
- **Fase 3**: Redis (para métricas de cache)

---

**Status**: ✅ **FASE 4 100% COMPLETA**  
**Próxima Fase**: Fase 5 - Segurança Avançada
