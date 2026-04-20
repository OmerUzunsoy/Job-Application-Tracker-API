using System.ComponentModel.DataAnnotations;
using JobApplicationTracker.Domain.Enums;

namespace JobApplicationTracker.Application.JobApplications;

public class CreateInterviewRequest
{
    [Required]
    public DateTime InterviewDate { get; set; }

    [Required]
    public InterviewType Type { get; set; }

    [MaxLength(500)]
    public string? Result { get; set; }
}
