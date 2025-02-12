using FluentValidation;

namespace FSH.Starter.WebApi.Portfolio.Application.Assets.Update.v1;
public class UpdateAssetCommandValidator : AbstractValidator<UpdateAssetCommand>
{
    public UpdateAssetCommandValidator()
    {
        When(cmd => !string.IsNullOrEmpty(cmd.Type), () =>
        {
            RuleFor(cmd => cmd.Type)
                .MinimumLength(2)
                .MaximumLength(100);
        });

        When(cmd => cmd.Value.HasValue, () =>
        {
            RuleFor(cmd => cmd.Value!.Value)
                .GreaterThan(0);
        });

        When(cmd => !string.IsNullOrEmpty(cmd.Currency), () =>
        {
            RuleFor(cmd => cmd.Currency)
                .MinimumLength(1)
                .MaximumLength(10);
        });
    }
}