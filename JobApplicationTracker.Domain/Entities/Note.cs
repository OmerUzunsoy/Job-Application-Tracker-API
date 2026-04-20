namespace JobApplicationTracker.Domain.Entities;

public class Note
{
    public Guid Id { get; set; }
    public Guid JobApplicationId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public JobApplication JobApplication { get; set; } = null!;
}
