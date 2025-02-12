using MediatR;

namespace FSH.Starter.WebApi.Portfolio.Application.Assets.Update.v1;
public sealed record UpdateAssetCommand(
    Guid Id,
    string? Type,
    decimal? Value,
    string? Currency = null) : IRequest<UpdateAssetResponse>;