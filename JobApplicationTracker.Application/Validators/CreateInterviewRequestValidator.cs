using FluentValidation;
using JobApplicationTracker.Application.JobApplications;

namespace JobApplicationTracker.Application.Validators;

public class CreateInterviewRequestValidator : AbstractValidator<CreateInterviewRequest>
{
    public CreateInterviewRequestValidator()
    {
        RuleFor(x => x.InterviewDate)
            .NotEmpty()
            .WithMessage("InterviewDate is required.");

        RuleFor(x => x.Result)
            .MaximumLength(500);
    }
}
