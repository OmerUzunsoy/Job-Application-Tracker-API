namespace JobApplicationTracker.Application.JobApplications;

public sealed record NoteDto(
    Guid Id,
    string Content,
    DateTime CreatedAt);
