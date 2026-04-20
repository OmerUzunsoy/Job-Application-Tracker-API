using JobApplicationTracker.Application.Common.Exceptions;
using JobApplicationTracker.Application.Common.Interfaces;
using JobApplicationTracker.Application.Dashboard;
using JobApplicationTracker.Application.JobApplications;
using JobApplicationTracker.Domain.Enums;
using JobApplicationTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace JobApplicationTracker.Persistence.Services;

public class JobApplicationService(AppDbContext dbContext) : IJobApplicationService
{
    public async Task<IReadOnlyList<JobApplicationDto>> GetAllAsync(Guid userId, GetJobApplicationsQuery query, CancellationToken cancellationToken = default)
    {
        var applicationQuery = QueryUserApplications(userId);

        if (query.Status.HasValue)
        {
            applicationQuery = applicationQuery.Where(x => x.Status == query.Status.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim().ToLower();
            applicationQuery = applicationQuery.Where(x =>
                x.CompanyName.ToLower().Contains(search) ||
                x.Position.ToLower().Contains(search));
        }

        applicationQuery = ApplySorting(applicationQuery, query);

        var applications = await applicationQuery
            .ToListAsync(cancellationToken);

        return applications.Select(x => x.ToDto()).ToList();
    }

    public async Task<JobApplicationDto> GetByIdAsync(Guid userId, Guid jobApplicationId, CancellationToken cancellationToken = default)
    {
        var application = await QueryUserApplications(userId)
            .FirstOrDefaultAsync(x => x.Id == jobApplicationId, cancellationToken);

        return application?.ToDto()
            ?? throw new NotFoundException("Job application was not found.");
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

    public async Task<JobApplicationDto> UpdateAsync(Guid userId, Guid jobApplicationId, UpdateJobApplicationRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.JobApplications
            .Include(x => x.Notes)
            .Include(x => x.Interviews)
            .FirstOrDefaultAsync(x => x.Id == jobApplicationId && x.UserId == userId, cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException("Job application was not found.");
        }

        entity.CompanyName = request.CompanyName.Trim();
        entity.Position = request.Position.Trim();
        entity.Status = request.Status;
        entity.AppliedDate = request.AppliedDate;

        await dbContext.SaveChangesAsync(cancellationToken);
        return entity.ToDto();
    }

    public async Task DeleteAsync(Guid userId, Guid jobApplicationId, CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.JobApplications
            .FirstOrDefaultAsync(x => x.Id == jobApplicationId && x.UserId == userId, cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException("Job application was not found.");
        }

        dbContext.JobApplications.Remove(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<NoteDto> AddNoteAsync(Guid userId, Guid jobApplicationId, CreateNoteRequest request, CancellationToken cancellationToken = default)
    {
        var exists = await dbContext.JobApplications
            .AnyAsync(x => x.Id == jobApplicationId && x.UserId == userId, cancellationToken);

        if (!exists)
        {
            throw new NotFoundException("Job application was not found.");
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

    public async Task<InterviewDto> AddInterviewAsync(Guid userId, Guid jobApplicationId, CreateInterviewRequest request, CancellationToken cancellationToken = default)
    {
        var exists = await dbContext.JobApplications
            .AnyAsync(x => x.Id == jobApplicationId && x.UserId == userId, cancellationToken);

        if (!exists)
        {
            throw new NotFoundException("Job application was not found.");
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

    public async Task<DashboardSummaryDto> GetDashboardAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var applications = dbContext.JobApplications
            .AsNoTracking()
            .Where(x => x.UserId == userId);

        return new DashboardSummaryDto(
            await applications.CountAsync(cancellationToken),
            await applications.CountAsync(x => x.Status == ApplicationStatus.Interview, cancellationToken),
            await applications.CountAsync(x => x.Status == ApplicationStatus.Offer, cancellationToken),
            await applications.CountAsync(x => x.Status == ApplicationStatus.Rejected, cancellationToken));
    }

    private IQueryable<JobApplication> QueryUserApplications(Guid userId)
    {
        return dbContext.JobApplications
            .AsNoTracking()
            .Include(x => x.Notes)
            .Include(x => x.Interviews)
            .Where(x => x.UserId == userId);
    }

    private static IQueryable<JobApplication> ApplySorting(IQueryable<JobApplication> query, GetJobApplicationsQuery request)
    {
        var sortBy = request.SortBy?.Trim().ToLowerInvariant();

        return (sortBy, request.Descending) switch
        {
            ("company", true) => query.OrderByDescending(x => x.CompanyName),
            ("company", false) => query.OrderBy(x => x.CompanyName),
            ("status", true) => query.OrderByDescending(x => x.Status),
            ("status", false) => query.OrderBy(x => x.Status),
            ("createdat", true) => query.OrderByDescending(x => x.CreatedAt),
            ("createdat", false) => query.OrderBy(x => x.CreatedAt),
            ("date", false) => query.OrderBy(x => x.AppliedDate),
            _ => query.OrderByDescending(x => x.AppliedDate)
        };
    }
}
