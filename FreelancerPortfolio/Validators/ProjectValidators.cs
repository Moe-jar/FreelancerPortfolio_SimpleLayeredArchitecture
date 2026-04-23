using FluentValidation;
using FreelancerPortfolio.DTOs;

namespace FreelancerPortfolio.Validators;

public class CreateProjectValidator : AbstractValidator<CreateProjectDto>
{
    public CreateProjectValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Project title is required.")
            .MaximumLength(200);

        RuleFor(x => x.Slug)
            .NotEmpty().WithMessage("Slug is required.")
            .MaximumLength(220)
            .Matches("^[a-z0-9-]+$").WithMessage("Slug may only contain lowercase letters, numbers, and hyphens.");

        RuleFor(x => x.ShortDescription).MaximumLength(500).When(x => x.ShortDescription is not null);
        RuleFor(x => x.ThumbnailUrl).MaximumLength(500).When(x => x.ThumbnailUrl is not null);
        RuleFor(x => x.LiveUrl).MaximumLength(500).When(x => x.LiveUrl is not null);
        RuleFor(x => x.RepoUrl).MaximumLength(500).When(x => x.RepoUrl is not null);

        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage("A valid category is required.");

        RuleFor(x => x.TechnologyIds)
            .NotNull().WithMessage("Technology IDs cannot be null.");
    }
}

public class UpdateProjectValidator : AbstractValidator<UpdateProjectDto>
{
    public UpdateProjectValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Project title is required.")
            .MaximumLength(200);

        RuleFor(x => x.Slug)
            .NotEmpty().WithMessage("Slug is required.")
            .MaximumLength(220)
            .Matches("^[a-z0-9-]+$").WithMessage("Slug may only contain lowercase letters, numbers, and hyphens.");

        RuleFor(x => x.ShortDescription).MaximumLength(500).When(x => x.ShortDescription is not null);
        RuleFor(x => x.ThumbnailUrl).MaximumLength(500).When(x => x.ThumbnailUrl is not null);
        RuleFor(x => x.LiveUrl).MaximumLength(500).When(x => x.LiveUrl is not null);
        RuleFor(x => x.RepoUrl).MaximumLength(500).When(x => x.RepoUrl is not null);

        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage("A valid category is required.");

        RuleFor(x => x.TechnologyIds)
            .NotNull().WithMessage("Technology IDs cannot be null.");
    }
}
