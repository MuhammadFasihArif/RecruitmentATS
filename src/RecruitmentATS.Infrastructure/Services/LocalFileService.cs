using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using RecruitmentATS.Application.Interfaces;

namespace RecruitmentATS.Infrastructure.Services;

public class LocalFileService : IFileService
{
    private readonly IWebHostEnvironment _environment;

    public LocalFileService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<string> SaveFileAsync(IFormFile file, string subDirectory)
    {
        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads", subDirectory);
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return Path.Combine("uploads", subDirectory, uniqueFileName);
    }

    public void DeleteFile(string filePath)
    {
        var fullPath = Path.Combine(Directory.GetCurrentDirectory(), filePath);
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
    }
}
