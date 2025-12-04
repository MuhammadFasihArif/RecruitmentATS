using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecruitmentATS.Application.DTOs;
using RecruitmentATS.Application.Interfaces;

namespace RecruitmentATS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobsController : ControllerBase
{
    private readonly IJobService _jobService;

    public JobsController(IJobService jobService)
    {
        _jobService = jobService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<JobDto>>> GetJobs([FromQuery] string? search, [FromQuery] bool? isActive)
    {
        var jobs = await _jobService.GetAllJobsAsync(search, isActive);
        return Ok(jobs);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<JobDto>> GetJob(Guid id)
    {
        var job = await _jobService.GetJobByIdAsync(id);
        if (job == null)
            return NotFound();

        return Ok(job);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Recruiter")]
    public async Task<ActionResult<JobDto>> CreateJob(CreateJobRequest request)
    {
        var job = await _jobService.CreateJobAsync(request);
        return CreatedAtAction(nameof(GetJob), new { id = job.Id }, job);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Recruiter")]
    public async Task<ActionResult<JobDto>> UpdateJob(Guid id, UpdateJobRequest request)
    {
        var job = await _jobService.UpdateJobAsync(id, request);
        if (job == null)
            return NotFound();

        return Ok(job);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteJob(Guid id)
    {
        var result = await _jobService.DeleteJobAsync(id);
        if (!result)
            return NotFound();

        return NoContent();
    }
}
