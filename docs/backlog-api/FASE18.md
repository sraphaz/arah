# Fase 18: Saúde Territorial e Monitoramento

**Duração**: 5 semanas (35 dias úteis)  
**Prioridade**: 🔴 ALTA (Soberania territorial e autonomia comunitária)  
**Depende de**: Fase 9 (Perfil de Usuário)  
**Integra com**: Fase 17 (Gamificação) - pode ser desenvolvido em paralelo  
**Estimativa Total**: 200 horas  
**Status**: ⏳ Pendente

---

## 🎯 Objetivo

Implementar sistema completo de **saúde territorial e monitoramento** que:
- Permite comunidades monitorarem a saúde do seu território (água, ar, solo, biodiversidade, resíduos)
- Facilita observações colaborativas de saúde
- Integra sensores físicos para monitoramento automatizado
- Calcula indicadores de saúde territorial
- Organiza ações territoriais (mutirões, plantio, coleta, manutenção)
- **Gamifica atividades territoriais** (integração com Fase 17)
- **Gera moeda territorial** por atividades (integração com Fase 20)

**Princípios**:
- ✅ **Colaboração Comunitária**: Observações e ações são comunitárias
- ✅ **Transparência**: Dados de saúde são públicos (ou para moradores)
- ✅ **Ação Local**: Foco em ações que melhoram o território
- ✅ **Gamificação Harmoniosa**: Atividades geram contribuições e moeda
- ✅ **Autonomia**: Comunidades decidem o que monitorar e como agir

---

## 📋 Contexto e Requisitos

### Estado Atual
- ✅ MER prevê estrutura completa (`HEALTH_OBSERVATION`, `TERRITORY_ACTION`, `SENSOR_DEVICE`, etc.)
- ✅ `HealthService` básico (apenas alertas simples)
- ✅ `HealthAlert` domain model básico
- ❌ Não existe sistema completo de observações de saúde
- ❌ Não existe sistema de sensores
- ❌ Não existe sistema de indicadores
- ❌ Não existe sistema de ações territoriais
- ❌ Não existe gamificação de atividades territoriais

### Requisitos Funcionais

#### 1. Sistema de Observações de Saúde
- ✅ Criar observação de saúde (água, ar, solo, biodiversidade, resíduos, segurança, mobilidade, bem-estar)
- ✅ Georreferenciamento (localização precisa)
- ✅ Severidade: INFO, WARNING, URGENT
- ✅ Visibilidade: PUBLIC, RESIDENT_ONLY
- ✅ Status: OPEN, UNDER_REVIEW, CONFIRMED, RESOLVED, REJECTED
- ✅ Confirmações colaborativas (outros usuários podem confirmar)
- ✅ Relacionamento com recursos naturais (`NATURAL_ASSET`)
- ✅ **Gamificação**: Observação confirmada gera contribuição (Fase 17)

#### 2. Sistema de Sensores
- ✅ Registrar sensores físicos (pluviômetro, qualidade do ar, nível de água, etc.)
- ✅ Tipos: RAIN_GAUGE, WATER_LEVEL, AIR_QUALITY, WATER_QUALITY, WEATHER
- ✅ Status: ACTIVE, MAINTENANCE, RETIRED
- ✅ Leituras automáticas (via API externa ou manual)
- ✅ Relacionamento com métricas de saúde
- ✅ **Gamificação**: Leitura confirmada gera contribuição (Fase 17)

#### 3. Indicadores de Saúde Territorial
- ✅ Calcular indicadores agregados (diário, semanal, mensal)
- ✅ Métodos: AVG, MAX, INDEX_FORMULA
- ✅ Visualização de tendências
- ✅ Alertas automáticos quando indicadores pioram
- ✅ Dashboard de saúde territorial

#### 4. Ações Territoriais
- ✅ Criar ação territorial (mutirão, manutenção, educação, restauração, monitoramento)
- ✅ Organizar ação (data, hora, localização)
- ✅ Participação de usuários
- ✅ Status: PLANNED, IN_PROGRESS, DONE, CANCELLED
- ✅ Relacionamento com observações (ação responde a observação)
- ✅ **Gamificação**: Participação gera contribuição (Fase 17)
- ✅ **Moeda**: Participação pode gerar moeda territorial (Fase 20)

#### 5. Atividades Específicas
- ✅ **Coleta de Resíduos**: Reportar coleta (tipo, volume, localização)
- ✅ **Plantio**: Reportar plantio (espécie, quantidade, localização)
- ✅ **Manutenção de Recursos Naturais**: Reportar manutenção (tipo, recurso)
- ✅ **Gamificação**: Cada atividade gera contribuição e pontos (Fase 17)
- ✅ **Moeda**: Cada atividade pode gerar moeda territorial (Fase 20)

---

## 📋 Tarefas Detalhadas

### Semana 1-2: Modelo de Domínio e Observações de Saúde

#### 18.1 Modelo de Domínio - Saúde Territorial
**Estimativa**: 24 horas (3 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar enum `HealthDomain`:
  - [ ] `WATER` (água)
  - [ ] `AIR` (ar)
  - [ ] `SOIL` (solo)
  - [ ] `BIODIVERSITY` (biodiversidade)
  - [ ] `WASTE` (resíduos)
  - [ ] `SAFETY` (segurança)
  - [ ] `MOBILITY` (mobilidade)
  - [ ] `WELLBEING` (bem-estar)
- [ ] Criar enum `HealthSeverity`:
  - [ ] `INFO` (informativo)
  - [ ] `WARNING` (aviso)
  - [ ] `URGENT` (urgente)
- [ ] Criar enum `HealthObservationStatus`:
  - [ ] `OPEN` (aberta)
  - [ ] `UNDER_REVIEW` (em revisão)
  - [ ] `CONFIRMED` (confirmada)
  - [ ] `RESOLVED` (resolvida)
  - [ ] `REJECTED` (rejeitada)
- [ ] Criar modelo `HealthDomain`:
  - [ ] `Id`, `Name`, `Description`, `CreatedAtUtc`
- [ ] Criar modelo `HealthMetric`:
  - [ ] `Id`, `DomainId`, `Key` (ex: "water.turbidity_ntu")
  - [ ] `Name`, `Unit` (NTU, PPM, UG_M3, MM, CM, INDEX)
  - [ ] `ValueType` (DECIMAL, INTEGER, BOOLEAN, TEXT, INDEX)
  - [ ] `Description`, `CreatedAtUtc`
- [ ] Criar modelo `HealthObservation`:
  - [ ] `Id`, `TerritoryId`, `DomainId`, `MetricId?` (nullable)
  - [ ] `ReporterUserId?` (nullable, pode ser anônimo)
  - [ ] `RelatedNaturalAssetId?` (nullable)
  - [ ] `Severity` (HealthSeverity)
  - [ ] `Visibility` (PUBLIC, RESIDENT_ONLY)
  - [ ] `LocationLat`, `LocationLng`
  - [ ] `Description` (text)
  - [ ] `Status` (HealthObservationStatus)
  - [ ] `ObservedAt`, `CreatedAtUtc`, `UpdatedAtUtc`
- [ ] Criar modelo `HealthObservationConfirmation`:
  - [ ] `Id`, `ObservationId`, `UserId`
  - [ ] `Action` (CONFIRM, DISCONFIRM, ADD_CONTEXT, REPORT)
  - [ ] `Note`, `CreatedAtUtc`
- [ ] Criar repositórios
- [ ] Criar migrations

**Arquivos a Criar**:
- `backend/Araponga.Domain/Health/HealthDomain.cs`
- `backend/Araponga.Domain/Health/HealthSeverity.cs`
- `backend/Araponga.Domain/Health/HealthObservationStatus.cs`
- `backend/Araponga.Domain/Health/HealthMetric.cs`
- `backend/Araponga.Domain/Health/HealthObservation.cs`
- `backend/Araponga.Domain/Health/HealthObservationConfirmation.cs`
- `backend/Araponga.Application/Interfaces/IHealthDomainRepository.cs`
- `backend/Araponga.Application/Interfaces/IHealthMetricRepository.cs`
- `backend/Araponga.Application/Interfaces/IHealthObservationRepository.cs`
- `backend/Araponga.Application/Interfaces/IHealthObservationConfirmationRepository.cs`
- `backend/Araponga.Infrastructure/Postgres/PostgresHealthDomainRepository.cs`
- `backend/Araponga.Infrastructure/Postgres/PostgresHealthMetricRepository.cs`
- `backend/Araponga.Infrastructure/Postgres/PostgresHealthObservationRepository.cs`
- `backend/Araponga.Infrastructure/Postgres/PostgresHealthObservationConfirmationRepository.cs`

**Critérios de Sucesso**:
- ✅ Modelos criados
- ✅ Repositórios implementados
- ✅ Migrations criadas
- ✅ Testes de repositório passando

---

#### 18.2 Sistema de Observações de Saúde
**Estimativa**: 32 horas (4 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `HealthObservationService`:
  - [ ] `CreateObservationAsync(Guid territoryId, Guid? userId, HealthDomain domain, ...)` → criar observação
  - [ ] `ListObservationsAsync(Guid territoryId, HealthDomain? domain, ...)` → listar observações
  - [ ] `GetObservationAsync(Guid observationId)` → obter observação
  - [ ] `ConfirmObservationAsync(Guid observationId, Guid userId, ...)` → confirmar observação
  - [ ] `UpdateStatusAsync(Guid observationId, HealthObservationStatus status)` → atualizar status
- [ ] Integrar com `ContributionService` (Fase 17):
  - [ ] Ao criar observação: registrar contribuição `HealthObservation`
  - [ ] Ao confirmar observação: registrar contribuição (pontos menores)
- [ ] Criar `HealthObservationController`:
  - [ ] `POST /api/v1/health/observations` → criar observação
  - [ ] `GET /api/v1/health/observations` → listar observações
  - [ ] `GET /api/v1/health/observations/{id}` → obter observação
  - [ ] `POST /api/v1/health/observations/{id}/confirm` → confirmar observação
  - [ ] `PATCH /api/v1/health/observations/{id}/status` → atualizar status (curadores)
- [ ] Feature flags: `HealthObservationsEnabled`, `HealthObservationsPublic`
- [ ] Validações
- [ ] Testes

**Arquivos a Criar**:
- `backend/Araponga.Application/Services/HealthObservationService.cs`
- `backend/Araponga.Api/Controllers/HealthObservationController.cs`
- `backend/Araponga.Api/Contracts/Health/CreateHealthObservationRequest.cs`
- `backend/Araponga.Api/Contracts/Health/HealthObservationResponse.cs`
- `backend/Araponga.Api/Validators/CreateHealthObservationRequestValidator.cs`

**Critérios de Sucesso**:
- ✅ Serviço implementado
- ✅ API funcionando
- ✅ Integração com gamificação funcionando
- ✅ Testes passando

---

### Semana 2-3: Sensores e Indicadores

#### 18.3 Sistema de Sensores
**Estimativa**: 32 horas (4 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar enum `SensorDeviceType`:
  - [ ] `RAIN_GAUGE` (pluviômetro)
  - [ ] `WATER_LEVEL` (nível de água)
  - [ ] `AIR_QUALITY` (qualidade do ar)
  - [ ] `WATER_QUALITY` (qualidade da água)
  - [ ] `WEATHER` (clima)
- [ ] Criar enum `SensorDeviceStatus`:
  - [ ] `ACTIVE` (ativo)
  - [ ] `MAINTENANCE` (manutenção)
  - [ ] `RETIRED` (desativado)
- [ ] Criar modelo `SensorDevice`:
  - [ ] `Id`, `TerritoryId`, `Name`
  - [ ] `DeviceType` (SensorDeviceType)
  - [ ] `Status` (SensorDeviceStatus)
  - [ ] `LocationLat`, `LocationLng`
  - [ ] `ExternalRef?` (nullable, referência externa)
  - [ ] `InstalledAt`, `CreatedAtUtc`
- [ ] Criar modelo `SensorReading`:
  - [ ] `Id`, `DeviceId`, `MetricId`
  - [ ] `ValueDecimal?`, `ValueInt?`, `ValueBool?`, `ValueText?`
  - [ ] `MeasuredAt`, `CreatedAtUtc`
- [ ] Criar `SensorDeviceService`:
  - [ ] `RegisterDeviceAsync(...)` → registrar sensor
  - [ ] `RecordReadingAsync(...)` → registrar leitura
  - [ ] `ListDevicesAsync(Guid territoryId)` → listar sensores
  - [ ] `ListReadingsAsync(Guid deviceId, ...)` → listar leituras
- [ ] Integrar com `ContributionService` (Fase 17):
  - [ ] Leitura confirmada gera contribuição `SensorReading`
- [ ] Criar `SensorDeviceController`:
  - [ ] `POST /api/v1/sensors/devices` → registrar sensor
  - [ ] `GET /api/v1/sensors/devices` → listar sensores
  - [ ] `POST /api/v1/sensors/devices/{id}/readings` → registrar leitura
  - [ ] `GET /api/v1/sensors/devices/{id}/readings` → listar leituras
- [ ] Feature flags: `SensorsEnabled`, `SensorReadingsPublic`
- [ ] Testes

**Arquivos a Criar**:
- `backend/Araponga.Domain/Health/SensorDevice.cs`
- `backend/Araponga.Domain/Health/SensorDeviceType.cs`
- `backend/Araponga.Domain/Health/SensorDeviceStatus.cs`
- `backend/Araponga.Domain/Health/SensorReading.cs`
- `backend/Araponga.Application/Interfaces/ISensorDeviceRepository.cs`
- `backend/Araponga.Application/Interfaces/ISensorReadingRepository.cs`
- `backend/Araponga.Application/Services/SensorDeviceService.cs`
- `backend/Araponga.Api/Controllers/SensorDeviceController.cs`

**Critérios de Sucesso**:
- ✅ Sistema de sensores funcionando
- ✅ Leituras sendo registradas
- ✅ Integração com gamificação funcionando
- ✅ Testes passando

---

#### 18.4 Sistema de Indicadores de Saúde
**Estimativa**: 24 horas (3 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar enum `IndicatorPeriod`:
  - [ ] `DAILY` (diário)
  - [ ] `WEEKLY` (semanal)
  - [ ] `MONTHLY` (mensal)
- [ ] Criar enum `CalculationMethod`:
  - [ ] `AVG` (média)
  - [ ] `MAX` (máximo)
  - [ ] `INDEX_FORMULA` (fórmula de índice)
- [ ] Criar modelo `TerritoryHealthIndicator`:
  - [ ] `Id`, `TerritoryId`, `MetricId`
  - [ ] `Period` (IndicatorPeriod)
  - [ ] `PeriodStart`, `PeriodEnd`
  - [ ] `ValueDecimal`
  - [ ] `CalculationMethod` (CalculationMethod)
  - [ ] `CreatedAtUtc`
- [ ] Criar `HealthIndicatorService`:
  - [ ] `CalculateIndicatorsAsync(Guid territoryId, IndicatorPeriod period)` → calcular indicadores
  - [ ] `GetIndicatorsAsync(Guid territoryId, ...)` → obter indicadores
  - [ ] `GetIndicatorTrendAsync(Guid territoryId, Guid metricId, ...)` → obter tendência
- [ ] Background job para calcular indicadores periodicamente
- [ ] Sistema de alertas quando indicadores pioram
- [ ] Criar `HealthIndicatorController`:
  - [ ] `GET /api/v1/health/indicators` → listar indicadores
  - [ ] `GET /api/v1/health/indicators/{metricId}/trend` → obter tendência
- [ ] Feature flags: `HealthIndicatorsEnabled`, `HealthIndicatorsPublic`
- [ ] Testes

**Arquivos a Criar**:
- `backend/Araponga.Domain/Health/TerritoryHealthIndicator.cs`
- `backend/Araponga.Domain/Health/IndicatorPeriod.cs`
- `backend/Araponga.Domain/Health/CalculationMethod.cs`
- `backend/Araponga.Application/Interfaces/ITerritoryHealthIndicatorRepository.cs`
- `backend/Araponga.Application/Services/HealthIndicatorService.cs`
- `backend/Araponga.Api/Controllers/HealthIndicatorController.cs`

**Critérios de Sucesso**:
- ✅ Indicadores sendo calculados
- ✅ Tendências disponíveis
- ✅ Alertas funcionando
- ✅ Testes passando

---

### Semana 3-4: Ações Territoriais

#### 18.5 Sistema de Ações Territoriais
**Estimativa**: 40 horas (5 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar enum `TerritoryActionType`:
  - [ ] `MUTIRAO` (mutirão)
  - [ ] `MAINTENANCE` (manutenção)
  - [ ] `EDUCATION` (educação)
  - [ ] `RESTORATION` (restauração)
  - [ ] `MONITORING` (monitoramento)
- [ ] Criar enum `TerritoryActionStatus`:
  - [ ] `PLANNED` (planejado)
  - [ ] `IN_PROGRESS` (em progresso)
  - [ ] `DONE` (concluído)
  - [ ] `CANCELLED` (cancelado)
- [ ] Criar modelo `TerritoryAction`:
  - [ ] `Id`, `TerritoryId`, `RelatedObservationId?` (nullable)
  - [ ] `OrganizerUserId` (organizador)
  - [ ] `Type` (TerritoryActionType)
  - [ ] `Title`, `Description` (text)
  - [ ] `StartDateTime`, `EndDateTime`
  - [ ] `Visibility` (PUBLIC, RESIDENT_ONLY)
  - [ ] `Status` (TerritoryActionStatus)
  - [ ] `CreatedAtUtc`, `UpdatedAtUtc`
- [ ] Criar modelo `TerritoryActionParticipant`:
  - [ ] `Id`, `ActionId`, `UserId`
  - [ ] `JoinedAtUtc`, `ConfirmedAtUtc?` (nullable)
- [ ] Criar `TerritoryActionService`:
  - [ ] `CreateActionAsync(...)` → criar ação
  - [ ] `ListActionsAsync(Guid territoryId, ...)` → listar ações
  - [ ] `JoinActionAsync(Guid actionId, Guid userId)` → participar
  - [ ] `ConfirmParticipationAsync(Guid actionId, Guid userId)` → confirmar participação
  - [ ] `UpdateStatusAsync(Guid actionId, TerritoryActionStatus status)` → atualizar status
- [ ] Integrar com `ContributionService` (Fase 17):
  - [ ] Participação gera contribuição `TerritoryAction`
  - [ ] Organizar ação gera mais pontos
- [ ] Criar `TerritoryActionController`:
  - [ ] `POST /api/v1/territory-actions` → criar ação
  - [ ] `GET /api/v1/territory-actions` → listar ações
  - [ ] `POST /api/v1/territory-actions/{id}/join` → participar
  - [ ] `POST /api/v1/territory-actions/{id}/confirm` → confirmar participação
  - [ ] `PATCH /api/v1/territory-actions/{id}/status` → atualizar status
- [ ] Feature flags: `TerritoryActionsEnabled`, `TerritoryActionsPublic`
- [ ] Testes

**Arquivos a Criar**:
- `backend/Araponga.Domain/Health/TerritoryAction.cs`
- `backend/Araponga.Domain/Health/TerritoryActionType.cs`
- `backend/Araponga.Domain/Health/TerritoryActionStatus.cs`
- `backend/Araponga.Domain/Health/TerritoryActionParticipant.cs`
- `backend/Araponga.Application/Interfaces/ITerritoryActionRepository.cs`
- `backend/Araponga.Application/Interfaces/ITerritoryActionParticipantRepository.cs`
- `backend/Araponga.Application/Services/TerritoryActionService.cs`
- `backend/Araponga.Api/Controllers/TerritoryActionController.cs`

**Critérios de Sucesso**:
- ✅ Sistema de ações funcionando
- ✅ Participação funcionando
- ✅ Integração com gamificação funcionando
- ✅ Testes passando

---

### Semana 4-5: Atividades Específicas e Integração

#### 18.6 Sistema de Coleta de Resíduos
**Estimativa**: 16 horas (2 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar modelo `WasteCollection`:
  - [ ] `Id`, `TerritoryId`, `UserId`
  - [ ] `WasteType` (string: ORGANIC, RECYCLABLE, HAZARDOUS, etc.)
  - [ ] `Volume` (decimal, em kg ou litros)
  - [ ] `LocationLat`, `LocationLng`
  - [ ] `Description?` (nullable)
  - [ ] `CollectedAt`, `CreatedAtUtc`
- [ ] Criar `WasteCollectionService`:
  - [ ] `ReportCollectionAsync(...)` → reportar coleta
  - [ ] `ListCollectionsAsync(Guid territoryId, ...)` → listar coletas
- [ ] Integrar com `ContributionService` (Fase 17):
  - [ ] Coleta gera contribuição `WasteCollection` (10-20 pontos)
- [ ] Criar `WasteCollectionController`:
  - [ ] `POST /api/v1/waste-collections` → reportar coleta
  - [ ] `GET /api/v1/waste-collections` → listar coletas
- [ ] Feature flags: `WasteCollectionEnabled`
- [ ] Testes

**Arquivos a Criar**:
- `backend/Araponga.Domain/Health/WasteCollection.cs`
- `backend/Araponga.Application/Interfaces/IWasteCollectionRepository.cs`
- `backend/Araponga.Application/Services/WasteCollectionService.cs`
- `backend/Araponga.Api/Controllers/WasteCollectionController.cs`

---

#### 18.7 Sistema de Plantio
**Estimativa**: 16 horas (2 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar modelo `TreePlanting`:
  - [ ] `Id`, `TerritoryId`, `UserId`
  - [ ] `Species` (string, espécie)
  - [ ] `Quantity` (int, quantidade)
  - [ ] `LocationLat`, `LocationLng`
  - [ ] `Description?` (nullable)
  - [ ] `PlantedAt`, `CreatedAtUtc`
- [ ] Criar `TreePlantingService`:
  - [ ] `ReportPlantingAsync(...)` → reportar plantio
  - [ ] `ListPlantingsAsync(Guid territoryId, ...)` → listar plantios
- [ ] Integrar com `ContributionService` (Fase 17):
  - [ ] Plantio gera contribuição `TreePlanting` (15-25 pontos)
- [ ] Criar `TreePlantingController`:
  - [ ] `POST /api/v1/tree-plantings` → reportar plantio
  - [ ] `GET /api/v1/tree-plantings` → listar plantios
- [ ] Feature flags: `TreePlantingEnabled`
- [ ] Testes

**Arquivos a Criar**:
- `backend/Araponga.Domain/Health/TreePlanting.cs`
- `backend/Araponga.Application/Interfaces/ITreePlantingRepository.cs`
- `backend/Araponga.Application/Services/TreePlantingService.cs`
- `backend/Araponga.Api/Controllers/TreePlantingController.cs`

---

#### 18.8 Integração com Gamificação e Moeda
**Estimativa**: 16 horas (2 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Integrar todos os serviços com `ContributionService` (Fase 17):
  - [ ] `HealthObservationService` → contribuição ao criar/confirmar
  - [ ] `SensorDeviceService` → contribuição ao confirmar leitura
  - [ ] `TerritoryActionService` → contribuição ao participar/organizar
  - [ ] `WasteCollectionService` → contribuição ao reportar coleta
  - [ ] `TreePlantingService` → contribuição ao reportar plantio
- [ ] Preparar integração com `CurrencyMintService` (Fase 20):
  - [ ] Estrutura para mint por atividades (será implementado na Fase 20)
- [ ] Testes de integração
- [ ] Documentação

**Critérios de Sucesso**:
- ✅ Todas as atividades geram contribuições
- ✅ Pontos sendo calculados corretamente
- ✅ Integração preparada para moeda territorial
- ✅ Testes passando

---

## 📊 Resumo da Fase 18

| Tarefa | Estimativa | Status | Prioridade |
|--------|------------|--------|------------|
| Modelo de Domínio - Saúde | 24h | ❌ Pendente | 🔴 Alta |
| Observações de Saúde | 32h | ❌ Pendente | 🔴 Alta |
| Sistema de Sensores | 32h | ❌ Pendente | 🔴 Alta |
| Indicadores de Saúde | 24h | ❌ Pendente | 🔴 Alta |
| Ações Territoriais | 40h | ❌ Pendente | 🔴 Alta |
| Coleta de Resíduos | 16h | ❌ Pendente | 🟡 Média |
| Plantio | 16h | ❌ Pendente | 🟡 Média |
| Integração Gamificação/Moeda | 16h | ❌ Pendente | 🔴 Alta |
| **Total** | **200h (35 dias)** | | |

---

## ✅ Critérios de Sucesso da Fase 18

### Funcionalidades
- ✅ Sistema completo de observações de saúde funcionando
- ✅ Sistema de sensores funcionando
- ✅ Indicadores de saúde sendo calculados
- ✅ Ações territoriais funcionando
- ✅ Coleta de resíduos e plantio funcionando
- ✅ Integração com gamificação funcionando
- ✅ Integração preparada para moeda territorial

### Qualidade
- ✅ Testes com cobertura adequada
- ✅ Documentação completa
- ✅ Feature flags implementados
- ✅ Validações e segurança implementadas

### Integração
- ✅ Integração com Fase 17 (Gamificação) funcionando
- ✅ Preparação para Fase 20 (Moeda Territorial)
- ✅ Integração com recursos naturais (MER)

---

## 🔗 Dependências

- **Fase 9**: Perfil de Usuário (para exibir contribuições)
- **Fase 17**: Gamificação (para gerar contribuições e pontos)
- **Fase 20**: Moeda Territorial (para gerar moeda por atividades)

---

## 📝 Notas de Implementação

### Gamificação de Atividades

**Pontos por Atividade**:
- Observação de saúde: 5-15 pontos (depende da severidade)
- Confirmação de observação: 2-5 pontos
- Leitura de sensor confirmada: 3-10 pontos
- Participação em ação territorial: 20-30 pontos
- Organizar ação territorial: +10 pontos
- Coleta de resíduos: 10-20 pontos (depende do volume/tipo)
- Plantio de árvore: 15-25 pontos (depende do tipo/espécie)

**Multiplicadores**:
- Alinhamento com interesses do território: +50%
- Qualidade alta (IA): +25%
- Combinado: até 1.875x

### Integração com Moeda Territorial (Fase 20)

**Preparação**:
- Estrutura de dados para mint por atividades
- Políticas de mint configuráveis por território
- Integração será implementada na Fase 20

### Privacidade e Visibilidade

- Observações podem ser PUBLIC ou RESIDENT_ONLY
- Sensores podem ter leituras públicas ou privadas
- Ações territoriais podem ser públicas ou apenas para moradores
- Respeitar preferências de privacidade do usuário

---

**Status**: ⏳ **FASE 18 PENDENTE**  
**Depende de**: Fases 9, 17 (Perfil, Gamificação)  
**Crítico para**: Soberania Territorial e Autonomia Comunitária
