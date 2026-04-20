using System.ComponentModel.DataAnnotations;

namespace JobApplicationTracker.Application.JobApplications;

public class CreateNoteRequest
{
    [Required]
    [MaxLength(1000)]
    public string Content { get; set; } = string.Empty;
}
