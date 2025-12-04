using RecruitmentATS.Application.DTOs;

namespace RecruitmentATS.Application.Interfaces;

public interface IJobService
{
    Task<IEnumerable<JobDto>> GetAllJobsAsync(string? searchTerm = null, bool? isActive = null);
    Task<JobDto?> GetJobByIdAsync(Guid id);
    Task<JobDto> CreateJobAsync(CreateJobRequest request);
    Task<JobDto?> UpdateJobAsync(Guid id, UpdateJobRequest request);
    Task<bool> DeleteJobAsync(Guid id);
}

public interface IApplicationService
{
    Task<ApplicationDto> ApplyToJobAsync(Guid jobId, ApplyToJobRequest request);
    Task<IEnumerable<ApplicationDto>> GetAllApplicationsAsync();
    Task<ApplicationDto?> GetApplicationByIdAsync(Guid id);
    Task<ApplicationDto?> UpdateApplicationStatusAsync(Guid id, UpdateApplicationStatusRequest request);
}
