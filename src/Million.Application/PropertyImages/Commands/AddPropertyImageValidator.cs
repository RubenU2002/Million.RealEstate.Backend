using FluentValidation;

namespace Million.Application.PropertyImages.Commands;

public class AddPropertyImageValidator : AbstractValidator<AddPropertyImageCommand>
{
    public AddPropertyImageValidator()
    {
        RuleFor(x => x.PropertyId)
            .NotEmpty()
            .WithMessage("Property ID is required");

        RuleFor(x => x.File)
            .NotEmpty()
            .WithMessage("File path is required")
            .MaximumLength(500)
            .WithMessage("File path cannot exceed 500 characters");
    }
}
