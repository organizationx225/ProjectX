using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.Portfolio.Application.Assets.Create.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Portfolio.Infrastructure.Endpoints.v1;

public static class AddAssetEndpoint
{
     internal static RouteHandlerBuilder MapAssetCreationEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPost("/", async (CreateAssetCommand request, ISender mediator) =>
            {
                var response = await mediator.Send(request);
                return Results.Ok(response);
            })
            .WithName(nameof(AddAssetEndpoint))
            .WithSummary("creates a brand")
            .WithDescription("creates a brand")
            .Produces<CreateAssetResponse>()
            .RequirePermission("Permissions.Assets.Create")
            .MapToApiVersion(1);
    }

}
