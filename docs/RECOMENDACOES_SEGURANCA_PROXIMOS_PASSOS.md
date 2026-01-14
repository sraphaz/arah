# Recomendações de Segurança e Próximos Passos

**Data**: 2026-01-13  
**Contexto**: Refatoração User-Centric Membership - SystemPermission, MembershipCapability, MembershipSettings

## Resumo Executivo

Este documento apresenta recomendações de segurança e próximos passos para o sistema Araponga, com foco nas novas entidades e serviços introduzidos na refatoração User-Centric Membership.

**Status Atual**: ✅ Código validado e seguro para produção  
**Prioridade**: Implementação incremental conforme necessidade

---

## 1. Recomendações de Segurança Críticas

### 1.1 Invalidação Automática de Cache

**Problema**: Quando `SystemPermission` ou `MembershipCapability` são revogadas, o cache do `AccessEvaluator` não é automaticamente invalidado, podendo causar permissões obsoletas em cache.

**Impacto**: 
- Usuários podem manter acesso após revogação por até 15 minutos (cache de system permissions)
- Usuários podem manter capabilities após revogação por até 10 minutos (cache de membership)

**Solução Recomendada**:
```csharp
// Opção 1: Eventos de Domínio (Recomendado)
public class SystemPermissionRevokedEvent : IDomainEvent
{
    public Guid UserId { get; }
    public SystemPermissionType PermissionType { get; }
}

// Handler que invalida cache automaticamente
public class SystemPermissionRevokedHandler : IDomainEventHandler<SystemPermissionRevokedEvent>
{
    private readonly AccessEvaluator _accessEvaluator;
    
    public async Task Handle(SystemPermissionRevokedEvent @event)
    {
        _accessEvaluator.InvalidateSystemPermissionCache(
            @event.UserId, 
            @event.PermissionType);
    }
}
```

**Implementação**:
1. Criar eventos de domínio para revogação
2. Implementar handlers que invalidam cache
3. Registrar handlers no container DI
4. Disparar eventos nos métodos `Revoke()`

**Prioridade**: 🔴 Alta (quando serviços administrativos forem criados)

---

### 1.2 Serviços Administrativos para Gerenciamento de Permissões

**Problema**: Não existem serviços dedicados para gerenciar `SystemPermission` e `MembershipCapability`, tornando difícil garantir invalidação de cache e auditoria.

**Solução Recomendada**:

#### SystemPermissionService
```csharp
public sealed class SystemPermissionService
{
    private readonly ISystemPermissionRepository _repository;
    private readonly AccessEvaluator _accessEvaluator;
    private readonly IAuditLogger _auditLogger;
    private readonly IUnitOfWork _unitOfWork;

    public async Task<Result<SystemPermission>> GrantAsync(
        Guid userId,
        SystemPermissionType permissionType,
        Guid grantedByUserId,
        CancellationToken cancellationToken)
    {
        // Validações
        // Criar permissão
        // Log de auditoria
        // Invalidar cache
        // Commit
    }

    public async Task<Result> RevokeAsync(
        Guid permissionId,
        Guid revokedByUserId,
        CancellationToken cancellationToken)
    {
        // Buscar permissão
        // Revogar
        // Log de auditoria
        // Invalidar cache
        // Commit
    }
}
```

#### MembershipCapabilityService
```csharp
public sealed class MembershipCapabilityService
{
    private readonly IMembershipCapabilityRepository _repository;
    private readonly ITerritoryMembershipRepository _membershipRepository;
    private readonly AccessEvaluator _accessEvaluator;
    private readonly IAuditLogger _auditLogger;
    private readonly IUnitOfWork _unitOfWork;

    public async Task<Result<MembershipCapability>> GrantAsync(
        Guid membershipId,
        MembershipCapabilityType capabilityType,
        Guid grantedByUserId,
        Guid? grantedByMembershipId,
        string? reason,
        CancellationToken cancellationToken)
    {
        // Validações
        // Verificar se membership existe
        // Criar capability
        // Log de auditoria
        // Invalidar cache de membership
        // Commit
    }

    public async Task<Result> RevokeAsync(
        Guid capabilityId,
        DateTime revokedAtUtc,
        CancellationToken cancellationToken)
    {
        // Buscar capability
        // Buscar membership para invalidar cache
        // Revogar
        // Log de auditoria
        // Invalidar cache
        // Commit
    }
}
```

**Implementação**:
1. Criar `SystemPermissionService` com métodos `GrantAsync` e `RevokeAsync`
2. Criar `MembershipCapabilityService` com métodos `GrantAsync` e `RevokeAsync`
3. Adicionar validações de autorização (apenas SystemAdmin pode conceder SystemPermissions)
4. Adicionar logs de auditoria
5. Invalidar cache após modificações
6. Criar controllers administrativos (protegidos por SystemAdmin)

**Prioridade**: 🟡 Média (quando funcionalidade administrativa for necessária)

---

### 1.3 Auditoria Completa

**Problema**: Não há logs de auditoria para criação/revogação de `SystemPermission` e `MembershipCapability`.

**Solução Recomendada**:
```csharp
// No SystemPermissionService.GrantAsync
await _auditLogger.LogAsync(
    new AuditEntry(
        "system_permission.granted",
        grantedByUserId,
        null, // territoryId (não aplicável)
        permission.Id,
        DateTime.UtcNow,
        new Dictionary<string, object>
        {
            ["userId"] = userId,
            ["permissionType"] = permissionType.ToString()
        }),
    cancellationToken);

// No SystemPermissionService.RevokeAsync
await _auditLogger.LogAsync(
    new AuditEntry(
        "system_permission.revoked",
        revokedByUserId,
        null,
        permission.Id,
        DateTime.UtcNow,
        new Dictionary<string, object>
        {
            ["userId"] = permission.UserId,
            ["permissionType"] = permission.PermissionType.ToString()
        }),
    cancellationToken);
```

**Implementação**:
1. Adicionar logs de auditoria em todos os métodos de grant/revoke
2. Incluir metadados relevantes (quem, quando, o quê, por quê)
3. Garantir rastreabilidade completa

**Prioridade**: 🟡 Média

---

## 2. Recomendações de Segurança Importantes

### 2.1 Rate Limiting para Verificações de Permissão

**Problema**: Verificações de permissão podem ser abusadas para causar carga no sistema.

**Solução Recomendada**:
- Implementar rate limiting no `AccessEvaluator`
- Limitar número de verificações por usuário/IP por minuto
- Usar cache para reduzir carga (já implementado)

**Prioridade**: 🟢 Baixa (quando houver necessidade)

---

### 2.2 Validação de Entrada em Controllers Administrativos

**Problema**: Quando controllers administrativos forem criados, devem validar entrada adequadamente.

**Solução Recomendada**:
```csharp
// Criar validators FluentValidation
public sealed class GrantSystemPermissionRequestValidator : AbstractValidator<GrantSystemPermissionRequest>
{
    public GrantSystemPermissionRequestValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");

        RuleFor(x => x.PermissionType)
            .IsInEnum().WithMessage("Invalid permission type.");

        RuleFor(x => x.GrantedByUserId)
            .NotEmpty().WithMessage("Granted by user ID is required.");
    }
}
```

**Prioridade**: 🟡 Média (quando controllers forem criados)

---

### 2.3 Proteção de Endpoints Administrativos

**Problema**: Endpoints administrativos devem ser protegidos por autorização adequada.

**Solução Recomendada**:
```csharp
[ApiController]
[Route("api/v1/admin/system-permissions")]
public sealed class SystemPermissionsAdminController : ControllerBase
{
    private readonly CurrentUserAccessor _currentUserAccessor;
    private readonly AccessEvaluator _accessEvaluator;
    private readonly SystemPermissionService _service;

    [HttpPost]
    public async Task<ActionResult> GrantPermission(
        [FromBody] GrantSystemPermissionRequest request,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        // Verificar se usuário é SystemAdmin
        var isAdmin = await _accessEvaluator.IsSystemAdminAsync(
            userContext.User.Id, 
            cancellationToken);
        
        if (!isAdmin)
        {
            return Forbid();
        }

        // Processar requisição
        var result = await _service.GrantAsync(
            request.UserId,
            request.PermissionType,
            userContext.User.Id,
            cancellationToken);

        // Retornar resultado
    }
}
```

**Prioridade**: 🟡 Média (quando controllers forem criados)

---

## 3. Próximos Passos Recomendados

### Fase 1: Fundação (Imediato)
- [x] ✅ Validação de segurança completa
- [x] ✅ Testes de segurança
- [x] ✅ Documentação de validação
- [ ] ⏳ Criar eventos de domínio para revogação
- [ ] ⏳ Implementar handlers de eventos para invalidação de cache

### Fase 2: Serviços Administrativos (Curto Prazo)
- [ ] ⏳ Criar `SystemPermissionService`
- [ ] ⏳ Criar `MembershipCapabilityService`
- [ ] ⏳ Adicionar logs de auditoria
- [ ] ⏳ Criar testes para serviços administrativos

### Fase 3: API Administrativa (Médio Prazo)
- [ ] ⏳ Criar controllers administrativos
- [ ] ⏳ Adicionar validação FluentValidation
- [ ] ⏳ Implementar autorização (SystemAdmin)
- [ ] ⏳ Adicionar documentação Swagger

### Fase 4: Melhorias (Longo Prazo)
- [ ] ⏳ Rate limiting para verificações de permissão
- [ ] ⏳ Monitoramento e alertas de segurança
- [ ] ⏳ Dashboard administrativo
- [ ] ⏳ Relatórios de auditoria

---

## 4. Checklist de Segurança para Novas Features

Ao adicionar novas funcionalidades relacionadas a permissões/capabilities:

- [ ] Validação de entrada adequada
- [ ] Sanitização de strings
- [ ] Validação de tamanho máximo
- [ ] Autorização verificada antes de operações sensíveis
- [ ] Logs de auditoria para operações críticas
- [ ] Invalidação de cache quando necessário
- [ ] Testes de segurança
- [ ] Documentação atualizada

---

## 5. Monitoramento Recomendado

### Métricas de Segurança
- Número de permissões concedidas/revogadas por dia
- Tentativas de acesso não autorizado
- Tempo de resposta de verificações de permissão
- Taxa de cache hit/miss

### Alertas
- Múltiplas revogações de permissão em curto período
- Tentativas de acesso não autorizado repetidas
- Degradação de performance em verificações de permissão

---

## 6. Referências

- [VALIDACAO_SEGURANCA.md](./VALIDACAO_SEGURANCA.md) - Validação completa de segurança realizada
- [VALIDACAO_REST_E_ESTRUTURA.md](./VALIDACAO_REST_E_ESTRUTURA.md) - Validação REST e estrutura
- [12_DOMAIN_MODEL.md](./12_DOMAIN_MODEL.md) - Modelo de domínio

---

## Conclusão

O código atual está **seguro para produção** com as validações implementadas. As recomendações apresentadas são para **melhorias incrementais** que podem ser implementadas conforme a necessidade e evolução do sistema.

**Priorização**: Implementar conforme necessidade de funcionalidades administrativas e crescimento do sistema.
