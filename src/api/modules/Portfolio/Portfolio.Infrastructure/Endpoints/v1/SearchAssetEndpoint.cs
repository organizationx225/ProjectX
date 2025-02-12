using FSH.Framework.Core.Paging;
using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.Portfolio.Application.Assets.Create.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Portfolio.Application.Assets.Get.v1;
using Portfolio.Application.Assets.Search.v1;

namespace Portfolio.Infrastructure.Endpoints.v1;

public static class SearchAssetEndpoint
{
    internal static RouteHandlerBuilder MapGetAssetListEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPost("/search", async (SearchAssetsCommand request, ISender mediator) =>
            {
                var response = await mediator.Send(request);
                return Results.Ok(response);
            })
            .WithName(nameof(SearchAssetEndpoint))
            .WithSummary("Get a list of assets")
            .WithDescription("Get a list of assets with pagination and filtering support")
            .Produces<PagedList<AssetResponse>>()
            .RequirePermission("Permissions.Assets.View")
            .MapToApiVersion(1);
    }

}
