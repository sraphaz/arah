using Araponga.Modules.Marketplace.Domain;
using Araponga.Modules.Marketplace.Infrastructure.Postgres.Entities;
using System.Reflection;

namespace Araponga.Modules.Marketplace.Infrastructure.Postgres;

internal static class MarketplaceMappers
{
    public static TerritoryStoreRecord ToRecord(this Store store)
    {
        return new TerritoryStoreRecord
        {
            Id = store.Id,
            TerritoryId = store.TerritoryId,
            OwnerUserId = store.OwnerUserId,
            DisplayName = store.DisplayName,
            Description = store.Description,
            Status = store.Status,
            PaymentsEnabled = store.PaymentsEnabled,
            ContactVisibility = store.ContactVisibility,
            Phone = store.Phone,
            Whatsapp = store.Whatsapp,
            Email = store.Email,
            Instagram = store.Instagram,
            Website = store.Website,
            PreferredContactMethod = store.PreferredContactMethod,
            CreatedAtUtc = store.CreatedAtUtc,
            UpdatedAtUtc = store.UpdatedAtUtc
        };
    }

    public static Store ToDomain(this TerritoryStoreRecord record)
    {
        return new Store(
            record.Id,
            record.TerritoryId,
            record.OwnerUserId,
            record.DisplayName,
            record.Description,
            record.Status,
            record.PaymentsEnabled,
            record.ContactVisibility,
            record.Phone,
            record.Whatsapp,
            record.Email,
            record.Instagram,
            record.Website,
            record.PreferredContactMethod,
            record.CreatedAtUtc,
            record.UpdatedAtUtc);
    }

    public static StoreItemRecord ToRecord(this StoreItem item)
    {
        return new StoreItemRecord
        {
            Id = item.Id,
            TerritoryId = item.TerritoryId,
            StoreId = item.StoreId,
            Type = item.Type,
            Title = item.Title,
            Description = item.Description,
            Category = item.Category,
            Tags = item.Tags,
            PricingType = item.PricingType,
            PriceAmount = item.PriceAmount,
            Currency = item.Currency,
            Unit = item.Unit,
            Latitude = item.Latitude,
            Longitude = item.Longitude,
            Status = item.Status,
            CreatedAtUtc = item.CreatedAtUtc,
            UpdatedAtUtc = item.UpdatedAtUtc
        };
    }

    public static StoreItem ToDomain(this StoreItemRecord record)
    {
        return new StoreItem(
            record.Id,
            record.TerritoryId,
            record.StoreId,
            record.Type,
            record.Title,
            record.Description,
            record.Category,
            record.Tags,
            record.PricingType,
            record.PriceAmount,
            record.Currency,
            record.Unit,
            record.Latitude,
            record.Longitude,
            record.Status,
            record.CreatedAtUtc,
            record.UpdatedAtUtc);
    }

    public static ItemInquiryRecord ToRecord(this ItemInquiry inquiry)
    {
        return new ItemInquiryRecord
        {
            Id = inquiry.Id,
            TerritoryId = inquiry.TerritoryId,
            ItemId = inquiry.ItemId,
            StoreId = inquiry.StoreId,
            FromUserId = inquiry.FromUserId,
            Message = inquiry.Message,
            Status = inquiry.Status,
            BatchId = inquiry.BatchId,
            CreatedAtUtc = inquiry.CreatedAtUtc
        };
    }

    public static ItemInquiry ToDomain(this ItemInquiryRecord record)
    {
        return new ItemInquiry(
            record.Id,
            record.TerritoryId,
            record.ItemId,
            record.StoreId,
            record.FromUserId,
            record.Message,
            record.Status,
            record.BatchId,
            record.CreatedAtUtc);
    }

    public static CartRecord ToRecord(this Cart cart)
    {
        return new CartRecord
        {
            Id = cart.Id,
            TerritoryId = cart.TerritoryId,
            UserId = cart.UserId,
            CreatedAtUtc = cart.CreatedAtUtc,
            UpdatedAtUtc = cart.UpdatedAtUtc
        };
    }

    public static Cart ToDomain(this CartRecord record)
    {
        return new Cart(
            record.Id,
            record.TerritoryId,
            record.UserId,
            record.CreatedAtUtc,
            record.UpdatedAtUtc);
    }

    public static CartItemRecord ToRecord(this CartItem item)
    {
        return new CartItemRecord
        {
            Id = item.Id,
            CartId = item.CartId,
            ItemId = item.ItemId,
            Quantity = item.Quantity,
            Notes = item.Notes,
            CreatedAtUtc = item.CreatedAtUtc,
            UpdatedAtUtc = item.UpdatedAtUtc
        };
    }

    public static CartItem ToDomain(this CartItemRecord record)
    {
        return new CartItem(
            record.Id,
            record.CartId,
            record.ItemId,
            record.Quantity,
            record.Notes,
            record.CreatedAtUtc,
            record.UpdatedAtUtc);
    }

    public static CheckoutRecord ToRecord(this Checkout checkout)
    {
        return new CheckoutRecord
        {
            Id = checkout.Id,
            TerritoryId = checkout.TerritoryId,
            BuyerUserId = checkout.BuyerUserId,
            StoreId = checkout.StoreId,
            Status = checkout.Status,
            Currency = checkout.Currency,
            ItemsSubtotalAmount = checkout.ItemsSubtotalAmount,
            PlatformFeeAmount = checkout.PlatformFeeAmount,
            TotalAmount = checkout.TotalAmount,
            CreatedAtUtc = checkout.CreatedAtUtc,
            UpdatedAtUtc = checkout.UpdatedAtUtc
        };
    }

    public static Checkout ToDomain(this CheckoutRecord record)
    {
        return new Checkout(
            record.Id,
            record.TerritoryId,
            record.BuyerUserId,
            record.StoreId,
            record.Status,
            record.Currency,
            record.ItemsSubtotalAmount,
            record.PlatformFeeAmount,
            record.TotalAmount,
            record.CreatedAtUtc,
            record.UpdatedAtUtc);
    }

    public static CheckoutItemRecord ToRecord(this CheckoutItem item)
    {
        return new CheckoutItemRecord
        {
            Id = item.Id,
            CheckoutId = item.CheckoutId,
            ItemId = item.ItemId,
            ItemType = item.ItemType,
            TitleSnapshot = item.TitleSnapshot,
            Quantity = item.Quantity,
            UnitPrice = item.UnitPrice,
            LineSubtotal = item.LineSubtotal,
            PlatformFeeLine = item.PlatformFeeLine,
            LineTotal = item.LineTotal,
            CreatedAtUtc = item.CreatedAtUtc
        };
    }

    public static CheckoutItem ToDomain(this CheckoutItemRecord record)
    {
        return new CheckoutItem(
            record.Id,
            record.CheckoutId,
            record.ItemId,
            record.ItemType,
            record.TitleSnapshot,
            record.Quantity,
            record.UnitPrice,
            record.LineSubtotal,
            record.PlatformFeeLine,
            record.LineTotal,
            record.CreatedAtUtc);
    }

    public static PlatformFeeConfigRecord ToRecord(this PlatformFeeConfig config)
    {
        return new PlatformFeeConfigRecord
        {
            Id = config.Id,
            TerritoryId = config.TerritoryId,
            ItemType = config.ItemType,
            FeeMode = config.FeeMode,
            FeeValue = config.FeeValue,
            Currency = config.Currency,
            IsActive = config.IsActive,
            CreatedAtUtc = config.CreatedAtUtc,
            UpdatedAtUtc = config.UpdatedAtUtc
        };
    }

    public static PlatformFeeConfig ToDomain(this PlatformFeeConfigRecord record)
    {
        return new PlatformFeeConfig(
            record.Id,
            record.TerritoryId,
            record.ItemType,
            record.FeeMode,
            record.FeeValue,
            record.Currency,
            record.IsActive,
            record.CreatedAtUtc,
            record.UpdatedAtUtc);
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

        var isActiveProp = typeof(TerritoryPayoutConfig).GetProperty("IsActive", BindingFlags.Public | BindingFlags.Instance);
        var createdAtProp = typeof(TerritoryPayoutConfig).GetProperty("CreatedAtUtc", BindingFlags.Public | BindingFlags.Instance);
        var updatedAtProp = typeof(TerritoryPayoutConfig).GetProperty("UpdatedAtUtc", BindingFlags.Public | BindingFlags.Instance);

        if (isActiveProp?.SetMethod != null) isActiveProp.SetValue(config, record.IsActive);
        if (createdAtProp?.SetMethod != null) createdAtProp.SetValue(config, record.CreatedAtUtc);
        if (updatedAtProp?.SetMethod != null) updatedAtProp.SetValue(config, record.UpdatedAtUtc);

        return config;
    }

    public static StoreRatingRecord ToRecord(this StoreRating rating)
    {
        return new StoreRatingRecord
        {
            Id = rating.Id,
            StoreId = rating.StoreId,
            UserId = rating.UserId,
            Rating = rating.Rating,
            Comment = rating.Comment,
            CreatedAtUtc = rating.CreatedAtUtc,
            UpdatedAtUtc = rating.UpdatedAtUtc
        };
    }

    public static StoreRating ToDomain(this StoreRatingRecord record)
    {
        return new StoreRating(
            record.Id,
            record.StoreId,
            record.UserId,
            record.Rating,
            record.Comment,
            record.CreatedAtUtc,
            record.UpdatedAtUtc);
    }

    public static StoreItemRatingRecord ToRecord(this StoreItemRating rating)
    {
        return new StoreItemRatingRecord
        {
            Id = rating.Id,
            StoreItemId = rating.StoreItemId,
            UserId = rating.UserId,
            Rating = rating.Rating,
            Comment = rating.Comment,
            CreatedAtUtc = rating.CreatedAtUtc,
            UpdatedAtUtc = rating.UpdatedAtUtc
        };
    }

    public static StoreItemRating ToDomain(this StoreItemRatingRecord record)
    {
        return new StoreItemRating(
            record.Id,
            record.StoreItemId,
            record.UserId,
            record.Rating,
            record.Comment,
            record.CreatedAtUtc,
            record.UpdatedAtUtc);
    }

    public static StoreRatingResponseRecord ToRecord(this StoreRatingResponse response)
    {
        return new StoreRatingResponseRecord
        {
            Id = response.Id,
            RatingId = response.RatingId,
            StoreId = response.StoreId,
            ResponseText = response.ResponseText,
            CreatedAtUtc = response.CreatedAtUtc
        };
    }

    public static StoreRatingResponse ToDomain(this StoreRatingResponseRecord record)
    {
        return new StoreRatingResponse(
            record.Id,
            record.RatingId,
            record.StoreId,
            record.ResponseText,
            record.CreatedAtUtc);
    }
}
