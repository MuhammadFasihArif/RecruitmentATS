using Microsoft.AspNetCore.Http;

namespace RecruitmentATS.Application.Interfaces;

public interface IFileService
{
    Task<string> SaveFileAsync(IFormFile file, string subDirectory);
    void DeleteFile(string filePath);
}

public interface IResumeParser
{
    Task<string> ParseResumeAsync(Stream stream, string fileName);
}
