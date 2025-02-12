using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Portfolio.Domain;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Portfolio.Application.Assets.Update.v1;
public sealed class UpdateAssetHandler(
    ILogger<UpdateAssetHandler> logger,
    [FromKeyedServices("portfolio:assets")] IRepository<Asset> repository)
    : IRequestHandler<UpdateAssetCommand, UpdateAssetResponse>
{
    public async Task<UpdateAssetResponse> Handle(UpdateAssetCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var asset = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (asset is null)
            throw new Exception($"Asset with id {request.Id} not found."); // Replace with a specific NotFound exception if available

        asset.Update(request.Type, request.Value, request.Currency);
        await repository.UpdateAsync(asset, cancellationToken);

        logger.LogInformation("Asset with id {AssetId} updated.", asset.Id);
        return new UpdateAssetResponse(asset.Id);
    }
}
