using FSH.Framework.Core.Domain.Events;

namespace FSH.Starter.WebApi.Portfolio.Domain.Events;

public sealed record AssetUpdated : DomainEvent
{
    public Asset? Asset { get; set; }
}
