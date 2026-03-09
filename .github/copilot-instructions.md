# Copilot Instructions for CV Skills Extractor

## Project Overview
This is a .NET 9 Minimal API for extracting skills from PDF CV files using PdfPig library.

## General Guidelines
- Follow SOLID principles for all service implementations
- Use dependency injection for services
- Prefer record types for DTOs and response models
- Use PascalCase for public members, camelCase for parameters
- Always include XML documentation comments for public APIs
- Target .NET 9 and C# 13.0 features

## Error Handling
- Return 400 Bad Request for invalid inputs with descriptive messages
- Return 500 Internal Server Error for parsing failures with error details
- Return 200 OK with empty array if SKILLS section is not found (not an error)
- Log all exceptions before returning error responses
- Never expose sensitive stack traces in production responses

## API Design
- Use minimal API endpoint syntax with route handlers
- Apply `.DisableAntiforgery()` to file upload endpoints
- Include Produces/ProducesProblem attributes for OpenAPI documentation
- Validate IFormFile for null, size limits, and PDF file extension
- Use TypedResults for strongly-typed responses

## PDF Processing
- Use PdfPig library for PDF text extraction
- Combine all pages' text with newline separator before parsing
- Handle malformed PDFs gracefully with try-catch
- Use streams efficiently with `using` statements
- Never load entire file into memory unnecessarily

## Skills Extraction Logic
- Locate "SKILLS" section using case-insensitive regex: `(?i)skills\s*:?`
- Extract content until next section header (EXPERIENCE, EDUCATION, etc.)
- Section headers pattern: `(?m)^[A-Z][A-Za-z\s\-]+:?$`
- Split by common delimiters: newlines, bullets (•), commas, semicolons, hyphens
- Return unique skills using HashSet with case-insensitive comparison
- Trim whitespace and filter empty entries
- Return empty list if SKILLS section not found

## CORS Configuration
- Default CORS policy allows http://localhost:3000 for React frontend
- Policy name: "AllowReactApp"
- Allows all methods and headers
- Document any additional origins in comments

## Testing
- Write unit tests for service classes using xUnit
- Mock IFormFile for controller/endpoint tests using MemoryStream
- Test edge cases: empty PDFs, missing sections, malformed content
- Follow Arrange-Act-Assert pattern
- Use descriptive test method names: `MethodName_Scenario_ExpectedBehavior`

## Dependencies
- PdfPig (UglyToad.PdfPig) for PDF text extraction
- Swashbuckle.AspNetCore for Swagger/OpenAPI documentation
- Prefer stable NuGet packages
- Document why each package is used in code comments or README

## Code Style
- Use file-scoped namespaces where possible
- Prefer expression-bodied members for simple methods
- Use tuple returns for methods that need to return multiple values
- Use const for constant string patterns (regex patterns, error messages)
- Keep methods focused and small (Single Responsibility Principle)
