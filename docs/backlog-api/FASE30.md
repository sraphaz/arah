# Fase 18: Sistema de Hospedagem Territorial

**Duração**: 8 semanas (56 dias úteis)  
**Prioridade**: 🔴 CRÍTICA (Economia local e diferencial competitivo)  
**Depende de**: Fase 6-7 (Marketplace/Pagamentos) - ✅ Já implementado  
**Integra com**: Fase 14 (Governança/Votação) - opcional para aprovação comunitária  
**Estimativa Total**: 360 horas  
**Status**: ⏳ Planejado  
**Nota**: Renumerada de Fase 30 para Fase 18, priorizada de P1 para P0 (Onda 3: Economia Local)  
**Referência**: [Proposta de Implementação](../PROPOSTA_IMPLEMENTACAO_HOSPEDAGEM.md) | [Análise de Inserção](../ANALISE_INSERCAO_HOSPEDAGEM_ROADMAP.md)

**Nota sobre Numeração**: Esta é a primeira fase nova com documento completo após as 29 fases existentes (FASE1.md até FASE29.md). As menções às fases 31-44 no roadmap estratégico são conceituais (sem documentos detalhados) e seguem a numeração sequencial correta.

---

## 🎯 Objetivo

Implementar sistema de **hospedagem territorial** que permite:
- Moradores validados cadastrarem propriedades privadas
- Configurar múltiplas formas de hospedagem por propriedade (casa inteira, quarto, cama compartilhada)
- Gerenciar agenda de disponibilidade (núcleo do sistema)
- Visitantes solicitarem estadias com aprovação humana (manual ou condicional)
- Sistema de pagamentos com escrow e split (Owner, Limpeza, Plataforma)
- Check-in/Check-out com liberação de pagamentos

**Princípios**:
- ✅ **Privacidade por Padrão**: Propriedades privadas até terem hospedagem ativa
- ✅ **Agenda como Núcleo**: Toda lógica gira em torno da agenda
- ✅ **Aprovação Humana**: Sempre requer consentimento (com auto-aprovação condicional)
- ✅ **Papéis Contextuais**: Host e Limpeza são específicos por configuração
- ✅ **Economia Local**: Fortalece circulação de recursos no território
- ✅ **Soberania Territorial**: Moradores validados, regulação territorial

**Diferenciais do Araponga**:
- Território-first (não global como Airbnb)
- Morador validado como pré-requisito
- Aprovação humana sempre presente
- Privacidade por padrão

---

## 📋 Contexto e Requisitos

### Estado Atual
- ✅ Sistema de pagamentos completo (Fase 6-7)
  - FinancialTransaction, escrow, split
  - Payout para vendedores
  - Platform fees configuráveis
- ✅ Sistema de aprovação humana (WorkItem)
  - Fila genérica de revisão
  - Suporta diferentes tipos
- ✅ Sistema de notificações completo
  - OutboxMessage e UserNotification
- ✅ Feature flags por território
- ✅ Membership e validação robusta
  - ResidencyVerification (Flags)
  - MembershipCapability
- ❌ Não existe sistema de hospedagem
- ❌ Não existe sistema de agenda de propriedades
- ❌ Não existe sistema de solicitações de estadia

### Requisitos Funcionais

#### 1. Property (Propriedade)
- ✅ Morador Validado pode criar propriedade (privada)
- ✅ Propriedade é privada por padrão
- ✅ Visível apenas para Owner até ter hospedagem ativa
- ✅ Pode existir indefinidamente sem hospedagem
- ✅ Dados: nome, descrição, localização, endereço

#### 2. HostingConfiguration (Configuração de Hospedagem)
- ✅ Uma Property pode ter múltiplas HostingConfigurations
- ✅ Tipo de acomodação: Casa Inteira, Quarto Privado, Cama Compartilhada
- ✅ Capacidade máxima
- ✅ Regras da casa, check-in/check-out, política de cancelamento
- ✅ Modalidades: Diária, Semanal, Mensal, Anual, Pacotes
- ✅ Política de aprovação: Manual ou Auto-aprovação Condicional
- ✅ Status: Active, Inactive
- ✅ Visibilidade pública: Ativa + ao menos uma data disponível

#### 3. HostingCalendar (Agenda - NÚCLEO)
- ✅ Agenda exclusiva por HostingConfiguration
- ✅ Estados por data: Available, BlockedByResident, PendingApproval, Reserved
- ✅ Agenda inicia totalmente bloqueada
- ✅ Host/Owner deve abrir datas explicitamente
- ✅ Padrões recorrentes (bloqueios, aberturas)
- ✅ Regras: antecedência mínima, janela máxima de abertura
- ✅ Datas reservadas não podem ser sobrescritas

#### 4. HostingRole (Papéis Contextuais)
- ✅ Owner: Dono da propriedade (sempre o criador)
- ✅ Host: Responsável por aprovar/rejeitar (pode ser delegado)
- ✅ Cleaning: Responsável pela limpeza
- ✅ Papéis são contextuais (por HostingConfiguration)
- ✅ Um morador pode acumular múltiplos papéis
- ✅ Host e Cleaning devem ser Moradores Validados do mesmo território

#### 5. StayRequest / Stay (Solicitação de Estadia)
- ✅ Visitante cria StayRequest (não precisa ser morador)
- ✅ Estado inicial: PendingApproval
- ✅ Bloqueia datas na agenda (PendingApproval)
- ✅ Calcula valor total
- ✅ Auto-aprovação condicional (se critérios atendidos)
- ✅ Aprovação manual via WorkItem
- ✅ Quando aprovada: cria Stay, cria FinancialTransaction (escrow), atualiza agenda (Reserved)

#### 6. Check-in / Check-out e Pagamento
- ✅ Check-in: marca estadia, notifica Host e Limpeza, libera primeira parcela (se configurado)
- ✅ Check-out: marca saída, notifica Limpeza, libera pagamento completo (split: Owner, Limpeza, Plataforma)
- ✅ Cancelamento: reembolso conforme política, libera datas na agenda

---

## 📋 Tarefas Detalhadas

### Semana 1-2: Fundação - Modelo de Domínio

#### 30.1 Modelo de Domínio - Property e HostingConfiguration
**Estimativa**: 24 horas (3 dias)  
**Status**: ⏳ Planejado

**Tarefas**:
- [ ] Criar enum `PropertyVisibility`:
  - [ ] `Private` (privada, apenas Owner)
  - [ ] `Public` (pública, quando tem hospedagem ativa)
- [ ] Criar enum `PropertyStatus`:
  - [ ] `Active` (ativa)
  - [ ] `Inactive` (inativa)
- [ ] Criar enum `AccommodationType`:
  - [ ] `EntirePlace` (casa inteira)
  - [ ] `PrivateRoom` (quarto privado)
  - [ ] `SharedRoom` (cama em quarto compartilhado)
- [ ] Criar enum `RentalModality`:
  - [ ] `Daily` (diária)
  - [ ] `Weekly` (semanal)
  - [ ] `Monthly` (mensal)
  - [ ] `Annual` (anual)
  - [ ] `Packages` (pacotes)
- [ ] Criar enum `ApprovalPolicy`:
  - [ ] `ManualOnly` (apenas manual)
  - [ ] `ConditionalAutoApprove` (auto-aprovação condicional)
- [ ] Criar enum `HostingConfigurationStatus`:
  - [ ] `Active` (ativa)
  - [ ] `Inactive` (inativa)
- [ ] Criar modelo `Property`:
  - [ ] `Id`, `OwnerUserId`, `TerritoryId`
  - [ ] `Visibility` (PropertyVisibility, Private por padrão)
  - [ ] `Name`, `Description?`, `Latitude?`, `Longitude?`, `Address?`
  - [ ] `Status` (PropertyStatus)
  - [ ] `CreatedAtUtc`, `UpdatedAtUtc`
  - [ ] Métodos: `UpdateVisibility()`, `IsVisibleToPublic()`
- [ ] Criar modelo `HostingConfiguration`:
  - [ ] `Id`, `PropertyId`, `TerritoryId`
  - [ ] `Type` (AccommodationType)
  - [ ] `MaxCapacity` (int)
  - [ ] `HouseRules?`, `CheckInTime?`, `CheckOutTime?`
  - [ ] `CancellationPolicy` (enum)
  - [ ] `Modality` (RentalModality)
  - [ ] `ApprovalPolicy` (ApprovalPolicy)
  - [ ] `AutoApproveCriteria?` (JSON com critérios)
  - [ ] `Status` (HostingConfigurationStatus)
  - [ ] `CreatedAtUtc`, `UpdatedAtUtc`
  - [ ] Métodos: `IsPubliclyVisible()`, `Activate()`, `Deactivate()`

#### 30.2 Modelo de Domínio - HostingCalendar (Núcleo)
**Estimativa**: 32 horas (4 dias)  
**Status**: ⏳ Planejado

**Tarefas**:
- [ ] Criar enum `CalendarDateState`:
  - [ ] `Available` (disponível)
  - [ ] `BlockedByResident` (bloqueada pelo morador)
  - [ ] `PendingApproval` (solicitação pendente)
  - [ ] `Reserved` (reservada)
- [ ] Criar enum `BlockReason`:
  - [ ] `Manual` (bloqueio manual)
  - [ ] `Recurring` (padrão recorrente)
  - [ ] `Maintenance` (manutenção)
- [ ] Criar modelo `HostingCalendar`:
  - [ ] `Id`, `HostingConfigurationId`
  - [ ] `Dates` (Dictionary<DateOnly, CalendarDateState>)
  - [ ] `Patterns` (List<CalendarPattern>)
  - [ ] `Rules` (CalendarRules: antecedência mínima, janela máxima)
  - [ ] Métodos: `OpenDate()`, `BlockDate()`, `ReserveDate()`, `ReleaseDate()`
  - [ ] Métodos: `IsDateAvailable()`, `GetAvailableDates()`
  - [ ] **Regra**: Agenda inicia totalmente bloqueada
- [ ] Criar modelo `CalendarPattern`:
  - [ ] `Id`, `CalendarId`
  - [ ] `Type` (RecurringBlock, RecurringOpen)
  - [ ] `DayOfWeek?`, `DayOfMonth?`, `StartDate`, `EndDate?`
  - [ ] `Reason?` (string)
- [ ] Criar modelo `CalendarRules`:
  - [ ] `MinAdvanceDays` (int, antecedência mínima)
  - [ ] `MaxAdvanceDays` (int, janela máxima)
  - [ ] `DefaultBlockReason` (BlockReason)

#### 30.3 Modelo de Domínio - HostingRole e Gestão pela Plataforma
**Estimativa**: 32 horas (4 dias) - **Aumentado para incluir gestão pela plataforma**  
**Status**: ⏳ Planejado

**Tarefas**:
- [ ] Criar enum `HostingRoleType`:
  - [ ] `Owner` (dono)
  - [ ] `Host` (responsável por aprovar)
  - [ ] `Cleaning` (responsável pela limpeza)
- [ ] Criar modelo `HostingRole`:
  - [ ] `Id`, `HostingConfigurationId`, `MembershipId`
  - [ ] `Type` (HostingRoleType)
  - [ ] `GrantedAtUtc`, `GrantedByUserId`
  - [ ] `RevokedAtUtc?`, `RevokedByUserId?`
  - [ ] Métodos: `Revoke()`, `IsActive()`
  - [ ] **Regra**: Owner é sempre o criador da Property
  - [ ] **Regra**: Host e Cleaning devem ser Moradores Validados do mesmo território
- [ ] Criar modelo `HostInvitation`:
  - [ ] `Id`, `HostingConfigurationId`, `InvitedMembershipId`, `InvitedByUserId`
  - [ ] `Status` (HostInvitationStatus: Pending, Accepted, Rejected, Expired)
  - [ ] `ExpiresAtUtc` (7 dias após criação)
  - [ ] Métodos: `Accept()`, `Reject()`, `IsExpired()`
  - [ ] **Regra**: Ao aceitar, cria `HostingRole` automaticamente
- [ ] Criar modelo `HostOffer`:
  - [ ] `Id`, `MembershipId`, `TerritoryId`
  - [ ] `AvailableFrom`, `AvailableUntil?`, `AvailableDaysOfWeek`
  - [ ] `SupportedTypes` (AccommodationType), `MaxPropertiesManaged?`
  - [ ] `Status` (HostOfferStatus: Active, Inactive, Paused)
  - [ ] `IsPubliclyVisible` (visível para moradores)
  - [ ] Métodos: `Activate()`, `Deactivate()`, `Pause()`, `IsAvailableFor()`
- [ ] Criar modelo `CleaningOffer`:
  - [ ] `Id`, `MembershipId`, `TerritoryId`
  - [ ] `AvailableDaysOfWeek`, `PreferredStartTime?`, `PreferredEndTime?`
  - [ ] `SupportedTypes`, `MaxPropertiesPerDay?`
  - [ ] `Status` (CleaningOfferStatus: Active, Inactive, Paused)
  - [ ] `IsPubliclyVisible` (visível para moradores)
  - [ ] Métodos: `Activate()`, `Deactivate()`, `Pause()`, `IsAvailableFor()`
- [ ] Criar modelo `CleaningServiceRequest`:
  - [ ] `Id`, `StayId`, `HostingConfigurationId`, `PropertyId`, `TerritoryId`
  - [ ] `ServiceDate` (data do checkout), `CheckoutTime`, `PreferredServiceTime?`
  - [ ] `Status` (CleaningServiceRequestStatus: Open, Assigned, InProgress, Completed, Cancelled)
  - [ ] `AssignedCleaningMembershipId?`, `EstimatedAmount?`, `FinalAmount?`
  - [ ] Métodos: `AssignTo()`, `MarkInProgress()`, `MarkCompleted()`, `Cancel()`
  - [ ] **Regra**: Criada automaticamente quando Stay é confirmada
- [ ] Criar modelo `CleaningServiceApplication`:
  - [ ] `Id`, `CleaningServiceRequestId`, `ApplicantMembershipId`, `TerritoryId`
  - [ ] `ProposedAmount?`, `Message?`
  - [ ] `Status` (CleaningApplicationStatus: Pending, Accepted, Rejected, Withdrawn)
  - [ ] Métodos: `Accept()`, `Reject()`, `Withdraw()`
  - [ ] **Regra**: Ao aceitar, atualiza CleaningServiceRequest para Assigned

#### 30.4 Feature Flag e Validações
**Estimativa**: 8 horas (1 dia)  
**Status**: ⏳ Planejado

**Tarefas**:
- [ ] Adicionar `HostingEnabled` ao enum `FeatureFlag`
- [ ] Adicionar `EnsureHostingEnabled()` ao `TerritoryFeatureFlagGuard`
- [ ] Criar `HostingAccessRules` helper:
  - [ ] `CanCreateProperty()` - valida Morador Validado
  - [ ] `CanCreateHostingConfiguration()` - valida Owner
  - [ ] `CanManageCalendar()` - valida Owner ou Host
  - [ ] `CanApproveStayRequest()` - valida Host

**Total Semana 1-2**: 80 horas (10 dias)

---

### Semana 3-4: Repositórios e Infraestrutura

#### 30.5 Repositórios de Domínio
**Estimativa**: 48 horas (6 dias) - **Aumentado para incluir novos repositórios**  
**Status**: ⏳ Planejado

**Tarefas**:
- [ ] Criar interface `IPropertyRepository`:
  - [ ] `GetByIdAsync()`, `GetByOwnerAsync()`, `GetPublicPropertiesAsync()`
  - [ ] `AddAsync()`, `UpdateAsync()`
- [ ] Criar interface `IHostingConfigurationRepository`:
  - [ ] `GetByIdAsync()`, `GetByPropertyAsync()`, `GetPublicConfigurationsAsync()`
  - [ ] `AddAsync()`, `UpdateAsync()`
- [ ] Criar interface `IHostingCalendarRepository`:
  - [ ] `GetByConfigurationAsync()`, `GetAvailableDatesAsync()`
  - [ ] `UpdateDateStateAsync()`, `UpdateDatesAsync()`
- [ ] Criar interface `IHostingRoleRepository`:
  - [ ] `GetByConfigurationAsync()`, `GetByMembershipAsync()`
  - [ ] `AddAsync()`, `UpdateAsync()`
- [ ] Criar interface `IHostInvitationRepository`:
  - [ ] `GetByIdAsync()`, `GetByConfigurationAsync()`, `GetByInvitedMembershipAsync()`
  - [ ] `GetPendingInvitationsAsync()`, `GetExpiredInvitationsAsync()`
  - [ ] `AddAsync()`, `UpdateAsync()`
- [ ] Criar interface `IHostOfferRepository`:
  - [ ] `GetByIdAsync()`, `GetByMembershipAsync()`, `GetPublicOffersAsync()`
  - [ ] `SearchAvailableOffersAsync()` (por território, datas, tipos)
  - [ ] `AddAsync()`, `UpdateAsync()`
- [ ] Criar interface `ICleaningOfferRepository`:
  - [ ] `GetByIdAsync()`, `GetByMembershipAsync()`, `GetPublicOffersAsync()`
  - [ ] `SearchAvailableOffersAsync()` (por território, data, horário)
  - [ ] `AddAsync()`, `UpdateAsync()`
- [ ] Criar interface `ICleaningServiceRequestRepository`:
  - [ ] `GetByIdAsync()`, `GetByStayAsync()`, `GetOpenRequestsAsync()`
  - [ ] `GetByAssignedCleaningAsync()`, `GetByPropertyAsync()`
  - [ ] `AddAsync()`, `UpdateAsync()`
- [ ] Criar interface `ICleaningServiceApplicationRepository`:
  - [ ] `GetByIdAsync()`, `GetByRequestAsync()`, `GetByApplicantAsync()`
  - [ ] `GetPendingApplicationsAsync()`
  - [ ] `AddAsync()`, `UpdateAsync()`
- [ ] Implementar repositórios InMemory
- [ ] Implementar repositórios Postgres (com migrations)

#### 30.6 Migrations e Schema
**Estimativa**: 24 horas (3 dias) - **Aumentado para incluir novas tabelas**  
**Status**: ⏳ Planejado

**Tarefas**:
- [ ] Criar migration `AddHostingModule`:
  - [ ] Tabela `properties`
  - [ ] Tabela `hosting_configurations`
  - [ ] Tabela `hosting_calendars`
  - [ ] Tabela `hosting_calendar_dates` (para performance)
  - [ ] Tabela `hosting_calendar_patterns`
  - [ ] Tabela `hosting_roles`
  - [ ] Tabela `host_invitations`
  - [ ] Tabela `host_offers`
  - [ ] Tabela `cleaning_offers`
  - [ ] Tabela `cleaning_service_requests`
  - [ ] Tabela `cleaning_service_applications`
  - [ ] Índices:
    - [ ] `properties(owner_user_id, territory_id)`
    - [ ] `hosting_configurations(property_id)`
    - [ ] `hosting_calendar_dates(configuration_id, date)`
    - [ ] `host_offers(membership_id, territory_id, status)`
    - [ ] `cleaning_offers(membership_id, territory_id, status)`
    - [ ] `cleaning_service_requests(stay_id, status, service_date)`
    - [ ] `cleaning_service_applications(request_id, applicant_membership_id, status)`

**Total Semana 3-4**: 48 horas (6 dias)

---

### Semana 5-6: StayRequest e Stay

#### 30.7 Modelo de Domínio - StayRequest e Stay
**Estimativa**: 24 horas (3 dias)  
**Status**: ⏳ Planejado

**Tarefas**:
- [ ] Criar enum `StayRequestStatus`:
  - [ ] `PendingApproval`, `AutoApproved`, `Approved`, `Rejected`, `Cancelled`, `Completed`
- [ ] Criar enum `ApprovalSource`:
  - [ ] `Manual`, `AutoApproved`
- [ ] Criar enum `StayStatus`:
  - [ ] `Confirmed`, `CheckedIn`, `CheckedOut`, `Cancelled`
- [ ] Criar enum `StayPaymentStatus`:
  - [ ] `Pending`, `Partial`, `Completed`, `Refunded`
- [ ] Criar modelo `StayRequest`:
  - [ ] `Id`, `VisitorUserId`, `HostingConfigurationId`, `TerritoryId`
  - [ ] `CheckInDate`, `CheckOutDate` (DateOnly)
  - [ ] `TotalAmount`, `Currency`
  - [ ] `Status` (StayRequestStatus)
  - [ ] `ApprovalSource`, `ApprovedByUserId?`, `ApprovedAtUtc?`
  - [ ] `RejectedAtUtc?`, `RejectionReason?`
  - [ ] `CreatedAtUtc`, `UpdatedAtUtc`
  - [ ] Métodos: `Approve()`, `AutoApprove()`, `Reject()`, `Cancel()`
- [ ] Criar modelo `Stay`:
  - [ ] `Id`, `StayRequestId`, `VisitorUserId`, `HostingConfigurationId`, `PropertyId`, `TerritoryId`
  - [ ] `CheckInDate`, `CheckOutDate`
  - [ ] `TotalAmount`, `Currency`, `PaymentStatus`
  - [ ] `Status` (StayStatus)
  - [ ] `CreatedAtUtc`, `CheckedInAtUtc?`, `CheckedOutAtUtc?`
  - [ ] Métodos: `MarkCheckedIn()`, `MarkCheckedOut()`, `Cancel()`

#### 30.8 Repositórios - StayRequest e Stay
**Estimativa**: 16 horas (2 dias)  
**Status**: ⏳ Planejado

**Tarefas**:
- [ ] Criar interface `IStayRequestRepository`:
  - [ ] `GetByIdAsync()`, `GetByVisitorAsync()`, `GetByConfigurationAsync()`
  - [ ] `GetPendingApprovalAsync()`
  - [ ] `AddAsync()`, `UpdateAsync()`
- [ ] Criar interface `IStayRepository`:
  - [ ] `GetByIdAsync()`, `GetByVisitorAsync()`, `GetByConfigurationAsync()`
  - [ ] `AddAsync()`, `UpdateAsync()`
- [ ] Implementar repositórios InMemory e Postgres
- [ ] Criar migration para `stay_requests` e `stays`

#### 30.9 Integração com WorkItem para Aprovação
**Estimativa**: 16 horas (2 dias)  
**Status**: ⏳ Planejado

**Tarefas**:
- [ ] Adicionar `StayRequestApproval` ao enum `WorkItemType`
- [ ] Criar `StayRequestApprovalService`:
  - [ ] `CreateApprovalWorkItemAsync()` - cria WorkItem quando requer aprovação manual
  - [ ] `ProcessApprovalAsync()` - processa aprovação via WorkItem
  - [ ] `ProcessRejectionAsync()` - processa rejeição via WorkItem
- [ ] Integrar com `WorkQueueService` existente

**Total Semana 5-6**: 56 horas (7 dias)

---

### Semana 7-8: Pagamentos e Check-in/out

#### 30.10 Integração com FinancialTransaction
**Estimativa**: 24 horas (3 dias)  
**Status**: ⏳ Planejado

**Tarefas**:
- [ ] Adicionar `HostingPayment` ao enum `TransactionType`
- [ ] Criar `HostingPaymentService`:
  - [ ] `CreateEscrowTransactionAsync()` - cria escrow quando Stay é criado
  - [ ] `ProcessCheckInPaymentAsync()` - libera primeira parcela (se configurado)
  - [ ] `ProcessCheckOutPaymentAsync()` - libera pagamento completo com split
  - [ ] `ProcessRefundAsync()` - processa reembolso conforme política
- [ ] Criar `HostingPaymentSplitConfig`:
  - [ ] Configuração por território: Owner %, Limpeza %, Plataforma %
  - [ ] Limpeza pode ser fixo ou percentual
  - [ ] Reutilizar padrão do Marketplace

#### 30.11 Check-in e Check-out
**Estimativa**: 24 horas (3 dias) - **Aumentado para incluir CleaningServiceRequest**  
**Status**: ⏳ Planejado

**Tarefas**:
- [ ] Criar `StayManagementService`:
  - [ ] `MarkCheckedInAsync()` - marca check-in, notifica Host e Limpeza (se já atribuído), libera primeira parcela
  - [ ] `MarkCheckedOutAsync()` - marca check-out, cria CleaningServiceRequest (se não atribuído), notifica Limpeza, libera pagamento completo, libera datas
  - [ ] `CancelStayAsync()` - cancela estadia, cancela CleaningServiceRequest (se existir), processa reembolso, libera datas
- [ ] Integrar com `HostingCalendar` para liberar datas
- [ ] Integrar com notificações
- [ ] **Nova funcionalidade**: Criar `CleaningServiceRequest` automaticamente quando Stay é confirmada (com data/horário de checkout)

#### 30.11.1 Serviços de Gestão de Host e Cleaning
**Estimativa**: 32 horas (4 dias) - **Nova seção**  
**Status**: ⏳ Planejado

**Tarefas**:
- [ ] Criar `HostInvitationService`:
  - [ ] `SendInvitationAsync()` - Owner envia convite para morador ser Host
  - [ ] `AcceptInvitationAsync()` - Morador aceita convite, cria HostingRole
  - [ ] `RejectInvitationAsync()` - Morador rejeita convite
  - [ ] `ExpireInvitationsAsync()` - Job para expirar convites antigos
- [ ] Criar `HostOfferService`:
  - [ ] `CreateOfferAsync()` - Morador cria oferta de hosting
  - [ ] `SearchAvailableOffersAsync()` - Busca ofertas disponíveis (para Owners)
  - [ ] `UpdateOfferAsync()` - Atualiza disponibilidade
  - [ ] `ActivateOfferAsync()`, `DeactivateOfferAsync()`, `PauseOfferAsync()`
- [ ] Criar `CleaningOfferService`:
  - [ ] `CreateOfferAsync()` - Morador cria oferta de limpeza
  - [ ] `SearchAvailableOffersAsync()` - Busca ofertas disponíveis (para Owners/Hosts)
  - [ ] `UpdateOfferAsync()` - Atualiza disponibilidade
  - [ ] `ActivateOfferAsync()`, `DeactivateOfferAsync()`, `PauseOfferAsync()`
- [ ] Criar `CleaningServiceRequestService`:
  - [ ] `CreateRequestAsync()` - Criado automaticamente quando Stay é confirmada
  - [ ] `GetOpenRequestsAsync()` - Lista solicitações abertas (para moradores com CleaningOffer)
  - [ ] `NotifyEligibleCleanersAsync()` - Notifica moradores com CleaningOffer ativa
- [ ] Criar `CleaningServiceApplicationService`:
  - [ ] `ApplyForServiceAsync()` - Morador se candidata a serviço de limpeza
  - [ ] `AcceptApplicationAsync()` - Owner/Host aceita candidatura, atribui serviço
  - [ ] `RejectApplicationAsync()` - Owner/Host rejeita candidatura
  - [ ] `WithdrawApplicationAsync()` - Morador retira candidatura
  - [ ] `MarkServiceInProgressAsync()` - Marca serviço em execução (no check-out)
  - [ ] `MarkServiceCompletedAsync()` - Marca serviço concluído, processa pagamento

**Total Semana 7-8**: 72 horas (9 dias) - **Aumentado para incluir gestão de Host e Cleaning**

---

### Semana 9: Notificações e Busca

#### 30.12 Notificações
**Estimativa**: 16 horas (2 dias)  
**Status**: ⏳ Planejado

**Tarefas**:
- [ ] Adicionar tipos de notificação:
  - [ ] `HostingRequest` (nova solicitação)
  - [ ] `HostingApproved` (solicitação aprovada)
  - [ ] `HostingRejected` (solicitação rejeitada)
  - [ ] `HostingCheckIn` (check-in realizado)
  - [ ] `HostingCheckOut` (check-out realizado)
  - [ ] `HostingCleaningRequired` (limpeza necessária)
- [ ] Criar handlers de notificação:
  - [ ] Notificar Host quando há nova solicitação
  - [ ] Notificar Visitante quando aprovada/rejeitada
  - [ ] Notificar Limpeza em check-in/check-out
  - [ ] Notificar Owner em eventos importantes

#### 30.13 Busca de Propriedades Disponíveis
**Estimativa**: 16 horas (2 dias)  
**Status**: ⏳ Planejado

**Tarefas**:
- [ ] Criar `HostingSearchService`:
  - [ ] `SearchAvailablePropertiesAsync()` - busca propriedades disponíveis
  - [ ] Filtros: Territory, datas (check-in/check-out), tipo, capacidade, preço
  - [ ] Apenas HostingConfigurations ativas + datas Available
  - [ ] Paginação
- [ ] Criar índices para performance:
  - [ ] `hosting_configurations(status, territory_id)`
  - [ ] `hosting_calendar_dates(configuration_id, date, state)`

**Total Semana 9**: 32 horas (4 dias)

---

### Semana 10: API e Testes

#### 30.14 Controllers e Endpoints
**Estimativa**: 40 horas (5 dias) - **Aumentado para incluir novos endpoints**  
**Status**: ⏳ Planejado

**Tarefas**:
- [ ] Criar `PropertiesController`:
  - [ ] `POST /api/v1/properties` - criar propriedade
  - [ ] `GET /api/v1/properties` - listar propriedades do usuário
  - [ ] `GET /api/v1/properties/{id}` - obter propriedade
  - [ ] `PUT /api/v1/properties/{id}` - atualizar propriedade
- [ ] Criar `HostingConfigurationsController`:
  - [ ] `POST /api/v1/properties/{propertyId}/configurations` - criar configuração
  - [ ] `GET /api/v1/properties/{propertyId}/configurations` - listar configurações
  - [ ] `PUT /api/v1/configurations/{id}` - atualizar configuração
  - [ ] `POST /api/v1/configurations/{id}/activate` - ativar
  - [ ] `POST /api/v1/configurations/{id}/deactivate` - desativar
- [ ] Criar `HostingCalendarController`:
  - [ ] `GET /api/v1/configurations/{id}/calendar` - obter agenda
  - [ ] `POST /api/v1/configurations/{id}/calendar/open` - abrir datas
  - [ ] `POST /api/v1/configurations/{id}/calendar/block` - bloquear datas
  - [ ] `GET /api/v1/configurations/{id}/calendar/available` - datas disponíveis
- [ ] Criar `HostingRolesController`:
  - [ ] `POST /api/v1/configurations/{id}/roles` - delegar papel
  - [ ] `GET /api/v1/configurations/{id}/roles` - listar papéis
  - [ ] `DELETE /api/v1/roles/{id}` - revogar papel
- [ ] Criar `StayRequestsController`:
  - [ ] `POST /api/v1/stay-requests` - criar solicitação
  - [ ] `GET /api/v1/stay-requests` - listar solicitações (Host/Visitor)
  - [ ] `GET /api/v1/stay-requests/{id}` - obter solicitação
  - [ ] `POST /api/v1/stay-requests/{id}/approve` - aprovar (Host)
  - [ ] `POST /api/v1/stay-requests/{id}/reject` - rejeitar (Host)
  - [ ] `POST /api/v1/stay-requests/{id}/cancel` - cancelar
- [ ] Criar `StaysController`:
  - [ ] `GET /api/v1/stays` - listar estadias
  - [ ] `GET /api/v1/stays/{id}` - obter estadia
  - [ ] `POST /api/v1/stays/{id}/check-in` - check-in
  - [ ] `POST /api/v1/stays/{id}/check-out` - check-out
  - [ ] `POST /api/v1/stays/{id}/cancel` - cancelar
- [ ] Criar `HostingSearchController`:
  - [ ] `GET /api/v1/hosting/search` - buscar propriedades disponíveis
  - [ ] Filtros: territoryId, checkIn, checkOut, type, capacity, maxPrice
- [ ] Criar `HostInvitationsController`:
  - [ ] `POST /api/v1/configurations/{id}/host-invitations` - enviar convite para Host
  - [ ] `GET /api/v1/configurations/{id}/host-invitations` - listar convites
  - [ ] `POST /api/v1/host-invitations/{id}/accept` - aceitar convite
  - [ ] `POST /api/v1/host-invitations/{id}/reject` - rejeitar convite
- [ ] Criar `HostOffersController`:
  - [ ] `POST /api/v1/host-offers` - criar oferta de hosting
  - [ ] `GET /api/v1/host-offers` - listar ofertas (próprias ou públicas)
  - [ ] `GET /api/v1/host-offers/search` - buscar ofertas disponíveis
  - [ ] `PUT /api/v1/host-offers/{id}` - atualizar oferta
  - [ ] `POST /api/v1/host-offers/{id}/activate` - ativar oferta
  - [ ] `POST /api/v1/host-offers/{id}/deactivate` - desativar oferta
- [ ] Criar `CleaningOffersController`:
  - [ ] `POST /api/v1/cleaning-offers` - criar oferta de limpeza
  - [ ] `GET /api/v1/cleaning-offers` - listar ofertas (próprias ou públicas)
  - [ ] `GET /api/v1/cleaning-offers/search` - buscar ofertas disponíveis
  - [ ] `PUT /api/v1/cleaning-offers/{id}` - atualizar oferta
  - [ ] `POST /api/v1/cleaning-offers/{id}/activate` - ativar oferta
  - [ ] `POST /api/v1/cleaning-offers/{id}/deactivate` - desativar oferta
- [ ] Criar `CleaningServiceRequestsController`:
  - [ ] `GET /api/v1/cleaning-service-requests` - listar solicitações (abertas, atribuídas, próprias)
  - [ ] `GET /api/v1/cleaning-service-requests/{id}` - obter solicitação
  - [ ] `POST /api/v1/cleaning-service-requests/{id}/assign` - atribuir serviço (Owner/Host)
  - [ ] `POST /api/v1/cleaning-service-requests/{id}/complete` - marcar concluído
  - [ ] `POST /api/v1/cleaning-service-requests/{id}/cancel` - cancelar
- [ ] Criar `CleaningServiceApplicationsController`:
  - [ ] `POST /api/v1/cleaning-service-requests/{id}/applications` - candidatar-se a serviço
  - [ ] `GET /api/v1/cleaning-service-requests/{id}/applications` - listar candidaturas
  - [ ] `POST /api/v1/cleaning-applications/{id}/accept` - aceitar candidatura (Owner/Host)
  - [ ] `POST /api/v1/cleaning-applications/{id}/reject` - rejeitar candidatura (Owner/Host)
  - [ ] `POST /api/v1/cleaning-applications/{id}/withdraw` - retirar candidatura

#### 30.15 Testes
**Estimativa**: 32 horas (4 dias)  
**Status**: ⏳ Planejado

**Tarefas**:
- [ ] Testes de domínio:
  - [ ] Property (visibilidade, regras)
  - [ ] HostingConfiguration (ativação, visibilidade)
  - [ ] HostingCalendar (estados, bloqueios, reservas)
  - [ ] HostingRole (delegação, revogação)
  - [ ] StayRequest (aprovação, auto-aprovação, rejeição)
  - [ ] Stay (check-in, check-out, cancelamento)
- [ ] Testes de aplicação:
  - [ ] PropertyService (criação, atualização)
  - [ ] HostingConfigurationService (criação, ativação)
  - [ ] HostingCalendarService (abrir, bloquear, reservar)
  - [ ] StayRequestService (criação, aprovação)
  - [ ] StayManagementService (check-in, check-out)
  - [ ] HostingPaymentService (escrow, split, reembolso)
  - [ ] HostingSearchService (busca, filtros)
- [ ] Testes de API (E2E):
  - [ ] Fluxo completo: criar propriedade → configurar → ativar → solicitar → aprovar → check-in → check-out
  - [ ] Testes de concorrência (múltiplas solicitações simultâneas)
  - [ ] Testes de edge cases (datas inválidas, valores zero, etc.)

**Total Semana 10**: 56 horas (7 dias)

---

## 📊 Resumo de Estimativas

| Semana | Tarefas | Horas | Dias |
|--------|---------|-------|------|
| 1-2 | Fundação - Modelo de Domínio | 96h | 12d | ⬆️ +16h (gestão Host/Cleaning) |
| 3-4 | Repositórios e Infraestrutura | 64h | 8d | ⬆️ +16h (novos repositórios) |
| 5-6 | StayRequest e Stay | 56h | 7d | - |
| 7-8 | Pagamentos e Check-in/out + Gestão Host/Cleaning | 72h | 9d | ⬆️ +32h (novos serviços) |
| 9 | Notificações e Busca | 32h | 4d | - |
| 10 | API e Testes | 72h | 9d | ⬆️ +16h (novos controllers) |
| **TOTAL** | **10 semanas** | **392h** | **49d** | ⬆️ +80h (+10 dias) |

**Buffer para imprevistos**: +7 dias (15% de buffer)  
**Total com Buffer**: **56 dias úteis (11 semanas)**

---

## 🔌 Integrações com Sistema Existente

### 1. Feature Flags
- ✅ Adicionar `HostingEnabled` ao enum `FeatureFlag`
- ✅ Usar `TerritoryFeatureFlagGuard.EnsureHostingEnabled()`

### 2. Notificações
- ✅ Reutilizar `OutboxMessage` e `UserNotification`
- ✅ Novos tipos: `HostingRequest`, `HostingApproved`, `HostingRejected`, `HostingCheckIn`, `HostingCheckOut`, `HostingCleaningRequired`

### 3. WorkItem para Aprovação
- ✅ Adicionar `StayRequestApproval` ao enum `WorkItemType`
- ✅ Reutilizar `WorkQueueService` existente

### 4. Pagamentos
- ✅ Adicionar `HostingPayment` ao enum `TransactionType`
- ✅ Reutilizar `FinancialTransaction` e split do Marketplace
- ✅ Criar `HostingPaymentSplitConfig` (similar ao Marketplace)

### 5. Regras de Acesso
- ✅ Criar `HostingAccessRules` (similar ao Marketplace)
- ✅ Validar Morador Validado para criar Property
- ✅ Validar Owner/Host para gerenciar configuração

---

## ⚠️ Riscos e Mitigações

### Risco 1: Complexidade da Agenda
**Mitigação**: 
- Agenda como entidade separada e bem testada
- Estados explícitos e imutáveis
- Testes de concorrência extensivos
- Índices no banco para performance

### Risco 2: Confusão com Marketplace
**Mitigação**:
- Domínio completamente separado (`Araponga.Domain/Hosting/`)
- Nomenclatura distinta (Property ≠ Store, StayRequest ≠ Checkout)
- Documentação clara das diferenças

### Risco 3: Performance da Agenda
**Mitigação**:
- Índices no banco (HostingConfigurationId + Date)
- Cache de datas disponíveis
- Paginação em buscas

### Risco 4: Split de Pagamento Complexo
**Mitigação**:
- Reutilizar padrão do Marketplace
- Configuração flexível por território
- Testes de edge cases (valores zero, percentuais totais)

---

## 📝 Critérios de Aceitação

### MVP Completo
- [ ] Morador Validado pode criar Property (privada)
- [ ] Morador pode criar HostingConfiguration
- [ ] Agenda funciona corretamente (abrir, bloquear, reservar)
- [ ] Visitante pode criar StayRequest
- [ ] Host pode aprovar/rejeitar via WorkItem
- [ ] Auto-aprovação condicional funciona
- [ ] Check-in/Check-out funciona
- [ ] Pagamento com escrow e split funciona
- [ ] Notificações são enviadas corretamente
- [ ] Busca de propriedades disponíveis funciona
- [ ] Testes com cobertura >90%

---

## 🔗 Referências

- [Proposta de Implementação de Hospedagem](../PROPOSTA_IMPLEMENTACAO_HOSPEDAGEM.md)
- [Análise de Inserção no Roadmap](../ANALISE_INSERCAO_HOSPEDAGEM_ROADMAP.md)
- [Marketplace (Fase 6-7)](./FASE6.md) - Referência de padrões
- [Sistema de Pagamentos (Fase 7)](./FASE7.md) - Escrow e split
- [WorkItem para Aprovação](../33_ADMIN_SYSTEM_CONFIG_WORKQUEUE.md)

---

**Status**: ⏳ **PLANEJADO**  
**Prioridade**: 🟡 **P1 (Alta)**  
**Onda**: **7 - Economia Circular**  
**Timeline**: **Mês 9-12** (após Fase 23, antes de Fase 24)
