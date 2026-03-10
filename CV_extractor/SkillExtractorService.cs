namespace CV_extractor;

using System.Text.RegularExpressions;
using UglyToad.PdfPig;
using Microsoft.AspNetCore.Http;

public interface ISkillExtractorService
{
    (List<string> skills, string? error, int statusCode) ExtractSkillsFromPdf(IFormFile file);
}

public class SkillExtractorService : ISkillExtractorService
{
    private readonly Func<IFormFile, string> _extractTextFromPdf;

    public SkillExtractorService(Func<IFormFile, string>? extractTextFromPdf = null)
    {
        _extractTextFromPdf = extractTextFromPdf ?? ExtractTextFromPdf;
    }

    public (List<string> skills, string? error, int statusCode) ExtractSkillsFromPdf(IFormFile file)
    {
        if (!IsValidPdfFile(file))
            return (new List<string>(), GetFileValidationError(file), StatusCodes.Status400BadRequest);

        string extractedText;
        try
        {
            extractedText = _extractTextFromPdf(file);
        }
        catch (Exception ex)
        {
            return (new List<string>(), $"Failed to parse PDF file. {ex.Message}", StatusCodes.Status500InternalServerError);
        }

        var skillsSection = GetSkillsSection(extractedText);

        if (string.IsNullOrWhiteSpace(skillsSection))
            return (new List<string>(), "Skills section not found.", StatusCodes.Status200OK);

        var skills = ParseSkills(skillsSection);
        return (skills, null, StatusCodes.Status200OK);
    }

    private static bool IsValidPdfFile(IFormFile file)
    {
        return file != null && file.Length > 0 &&
               file.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase);
    }

    private static string GetFileValidationError(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return "No file uploaded.";

        return "Only PDF files are supported.";
    }

    private static string ExtractTextFromPdf(IFormFile file)
    {
        using var stream = file.OpenReadStream();
        using var pdf = PdfDocument.Open(stream);

        return string.Join('\n', pdf.GetPages().Select(p => p.Text));
    }

    private static string? GetSkillsSection(string text)
    {
        const string skillsHeaderPattern = @"(?i)skills\s*:?";
        const string sectionHeaderPattern = @"(?m)^[A-Z][A-Za-z\s\-]+:?$";

        var skillsMatch = Regex.Match(text, skillsHeaderPattern);
        if (!skillsMatch.Success)
            return null!;

        var startIndex = skillsMatch.Index + skillsMatch.Length;
        var remainingText = text.Substring(startIndex);

        var nextHeaderMatch = Regex.Match(remainingText, sectionHeaderPattern);
        if (nextHeaderMatch.Success)
            return remainingText.Substring(0, nextHeaderMatch.Index).Trim();

        return remainingText.Trim();
    }

    private static List<string> ParseSkills(string skillsSection)
    {
        var separators = new[] { '\n', '\r', ',', ';', '•', '-' };
        var skills = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var skill in skillsSection.Split(separators, StringSplitOptions.RemoveEmptyEntries))
        {
            var trimmed = skill.Trim();
            if (!string.IsNullOrWhiteSpace(trimmed))
                skills.Add(trimmed);
        }

        return skills.ToList();
    }
}
