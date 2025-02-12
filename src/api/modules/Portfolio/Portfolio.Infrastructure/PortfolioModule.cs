using Carter;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Infrastructure.Persistence;
using FSH.Starter.WebApi.Portfolio.Domain;
using FSH.Starter.WebApi.Portfolio.Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Portfolio.Infrastructure.Endpoints.v1;
using Portfolio.Infrastructure.Persistence;

namespace FSH.Starter.WebApi.Portfolio.Infrastructure;

public static class PortfolioModule
{

    public class Endpoints : CarterModule
    {
        public Endpoints() : base("portfolio") { }
        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            var assetGroup = app.MapGroup("assets").WithTags("assets");
            assetGroup.MapAssetCreationEndpoint();   
            assetGroup.MapGetAssetListEndpoint();   
        }
    }
    public static WebApplicationBuilder RegisterPortfolioServices(this WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.Services.BindDbContext<PortfolioDbContext>();
        builder.Services.AddScoped<IDbInitializer, PortfolioDbInitializer>();
        builder.Services.AddKeyedScoped<IRepository<Asset>, PortfolioRepository<Asset>>("portfolio:assets");
        builder.Services.AddKeyedScoped<IReadRepository<Asset>, PortfolioRepository<Asset>>("portfolio:assets");
        return builder;
    }
    public static WebApplication UsePortfolioModule(this WebApplication app)
    {
        return app;
    }

}
