using FluentValidation;
using FreelancerPortfolio.DTOs;

namespace FreelancerPortfolio.Validators;

public class CreateTechnologyValidator : AbstractValidator<CreateTechnologyDto>
{
    public CreateTechnologyValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Technology name is required.")
            .MaximumLength(100);

        RuleFor(x => x.Slug)
            .NotEmpty().WithMessage("Slug is required.")
            .MaximumLength(120)
            .Matches("^[a-z0-9-]+$").WithMessage("Slug may only contain lowercase letters, numbers, and hyphens.");

        RuleFor(x => x.IconUrl)
            .MaximumLength(500).When(x => x.IconUrl is not null);
    }
}

public class UpdateTechnologyValidator : AbstractValidator<UpdateTechnologyDto>
{
    public UpdateTechnologyValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Technology name is required.")
            .MaximumLength(100);

        RuleFor(x => x.Slug)
            .NotEmpty().WithMessage("Slug is required.")
            .MaximumLength(120)
            .Matches("^[a-z0-9-]+$").WithMessage("Slug may only contain lowercase letters, numbers, and hyphens.");

        RuleFor(x => x.IconUrl)
            .MaximumLength(500).When(x => x.IconUrl is not null);
    }
}
