using FluentValidation;

namespace Million.Application.Properties.Commands;

public class CreatePropertyValidator : AbstractValidator<CreatePropertyCommand>
{
    public CreatePropertyValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Property name is required")
            .MaximumLength(200)
            .WithMessage("Property name cannot exceed 200 characters");

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .WithMessage("Description cannot exceed 1000 characters");

        RuleFor(x => x.Address)
            .NotEmpty()
            .WithMessage("Address is required")
            .MaximumLength(500)
            .WithMessage("Address cannot exceed 500 characters");

        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithMessage("Price must be greater than 0")
            .LessThanOrEqualTo(99999999999m)
            .WithMessage("Price cannot exceed 99,999,999,999");

        RuleFor(x => x.Year)
            .GreaterThan(1800)
            .WithMessage("Year must be greater than 1800")
            .LessThanOrEqualTo(DateTime.Now.Year + 5)
            .WithMessage($"Year cannot be more than 5 years in the future");
    }
}
