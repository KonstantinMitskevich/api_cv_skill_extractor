# Testing Instructions

Apply to: `*Tests.cs`, `Tests/*.cs`, `*.Tests/*.cs`

## Test Framework
- Use xUnit as the primary testing framework
- Use Moq or NSubstitute for mocking dependencies
- Use FluentAssertions for readable assertions (optional but recommended)

## Test File Organization
- Name test files: `<ClassUnderTest>Tests.cs`
- Place tests in separate test project: `CV_extractor.Tests`
- Mirror folder structure of main project in test project
- Group related tests in nested classes using `public class <Scenario>`

## Test Method Naming
- Pattern: `MethodName_Scenario_ExpectedBehavior`
- Examples:
  - `ExtractSkillsFromPdf_ValidPdfWithSkills_ReturnsSkillsList`
  - `ExtractSkillsFromPdf_NullFile_Returns400BadRequest`
  - `ExtractSkillsFromPdf_MalformedPdf_Returns500ServerError`
  - `GetSkillsSection_NoSkillsHeader_ReturnsNull`

## Test Structure (Arrange-Act-Assert)
```csharp
[Fact]
public void MethodName_Scenario_ExpectedBehavior()
{
    // Arrange
    var service = new SkillExtractorService();
    var mockFile = CreateMockPdfFile("Skills:\nC#\nPython");
    
    // Act
    var (skills, error, statusCode) = service.ExtractSkillsFromPdf(mockFile);
    
    // Assert
    Assert.Equal(StatusCodes.Status200OK, statusCode);
    Assert.Null(error);
    Assert.Contains("C#", skills);
    Assert.Contains("Python", skills);
}
```

## Mocking IFormFile
```csharp
private static IFormFile CreateMockPdfFile(string content)
{
    var bytes = Encoding.UTF8.GetBytes(content);
    var stream = new MemoryStream(bytes);
    var formFile = new Mock<IFormFile>();
    
    formFile.Setup(f => f.FileName).Returns("test.pdf");
    formFile.Setup(f => f.Length).Returns(stream.Length);
    formFile.Setup(f => f.OpenReadStream()).Returns(stream);
    formFile.Setup(f => f.ContentType).Returns("application/pdf");
    
    return formFile.Object;
}
```

## Test Categories
### Unit Tests
- Test individual methods in isolation
- Mock all external dependencies
- Fast execution (< 100ms per test)
- No file I/O or network calls

### Integration Tests
- Test endpoint-to-service flow
- Use WebApplicationFactory for API testing
- Test with real PDF files from test fixtures
- Clean up resources after tests

## Test Coverage Requirements
- Test happy path (valid inputs, expected outputs)
- Test edge cases:
  - Null inputs
  - Empty inputs
  - Missing sections
  - Malformed data
  - Large files
  - Special characters in content
- Test error handling:
  - Invalid file types
  - Corrupted PDFs
  - Exception scenarios

## Parameterized Tests
```csharp
[Theory]
[InlineData("test.pdf", true)]
[InlineData("test.PDF", true)]
[InlineData("test.doc", false)]
[InlineData("test.txt", false)]
[InlineData("", false)]
public void IsValidPdfFile_VariousFileNames_ReturnsExpectedResult(
    string fileName, bool expected)
{
    // Arrange
    var file = CreateMockFile(fileName);
    
    // Act
    var result = SkillExtractorService.IsValidPdfFile(file);
    
    // Assert
    Assert.Equal(expected, result);
}
```

## Testing Best Practices
- Each test should test one behavior
- Tests should be independent (no shared state)
- Use descriptive variable names in tests
- Avoid test logic - tests should be straightforward
- Clean up resources (dispose streams, mocks)
- Use test fixtures for common setup
- Avoid magic numbers/strings - use constants

## Test Data Files
- Store sample PDF files in `TestData/` folder
- Mark test files as "Copy to Output Directory: Copy if newer"
- Use minimal, purpose-built test files
- Document what each test file contains

## Assertion Examples
```csharp
// Collections
Assert.Empty(skills);
Assert.NotEmpty(skills);
Assert.Contains("C#", skills);
Assert.DoesNotContain("Java", skills);
Assert.Equal(5, skills.Count);

// Strings
Assert.Null(error);
Assert.NotNull(error);
Assert.Equal("Expected message", error);
Assert.StartsWith("Failed to parse", error);

// Status Codes
Assert.Equal(StatusCodes.Status200OK, statusCode);
Assert.Equal(StatusCodes.Status400BadRequest, statusCode);
Assert.Equal(StatusCodes.Status500InternalServerError, statusCode);
```

## Anti-Patterns to Avoid
- Don't test framework code (LINQ, ASP.NET Core internals)
- Don't use `Assert.True(skills.Count == 5)` - use `Assert.Equal(5, skills.Count)`
- Don't catch exceptions in tests unless testing exception handling
- Don't use `Thread.Sleep` - use proper async/await patterns
- Don't test private methods directly - test through public API
- Don't share state between tests
