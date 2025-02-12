using FSH.Framework.Core.Domain;
using FSH.Framework.Core.Domain.Contracts;
using FSH.Starter.WebApi.Portfolio.Domain.Events;


namespace FSH.Starter.WebApi.Portfolio.Domain;


public class Asset : AuditableEntity, IAggregateRoot
{
    public string Type { get; private set; } = default!; // Cash, Stocks, Gold, Property, Crypto
    public decimal Value { get; private set; }
    public string Currency { get; private set; } = "USD";
    public DateTime LastUpdated { get; private set; } = DateTime.UtcNow;

    public static Asset Create(string type, decimal value, string currency)
    {
        var asset = new Asset
        {
            Type = type,
            Value = value,
            Currency = currency
        };

        asset.QueueDomainEvent(new AssetCreated() { Asset = asset });

        return asset;
    }

    public Asset Update(string? type, decimal? value, string? currency)
    {
        if (!string.IsNullOrEmpty(type) && !Type.Equals(type, StringComparison.OrdinalIgnoreCase))
            Type = type;
        if (value.HasValue && Value != value.Value)
            Value = value.Value;
        if (!string.IsNullOrEmpty(currency) && !Currency.Equals(currency, StringComparison.OrdinalIgnoreCase))
            Currency = currency;

        LastUpdated = DateTime.UtcNow;
        this.QueueDomainEvent(new AssetUpdated() { Asset = this });

        return this;
    }

    public static Asset Update(Guid id, string type, decimal value, string currency)
    {
        var asset = new Asset
        {
            Id = id,
            Type = type,
            Value = value,
            Currency = currency,
            LastUpdated = DateTime.UtcNow
        };

        asset.QueueDomainEvent(new AssetUpdated() { Asset = asset });

        return asset;
    }
}
