using Arah.Domain.Financial;
using Arah.Modules.Marketplace.Domain;
using Arah.Infrastructure.Postgres.Entities;
using System.Text.Json;

namespace Arah.Infrastructure.Postgres;

public static partial class PostgresMappers
{
    // -----------------------
    // Financial
    // -----------------------
    public static FinancialTransactionRecord ToRecord(this FinancialTransaction transaction)
    {
        return new FinancialTransactionRecord
        {
            Id = transaction.Id,
            TerritoryId = transaction.TerritoryId,
            Type = (int)transaction.Type,
            Status = (int)transaction.Status,
            AmountInCents = transaction.AmountInCents,
            Currency = transaction.Currency,
            Description = transaction.Description,
            RelatedEntityId = transaction.RelatedEntityId,
            RelatedEntityType = transaction.RelatedEntityType,
            RelatedTransactionIdsJson = JsonSerializer.Serialize(transaction.RelatedTransactionIds),
            MetadataJson = transaction.Metadata != null ? JsonSerializer.Serialize(transaction.Metadata) : null,
            CreatedAtUtc = transaction.CreatedAtUtc,
            UpdatedAtUtc = transaction.UpdatedAtUtc
        };
    }

    public static FinancialTransaction ToDomain(this FinancialTransactionRecord record)
    {
        var transaction = new FinancialTransaction(
            record.Id,
            record.TerritoryId,
            (TransactionType)record.Type,
            record.AmountInCents,
            record.Currency,
            record.Description,
            record.RelatedEntityId,
            record.RelatedEntityType,
            record.MetadataJson != null ? JsonSerializer.Deserialize<Dictionary<string, string>>(record.MetadataJson) : null);

        // Set status and related transaction IDs using reflection or public setters
        // Since we can't modify private setters, we'll need to use reflection or add a method
        var relatedIds = JsonSerializer.Deserialize<List<Guid>>(record.RelatedTransactionIdsJson) ?? new List<Guid>();
        foreach (var id in relatedIds)
        {
            transaction.AddRelatedTransaction(id);
        }

        // Restore status and timestamps using reflection
        var statusProp = typeof(FinancialTransaction).GetProperty("Status", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        var createdAtProp = typeof(FinancialTransaction).GetProperty("CreatedAtUtc", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        var updatedAtProp = typeof(FinancialTransaction).GetProperty("UpdatedAtUtc", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

        if (statusProp?.SetMethod != null) statusProp.SetValue(transaction, (TransactionStatus)record.Status);
        if (createdAtProp?.SetMethod != null) createdAtProp.SetValue(transaction, record.CreatedAtUtc);
        if (updatedAtProp?.SetMethod != null) updatedAtProp.SetValue(transaction, record.UpdatedAtUtc);

        return transaction;
    }

    public static TransactionStatusHistoryRecord ToRecord(this TransactionStatusHistory history)
    {
        return new TransactionStatusHistoryRecord
        {
            Id = history.Id,
            FinancialTransactionId = history.FinancialTransactionId,
            PreviousStatus = (int)history.PreviousStatus,
            NewStatus = (int)history.NewStatus,
            ChangedByUserId = history.ChangedByUserId,
            Reason = history.Reason,
            ChangedAtUtc = history.ChangedAtUtc
        };
    }

    public static TransactionStatusHistory ToDomain(this TransactionStatusHistoryRecord record)
    {
        return new TransactionStatusHistory(
            record.Id,
            record.FinancialTransactionId,
            (TransactionStatus)record.PreviousStatus,
            (TransactionStatus)record.NewStatus,
            record.ChangedByUserId,
            record.Reason);
    }

    public static SellerBalanceRecord ToRecord(this SellerBalance balance)
    {
        return new SellerBalanceRecord
        {
            Id = balance.Id,
            TerritoryId = balance.TerritoryId,
            SellerUserId = balance.SellerUserId,
            PendingAmountInCents = balance.PendingAmountInCents,
            ReadyForPayoutAmountInCents = balance.ReadyForPayoutAmountInCents,
            PaidAmountInCents = balance.PaidAmountInCents,
            Currency = balance.Currency,
            CreatedAtUtc = balance.CreatedAtUtc,
            UpdatedAtUtc = balance.UpdatedAtUtc
        };
    }

    public static SellerBalance ToDomain(this SellerBalanceRecord record)
    {
        var balance = new SellerBalance(
            record.Id,
            record.TerritoryId,
            record.SellerUserId,
            record.Currency);

        // Restore state using reflection
        var pendingProp = typeof(SellerBalance).GetProperty("PendingAmountInCents", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        var readyProp = typeof(SellerBalance).GetProperty("ReadyForPayoutAmountInCents", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        var paidProp = typeof(SellerBalance).GetProperty("PaidAmountInCents", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        var createdAtProp = typeof(SellerBalance).GetProperty("CreatedAtUtc", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        var updatedAtProp = typeof(SellerBalance).GetProperty("UpdatedAtUtc", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

        if (pendingProp?.SetMethod != null) pendingProp.SetValue(balance, record.PendingAmountInCents);
        if (readyProp?.SetMethod != null) readyProp.SetValue(balance, record.ReadyForPayoutAmountInCents);
        if (paidProp?.SetMethod != null) paidProp.SetValue(balance, record.PaidAmountInCents);
        if (createdAtProp?.SetMethod != null) createdAtProp.SetValue(balance, record.CreatedAtUtc);
        if (updatedAtProp?.SetMethod != null) updatedAtProp.SetValue(balance, record.UpdatedAtUtc);

        return balance;
    }

    public static SellerTransactionRecord ToRecord(this SellerTransaction transaction)
    {
        return new SellerTransactionRecord
        {
            Id = transaction.Id,
            TerritoryId = transaction.TerritoryId,
            StoreId = transaction.StoreId,
            CheckoutId = transaction.CheckoutId,
            SellerUserId = transaction.SellerUserId,
            GrossAmountInCents = transaction.GrossAmountInCents,
            PlatformFeeInCents = transaction.PlatformFeeInCents,
            NetAmountInCents = transaction.NetAmountInCents,
            Currency = transaction.Currency,
            Status = (int)transaction.Status,
            PayoutId = transaction.PayoutId,
            ReadyForPayoutAtUtc = transaction.ReadyForPayoutAtUtc,
            PaidAtUtc = transaction.PaidAtUtc,
            FinancialTransactionId = transaction.FinancialTransactionId,
            CreatedAtUtc = transaction.CreatedAtUtc,
            UpdatedAtUtc = transaction.UpdatedAtUtc
        };
    }

    public static SellerTransaction ToDomain(this SellerTransactionRecord record)
    {
        var transaction = new SellerTransaction(
            record.Id,
            record.TerritoryId,
            record.StoreId,
            record.CheckoutId,
            record.SellerUserId,
            record.GrossAmountInCents,
            record.PlatformFeeInCents,
            record.Currency);

        // Restore state using reflection
        var statusProp = typeof(SellerTransaction).GetProperty("Status", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        var payoutIdProp = typeof(SellerTransaction).GetProperty("PayoutId", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        var readyForPayoutAtProp = typeof(SellerTransaction).GetProperty("ReadyForPayoutAtUtc", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        var paidAtProp = typeof(SellerTransaction).GetProperty("PaidAtUtc", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        var financialTransactionIdProp = typeof(SellerTransaction).GetProperty("FinancialTransactionId", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        var createdAtProp = typeof(SellerTransaction).GetProperty("CreatedAtUtc", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        var updatedAtProp = typeof(SellerTransaction).GetProperty("UpdatedAtUtc", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

        if (statusProp?.SetMethod != null) statusProp.SetValue(transaction, (SellerTransactionStatus)record.Status);
        if (payoutIdProp?.SetMethod != null) payoutIdProp.SetValue(transaction, record.PayoutId);
        if (readyForPayoutAtProp?.SetMethod != null) readyForPayoutAtProp.SetValue(transaction, record.ReadyForPayoutAtUtc);
        if (paidAtProp?.SetMethod != null) paidAtProp.SetValue(transaction, record.PaidAtUtc);
        if (financialTransactionIdProp?.SetMethod != null) financialTransactionIdProp.SetValue(transaction, record.FinancialTransactionId);
        if (createdAtProp?.SetMethod != null) createdAtProp.SetValue(transaction, record.CreatedAtUtc);
        if (updatedAtProp?.SetMethod != null) updatedAtProp.SetValue(transaction, record.UpdatedAtUtc);

        return transaction;
    }

    public static PlatformFinancialBalanceRecord ToRecord(this PlatformFinancialBalance balance)
    {
        return new PlatformFinancialBalanceRecord
        {
            Id = balance.Id,
            TerritoryId = balance.TerritoryId,
            TotalRevenueInCents = balance.TotalRevenueInCents,
            TotalExpensesInCents = balance.TotalExpensesInCents,
            NetBalanceInCents = balance.NetBalanceInCents,
            Currency = balance.Currency,
            CreatedAtUtc = balance.CreatedAtUtc,
            UpdatedAtUtc = balance.UpdatedAtUtc
        };
    }

    public static PlatformFinancialBalance ToDomain(this PlatformFinancialBalanceRecord record)
    {
        var balance = new PlatformFinancialBalance(
            record.Id,
            record.TerritoryId,
            record.Currency);

        // Restore state using reflection
        var revenueProp = typeof(PlatformFinancialBalance).GetProperty("TotalRevenueInCents", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        var expenseProp = typeof(PlatformFinancialBalance).GetProperty("TotalExpensesInCents", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        var netBalanceProp = typeof(PlatformFinancialBalance).GetProperty("NetBalanceInCents", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        var createdAtProp = typeof(PlatformFinancialBalance).GetProperty("CreatedAtUtc", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        var updatedAtProp = typeof(PlatformFinancialBalance).GetProperty("UpdatedAtUtc", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

        if (revenueProp?.SetMethod != null) revenueProp.SetValue(balance, record.TotalRevenueInCents);
        if (expenseProp?.SetMethod != null) expenseProp.SetValue(balance, record.TotalExpensesInCents);
        if (netBalanceProp?.SetMethod != null) netBalanceProp.SetValue(balance, record.NetBalanceInCents);
        if (createdAtProp?.SetMethod != null) createdAtProp.SetValue(balance, record.CreatedAtUtc);
        if (updatedAtProp?.SetMethod != null) updatedAtProp.SetValue(balance, record.UpdatedAtUtc);

        return balance;
    }

    public static PlatformRevenueTransactionRecord ToRecord(this PlatformRevenueTransaction transaction)
    {
        return new PlatformRevenueTransactionRecord
        {
            Id = transaction.Id,
            TerritoryId = transaction.TerritoryId,
            CheckoutId = transaction.CheckoutId,
            FeeAmountInCents = transaction.FeeAmountInCents,
            Currency = transaction.Currency,
            FinancialTransactionId = transaction.FinancialTransactionId,
            CreatedAtUtc = transaction.CreatedAtUtc
        };
    }

    public static PlatformRevenueTransaction ToDomain(this PlatformRevenueTransactionRecord record)
    {
        var transaction = new PlatformRevenueTransaction(
            record.Id,
            record.TerritoryId,
            record.CheckoutId,
            record.FeeAmountInCents,
            record.Currency);

        if (record.FinancialTransactionId.HasValue)
        {
            transaction.SetFinancialTransactionId(record.FinancialTransactionId.Value);
        }

        return transaction;
    }

    public static FeeSplitRuleRecord ToRecord(this FeeSplitRule rule)
    {
        return new FeeSplitRuleRecord
        {
            Id = rule.Id,
            TerritoryId = rule.TerritoryId,
            RevenueType = rule.RevenueType,
            ImplementerSharePercent = rule.ImplementerSharePercent,
            TerritoryFundSharePercent = rule.TerritoryFundSharePercent,
            PlatformSharePercent = rule.PlatformSharePercent,
            EffectiveFromUtc = rule.EffectiveFromUtc,
            SupersededAtUtc = rule.SupersededAtUtc
        };
    }

    public static FeeSplitRule ToDomain(this FeeSplitRuleRecord record)
    {
        var rule = new FeeSplitRule(
            record.Id,
            record.TerritoryId,
            record.RevenueType,
            record.ImplementerSharePercent,
            record.TerritoryFundSharePercent,
            record.PlatformSharePercent,
            record.EffectiveFromUtc);

        if (record.SupersededAtUtc.HasValue)
        {
            var supersededProp = typeof(FeeSplitRule).GetProperty(
                nameof(FeeSplitRule.SupersededAtUtc),
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            supersededProp?.SetValue(rule, record.SupersededAtUtc);
        }

        return rule;
    }

    public static PlatformExpenseTransactionRecord ToRecord(this PlatformExpenseTransaction transaction)
    {
        return new PlatformExpenseTransactionRecord
        {
            Id = transaction.Id,
            TerritoryId = transaction.TerritoryId,
            SellerTransactionId = transaction.SellerTransactionId,
            PayoutAmountInCents = transaction.PayoutAmountInCents,
            Currency = transaction.Currency,
            PayoutId = transaction.PayoutId,
            FinancialTransactionId = transaction.FinancialTransactionId,
            CreatedAtUtc = transaction.CreatedAtUtc
        };
    }

    public static PlatformExpenseTransaction ToDomain(this PlatformExpenseTransactionRecord record)
    {
        var transaction = new PlatformExpenseTransaction(
            record.Id,
            record.TerritoryId,
            record.SellerTransactionId,
            record.PayoutAmountInCents,
            record.Currency,
            record.PayoutId);

        if (record.FinancialTransactionId.HasValue)
        {
            transaction.SetFinancialTransactionId(record.FinancialTransactionId.Value);
        }

        return transaction;
    }

    public static ReconciliationRecordRecord ToRecord(this ReconciliationRecord record)
    {
        return new ReconciliationRecordRecord
        {
            Id = record.Id,
            TerritoryId = record.TerritoryId,
            ReconciliationDate = record.ReconciliationDate,
            ExpectedAmountInCents = record.ExpectedAmountInCents,
            ActualAmountInCents = record.ActualAmountInCents,
            DifferenceInCents = record.DifferenceInCents,
            Currency = record.Currency,
            Status = (int)record.Status,
            Notes = record.Notes,
            ReconciledByUserId = record.ReconciledByUserId,
            CreatedAtUtc = record.CreatedAtUtc,
            UpdatedAtUtc = record.UpdatedAtUtc
        };
    }

    public static ReconciliationRecord ToDomain(this ReconciliationRecordRecord record)
    {
        var reconciliation = new ReconciliationRecord(
            record.Id,
            record.TerritoryId,
            record.ReconciliationDate,
            record.ExpectedAmountInCents,
            record.ActualAmountInCents,
            record.Currency,
            record.ReconciledByUserId,
            record.Notes);

        // Restore status and timestamps using reflection
        var statusProp = typeof(ReconciliationRecord).GetProperty("Status", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        var createdAtProp = typeof(ReconciliationRecord).GetProperty("CreatedAtUtc", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        var updatedAtProp = typeof(ReconciliationRecord).GetProperty("UpdatedAtUtc", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

        if (statusProp?.SetMethod != null) statusProp.SetValue(reconciliation, (ReconciliationStatus)record.Status);
        if (createdAtProp?.SetMethod != null) createdAtProp.SetValue(reconciliation, record.CreatedAtUtc);
        if (updatedAtProp?.SetMethod != null) updatedAtProp.SetValue(reconciliation, record.UpdatedAtUtc);

        return reconciliation;
    }

    public static TerritoryPayoutConfigRecord ToRecord(this TerritoryPayoutConfig config)
    {
        return new TerritoryPayoutConfigRecord
        {
            Id = config.Id,
            TerritoryId = config.TerritoryId,
            RetentionPeriodDays = config.RetentionPeriodDays,
            MinimumPayoutAmountInCents = config.MinimumPayoutAmountInCents,
            MaximumPayoutAmountInCents = config.MaximumPayoutAmountInCents,
            Frequency = (int)config.Frequency,
            AutoPayoutEnabled = config.AutoPayoutEnabled,
            RequiresApproval = config.RequiresApproval,
            Currency = config.Currency,
            IsActive = config.IsActive,
            CreatedAtUtc = config.CreatedAtUtc,
            UpdatedAtUtc = config.UpdatedAtUtc
        };
    }

    public static TerritoryPayoutConfig ToDomain(this TerritoryPayoutConfigRecord record)
    {
        var config = new TerritoryPayoutConfig(
            record.Id,
            record.TerritoryId,
            record.RetentionPeriodDays,
            record.MinimumPayoutAmountInCents,
            record.MaximumPayoutAmountInCents,
            (PayoutFrequency)record.Frequency,
            record.AutoPayoutEnabled,
            record.RequiresApproval,
            record.Currency);

        // Restore state using reflection
        var isActiveProp = typeof(TerritoryPayoutConfig).GetProperty("IsActive", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        var createdAtProp = typeof(TerritoryPayoutConfig).GetProperty("CreatedAtUtc", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        var updatedAtProp = typeof(TerritoryPayoutConfig).GetProperty("UpdatedAtUtc", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

        if (isActiveProp?.SetMethod != null) isActiveProp.SetValue(config, record.IsActive);
        if (createdAtProp?.SetMethod != null) createdAtProp.SetValue(config, record.CreatedAtUtc);
        if (updatedAtProp?.SetMethod != null) updatedAtProp.SetValue(config, record.UpdatedAtUtc);

        return config;
    }
}
