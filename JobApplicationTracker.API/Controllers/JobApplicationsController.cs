using JobApplicationTracker.Application.Common.Interfaces;
using JobApplicationTracker.Application.Common.Models;
using JobApplicationTracker.Application.JobApplications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobApplicationTracker.API.Controllers;

[ApiController]
[Authorize]
[Route("api/jobs")]
public class JobApplicationsController(
    IJobApplicationService jobApplicationService,
    ICurrentUserService currentUserService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<JobApplicationDto>>>> GetAll(
        [FromQuery] GetJobApplicationsQuery query,
        CancellationToken cancellationToken)
    {
        var items = await jobApplicationService.GetAllAsync(currentUserService.GetUserId(), query, cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<JobApplicationDto>>.Ok(items));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<JobApplicationDto>>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var item = await jobApplicationService.GetByIdAsync(currentUserService.GetUserId(), id, cancellationToken);
        return Ok(ApiResponse<JobApplicationDto>.Ok(item));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<JobApplicationDto>>> Create(CreateJobApplicationRequest request, CancellationToken cancellationToken)
    {
        var item = await jobApplicationService.CreateAsync(currentUserService.GetUserId(), request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, ApiResponse<JobApplicationDto>.Ok(item));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<JobApplicationDto>>> Update(Guid id, UpdateJobApplicationRequest request, CancellationToken cancellationToken)
    {
        var item = await jobApplicationService.UpdateAsync(currentUserService.GetUserId(), id, request, cancellationToken);
        return Ok(ApiResponse<JobApplicationDto>.Ok(item));
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse<string>>> Delete(Guid id, CancellationToken cancellationToken)
    {
        await jobApplicationService.DeleteAsync(currentUserService.GetUserId(), id, cancellationToken);
        return Ok(ApiResponse<string>.Ok("Job application deleted successfully."));
    }

    [HttpPost("{id:guid}/notes")]
    public async Task<ActionResult<ApiResponse<NoteDto>>> AddNote(Guid id, CreateNoteRequest request, CancellationToken cancellationToken)
    {
        var note = await jobApplicationService.AddNoteAsync(currentUserService.GetUserId(), id, request, cancellationToken);
        return Ok(ApiResponse<NoteDto>.Ok(note));
    }

    [HttpPost("{id:guid}/interviews")]
    public async Task<ActionResult<ApiResponse<InterviewDto>>> AddInterview(Guid id, CreateInterviewRequest request, CancellationToken cancellationToken)
    {
        var interview = await jobApplicationService.AddInterviewAsync(currentUserService.GetUserId(), id, request, cancellationToken);
        return Ok(ApiResponse<InterviewDto>.Ok(interview));
    }
}
