using JobApplicationTracker.Application.JobApplications;
using JobApplicationTracker.Domain.Entities;

namespace JobApplicationTracker.Persistence.Services;

internal static class JobApplicationMappings
{
    public static JobApplicationDto ToDto(this JobApplication entity)
    {
        return new JobApplicationDto(
            entity.Id,
            entity.CompanyName,
            entity.Position,
            entity.Status,
            entity.AppliedDate,
            entity.CreatedAt,
            entity.Notes
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new NoteDto(x.Id, x.Content, x.CreatedAt))
                .ToList(),
            entity.Interviews
                .OrderByDescending(x => x.InterviewDate)
                .Select(x => new InterviewDto(x.Id, x.InterviewDate, x.Type, x.Result))
                .ToList());
    }
}
