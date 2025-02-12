
using FSH.Framework.Core.Domain.Events;

namespace FSH.Starter.WebApi.Forecast.Domain.Events;

public sealed record ForecastCreated : DomainEvent
{
    public Forecast Forecast { get; set; } = default!;
}
