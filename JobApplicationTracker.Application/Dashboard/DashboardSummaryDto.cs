namespace JobApplicationTracker.Application.Dashboard;

public sealed record DashboardSummaryDto(
    int TotalApplications,
    int Interviews,
    int Offers,
    int Rejected);
