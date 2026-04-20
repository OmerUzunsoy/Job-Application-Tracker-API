using JobApplicationTracker.Domain.Enums;

namespace JobApplicationTracker.Domain.Entities;

public class Interview
{
    public Guid Id { get; set; }
    public Guid JobApplicationId { get; set; }
    public DateTime InterviewDate { get; set; }
    public InterviewType Type { get; set; }
    public string? Result { get; set; }

    public JobApplication JobApplication { get; set; } = null!;
}
