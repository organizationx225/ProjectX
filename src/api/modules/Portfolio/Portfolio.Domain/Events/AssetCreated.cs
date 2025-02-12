using FSH.Framework.Core.Domain.Events;

namespace FSH.Starter.WebApi.Portfolio.Domain.Events;


public record AssetCreated : DomainEvent
{
    public Asset? Asset { get; set; }
}
