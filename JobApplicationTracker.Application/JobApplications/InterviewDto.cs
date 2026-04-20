using JobApplicationTracker.Domain.Enums;

namespace JobApplicationTracker.Application.JobApplications;

public sealed record InterviewDto(
    Guid Id,
    DateTime InterviewDate,
    InterviewType Type,
    string? Result);
