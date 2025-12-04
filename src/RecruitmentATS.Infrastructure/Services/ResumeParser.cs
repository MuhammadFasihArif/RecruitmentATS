using DocumentFormat.OpenXml.Packaging;
using RecruitmentATS.Application.Interfaces;
using UglyToad.PdfPig;

namespace RecruitmentATS.Infrastructure.Services;

public class ResumeParser : IResumeParser
{
    public async Task<string> ParseResumeAsync(Stream stream, string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLower();

        return extension switch
        {
            ".pdf" => ParsePdf(stream),
            ".docx" => ParseDocx(stream),
            _ => throw new NotSupportedException("Unsupported file format")
        };
    }

    private string ParsePdf(Stream stream)
    {
        try
        {
            using var document = PdfDocument.Open(stream);
            var text = string.Empty;
            foreach (var page in document.GetPages())
            {
                text += page.Text + " ";
            }
            return text.Trim();
        }
        catch (Exception ex)
        {
            // Log exception
            return string.Empty;
        }
    }

    private string ParseDocx(Stream stream)
    {
        try
        {
            using var wordDoc = WordprocessingDocument.Open(stream, false);
            var body = wordDoc.MainDocumentPart?.Document.Body;
            return body?.InnerText ?? string.Empty;
        }
        catch (Exception ex)
        {
            // Log exception
            return string.Empty;
        }
    }
}
