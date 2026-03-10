using Microsoft.AspNetCore.Http;
using NSubstitute;
using System.Text;

namespace CV_extractor.Tests;

public class SkillExtractorServiceTests
{
	[Fact]
	public void ExtractSkillsFromPdf_ValidPdfWithSkills_ReturnsSkillsList()
	{
		// Arrange
		// Use comma and newline as delimiters to match parser expectations
		var pdfContent = "SKILLS:\nC#, Python, JavaScript\n\nEXPERIENCE:\nSoftware Engineer";
		var service = new SkillExtractorService((_) => pdfContent);
		var mockFile = CreateMockPdfFile("test.pdf", pdfContent);

		// Act
		var (skills, error, statusCode) = service.ExtractSkillsFromPdf(mockFile);

		// Assert
		Assert.Null(error);
		Assert.Equal(StatusCodes.Status200OK, statusCode);
		Assert.Equal(3, skills.Count);
		Assert.Contains("C#", skills);
		Assert.Contains("Python", skills);
		Assert.Contains("JavaScript", skills);
	}

	[Fact]
	public void ExtractSkillsFromPdf_NullFile_Returns400BadRequest()
	{
		// Arrange
		var service = new SkillExtractorService((_) => "");

		// Act
		var (skills, error, statusCode) = service.ExtractSkillsFromPdf(null!);

		// Assert
		Assert.Empty(skills);
		Assert.Equal("No file uploaded.", error);
		Assert.Equal(StatusCodes.Status400BadRequest, statusCode);
	}

	[Fact]
	public void ExtractSkillsFromPdf_EmptyFile_Returns400BadRequest()
	{
		// Arrange
		var service = new SkillExtractorService((_) => "");
		var mockFile = CreateMockPdfFile("test.pdf", string.Empty, 0);

		// Act
		var (skills, error, statusCode) = service.ExtractSkillsFromPdf(mockFile);

		// Assert
		Assert.Empty(skills);
		Assert.Equal("No file uploaded.", error);
		Assert.Equal(StatusCodes.Status400BadRequest, statusCode);
	}

	[Theory]
	[InlineData("test.doc")]
	[InlineData("test.txt")]
	[InlineData("test.docx")]
	[InlineData("test")]
	public void ExtractSkillsFromPdf_NonPdfFile_Returns400BadRequest(string fileName)
	{
		// Arrange
		var service = new SkillExtractorService((_) => "");
		var mockFile = CreateMockPdfFile(fileName, "content");

		// Act
		var (skills, error, statusCode) = service.ExtractSkillsFromPdf(mockFile);

		// Assert
		Assert.Empty(skills);
		Assert.Equal("Only PDF files are supported.", error);
		Assert.Equal(StatusCodes.Status400BadRequest, statusCode);
	}

	[Fact]
	public void ExtractSkillsFromPdf_NoSkillsSection_ReturnsEmptyList()
	{
		// Arrange
		var pdfContent = "EXPERIENCE:\nSoftware Engineer\n\nEDUCATION:\nBachelor's Degree";
		var service = new SkillExtractorService((_) => pdfContent);
		var mockFile = CreateMockPdfFile("test.pdf", pdfContent);

		// Act
		var (skills, error, statusCode) = service.ExtractSkillsFromPdf(mockFile);

		// Assert
		Assert.Empty(skills);
		Assert.Equal("Skills section not found.", error);
		Assert.Equal(StatusCodes.Status200OK, statusCode);
	}

	[Fact]
	public void ExtractSkillsFromPdf_SkillsWithVariousSeparators_ParsesCorrectly()
	{
		// Arrange
		var pdfContent = "SKILLS:\nC#, Python; JavaScript\n• Java\n- Ruby\n\nEXPERIENCE:";
		var service = new SkillExtractorService((_) => pdfContent);
		var mockFile = CreateMockPdfFile("test.pdf", pdfContent);

		// Act
		var (skills, error, statusCode) = service.ExtractSkillsFromPdf(mockFile);

		// Assert
		Assert.Null(error);
		Assert.Equal(5, skills.Count);
		Assert.Contains("C#", skills);
		Assert.Contains("Python", skills);
		Assert.Contains("JavaScript", skills);
		Assert.Contains("Java", skills);
		Assert.Contains("Ruby", skills);
	}

	[Fact]
	public void ExtractSkillsFromPdf_DuplicateSkills_ReturnsUniqueSkills()
	{
		// Arrange
		// Use comma and newline as delimiters to match parser expectations
		var pdfContent = "SKILLS:\nC#, c#, C#, Python, python\n\nEXPERIENCE:";
		var service = new SkillExtractorService((_) => pdfContent);
		var mockFile = CreateMockPdfFile("test.pdf", pdfContent);

		// Act
		var (skills, error, statusCode) = service.ExtractSkillsFromPdf(mockFile);

		// Assert
		Assert.Null(error);
		Assert.Equal(2, skills.Count);
		Assert.Contains("C#", skills, StringComparer.OrdinalIgnoreCase);
		Assert.Contains("Python", skills, StringComparer.OrdinalIgnoreCase);
	}

	// Helper method to create mock IFormFile
	private static IFormFile CreateMockPdfFile(string fileName, string content, long? length = null)
	{
		var bytes = Encoding.UTF8.GetBytes(content);
		var stream = new MemoryStream(bytes);
		var mockFile = Substitute.For<IFormFile>();
		mockFile.FileName.Returns(fileName);
		mockFile.Length.Returns(length ?? stream.Length);
		mockFile.OpenReadStream().Returns(stream);
		mockFile.ContentType.Returns("application/pdf");
		return mockFile;
	}
}
