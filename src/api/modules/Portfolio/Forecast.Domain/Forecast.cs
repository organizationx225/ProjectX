
using FSH.Framework.Core.Domain;
using FSH.Framework.Core.Domain.Contracts;


namespace FSH.Starter.WebApi.Forecast.Domain.Events;

public class Forecast : AuditableEntity, IAggregateRoot
{
    public string AssetType { get; private set; } = default!; // Stocks, Gold, Crypto
    public decimal PredictedValue { get; private set; }
    public DateTime PredictionDate { get; private set; }
    public decimal ConfidenceLevel { get; private set; } // AI Confidence in %

    public static Forecast Create(string assetType, decimal predictedValue, DateTime predictionDate, decimal confidenceLevel)
    {
        var forecast = new Forecast
        {
            AssetType = assetType,
            PredictedValue = predictedValue,
            PredictionDate = predictionDate,
            ConfidenceLevel = confidenceLevel
        };

        forecast.QueueDomainEvent(new ForecastCreated() { Forecast = forecast });

        return forecast;
    }

    public Forecast Update(string? assetType, decimal? predictedValue, DateTime? predictionDate, decimal? confidenceLevel)
    {
        if (!string.IsNullOrEmpty(assetType) && !AssetType.Equals(assetType, StringComparison.OrdinalIgnoreCase))
            AssetType = assetType;
        if (predictedValue.HasValue && PredictedValue != predictedValue.Value)
            PredictedValue = predictedValue.Value;
        if (predictionDate.HasValue && PredictionDate != predictionDate.Value)
            PredictionDate = predictionDate.Value;
        if (confidenceLevel.HasValue && ConfidenceLevel != confidenceLevel.Value)
            ConfidenceLevel = confidenceLevel.Value;

        this.QueueDomainEvent(new ForecastUpdated() { Forecast = this });

        return this;
    }

    public static Forecast Update(Guid id, string assetType, decimal predictedValue, DateTime predictionDate, decimal confidenceLevel)
    {
        var forecast = new Forecast
        {
            Id = id,
            AssetType = assetType,
            PredictedValue = predictedValue,
            PredictionDate = predictionDate,
            ConfidenceLevel = confidenceLevel
        };

        forecast.QueueDomainEvent(new ForecastUpdated() { Forecast = forecast });

        return forecast;
    }
}
