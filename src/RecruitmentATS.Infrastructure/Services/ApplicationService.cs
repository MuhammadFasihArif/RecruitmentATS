using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RecruitmentATS.Application.DTOs;
using RecruitmentATS.Application.Interfaces;
using RecruitmentATS.Domain.Entities;
using RecruitmentATS.Infrastructure.Persistence;

namespace RecruitmentATS.Infrastructure.Services;

public class ApplicationService : IApplicationService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IFileService _fileService;
    private readonly IResumeParser _resumeParser;
    private readonly IEmailService _emailService;
    private readonly IAtsMatchingService _atsMatchingService;

    public ApplicationService(ApplicationDbContext context, IMapper mapper, IFileService fileService, IResumeParser resumeParser, IEmailService emailService, IAtsMatchingService atsMatchingService)
    {
        _context = context;
        _mapper = mapper;
        _fileService = fileService;
        _resumeParser = resumeParser;
        _emailService = emailService;
        _atsMatchingService = atsMatchingService;
    }

    public async Task<ApplicationDto> ApplyToJobAsync(Guid jobId, ApplyToJobRequest request)
    {
        // Check if job exists
        var job = await _context.Jobs.FindAsync(jobId);
        if (job == null)
            throw new InvalidOperationException("Job not found");

        // Handle file upload
        string? resumePath = null;
        string resumeText = string.Empty;

        if (request.ResumeFile != null)
        {
            resumePath = await _fileService.SaveFileAsync(request.ResumeFile, "resumes");
            using (var stream = request.ResumeFile.OpenReadStream())
            {
                resumeText = await _resumeParser.ParseResumeAsync(stream, request.ResumeFile.FileName);
            }
        }

        // Calculate ATS Score
        var (score, matchDetails) = await _atsMatchingService.CalculateMatchAsync(resumeText, job.Description, job.Requirements);

        // Validate request
        if (string.IsNullOrWhiteSpace(request.Email))
            throw new ArgumentException("Email is required");

        // Check if candidate already exists by email
        var candidate = await _context.Candidates.FirstOrDefaultAsync(c => c.Email == request.Email);
        
        if (candidate == null)
        {
            // Create new candidate - Manual mapping to ensure data is set
            candidate = new Candidate
            {
                Id = Guid.NewGuid(),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Phone = request.Phone,
                LinkedInUrl = request.LinkedInUrl,
                ResumeUrl = resumePath,
                CreatedAt = DateTime.UtcNow
            };
            _context.Candidates.Add(candidate);
        }
        else
        {
            // Update existing candidate details
            candidate.FirstName = request.FirstName;
            candidate.LastName = request.LastName;
            candidate.Phone = request.Phone;
            candidate.LinkedInUrl = request.LinkedInUrl;

            if (resumePath != null)
            {
                candidate.ResumeUrl = resumePath;
            }
        }

        // Create application
        var application = new Domain.Entities.Application
        {
            Id = Guid.NewGuid(),
            JobId = jobId,
            CandidateId = candidate.Id,
            Status = Domain.Enums.ApplicationStatus.Applied,
            AppliedAt = DateTime.UtcNow,
            Notes = !string.IsNullOrEmpty(resumeText) ? $"[Resume Extracted Text]:\n{resumeText}" : null,
            AtsScore = score,
            MatchDetails = matchDetails
        };

        _context.Applications.Add(application);
        await _context.SaveChangesAsync();

        // Send confirmation email
        await _emailService.SendEmailAsync(
            request.Email, 
            "Application Received", 
            $"Dear {request.FirstName},\n\nThank you for applying to the position. We have received your application.\n\nYour ATS Score: {score}/100\n\nBest regards,\nRecruitment Team");

        // Load navigation properties for DTO mapping
        await _context.Entry(application).Reference(a => a.Job).LoadAsync();
        await _context.Entry(application).Reference(a => a.Candidate).LoadAsync();

        return _mapper.Map<ApplicationDto>(application);
    }

    public async Task<IEnumerable<ApplicationDto>> GetAllApplicationsAsync()
    {
        var applications = await _context.Applications
            .Include(a => a.Job)
            .Include(a => a.Candidate)
            .OrderByDescending(a => a.AppliedAt)
            .ToListAsync();

        return _mapper.Map<IEnumerable<ApplicationDto>>(applications);
    }

    public async Task<ApplicationDto?> GetApplicationByIdAsync(Guid id)
    {
        var application = await _context.Applications
            .Include(a => a.Job)
            .Include(a => a.Candidate)
            .FirstOrDefaultAsync(a => a.Id == id);

        return application == null ? null : _mapper.Map<ApplicationDto>(application);
    }

    public async Task<ApplicationDto?> UpdateApplicationStatusAsync(Guid id, UpdateApplicationStatusRequest request)
    {
        var application = await _context.Applications
            .Include(a => a.Job)
            .Include(a => a.Candidate)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (application == null) return null;

        application.Status = request.Status;
        application.Notes = request.Notes;

        await _context.SaveChangesAsync();

        return _mapper.Map<ApplicationDto>(application);
    }
}
