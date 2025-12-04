using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RecruitmentATS.Application.DTOs;
using RecruitmentATS.Application.Interfaces;
using RecruitmentATS.Domain.Entities;
using RecruitmentATS.Infrastructure.Persistence;

namespace RecruitmentATS.Infrastructure.Services;

public class JobService : IJobService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public JobService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<JobDto>> GetAllJobsAsync(string? searchTerm = null, bool? isActive = null)
    {
        var query = _context.Jobs.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(j => j.Title.Contains(searchTerm) || j.Location.Contains(searchTerm));
        }

        if (isActive.HasValue)
        {
            query = query.Where(j => j.IsActive == isActive.Value);
        }

        var jobs = await query.OrderByDescending(j => j.CreatedAt).ToListAsync();
        return _mapper.Map<IEnumerable<JobDto>>(jobs);
    }

    public async Task<JobDto?> GetJobByIdAsync(Guid id)
    {
        var job = await _context.Jobs.FindAsync(id);
        return job == null ? null : _mapper.Map<JobDto>(job);
    }

    public async Task<JobDto> CreateJobAsync(CreateJobRequest request)
    {
        var job = _mapper.Map<Job>(request);
        job.Id = Guid.NewGuid();
        job.CreatedAt = DateTime.UtcNow;
        job.IsActive = true;

        _context.Jobs.Add(job);
        await _context.SaveChangesAsync();

        return _mapper.Map<JobDto>(job);
    }

    public async Task<JobDto?> UpdateJobAsync(Guid id, UpdateJobRequest request)
    {
        var job = await _context.Jobs.FindAsync(id);
        if (job == null) return null;

        _mapper.Map(request, job);
        await _context.SaveChangesAsync();

        return _mapper.Map<JobDto>(job);
    }

    public async Task<bool> DeleteJobAsync(Guid id)
    {
        var job = await _context.Jobs.FindAsync(id);
        if (job == null) return false;

        _context.Jobs.Remove(job);
        await _context.SaveChangesAsync();
        return true;
    }
}
