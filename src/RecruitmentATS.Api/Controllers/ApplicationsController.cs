using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecruitmentATS.Application.DTOs;
using RecruitmentATS.Application.Interfaces;

namespace RecruitmentATS.Api.Controllers;

[ApiController]
[Route("api")]
public class ApplicationsController : ControllerBase
{
    private readonly IApplicationService _applicationService;

    public ApplicationsController(IApplicationService applicationService)
    {
        _applicationService = applicationService;
    }

    [HttpPost("jobs/{jobId}/apply")]
    public async Task<ActionResult<ApplicationDto>> ApplyToJob(Guid jobId, [FromForm] ApplyToJobRequest request)
    {
        try
        {
            var application = await _applicationService.ApplyToJobAsync(jobId, request);
            return CreatedAtAction(nameof(GetApplication), new { id = application.Id }, application);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("applications")]
    [Authorize(Roles = "Admin,Recruiter")]
    public async Task<ActionResult<IEnumerable<ApplicationDto>>> GetApplications()
    {
        var applications = await _applicationService.GetAllApplicationsAsync();
        return Ok(applications);
    }

    [HttpGet("applications/{id}")]
    [Authorize(Roles = "Admin,Recruiter")]
    public async Task<ActionResult<ApplicationDto>> GetApplication(Guid id)
    {
        var application = await _applicationService.GetApplicationByIdAsync(id);
        if (application == null)
            return NotFound();

        return Ok(application);
    }

    [HttpPut("applications/{id}/status")]
    [Authorize(Roles = "Admin,Recruiter")]
    public async Task<ActionResult<ApplicationDto>> UpdateApplicationStatus(Guid id, UpdateApplicationStatusRequest request)
    {
        var application = await _applicationService.UpdateApplicationStatusAsync(id, request);
        if (application == null)
            return NotFound();

        return Ok(application);
    }
}
