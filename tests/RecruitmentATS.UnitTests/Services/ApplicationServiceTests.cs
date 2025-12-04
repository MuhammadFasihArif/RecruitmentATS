using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using RecruitmentATS.Application.DTOs;
using RecruitmentATS.Application.Interfaces;
using RecruitmentATS.Domain.Entities;
using RecruitmentATS.Infrastructure.Persistence;
using RecruitmentATS.Infrastructure.Services;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using AppEntity = RecruitmentATS.Domain.Entities.Application;

namespace RecruitmentATS.UnitTests.Services;

public class ApplicationServiceTests
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IFileService> _fileServiceMock;
    private readonly Mock<IResumeParser> _resumeParserMock;
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly ApplicationService _applicationService;

    public ApplicationServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _mapperMock = new Mock<IMapper>();
        _fileServiceMock = new Mock<IFileService>();
        _resumeParserMock = new Mock<IResumeParser>();
        _emailServiceMock = new Mock<IEmailService>();

        _applicationService = new ApplicationService(
            _context, 
            _mapperMock.Object, 
            _fileServiceMock.Object, 
            _resumeParserMock.Object,
            _emailServiceMock.Object);
    }

    [Fact]
    public async Task ApplyToJobAsync_ShouldCreateApplication_WhenValidRequest()
    {
        // Arrange
        var jobId = Guid.NewGuid();
        var job = new Job { Id = jobId, Title = "Test Job", IsActive = true };
        await _context.Jobs.AddAsync(job);
        await _context.SaveChangesAsync();

        var request = new ApplyToJobRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            ResumeFile = new FormFile(new MemoryStream(), 0, 0, "resume", "resume.pdf")
        };

        var candidate = new Candidate { Id = Guid.NewGuid(), Email = request.Email };
        var applicationEntity = new AppEntity { Id = Guid.NewGuid(), JobId = jobId, CandidateId = candidate.Id };
        var applicationDto = new ApplicationDto { Id = applicationEntity.Id, Status = RecruitmentATS.Domain.Enums.ApplicationStatus.Applied };

        _mapperMock.Setup(m => m.Map<Candidate>(request)).Returns(candidate);
        _mapperMock.Setup(m => m.Map<ApplicationDto>(It.IsAny<AppEntity>())).Returns(applicationDto);
        _fileServiceMock.Setup(f => f.SaveFileAsync(It.IsAny<IFormFile>(), It.IsAny<string>())).ReturnsAsync("path/to/resume.pdf");
        _resumeParserMock.Setup(p => p.ParseResumeAsync(It.IsAny<Stream>(), It.IsAny<string>())).ReturnsAsync("Parsed resume content");

        // Act
        var result = await _applicationService.ApplyToJobAsync(jobId, request);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(RecruitmentATS.Domain.Enums.ApplicationStatus.Applied);

        var savedApp = await _context.Applications.FirstOrDefaultAsync(a => a.JobId == jobId);
        savedApp.Should().NotBeNull();
        savedApp!.Notes.Should().Contain("Parsed resume content");

        _emailServiceMock.Verify(e => e.SendEmailAsync(request.Email, It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }
}
