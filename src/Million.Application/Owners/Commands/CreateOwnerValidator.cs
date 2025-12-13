using FluentValidation;

namespace Million.Application.Owners.Commands;

public class CreateOwnerValidator : AbstractValidator<CreateOwnerCommand>
{
    public CreateOwnerValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Address is required.")
            .MaximumLength(200).WithMessage("Address must not exceed 200 characters.");

        RuleFor(x => x.Birthday)
            .NotEmpty().WithMessage("Birthday is required.")
            .LessThan(DateTime.Now).WithMessage("Birthday must be in the past.");

        RuleFor(x => x.Photo)
            .MaximumLength(500).WithMessage("Photo URL must not exceed 500 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Photo));
    }
}
