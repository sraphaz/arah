using Araponga.Domain.Territories;

namespace Araponga.Domain.Financial;

/// <summary>
/// Registro de conciliação bancária.
/// </summary>
public sealed class ReconciliationRecord
{
    public Guid Id { get; private set; }
    public Guid TerritoryId { get; private set; }
    
    /// <summary>
    /// Data da conciliação.
    /// </summary>
    public DateTime ReconciliationDate { get; private set; }
    
    /// <summary>
    /// Valor esperado em centavos.
    /// </summary>
    public long ExpectedAmountInCents { get; private set; }
    
    /// <summary>
    /// Valor real em centavos.
    /// </summary>
    public long ActualAmountInCents { get; private set; }
    
    /// <summary>
    /// Diferença (real - esperado) em centavos.
    /// </summary>
    public long DifferenceInCents { get; private set; }
    
    /// <summary>
    /// Moeda da conciliação (ex: "BRL", "USD").
    /// </summary>
    public string Currency { get; private set; }
    
    /// <summary>
    /// Status da conciliação.
    /// </summary>
    public ReconciliationStatus Status { get; private set; }
    
    /// <summary>
    /// Observações sobre a conciliação.
    /// </summary>
    public string? Notes { get; private set; }
    
    /// <summary>
    /// ID do usuário que realizou a conciliação.
    /// </summary>
    public Guid? ReconciledByUserId { get; private set; }
    
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime UpdatedAtUtc { get; private set; }
    
    // Navigation property
    public Territory Territory { get; private set; } = null!;
    
    private ReconciliationRecord() 
    {
        // Inicializar propriedades para EF Core
        Currency = string.Empty;
    } // EF Core
    
    public ReconciliationRecord(
        Guid id,
        Guid territoryId,
        DateTime reconciliationDate,
        long expectedAmountInCents,
        long actualAmountInCents,
        string currency,
        Guid? reconciledByUserId = null,
        string? notes = null)
    {
        Id = id;
        TerritoryId = territoryId;
        ReconciliationDate = reconciliationDate;
        ExpectedAmountInCents = expectedAmountInCents;
        ActualAmountInCents = actualAmountInCents;
        Currency = currency;
        ReconciledByUserId = reconciledByUserId;
        Notes = notes;
        
        DifferenceInCents = actualAmountInCents - expectedAmountInCents;
        Status = DifferenceInCents == 0 ? ReconciliationStatus.Reconciled : ReconciliationStatus.Discrepancy;
        
        CreatedAtUtc = DateTime.UtcNow;
        UpdatedAtUtc = DateTime.UtcNow;
    }
    
    public void UpdateActualAmount(long actualAmountInCents)
    {
        ActualAmountInCents = actualAmountInCents;
        DifferenceInCents = actualAmountInCents - ExpectedAmountInCents;
        Status = DifferenceInCents == 0 ? ReconciliationStatus.Reconciled : ReconciliationStatus.Discrepancy;
        UpdatedAtUtc = DateTime.UtcNow;
    }
    
    public void MarkAsReconciled(Guid reconciledByUserId, string? notes = null)
    {
        Status = ReconciliationStatus.Reconciled;
        ReconciledByUserId = reconciledByUserId;
        Notes = notes;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}

/// <summary>
/// Status de uma conciliação.
/// </summary>
public enum ReconciliationStatus
{
    /// <summary>
    /// Conciliação pendente.
    /// </summary>
    Pending = 1,
    
    /// <summary>
    /// Conciliação realizada com sucesso (valores batem).
    /// </summary>
    Reconciled = 2,
    
    /// <summary>
    /// Discrepância encontrada (valores não batem).
    /// </summary>
    Discrepancy = 3
}
