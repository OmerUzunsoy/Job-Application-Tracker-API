using FluentValidation;
using JobApplicationTracker.Application.JobApplications;

namespace JobApplicationTracker.Application.Validators;

public class CreateNoteRequestValidator : AbstractValidator<CreateNoteRequest>
{
    public CreateNoteRequestValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty()
            .WithMessage("Content is required.")
            .MaximumLength(1000);
    }
}
