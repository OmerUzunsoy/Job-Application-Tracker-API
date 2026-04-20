using JobApplicationTracker.Application.Common.Interfaces;
using JobApplicationTracker.Application.Common.Models;
using JobApplicationTracker.Application.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobApplicationTracker.API.Controllers;

[ApiController]
[Authorize]
[Route("api/dashboard")]
public class DashboardController(
    IJobApplicationService jobApplicationService,
    ICurrentUserService currentUserService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<DashboardSummaryDto>>> Get(CancellationToken cancellationToken)
    {
        var data = await jobApplicationService.GetDashboardAsync(currentUserService.GetUserId(), cancellationToken);
        return Ok(ApiResponse<DashboardSummaryDto>.Ok(data));
    }
}
