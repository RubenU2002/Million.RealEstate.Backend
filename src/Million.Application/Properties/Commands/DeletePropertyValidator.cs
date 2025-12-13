using FluentValidation;

namespace Million.Application.Properties.Commands;

public class DeletePropertyValidator : AbstractValidator<DeletePropertyCommand>
{
    public DeletePropertyValidator()
    {
        RuleFor(x => x.PropertyId)
            .NotEmpty()
            .WithMessage("Property ID is required");
    }
}
