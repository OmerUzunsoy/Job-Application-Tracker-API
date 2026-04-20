using JobApplicationTracker.Domain.Enums;

namespace JobApplicationTracker.Application.JobApplications;

public class GetJobApplicationsQuery
{
    public ApplicationStatus? Status { get; set; }
    public string? Search { get; set; }
    public string? SortBy { get; set; }
    public bool Descending { get; set; }
}
