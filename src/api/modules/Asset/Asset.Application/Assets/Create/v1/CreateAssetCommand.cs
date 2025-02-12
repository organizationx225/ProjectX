using System.ComponentModel;
using MediatR;

namespace FSH.Starter.WebApi.Portfolio.Application.Assets.Create.v1;
public sealed record CreateAssetCommand(
    [property: DefaultValue("Cash")] string Type,
    decimal Value,
    [property: DefaultValue("USD")] string Currency) : IRequest<CreateAssetResponse>;