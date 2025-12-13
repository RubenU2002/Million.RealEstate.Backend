using FluentValidation;

namespace Million.Application.Properties.Commands;

public class UpdatePropertyPriceValidator : AbstractValidator<UpdatePropertyPriceCommand>
{
    public UpdatePropertyPriceValidator()
    {
        RuleFor(x => x.PropertyId)
            .NotEmpty().WithMessage("PropertyId is required.");

        RuleFor(x => x.NewPrice)
            .GreaterThanOrEqualTo(0).WithMessage("Price must be greater than or equal to zero.");
    }
}
