using Ardalis.Specification;
using FSH.Framework.Core.Paging;
using FSH.Framework.Core.Specifications;
using FSH.Starter.WebApi.Portfolio.Domain;
using Microsoft.EntityFrameworkCore;
using Portfolio.Application.Assets.Get.v1;

namespace Portfolio.Application.Assets.Search.v1;
public class SearchAssetsSpecs : EntitiesByPaginationFilterSpec<Asset, AssetResponse>
{
    public SearchAssetsSpecs(SearchAssetsCommand command)
        : base(command) =>
        Query
            .OrderBy(c => c.Type, !command.HasOrderBy());
            //.Where(b => EF.Functions.Like(b.Type, $"%{command.Keyword}%"), !string.IsNullOrEmpty(command.Keyword));
}
