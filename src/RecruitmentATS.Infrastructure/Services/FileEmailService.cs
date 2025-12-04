using Microsoft.AspNetCore.Hosting;
using RecruitmentATS.Application.Interfaces;

namespace RecruitmentATS.Infrastructure.Services;

public class FileEmailService : IEmailService
{
    private readonly IWebHostEnvironment _environment;

    public FileEmailService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var emailDirectory = Path.Combine(_environment.ContentRootPath, "emails");
        if (!Directory.Exists(emailDirectory))
        {
            Directory.CreateDirectory(emailDirectory);
        }

        var fileName = $"email_{DateTime.UtcNow:yyyyMMdd_HHmmss}_{Guid.NewGuid()}.txt";
        var filePath = Path.Combine(emailDirectory, fileName);

        var emailContent = $"To: {to}\nSubject: {subject}\nDate: {DateTime.UtcNow}\n\n{body}";

        await File.WriteAllTextAsync(filePath, emailContent);
    }
}
