namespace RecruitmentATS.Application.Interfaces;

public interface IAtsMatchingService
{
    Task<(int Score, string Details)> CalculateMatchAsync(string resumeText, string jobDescription, string jobRequirements);
}
