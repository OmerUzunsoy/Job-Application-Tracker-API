using JobApplicationTracker.Application.Common.Interfaces;
using JobApplicationTracker.Application.JobApplications;
using JobApplicationTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace JobApplicationTracker.Persistence.Services;

public class JobApplicationService(AppDbContext dbContext) : IJobApplicationService
{
    public async Task<IReadOnlyList<JobApplicationDto>> GetAllAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var applications = await QueryUserApplications(userId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        return applications.Select(x => x.ToDto()).ToList();
    }

    public async Task<JobApplicationDto?> GetByIdAsync(Guid userId, Guid jobApplicationId, CancellationToken cancellationToken = default)
    {
        var application = await QueryUserApplications(userId)
            .FirstOrDefaultAsync(x => x.Id == jobApplicationId, cancellationToken);

        return application?.ToDto();
    }

    public async Task<JobApplicationDto> CreateAsync(Guid userId, CreateJobApplicationRequest request, CancellationToken cancellationToken = default)
    {
        var entity = new JobApplication
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CompanyName = request.CompanyName.Trim(),
            Position = request.Position.Trim(),
            Status = request.Status,
            AppliedDate = request.AppliedDate,
            CreatedAt = DateTime.UtcNow
        };

        dbContext.JobApplications.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return entity.ToDto();
    }

    public async Task<JobApplicationDto?> UpdateAsync(Guid userId, Guid jobApplicationId, UpdateJobApplicationRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.JobApplications
            .Include(x => x.Notes)
            .Include(x => x.Interviews)
            .FirstOrDefaultAsync(x => x.Id == jobApplicationId && x.UserId == userId, cancellationToken);

        if (entity is null)
        {
            return null;
        }

        entity.CompanyName = request.CompanyName.Trim();
        entity.Position = request.Position.Trim();
        entity.Status = request.Status;
        entity.AppliedDate = request.AppliedDate;

        await dbContext.SaveChangesAsync(cancellationToken);
        return entity.ToDto();
    }

    public async Task<bool> DeleteAsync(Guid userId, Guid jobApplicationId, CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.JobApplications
            .FirstOrDefaultAsync(x => x.Id == jobApplicationId && x.UserId == userId, cancellationToken);

        if (entity is null)
        {
            return false;
        }

        dbContext.JobApplications.Remove(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<NoteDto?> AddNoteAsync(Guid userId, Guid jobApplicationId, CreateNoteRequest request, CancellationToken cancellationToken = default)
    {
        var exists = await dbContext.JobApplications
            .AnyAsync(x => x.Id == jobApplicationId && x.UserId == userId, cancellationToken);

        if (!exists)
        {
            return null;
        }

        var note = new Note
        {
            Id = Guid.NewGuid(),
            JobApplicationId = jobApplicationId,
            Content = request.Content.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        dbContext.Notes.Add(note);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new NoteDto(note.Id, note.Content, note.CreatedAt);
    }

    public async Task<InterviewDto?> AddInterviewAsync(Guid userId, Guid jobApplicationId, CreateInterviewRequest request, CancellationToken cancellationToken = default)
    {
        var exists = await dbContext.JobApplications
            .AnyAsync(x => x.Id == jobApplicationId && x.UserId == userId, cancellationToken);

        if (!exists)
        {
            return null;
        }

        var interview = new Interview
        {
            Id = Guid.NewGuid(),
            JobApplicationId = jobApplicationId,
            InterviewDate = request.InterviewDate,
            Type = request.Type,
            Result = request.Result?.Trim()
        };

        dbContext.Interviews.Add(interview);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new InterviewDto(interview.Id, interview.InterviewDate, interview.Type, interview.Result);
    }

    private IQueryable<JobApplication> QueryUserApplications(Guid userId)
    {
        return dbContext.JobApplications
            .AsNoTracking()
            .Include(x => x.Notes)
            .Include(x => x.Interviews)
            .Where(x => x.UserId == userId);
    }
}
