# 🏠 Proposta de Implementação: Domínio de Hospedagem

**Data**: 2026-01-25  
**Status**: 📋 Proposta  
**Referência**: Airbnb (adaptado aos princípios do Araponga)

---

## 📋 Resumo Executivo

Esta proposta detalha a implementação do domínio de **Hospedagem** no Araponga, inspirada no modelo do Airbnb mas adaptada aos princípios territoriais e comunitários da plataforma. A implementação reutiliza infraestrutura existente (notificações, pagamentos, aprovação humana) e introduz conceitos específicos de hospedagem (propriedades privadas, agenda, papéis contextuais).

---

## 🎯 Princípios de Design

### 1. Reutilização Inteligente
- ✅ **Reutilizar**: Notificações, Pagamentos, WorkItem, FeatureFlags
- ✅ **Adaptar**: Regras compostas do Marketplace para Hospedagem
- ❌ **Não reutilizar**: Abstrações diretas do Marketplace (Store, StoreItem)

### 2. Privacidade por Padrão
- Propriedades são **privadas** até terem hospedagem ativa
- Visibilidade controlada por `HostingConfiguration` ativa + agenda disponível

### 3. Agenda como Núcleo
- Agenda é o **coração** do sistema de hospedagem
- Estados explícitos: Available, BlockedByResident, PendingApproval, Reserved
- Agenda inicia **totalmente bloqueada** por padrão

### 4. Aprovação Humana com Auto-Aprovação Condicional
- Toda estadia exige consentimento humano
- Auto-aprovação apenas se **todos** os critérios forem atendidos
- Todas as decisões automáticas são auditáveis

---

## 🏗️ Arquitetura Proposta

### Camada de Domínio (`Araponga.Domain/Hosting/`)

#### 1. Property (Propriedade)
```csharp
public sealed class Property
{
    public Guid Id { get; }
    public Guid OwnerUserId { get; } // Morador Validado
    public Guid TerritoryId { get; }
    
    // Privacidade
    public PropertyVisibility Visibility { get; private set; } // Private por padrão
    
    // Dados da propriedade
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public double? Latitude { get; private set; }
    public double? Longitude { get; private set; }
    public string? Address { get; private set; }
    
    // Status
    public PropertyStatus Status { get; private set; } // Active, Inactive
    
    // Timestamps
    public DateTime CreatedAtUtc { get; }
    public DateTime UpdatedAtUtc { get; private set; }
    
    // Relacionamentos
    public IReadOnlyList<HostingConfiguration> Configurations { get; }
    
    // Regras de negócio
    public void UpdateVisibility(PropertyVisibility visibility);
    public bool IsVisibleToPublic(); // Só se tiver HostingConfiguration ativa + agenda disponível
}
```

**Características**:
- ✅ Privada por padrão (`Visibility = Private`)
- ✅ Visível apenas para o Owner até ter hospedagem ativa
- ✅ Pode existir indefinidamente sem hospedagem

#### 2. HostingConfiguration (Configuração de Hospedagem)
```csharp
public sealed class HostingConfiguration
{
    public Guid Id { get; }
    public Guid PropertyId { get; }
    public Guid TerritoryId { get; }
    
    // Tipo de acomodação
    public AccommodationType Type { get; private set; } // EntirePlace, PrivateRoom, SharedRoom
    public int MaxCapacity { get; private set; }
    
    // Regras e políticas
    public string? HouseRules { get; private set; }
    public TimeSpan? CheckInTime { get; private set; }
    public TimeSpan? CheckOutTime { get; private set; }
    public CancellationPolicy CancellationPolicy { get; private set; }
    
    // Modalidades de locação
    public RentalModality Modality { get; private set; } // Daily, Weekly, Monthly, Annual, Packages
    
    // Política de aprovação
    public ApprovalPolicy ApprovalPolicy { get; private set; } // ManualOnly, ConditionalAutoApprove
    public ApprovalCriteria? AutoApproveCriteria { get; private set; } // JSON com critérios
    
    // Status
    public HostingConfigurationStatus Status { get; private set; } // Active, Inactive
    
    // Timestamps
    public DateTime CreatedAtUtc { get; }
    public DateTime UpdatedAtUtc { get; private set; }
    
    // Relacionamentos
    public HostingCalendar Calendar { get; } // Agenda exclusiva
    public IReadOnlyList<HostingRole> Roles { get; } // Papéis contextuais
    
    // Regras de negócio
    public bool IsPubliclyVisible(); // Ativa + ao menos uma data disponível
    public void Activate();
    public void Deactivate();
}
```

**Características**:
- ✅ Uma Property pode ter múltiplas HostingConfigurations
- ✅ Cada HostingConfiguration tem sua própria agenda
- ✅ Visibilidade pública depende de: Status=Active + Calendar tem datas Available

#### 3. HostingCalendar (Agenda - NÚCLEO)
```csharp
public sealed class HostingCalendar
{
    public Guid Id { get; }
    public Guid HostingConfigurationId { get; }
    
    // Estados por data
    public IReadOnlyDictionary<DateOnly, CalendarDateState> Dates { get; }
    
    // Padrões e regras
    public IReadOnlyList<CalendarPattern> Patterns { get; } // Bloqueios recorrentes
    public CalendarRules Rules { get; private set; } // Antecedência mínima, janela máxima
    
    // Regras de negócio
    public void OpenDate(DateOnly date);
    public void BlockDate(DateOnly date, BlockReason reason);
    public void ReserveDate(DateOnly date, Guid stayRequestId);
    public void ReleaseDate(DateOnly date);
    
    public bool IsDateAvailable(DateOnly date);
    public IReadOnlyList<DateOnly> GetAvailableDates(DateOnly start, DateOnly end);
    
    // Agenda inicia totalmente bloqueada
    public HostingCalendar(Guid id, Guid hostingConfigurationId)
    {
        Dates = new Dictionary<DateOnly, CalendarDateState>();
        // Todas as datas começam como BlockedByResident
    }
}

public enum CalendarDateState
{
    Available = 1,              // Data aberta para reserva
    BlockedByResident = 2,     // Bloqueada pelo morador
    PendingApproval = 3,        // Solicitação pendente
    Reserved = 4                // Reservada (StayRequest aprovada)
}
```

**Características**:
- ✅ **Núcleo do domínio**: toda lógica de hospedagem gira em torno da agenda
- ✅ Inicia totalmente bloqueada
- ✅ Host/Owner deve abrir datas explicitamente
- ✅ Datas reservadas não podem ser sobrescritas

#### 4. HostingRole (Papéis Contextuais) - Gestão pela Plataforma
```csharp
public sealed class HostingRole
{
    public Guid Id { get; }
    public Guid HostingConfigurationId { get; }
    public Guid MembershipId { get; } // Morador Validado do mesmo território
    public HostingRoleType Type { get; } // Owner, Host, Cleaning
    
    // Metadados
    public DateTime GrantedAtUtc { get; }
    public Guid GrantedByUserId { get; }
    public DateTime? RevokedAtUtc { get; private set; }
    public Guid? RevokedByUserId { get; private set; }
    
    // Regras de negócio
    public void Revoke(Guid revokedByUserId, DateTime revokedAtUtc);
    public bool IsActive();
}

public enum HostingRoleType
{
    Owner = 1,      // Dono da propriedade (sempre o criador)
    Host = 2,       // Responsável por aprovar/rejeitar
    Cleaning = 3    // Responsável pela limpeza
}
```

**Características**:
- ✅ Papéis são **contextuais** (por HostingConfiguration)
- ✅ Um morador pode acumular múltiplos papéis
- ✅ Owner é sempre o criador da Property
- ✅ Host e Cleaning devem ser Moradores Validados do mesmo território
- ✅ **Gestão pela plataforma**: Ofertas e convites gerenciados pelo sistema

#### 4.1 HostInvitation (Convite para Host)
```csharp
public sealed class HostInvitation
{
    public Guid Id { get; }
    public Guid HostingConfigurationId { get; }
    public Guid InvitedMembershipId { get; } // Morador convidado
    public Guid InvitedByUserId { get; } // Owner ou Host atual
    
    // Status
    public HostInvitationStatus Status { get; private set; } // Pending, Accepted, Rejected, Expired
    public DateTime? AcceptedAtUtc { get; private set; }
    public DateTime? RejectedAtUtc { get; private set; }
    public DateTime ExpiresAtUtc { get; } // 7 dias após criação
    
    // Timestamps
    public DateTime CreatedAtUtc { get; }
    public DateTime UpdatedAtUtc { get; private set; }
    
    // Regras de negócio
    public void Accept(DateTime acceptedAtUtc); // Cria HostingRole automaticamente
    public void Reject(DateTime rejectedAtUtc);
    public bool IsExpired();
}

public enum HostInvitationStatus
{
    Pending = 1,
    Accepted = 2,
    Rejected = 3,
    Expired = 4
}
```

**Características**:
- ✅ Owner pode enviar convite para morador validado ser Host
- ✅ Convite expira em 7 dias
- ✅ Ao aceitar, cria `HostingRole` automaticamente
- ✅ Notificação enviada ao convidado

#### 4.2 HostOffer (Oferta de Serviço de Hosting)
```csharp
public sealed class HostOffer
{
    public Guid Id { get; }
    public Guid MembershipId { get; } // Morador que oferece serviço
    public Guid TerritoryId { get; }
    
    // Disponibilidade
    public DateOnly AvailableFrom { get; private set; }
    public DateOnly? AvailableUntil { get; private set; }
    public List<DayOfWeek> AvailableDaysOfWeek { get; private set; }
    
    // Capacidades
    public List<AccommodationType> SupportedTypes { get; private set; } // EntirePlace, PrivateRoom, SharedRoom
    public int? MaxPropertiesManaged { get; private set; }
    
    // Status
    public HostOfferStatus Status { get; private set; } // Active, Inactive, Paused
    public bool IsPubliclyVisible { get; private set; } // Visível para moradores
    
    // Timestamps
    public DateTime CreatedAtUtc { get; }
    public DateTime UpdatedAtUtc { get; private set; }
    
    // Regras de negócio
    public void Activate();
    public void Deactivate();
    public void Pause();
    public void UpdateAvailability(DateOnly from, DateOnly? until, List<DayOfWeek> days);
    public bool IsAvailableFor(DateOnly date);
}

public enum HostOfferStatus
{
    Active = 1,
    Inactive = 2,
    Paused = 3
}
```

**Características**:
- ✅ Morador validado pode criar oferta de serviço de hosting
- ✅ Visível para outros moradores do território
- ✅ Owner pode visualizar ofertas ao configurar hospedagem
- ✅ Permite encontrar hosts disponíveis

#### 4.3 CleaningOffer (Oferta de Serviço de Limpeza)
```csharp
public sealed class CleaningOffer
{
    public Guid Id { get; }
    public Guid MembershipId { get; } // Morador que oferece serviço
    public Guid TerritoryId { get; }
    
    // Disponibilidade
    public List<DayOfWeek> AvailableDaysOfWeek { get; private set; }
    public TimeSpan? PreferredStartTime { get; private set; }
    public TimeSpan? PreferredEndTime { get; private set; }
    public int? MaxHoursPerDay { get; private set; }
    
    // Capacidades
    public List<AccommodationType> SupportedTypes { get; private set; }
    public int? MaxPropertiesPerDay { get; private set; }
    
    // Status
    public CleaningOfferStatus Status { get; private set; } // Active, Inactive, Paused
    public bool IsPubliclyVisible { get; private set; } // Visível para moradores
    
    // Timestamps
    public DateTime CreatedAtUtc { get; }
    public DateTime UpdatedAtUtc { get; private set; }
    
    // Regras de negócio
    public void Activate();
    public void Deactivate();
    public void Pause();
    public bool IsAvailableFor(DateOnly date, TimeSpan? checkoutTime);
}

public enum CleaningOfferStatus
{
    Active = 1,
    Inactive = 2,
    Paused = 3
}
```

**Características**:
- ✅ Morador validado pode criar oferta de serviço de limpeza
- ✅ Visível para outros moradores do território
- ✅ Permite especificar disponibilidade por dia da semana e horário

#### 4.4 CleaningServiceRequest (Solicitação de Serviço de Limpeza)
```csharp
public sealed class CleaningServiceRequest
{
    public Guid Id { get; }
    public Guid StayId { get; } // Estadia que gerou a solicitação
    public Guid HostingConfigurationId { get; }
    public Guid PropertyId { get; }
    public Guid TerritoryId { get; }
    
    // Data e horário do serviço
    public DateOnly ServiceDate { get; } // Data do checkout
    public TimeSpan CheckoutTime { get; } // Horário do checkout
    public TimeSpan? PreferredServiceTime { get; private set; } // Horário preferido para limpeza
    
    // Status
    public CleaningServiceRequestStatus Status { get; private set; } // Open, Assigned, InProgress, Completed, Cancelled
    public Guid? AssignedCleaningMembershipId { get; private set; }
    public DateTime? AssignedAtUtc { get; private set; }
    public DateTime? CompletedAtUtc { get; private set; }
    
    // Valor
    public decimal? EstimatedAmount { get; private set; }
    public decimal? FinalAmount { get; private set; }
    
    // Timestamps
    public DateTime CreatedAtUtc { get; }
    public DateTime UpdatedAtUtc { get; private set; }
    
    // Relacionamentos
    public IReadOnlyList<CleaningServiceApplication> Applications { get; } // Moradores que se candidataram
    
    // Regras de negócio
    public void AssignTo(Guid cleaningMembershipId, DateTime assignedAtUtc);
    public void MarkInProgress();
    public void MarkCompleted(decimal finalAmount, DateTime completedAtUtc);
    public void Cancel(string? reason);
    public bool CanAcceptApplications(); // Apenas se Status = Open
}

public enum CleaningServiceRequestStatus
{
    Open = 1,           // Aberto para candidaturas
    Assigned = 2,       // Atribuído a um morador
    InProgress = 3,    // Em execução
    Completed = 4,     // Concluído
    Cancelled = 5      // Cancelado
}
```

**Características**:
- ✅ Criada automaticamente quando Stay é confirmada (com data de checkout)
- ✅ Visível para moradores com CleaningOffer ativa
- ✅ Moradores podem se candidatar
- ✅ Owner/Host confirma candidato selecionado

#### 4.5 CleaningServiceApplication (Candidatura para Serviço de Limpeza)
```csharp
public sealed class CleaningServiceApplication
{
    public Guid Id { get; }
    public Guid CleaningServiceRequestId { get; }
    public Guid ApplicantMembershipId { get; } // Morador que se candidata
    public Guid TerritoryId { get; }
    
    // Proposta
    public decimal? ProposedAmount { get; private set; }
    public string? Message { get; private set; }
    
    // Status
    public CleaningApplicationStatus Status { get; private set; } // Pending, Accepted, Rejected, Withdrawn
    public DateTime? AcceptedAtUtc { get; private set; }
    public DateTime? RejectedAtUtc { get; private set; }
    
    // Timestamps
    public DateTime CreatedAtUtc { get; }
    public DateTime UpdatedAtUtc { get; private set; }
    
    // Regras de negócio
    public void Accept(DateTime acceptedAtUtc); // Atualiza CleaningServiceRequest para Assigned
    public void Reject(DateTime rejectedAtUtc);
    public void Withdraw(DateTime withdrawnAtUtc);
}

public enum CleaningApplicationStatus
{
    Pending = 1,
    Accepted = 2,
    Rejected = 3,
    Withdrawn = 4
}
```

**Características**:
- ✅ Morador com CleaningOffer ativa pode se candidatar
- ✅ Pode incluir proposta de valor
- ✅ Owner/Host pode aceitar ou rejeitar
- ✅ Ao aceitar, atualiza CleaningServiceRequest para Assigned

#### 5. StayRequest / Stay (Solicitação de Estadia)
```csharp
public sealed class StayRequest
{
    public Guid Id { get; }
    public Guid VisitorUserId { get; } // Visitante (não precisa ser morador)
    public Guid HostingConfigurationId { get; }
    public Guid TerritoryId { get; }
    
    // Datas
    public DateOnly CheckInDate { get; }
    public DateOnly CheckOutDate { get; }
    
    // Valor
    public decimal TotalAmount { get; private set; }
    public string Currency { get; }
    
    // Estado
    public StayRequestStatus Status { get; private set; }
    
    // Aprovação
    public ApprovalSource ApprovalSource { get; private set; } // Manual, AutoApproved
    public Guid? ApprovedByUserId { get; private set; }
    public DateTime? ApprovedAtUtc { get; private set; }
    public DateTime? RejectedAtUtc { get; private set; }
    public string? RejectionReason { get; private set; }
    
    // Timestamps
    public DateTime CreatedAtUtc { get; }
    public DateTime UpdatedAtUtc { get; private set; }
    
    // Relacionamentos
    public Stay? Stay { get; } // Criado quando aprovado
    public FinancialTransaction? PaymentTransaction { get; }
    
    // Regras de negócio
    public void Approve(Guid approvedByUserId, DateTime approvedAtUtc);
    public void AutoApprove(DateTime approvedAtUtc);
    public void Reject(Guid rejectedByUserId, string reason, DateTime rejectedAtUtc);
    public void Cancel(string? reason);
}

public enum StayRequestStatus
{
    PendingApproval = 1,
    AutoApproved = 2,
    Approved = 3,
    Rejected = 4,
    Cancelled = 5,
    Completed = 6
}

public enum ApprovalSource
{
    Manual = 1,
    AutoApproved = 2
}
```

**Características**:
- ✅ Criada por visitante (não precisa ser morador)
- ✅ Estado inicial: `PendingApproval`
- ✅ Pode ser auto-aprovada se critérios forem atendidos
- ✅ Quando aprovada, cria `Stay` e bloqueia datas na agenda

#### 6. Stay (Estadia Confirmada)
```csharp
public sealed class Stay
{
    public Guid Id { get; }
    public Guid StayRequestId { get; }
    public Guid VisitorUserId { get; }
    public Guid HostingConfigurationId { get; }
    public Guid PropertyId { get; }
    public Guid TerritoryId { get; }
    
    // Datas
    public DateOnly CheckInDate { get; }
    public DateOnly CheckOutDate { get; }
    
    // Valor e pagamento
    public decimal TotalAmount { get; }
    public string Currency { get; }
    public StayPaymentStatus PaymentStatus { get; private set; }
    
    // Status
    public StayStatus Status { get; private set; } // Confirmed, CheckedIn, CheckedOut, Cancelled
    
    // Timestamps
    public DateTime CreatedAtUtc { get; }
    public DateTime? CheckedInAtUtc { get; private set; }
    public DateTime? CheckedOutAtUtc { get; private set; }
    
    // Relacionamentos
    public FinancialTransaction? PaymentTransaction { get; }
    
    // Regras de negócio
    public void MarkCheckedIn(DateTime checkedInAtUtc);
    public void MarkCheckedOut(DateTime checkedOutAtUtc);
    public void Cancel(string reason);
}
```

---

## 🔄 Fluxos Principais

### Fluxo 1: Morador Cadastra Propriedade e Ativa Hospedagem

```
1. Morador Validado cria Property (privada)
   → Property.Visibility = Private
   → Property.Status = Active

2. Morador cria HostingConfiguration
   → HostingConfiguration.Status = Inactive
   → HostingCalendar criado (todas as datas bloqueadas)
   → HostingRole criado (Type=Owner, MembershipId do morador)

3. Morador configura agenda
   → Abre datas específicas
   → Define padrões recorrentes
   → Configura regras (antecedência, janela)

4. Morador ativa HostingConfiguration
   → HostingConfiguration.Status = Active
   → Se tiver datas Available → Property.Visibility = Public
```

### Fluxo 2: Visitante Solicita Estadia

```
1. Visitante busca propriedades disponíveis
   → Filtra por: Territory, datas, tipo, capacidade
   → Apenas HostingConfigurations ativas + datas Available

2. Visitante cria StayRequest
   → StayRequest.Status = PendingApproval
   → Bloqueia datas na agenda (PendingApproval)
   → Calcula valor total

3. Sistema avalia auto-aprovação
   → Se ApprovalPolicy = ConditionalAutoApprove
   → Verifica critérios (identidade verificada, duração, valor, antecedência)
   → Se todos atendidos → AutoApprove()
   → Senão → Requer aprovação manual

4. Se requer aprovação manual
   → Cria WorkItem (Type=StayRequestApproval)
   → Notifica Host via UserNotification
   → Host aprova/rejeita via WorkItem

5. Se aprovada
   → Cria Stay
   → Cria FinancialTransaction (escrow)
   → Atualiza agenda (Reserved)
   → Notifica visitante e limpeza
```

### Fluxo 3: Owner Envia Convite para Host

```
1. Owner visualiza ofertas de hosting disponíveis
   → Busca HostOffer ativas no território
   → Filtra por disponibilidade e capacidades

2. Owner envia convite para morador
   → Cria HostInvitation
   → Status = Pending
   → Expira em 7 dias
   → Notifica morador convidado

3. Morador recebe notificação e aceita
   → HostInvitation.Accept()
   → Cria HostingRole automaticamente (Type = Host)
   → Notifica Owner
   → Host pode agora aprovar/rejeitar StayRequests
```

### Fluxo 4: Morador Cria Oferta de Hosting

```
1. Morador Validado cria HostOffer
   → Define disponibilidade (datas, dias da semana)
   → Define capacidades (tipos suportados, max propriedades)
   → Status = Active, IsPubliclyVisible = true

2. Oferta fica visível para Owners
   → Aparece em busca de hosts disponíveis
   → Owners podem enviar convites
```

### Fluxo 5: Morador Cria Oferta de Limpeza

```
1. Morador Validado cria CleaningOffer
   → Define disponibilidade (dias da semana, horários)
   → Define capacidades (tipos suportados, max por dia)
   → Status = Active, IsPubliclyVisible = true

2. Oferta fica visível para Owners/Hosts
   → Aparece quando CleaningServiceRequest é criada
   → Moradores podem se candidatar a serviços
```

### Fluxo 6: Check-in / Check-out e Solicitação de Limpeza

```
1. Check-in
   → Stay.MarkCheckedIn()
   → Notifica Host e Limpeza (se já atribuído)
   → Libera primeira parcela (se configurado)

2. Quando Stay é confirmada, cria CleaningServiceRequest
   → ServiceDate = CheckOutDate
   → CheckoutTime = CheckOutTime da configuração
   → Status = Open (aberto para candidaturas)
   → Notifica moradores com CleaningOffer ativa

3. Moradores se candidatam
   → Criam CleaningServiceApplication
   → Podem incluir proposta de valor
   → Status = Pending

4. Owner/Host seleciona candidato
   → CleaningServiceApplication.Accept()
   → CleaningServiceRequest.AssignTo()
   → Status = Assigned
   → Notifica morador selecionado

5. Check-out
   → Stay.MarkCheckedOut()
   → CleaningServiceRequest.MarkInProgress()
   → Notifica morador de limpeza
   → Libera pagamento completo (split: Owner, Limpeza, Plataforma)

6. Limpeza concluída
   → CleaningServiceRequest.MarkCompleted()
   → Status = Completed
   → Processa pagamento para morador de limpeza
```
   → Libera primeira parcela do pagamento (se configurado)

2. Check-out
   → Stay.MarkCheckedOut()
   → Notifica Limpeza
   → Libera pagamento completo (split: Owner, Limpeza, Plataforma)
   → Libera datas na agenda (Available)

3. Cancelamento
   → Se antes do check-in → reembolso conforme política
   → Se após check-in → sem reembolso (ou parcial conforme política)
   → Libera datas na agenda
```

---

## 🔌 Integração com Sistema Existente

### 1. Feature Flags
```csharp
// Adicionar ao enum FeatureFlag
public enum FeatureFlag
{
    // ... existentes
    HostingEnabled = 24
}

// Uso
var guard = new TerritoryFeatureFlagGuard(_flags);
var result = guard.EnsureHostingEnabled(territoryId);
if (!result.IsSuccess) return NotFound();
```

### 2. Notificações
```csharp
// Reutilizar OutboxMessage e UserNotification
await _notificationService.SendAsync(
    userId: hostUserId,
    title: "Nova solicitação de estadia",
    body: $"Visitante {visitorName} solicitou estadia de {checkIn} a {checkOut}",
    kind: NotificationKind.HostingRequest,
    dataJson: JsonSerializer.Serialize(new { stayRequestId, propertyId })
);
```

### 3. WorkItem para Aprovação
```csharp
// Reutilizar WorkItem existente
var workItem = new WorkItem(
    id: Guid.NewGuid(),
    type: WorkItemType.StayRequestApproval, // Novo tipo
    status: WorkItemStatus.Pending,
    territoryId: territoryId,
    createdByUserId: visitorUserId,
    createdAtUtc: DateTime.UtcNow,
    requiredCapability: MembershipCapabilityType.Curator, // Host pode aprovar
    subjectType: "StayRequest",
    subjectId: stayRequestId,
    payloadJson: JsonSerializer.Serialize(stayRequestData)
);
```

### 4. Pagamentos (Escrow e Split)
```csharp
// Reutilizar FinancialTransaction e split do Marketplace
var transaction = new FinancialTransaction(
    id: Guid.NewGuid(),
    territoryId: territoryId,
    type: TransactionType.HostingPayment, // Novo tipo
    amountInCents: (long)(totalAmount * 100),
    currency: currency,
    description: $"Pagamento estadia {stayId}",
    relatedEntityId: stayId,
    relatedEntityType: "Stay"
);

// Split configurável (similar ao Marketplace)
// - Owner: X%
// - Limpeza: Y% (fixo ou percentual)
// - Plataforma: Z% (configurável por território)
```

### 5. Regras Compostas (Similar ao Marketplace)
```csharp
// Criar helper similar ao Marketplace
public class HostingAccessRules
{
    public static OperationResult CanCreateProperty(
        TerritoryMembership membership,
        MembershipSettings settings)
    {
        // Morador Validado
        if (membership.Role != MembershipRole.Resident)
            return OperationResult.Failure("Apenas moradores podem criar propriedades");
        
        if (membership.ResidencyVerification == ResidencyVerification.None)
            return OperationResult.Failure("Morador deve estar validado");
        
        // Feature flag
        // Territory.FeatureFlags.HostingEnabled
        
        return OperationResult.Success();
    }
}
```

---

## 📊 Modelo de Dados (Resumo)

```
Property (1) ──< (N) HostingConfiguration (1) ──< (1) HostingCalendar
Property (1) ──< (N) HostingConfiguration (1) ──< (N) HostingRole
HostingConfiguration (1) ──< (N) StayRequest (0..1) ──< (1) Stay
StayRequest (1) ──< (1) FinancialTransaction
Stay (1) ──< (1) FinancialTransaction
TerritoryMembership (1) ──< (N) HostingRole (via MembershipId)
```

---

## 🚀 Fases de Implementação (MVP)

### Fase 1: Fundação (2 semanas)
- [ ] Entidades de domínio: Property, HostingConfiguration, HostingCalendar
- [ ] Feature flag: HostingEnabled
- [ ] Repositórios básicos
- [ ] Validação: Morador Validado

### Fase 2: Agenda e Papéis (2 semanas)
- [ ] HostingCalendar completo (estados, padrões, regras)
- [ ] HostingRole (Owner, Host, Cleaning)
- [ ] API: CRUD de Property e HostingConfiguration
- [ ] API: Gerenciamento de agenda

### Fase 3: Solicitações e Aprovação (2 semanas)
- [ ] StayRequest e Stay
- [ ] Integração com WorkItem para aprovação
- [ ] Auto-aprovação condicional
- [ ] API: Criar e gerenciar solicitações

### Fase 4: Pagamentos e Check-in/out (2 semanas)
- [ ] Integração com FinancialTransaction
- [ ] Split de pagamento (Owner, Limpeza, Plataforma)
- [ ] Check-in/Check-out
- [ ] API: Pagamentos e marcos

### Fase 5: Notificações e Busca (1 semana)
- [ ] Notificações para Host, Limpeza, Visitante
- [ ] Busca de propriedades disponíveis
- [ ] API: Busca e filtros

**Total MVP**: ~9 semanas

---

## ⚠️ Riscos e Mitigações

### Risco 1: Complexidade da Agenda
**Mitigação**: 
- Agenda como entidade separada e bem testada
- Estados explícitos e imutáveis
- Testes de concorrência (múltiplas solicitações simultâneas)

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

## 📝 Próximos Passos

1. **Revisar proposta** com equipe
2. **Validar modelo conceitual** com stakeholders
3. **Criar ADR** (Architecture Decision Record) para Hospedagem
4. **Iniciar Fase 1** (Fundação)
5. **Documentar** diferenças com Marketplace

---

## 🔗 Referências

- [Modelo de Domínio Atual](./12_DOMAIN_MODEL.md)
- [Marketplace como Regra Composta](./12_DOMAIN_MODEL.md#marketplace-como-regra-composta)
- [WorkItem para Aprovação Humana](./33_ADMIN_SYSTEM_CONFIG_WORKQUEUE.md)
- [Sistema de Pagamentos](./backlog-api/FASE7.md)
