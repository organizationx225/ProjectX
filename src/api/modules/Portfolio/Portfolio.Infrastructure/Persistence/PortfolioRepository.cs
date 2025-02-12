using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using FSH.Framework.Core.Domain.Contracts;
using FSH.Framework.Core.Persistence;
using Mapster;

namespace FSH.Starter.WebApi.Portfolio.Infrastructure.Persistence;
internal sealed class PortfolioRepository<T> : RepositoryBase<T>, IReadRepository<T>, IRepository<T>
    where T : class, IAggregateRoot
{
    public PortfolioRepository(PortfolioDbContext context)
        : base(context)
    {
    }

    // Override for mapping to a DTO using Mapster's ProjectToType.
    protected override IQueryable<TResult> ApplySpecification<TResult>(ISpecification<T, TResult> specification) =>
        specification.Selector is not null
            ? base.ApplySpecification(specification)
            : ApplySpecification(specification, false)
                .ProjectToType<TResult>();
}
