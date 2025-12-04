using System.Text.RegularExpressions;
using RecruitmentATS.Application.Interfaces;

namespace RecruitmentATS.Infrastructure.Services;

public class AtsMatchingService : IAtsMatchingService
{
    private static readonly HashSet<string> StopWords = new(StringComparer.OrdinalIgnoreCase)
    {
        "a", "an", "the", "and", "or", "but", "if", "then", "else", "when", "at", "by", "for", "with", "about", "against", "between", "into", "through", "during", "before", "after", "above", "below", "to", "from", "up", "down", "in", "out", "on", "off", "over", "under", "again", "further", "then", "once", "here", "there", "when", "where", "why", "how", "all", "any", "both", "each", "few", "more", "most", "other", "some", "such", "no", "nor", "not", "only", "own", "same", "so", "than", "too", "very", "s", "t", "can", "will", "just", "don", "should", "now"
    };

    public Task<(int Score, string Details)> CalculateMatchAsync(string resumeText, string jobDescription, string jobRequirements)
    {
        if (string.IsNullOrWhiteSpace(resumeText))
            return Task.FromResult((0, "No resume text found to analyze."));

        // 1. Extract keywords from Job Description and Requirements
        var jobText = $"{jobDescription} {jobRequirements}";
        var jobKeywords = ExtractKeywords(jobText);

        if (jobKeywords.Count == 0)
            return Task.FromResult((100, "No specific keywords found in job description."));

        // 2. Check for matches in Resume
        var resumeKeywords = ExtractKeywords(resumeText);
        var matchedKeywords = new List<string>();
        var missingKeywords = new List<string>();

        foreach (var keyword in jobKeywords)
        {
            if (resumeKeywords.Contains(keyword))
            {
                matchedKeywords.Add(keyword);
            }
            else
            {
                missingKeywords.Add(keyword);
            }
        }

        // 3. Calculate Score
        double matchPercentage = (double)matchedKeywords.Count / jobKeywords.Count * 100;
        int score = (int)Math.Round(matchPercentage);

        // 4. Generate Details
        var details = $"Matched {matchedKeywords.Count} out of {jobKeywords.Count} keywords.\n\n" +
                      $"✅ Matched: {string.Join(", ", matchedKeywords.Take(10))}{(matchedKeywords.Count > 10 ? "..." : "")}\n" +
                      $"❌ Missing: {string.Join(", ", missingKeywords.Take(10))}{(missingKeywords.Count > 10 ? "..." : "")}";

        return Task.FromResult((score, details));
    }

    private HashSet<string> ExtractKeywords(string text)
    {
        // Remove special characters and split by whitespace
        var words = Regex.Replace(text, "[^a-zA-Z0-9 ]", " ")
            .Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

        var keywords = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var word in words)
        {
            if (word.Length > 2 && !StopWords.Contains(word))
            {
                keywords.Add(word);
            }
        }

        return keywords;
    }
}
