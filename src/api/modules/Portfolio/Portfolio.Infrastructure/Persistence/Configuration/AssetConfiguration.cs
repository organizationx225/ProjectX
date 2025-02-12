using Finbuckle.MultiTenant;
using FSH.Starter.WebApi.Portfolio.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Portfolio.Infrastructure.Persistence.Configuration;
internal sealed class AssetConfiguration : IEntityTypeConfiguration<Asset>
{
    public void Configure(EntityTypeBuilder<Asset> builder)
    {
        builder.IsMultiTenant();
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Type).HasMaxLength(100);
        builder.Property(x => x.Currency).HasMaxLength(10);
        // Value and LastUpdated use default configurations
    }
}
