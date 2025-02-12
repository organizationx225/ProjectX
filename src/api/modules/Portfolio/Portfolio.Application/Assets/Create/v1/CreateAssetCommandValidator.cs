using FluentValidation;

namespace FSH.Starter.WebApi.Portfolio.Application.Assets.Create.v1;
public class CreateAssetCommandValidator : AbstractValidator<CreateAssetCommand>
{
    public CreateAssetCommandValidator()
    {
        RuleFor(a => a.Type)
            .NotEmpty()
            .MinimumLength(2)
            .MaximumLength(100);
        
        RuleFor(a => a.Value)
            .GreaterThan(0);
        
        RuleFor(a => a.Currency)
            .NotEmpty()
            .MinimumLength(1)
            .MaximumLength(10);
    }
}