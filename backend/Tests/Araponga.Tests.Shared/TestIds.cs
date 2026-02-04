namespace Araponga.Tests.Shared;

/// <summary>
/// Constantes com IDs pré-populados no InMemoryDataStore e IDs disponíveis para testes.
/// Usar estas constantes garante coerência e evita conflitos com a massa de testes.
/// </summary>
public static class TestIds
{
    // ============================================
    // IDs PRÉ-POPULADOS NO InMemoryDataStore
    // ============================================
    // ⚠️ NÃO USAR ESTES IDs EM TESTES QUE CRIAM NOVAS ENTIDADES
    // ⚠️ Use-os apenas quando precisar referenciar entidades pré-populadas

    /// <summary>
    /// Território: "Sertão do Camburi" (Active, Ubatuba/SP)
    /// </summary>
    public static readonly Guid Territory1 = Guid.Parse("11111111-1111-1111-1111-111111111111");

    /// <summary>
    /// Território: "Vale do Itamambuca" (Active, Ubatuba/SP)
    /// </summary>
    public static readonly Guid Territory2 = Guid.Parse("22222222-2222-2222-2222-222222222222");

    /// <summary>
    /// Território: "Reserva do Silêncio" (Inactive, Paraty/RJ)
    /// </summary>
    public static readonly Guid Territory3 = Guid.Parse("33333333-3333-3333-3333-333333333333");

    /// <summary>
    /// Usuário: "Morador Teste" (Resident, resident-external)
    /// </summary>
    public static readonly Guid ResidentUser = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

    /// <summary>
    /// Usuário: "Curador" (Curator, curator-external)
    /// </summary>
    public static readonly Guid CuratorUser = Guid.Parse("cccccccc-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

    /// <summary>
    /// Membership: Resident membership do residentUser no Territory2
    /// </summary>
    public static readonly Guid Membership1 = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

    /// <summary>
    /// Membership: Visitor membership no Territory2
    /// </summary>
    public static readonly Guid Membership2 = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd");

    /// <summary>
    /// Post ID pré-populado
    /// </summary>
    public static readonly Guid Post1 = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");

    /// <summary>
    /// Post ID pré-populado
    /// </summary>
    public static readonly Guid Post2 = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee");

    /// <summary>
    /// Asset ID pré-populado
    /// </summary>
    public static readonly Guid Asset1 = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee");

    /// <summary>
    /// Asset ID pré-populado
    /// </summary>
    public static readonly Guid Asset2 = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff");

    // ============================================
    // IDs DISPONÍVEIS PARA TESTES
    // ============================================
    // ✅ Use estes IDs em testes que criam novas entidades
    // ✅ Eles NÃO conflitam com IDs pré-populados

    /// <summary>
    /// UserId disponível para testes (usado em MembershipServiceTests).
    /// NÃO conflita com IDs pré-populados.
    /// </summary>
    public static readonly Guid TestUserId1 = Guid.Parse("99999999-9999-9999-9999-999999999999");

    /// <summary>
    /// UserId disponível para testes de 2FA.
    /// NÃO conflita com IDs pré-populados.
    /// </summary>
    public static readonly Guid TestUserId2FA = Guid.Parse("88888888-8888-8888-8888-888888888888");

    /// <summary>
    /// UserId disponível para testes adicionais.
    /// NÃO conflita com IDs pré-populados.
    /// </summary>
    public static readonly Guid TestUserId3 = Guid.Parse("77777777-7777-7777-7777-777777777777");
}
