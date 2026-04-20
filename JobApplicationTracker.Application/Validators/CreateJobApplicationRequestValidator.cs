using FluentValidation;
using JobApplicationTracker.Application.JobApplications;

namespace JobApplicationTracker.Application.Validators;

public class CreateJobApplicationRequestValidator : AbstractValidator<CreateJobApplicationRequest>
{
    public CreateJobApplicationRequestValidator()
    {
        RuleFor(x => x.CompanyName)
            .NotEmpty()
            .WithMessage("CompanyName is required.")
            .MaximumLength(150);

        RuleFor(x => x.Position)
            .NotEmpty()
            .WithMessage("Position is required.")
            .MaximumLength(150);

        RuleFor(x => x.AppliedDate)
            .NotEmpty()
            .WithMessage("AppliedDate is required.");
    }
}
