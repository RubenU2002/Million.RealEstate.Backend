using FluentValidation;

namespace Million.Application.PropertyTraces.Commands;

public class AddPropertyTraceValidator : AbstractValidator<AddPropertyTraceCommand>
{
    public AddPropertyTraceValidator()
    {
        RuleFor(x => x.PropertyId)
            .NotEmpty()
            .WithMessage("Property ID is required");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Trace name is required")
            .MaximumLength(200)
            .WithMessage("Trace name cannot exceed 200 characters");

        RuleFor(x => x.Value)
            .GreaterThan(0)
            .WithMessage("Value must be greater than zero");

        RuleFor(x => x.Tax)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Tax cannot be negative");

        RuleFor(x => x.DateSale)
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("Sale date cannot be in the future");
    }
}
