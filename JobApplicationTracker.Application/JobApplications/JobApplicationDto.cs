using JobApplicationTracker.Domain.Enums;

namespace JobApplicationTracker.Application.JobApplications;

public sealed record JobApplicationDto(
    Guid Id,
    string CompanyName,
    string Position,
    ApplicationStatus Status,
    DateTime AppliedDate,
    DateTime CreatedAt,
    IReadOnlyList<NoteDto> Notes,
    IReadOnlyList<InterviewDto> Interviews);
