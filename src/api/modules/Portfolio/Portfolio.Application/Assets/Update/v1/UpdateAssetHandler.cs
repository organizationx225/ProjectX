using FluentValidation;
using FSH.Framework.Core.Exceptions;
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
        ValidateRequest(request);

        var asset = await GetAssetByIdAsync(request.Id, cancellationToken);
        UpdateAsset(asset, request);

        await repository.UpdateAsync(asset, cancellationToken);
        logger.LogInformation("Asset with id {AssetId} updated.", asset.Id);

        return new UpdateAssetResponse(asset.Id);
    }

    private static void ValidateRequest(UpdateAssetCommand request)
    {
        if (string.IsNullOrWhiteSpace(request.Type))
            throw new ValidationException("Asset type cannot be empty.");
        if (request.Value <= 0)
            throw new ValidationException("Asset value must be greater than zero.");
        if (string.IsNullOrWhiteSpace(request.Currency))
            throw new ValidationException("Currency cannot be empty.");
    }

    private async Task<Asset> GetAssetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var asset = await repository.GetByIdAsync(id, cancellationToken);
        if (asset is null)
            throw new NotFoundException($"Asset with id {id} not found.");
        return asset;
    }

    private void UpdateAsset(Asset asset, UpdateAssetCommand request)
    {
        asset.Update(request.Type, request.Value, request.Currency);
        logger.LogInformation("Updating asset with id {AssetId}. New values - Type: {Type}, Value: {Value}, Currency: {Currency}",
            asset.Id, request.Type, request.Value, request.Currency);
    }

}
