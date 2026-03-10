# CV Skills Extractor (.NET 9 Minimal API)

A modern .NET 9 Minimal API for extracting skills from PDF CV files using PdfPig. Designed for reliability, testability, and extensibility.

## Features
- Upload PDF CVs and extract unique skills
- Robust PDF parsing with UglyToad.PdfPig
- Section-based skill extraction using regex
- Comprehensive error handling and logging
- SOLID service architecture with dependency injection
- Unit tests with xUnit and NSubstitute
- Custom Copilot instructions and prompts for rapid development

## Getting Started

### Prerequisites
- .NET 9 SDK
- Visual Studio 2022+ or VS Code

### Setup
1. Clone the repository:
   ```sh
   git clone https://github.com/KonstantinMitskevich/Skill_extractor.git
   cd Skill_extractor
   ```
2. Restore dependencies:
   ```sh
   dotnet restore
   ```
3. Build the solution:
   ```sh
   dotnet build
   ```
4. Run the API:
   ```sh
   dotnet run --project CV_extractor
   ```

### API Usage

#### Upload CV and Extract Skills
- **Endpoint:** `POST /extract-skills`
- **Request:** Multipart form with PDF file (`IFormFile`)
- **Response:** JSON array of unique skills, error message, and status code

#### Example Request (cURL)
```sh
curl -F "file=@your_cv.pdf" http://localhost:5000/extract-skills
```

#### Response
```json
{
  "skills": ["C#", "Python", "JavaScript"],
  "error": null,
  "statusCode": 200
}
```

## Project Structure

```
Skill_extractor/
??? CV_extractor/                # Main API project
?   ??? Program.cs               # Minimal API endpoints
?   ??? SkillExtractorService.cs # Skills extraction logic
?   ??? appsettings.json         # Configuration
?   ??? ...
??? Skill.Extractor.L0.Tests/    # Unit tests (xUnit)
?   ??? SkillExtractorServiceTests.cs
??? .github/
?   ??? copilot-instructions.md  # Global Copilot instructions
?   ??? copilot-prompts/         # Custom Copilot prompt templates
?   ??? ...
```

## Skills Extraction Logic
- Locates "SKILLS" section using regex: `(?i)skills\s*:?`
- Extracts content until next section header (e.g., EXPERIENCE, EDUCATION)
- Splits by newlines, commas, semicolons, bullets, hyphens
- Returns unique, trimmed skills (case-insensitive)

## Error Handling
- 400 Bad Request for invalid files (null, empty, not PDF)
- 500 Internal Server Error for PDF parsing failures
- 200 OK with empty array if SKILLS section not found
- All exceptions logged (see service code)

## Testing
- Unit tests in `Skill.Extractor.L0.Tests`
- Mocked `IFormFile` for PDF input
- Edge cases: empty PDFs, missing SKILLS, malformed files
- Arrange-Act-Assert pattern
- Run tests:
  ```sh
  dotnet test Skill.Extractor.L0.Tests
  ```

## Custom Copilot Instructions & Prompts
- `.github/copilot-instructions.md`: Project-wide coding standards
- `.github/copilot-prompts/`: Templates for services, endpoints, tests
- Use in Copilot Chat: `#add-service`, `#add-tests`, `#add-endpoint`

## Extending the API
- Add new services using `add-service.prompt`
- Register services in `Program.cs` with DI
- Follow SOLID and SRP for all new code
- Add unit tests for new features

## Contributing
1. Fork the repo and create a feature branch
2. Follow instructions in `.github/copilot-instructions.md`
3. Add/modify Copilot prompts as needed
4. Write unit tests for all new code
5. Submit a pull request

## Learn More
- [GitHub Copilot Documentation](https://docs.github.com/en/copilot)
- [.NET Coding Conventions](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- [PdfPig Library](https://github.com/UglyToad/PdfPig)

---

For questions or feature requests, open an issue or use Copilot Chat with custom prompts!
