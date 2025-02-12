using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSH.Framework.Core.Paging;
using MediatR;
using Portfolio.Application.Assets.Get.v1;

namespace Portfolio.Application.Assets.Search.v1;
public class SearchAssetsCommand : PaginationFilter, IRequest<PagedList<AssetResponse>>
{
    public string? AssetType { get; set; }
}
