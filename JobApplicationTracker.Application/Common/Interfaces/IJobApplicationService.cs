using JobApplicationTracker.Application.JobApplications;

namespace JobApplicationTracker.Application.Common.Interfaces;

public interface IJobApplicationService
{
    Task<IReadOnlyList<JobApplicationDto>> GetAllAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<JobApplicationDto?> GetByIdAsync(Guid userId, Guid jobApplicationId, CancellationToken cancellationToken = default);
    Task<JobApplicationDto> CreateAsync(Guid userId, CreateJobApplicationRequest request, CancellationToken cancellationToken = default);
    Task<JobApplicationDto?> UpdateAsync(Guid userId, Guid jobApplicationId, UpdateJobApplicationRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid userId, Guid jobApplicationId, CancellationToken cancellationToken = default);
    Task<NoteDto?> AddNoteAsync(Guid userId, Guid jobApplicationId, CreateNoteRequest request, CancellationToken cancellationToken = default);
    Task<InterviewDto?> AddInterviewAsync(Guid userId, Guid jobApplicationId, CreateInterviewRequest request, CancellationToken cancellationToken = default);
}
