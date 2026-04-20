using JobApplicationTracker.Application.Common.Interfaces;
using JobApplicationTracker.Application.JobApplications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobApplicationTracker.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class JobApplicationsController(
    IJobApplicationService jobApplicationService,
    ICurrentUserService currentUserService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<JobApplicationDto>>> GetAll(CancellationToken cancellationToken)
    {
        var items = await jobApplicationService.GetAllAsync(currentUserService.GetUserId(), cancellationToken);
        return Ok(items);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<JobApplicationDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var item = await jobApplicationService.GetByIdAsync(currentUserService.GetUserId(), id, cancellationToken);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<JobApplicationDto>> Create(CreateJobApplicationRequest request, CancellationToken cancellationToken)
    {
        var item = await jobApplicationService.CreateAsync(currentUserService.GetUserId(), request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<JobApplicationDto>> Update(Guid id, UpdateJobApplicationRequest request, CancellationToken cancellationToken)
    {
        var item = await jobApplicationService.UpdateAsync(currentUserService.GetUserId(), id, request, cancellationToken);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await jobApplicationService.DeleteAsync(currentUserService.GetUserId(), id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }

    [HttpPost("{id:guid}/notes")]
    public async Task<ActionResult<NoteDto>> AddNote(Guid id, CreateNoteRequest request, CancellationToken cancellationToken)
    {
        var note = await jobApplicationService.AddNoteAsync(currentUserService.GetUserId(), id, request, cancellationToken);
        return note is null ? NotFound() : Ok(note);
    }

    [HttpPost("{id:guid}/interviews")]
    public async Task<ActionResult<InterviewDto>> AddInterview(Guid id, CreateInterviewRequest request, CancellationToken cancellationToken)
    {
        var interview = await jobApplicationService.AddInterviewAsync(currentUserService.GetUserId(), id, request, cancellationToken);
        return interview is null ? NotFound() : Ok(interview);
    }
}
