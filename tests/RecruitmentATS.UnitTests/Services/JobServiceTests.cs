using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using RecruitmentATS.Application.DTOs;
using RecruitmentATS.Domain.Entities;
using RecruitmentATS.Infrastructure.Persistence;
using RecruitmentATS.Infrastructure.Services;
using AutoMapper;

namespace RecruitmentATS.UnitTests.Services;

public class JobServiceTests
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<IMapper> _mapperMock;
    private readonly JobService _jobService;

    public JobServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _mapperMock = new Mock<IMapper>();
        _jobService = new JobService(_context, _mapperMock.Object);
    }

    [Fact]
    public async Task CreateJobAsync_ShouldAddJobToDatabase()
    {
        // Arrange
        var request = new CreateJobRequest
        {
            Title = "Software Engineer",
            Description = "Develop software",
            Location = "Remote",
            Requirements = "C#",
            SalaryRangeMin = 50000,
            SalaryRangeMax = 100000
        };

        var jobEntity = new Job
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            Location = request.Location,
            Requirements = request.Requirements,
            SalaryRangeMin = request.SalaryRangeMin,
            SalaryRangeMax = request.SalaryRangeMax,
            IsActive = true
        };

        var jobDto = new JobDto { Id = jobEntity.Id, Title = request.Title };

        _mapperMock.Setup(m => m.Map<Job>(request)).Returns(jobEntity);
        _mapperMock.Setup(m => m.Map<JobDto>(jobEntity)).Returns(jobDto);

        // Act
        var result = await _jobService.CreateJobAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be(request.Title);
        
        var savedJob = await _context.Jobs.FirstOrDefaultAsync(j => j.Id == jobEntity.Id);
        savedJob.Should().NotBeNull();
        savedJob!.Title.Should().Be(request.Title);
    }

    [Fact]
    public async Task GetJobByIdAsync_ShouldReturnJob_WhenJobExists()
    {
        // Arrange
        var jobId = Guid.NewGuid();
        var job = new Job { Id = jobId, Title = "Test Job", IsActive = true };
        await _context.Jobs.AddAsync(job);
        await _context.SaveChangesAsync();

        var jobDto = new JobDto { Id = jobId, Title = "Test Job" };
        _mapperMock.Setup(m => m.Map<JobDto>(job)).Returns(jobDto);

        // Act
        var result = await _jobService.GetJobByIdAsync(jobId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(jobId);
    }

    [Fact]
    public async Task GetJobByIdAsync_ShouldReturnNull_WhenJobDoesNotExist()
    {
        // Act
        var result = await _jobService.GetJobByIdAsync(Guid.NewGuid());

        // Assert
        result.Should().BeNull();
    }
}
