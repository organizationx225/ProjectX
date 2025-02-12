using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Portfolio.Domain;
using FSH.Starter.WebApi.Portfolio.Domain.Events;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;


namespace FSH.Starter.WebApi.Portfolio.Application.Assets.Create.v1;
public sealed class CreateAssetHandler(
    ILogger<CreateAssetHandler> logger,
    [FromKeyedServices("portfolio:assets")] IRepository<Asset> repository)
    : IRequestHandler<CreateAssetCommand, CreateAssetResponse>
{
    public async Task<CreateAssetResponse> Handle(CreateAssetCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var asset = Asset.Create(request.Type, request.Value, request.Currency);
        await repository.AddAsync(asset, cancellationToken);
        logger.LogInformation("Asset created {AssetId}", asset.Id);
        return new CreateAssetResponse(asset.Id);
    }
}
