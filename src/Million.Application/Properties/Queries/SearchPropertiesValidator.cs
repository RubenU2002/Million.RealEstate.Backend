using FluentValidation;

namespace Million.Application.Properties.Queries;

public class SearchPropertiesValidator : AbstractValidator<SearchPropertiesQuery>
{
    public SearchPropertiesValidator()
    {
        RuleFor(x => x.MinPrice)
            .GreaterThanOrEqualTo(0).WithMessage("Minimum price must be greater than or equal to zero.")
            .When(x => x.MinPrice.HasValue);

        RuleFor(x => x.MaxPrice)
            .GreaterThanOrEqualTo(0).WithMessage("Maximum price must be greater than or equal to zero.")
            .When(x => x.MaxPrice.HasValue);

        RuleFor(x => x)
            .Must(x => !x.MinPrice.HasValue || !x.MaxPrice.HasValue || x.MinPrice <= x.MaxPrice)
            .WithMessage("Minimum price must be less than or equal to maximum price.")
            .When(x => x.MinPrice.HasValue && x.MaxPrice.HasValue);

        RuleFor(x => x.Name)
            .MaximumLength(100).WithMessage("Name filter must not exceed 100 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Name));

        RuleFor(x => x.Address)
            .MaximumLength(200).WithMessage("Address filter must not exceed 200 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Address));
    }
}
