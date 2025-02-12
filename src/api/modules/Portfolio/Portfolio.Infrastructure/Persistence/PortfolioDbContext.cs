using Finbuckle.MultiTenant.Abstractions;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Infrastructure.Persistence;
using FSH.Framework.Infrastructure.Tenant;
using FSH.Starter.WebApi.Portfolio.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FSH.Starter.WebApi.Portfolio.Infrastructure.Persistence;
public sealed class PortfolioDbContext : FshDbContext
{
    public PortfolioDbContext(IMultiTenantContextAccessor<FshTenantInfo> multiTenantContextAccessor,
                          DbContextOptions<PortfolioDbContext> options,
                          IPublisher publisher,
                          IOptions<DatabaseOptions> settings)
        : base(multiTenantContextAccessor, options, publisher, settings)
    {
    }

    public DbSet<Asset> Assets { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PortfolioDbContext).Assembly);
        modelBuilder.HasDefaultSchema("asset");
    }
}
