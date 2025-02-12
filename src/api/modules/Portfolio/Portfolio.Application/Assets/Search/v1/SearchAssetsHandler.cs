using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSH.Framework.Core.Paging;
using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Portfolio.Domain;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Portfolio.Application.Assets.Get.v1;

namespace Portfolio.Application.Assets.Search.v1;
public sealed class SearchAssetsHandler(
    [FromKeyedServices("portfolio:assets")] IReadRepository<Asset> repository)
    : IRequestHandler<SearchAssetsCommand, PagedList<AssetResponse>>
{

    public async Task<PagedList<AssetResponse>> Handle(SearchAssetsCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var spec = new SearchAssetsSpecs(request);
        var items = await repository.ListAsync(spec, cancellationToken).ConfigureAwait(false) ;
        var totalCount = await repository.CountAsync(spec, cancellationToken).ConfigureAwait(false);

        return new PagedList<AssetResponse>(items, request!.PageNumber, request.PageSize, totalCount);
    }
}

