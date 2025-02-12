namespace Portfolio.Application.Assets.Get.v1;
public sealed record AssetResponse(Guid? Id, string Type, Decimal? Value, string? Currency);


