using System.ComponentModel.DataAnnotations;
using JobApplicationTracker.Domain.Enums;

namespace JobApplicationTracker.Application.JobApplications;

public class UpdateJobApplicationRequest
{
    [Required]
    [MaxLength(150)]
    public string CompanyName { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    public string Position { get; set; } = string.Empty;

    [Required]
    public DateTime AppliedDate { get; set; }

    [Required]
    public ApplicationStatus Status { get; set; }
}
