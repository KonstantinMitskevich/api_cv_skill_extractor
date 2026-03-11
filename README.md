# CV Skills Extractor

A **.NET 9 Minimal API** that extracts skills from the `SKILLS` section of uploaded PDF CV files using the [PdfPig](https://github.com/UglyToad/PdfPig) library.

---

## Table of Contents

- [Overview](#overview)
- [Project Structure](#project-structure)
- [Prerequisites](#prerequisites)
- [Getting Started](#getting-started)
- [API Reference](#api-reference)
- [Skills Extraction Logic](#skills-extraction-logic)
- [CORS Configuration](#cors-configuration)
- [Dependencies](#dependencies)
- [Testing](#testing)
- [Best Practices](#best-practices)
- [Extending the API](#extending-the-api)
- [Contributing](#contributing)

---

## Overview

The CV Skills Extractor API accepts a PDF CV file uploaded via a multipart form and returns a deduplicated list of skills found under the `SKILLS` section of that document. It is designed to integrate with a React frontend running on `http://localhost:3000`.

---

## Project Structure

```
Skill_extractor/
??? .github/
?   ??? copilot-instructions.md          # GitHub Copilot workspace-wide coding standards
?   ??? copilot-instructions/
?   ?   ??? Configuration.md             # Configuration guidelines
?   ?   ??? Endpoints.md                 # Endpoint design guidelines
?   ?   ??? Services.md                  # Service layer guidelines
?   ?   ??? Testing.md                   # Testing guidelines
?   ??? copilot-prompts/
?       ??? add-configuration.prompt     # Prompt template for adding configuration
?       ??? add-endpoint.prompt          # Prompt template for adding endpoints
?       ??? add-logging.prompt           # Prompt template for adding logging
?       ??? add-service.prompt           # Prompt template for adding services
?       ??? add-test.prompt              # Prompt template for adding tests
??? CV_extractor/                        # Main API project (.NET 9 Minimal API)
?   ??? Properties/
?   ?   ??? launchSettings.json          # Launch profiles (HTTP / HTTPS / OpenAPI)
?   ??? appsettings.json                 # Application configuration
?   ??? appsettings.Development.json     # Development-specific overrides
?   ??? CV_extractor.csproj              # Project file
?   ??? Program.cs                       # Entry point – DI registration, middleware, endpoints
?   ??? SkillExtractorService.cs         # PDF parsing and skills extraction logic
??? Skill.Extractor.L0.Tests/            # Unit test project (xUnit + NSubstitute)
?   ??? SkillExtractorServiceTests.cs    # Unit tests for SkillExtractorService
?   ??? Skill.Extractor.L0.Tests.csproj # Test project file
??? README.md
```

---

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- Visual Studio 2022+ or VS Code with the C# Dev Kit extension

---

## Getting Started

### 1. Clone the repository

```sh
git clone https://github.com/KonstantinMitskevich/Skill_extractor.git
cd Skill_extractor
```

### 2. Restore dependencies

```sh
dotnet restore
```

### 3. Build the solution

```sh
dotnet build
```

### 4. Run the API

```sh
dotnet run --project CV_extractor
```

The API will start on the ports defined in `CV_extractor/Properties/launchSettings.json`.

### 5. Open Swagger UI

Navigate to `https://localhost:<port>/swagger` in your browser to explore and test the API interactively.

---

## API Reference

### `POST /extract-skills`

Extracts skills from the `SKILLS` section of an uploaded PDF CV.

**Request**

| Parameter | Type        | Required | Description                                        |
|-----------|-------------|----------|----------------------------------------------------|
| `file`    | `IFormFile` | ? Yes   | A PDF file (`.pdf` extension, non-empty content)   |

**Content-Type:** `multipart/form-data`

**Responses**

| Status | Description                                    | Body Example |
|--------|------------------------------------------------|--------------|
| `200`  | Skills extracted successfully                  | `{ "fileName": "cv.pdf", "skills": ["C#", "Python"] }` |
| `200`  | `SKILLS` section not found in the PDF          | `{ "fileName": "cv.pdf", "skills": [], "message": "Skills section not found." }` |
| `400`  | Invalid input (null, empty, or non-PDF file)   | `{ "error": "Only PDF files are supported." }` |
| `500`  | Failed to parse the PDF                        | `{ "error": "Failed to parse PDF file. <details>" }` |

**Example using `curl`**

```sh
curl -X POST https://localhost:<port>/extract-skills \
  -F "file=@/path/to/your/cv.pdf"
```

---

## Skills Extraction Logic

1. **Validation** – The uploaded file is checked for: non-null, non-empty, `.pdf` extension.
2. **Text extraction** – All pages are read using PdfPig and joined with newline separators.
3. **Section location** – The `SKILLS` header is found with the regex `(?i)skills\s*:?`.
4. **Section boundary** – Content is captured until the next section header matching `(?m)^[A-Z][A-Za-z\s\-]+:?$` (e.g., `EXPERIENCE`, `EDUCATION`).
5. **Parsing** – Skills are split by: newlines, commas (`,`), semicolons (`;`), bullets (`•`), and hyphens (`-`).
6. **Deduplication** – A case-insensitive `HashSet` ensures each skill appears only once.
7. **Result** – Trimmed, non-empty skill strings are returned as a flat list.

---

## CORS Configuration

The API is pre-configured to allow requests from a React frontend at `http://localhost:3000`.

| Setting | Value                   |
|---------|-------------------------|
| Policy  | `AllowReactApp`         |
| Origin  | `http://localhost:3000` |
| Methods | All                     |
| Headers | All                     |

To allow additional origins, update the `WithOrigins(...)` call in `Program.cs`.

---

## Dependencies

### `CV_extractor` (Main API)

| Package | Version | Purpose |
|---------|---------|---------|
| [PdfPig](https://www.nuget.org/packages/PdfPig) (`UglyToad.PdfPig`) | 0.1.13 | PDF text extraction across all pages |
| [Swashbuckle.AspNetCore.Swagger](https://www.nuget.org/packages/Swashbuckle.AspNetCore.Swagger) | 10.1.0 | Swagger / OpenAPI JSON generation |
| [Swashbuckle.AspNetCore.SwaggerGen](https://www.nuget.org/packages/Swashbuckle.AspNetCore.SwaggerGen) | 10.1.0 | OpenAPI document generation from attributes |
| [Swashbuckle.AspNetCore.SwaggerUI](https://www.nuget.org/packages/Swashbuckle.AspNetCore.SwaggerUI) | 10.1.0 | Interactive Swagger browser UI |
| [Microsoft.AspNetCore.OpenApi](https://www.nuget.org/packages/Microsoft.AspNetCore.OpenApi) | 9.0.11 | Built-in .NET 9 OpenAPI support |

### `Skill.Extractor.L0.Tests` (Unit Tests)

| Package | Version | Purpose |
|---------|---------|---------|
| [xunit](https://www.nuget.org/packages/xunit) | 2.9.2 | Unit testing framework |
| [xunit.runner.visualstudio](https://www.nuget.org/packages/xunit.runner.visualstudio) | 2.8.2 | xUnit runner for Visual Studio Test Explorer |
| [NSubstitute](https://www.nuget.org/packages/NSubstitute) | 5.3.0 | Mocking `IFormFile` and other interfaces |
| [Microsoft.NET.Test.Sdk](https://www.nuget.org/packages/Microsoft.NET.Test.Sdk) | 17.12.0 | .NET test SDK and runner infrastructure |
| [coverlet.collector](https://www.nuget.org/packages/coverlet.collector) | 6.0.2 | Code coverage data collection |

---

## Testing

Unit tests live in `Skill.Extractor.L0.Tests` and use **xUnit** with **NSubstitute** for mocking.

### Run all tests

```sh
dotnet test
```

### Run with code coverage

```sh
dotnet test --collect:"XPlat Code Coverage"
```

### Test cases

| Test | Scenario |
|------|----------|
| `ExtractSkillsFromPdf_ValidPdfWithSkills_ReturnsSkillsList` | Happy path – parses comma/newline-separated skills |
| `ExtractSkillsFromPdf_NullFile_Returns400BadRequest` | Null file input guard |
| `ExtractSkillsFromPdf_EmptyFile_Returns400BadRequest` | Zero-length file guard |
| `ExtractSkillsFromPdf_NonPdfFile_Returns400BadRequest` | Non-PDF extension guard (`.doc`, `.txt`, `.docx`) |
| `ExtractSkillsFromPdf_NoSkillsSection_ReturnsEmptyList` | Missing `SKILLS` section returns `200 OK` with empty list |
| `ExtractSkillsFromPdf_SkillsWithVariousSeparators_ParsesCorrectly` | Multi-delimiter parsing (`,` `;` `•` `-`) |
| `ExtractSkillsFromPdf_DuplicateSkills_ReturnsUniqueSkills` | Case-insensitive deduplication |

### Testing conventions

- Follow the **Arrange-Act-Assert** pattern.
- Name tests as `MethodName_Scenario_ExpectedBehavior`.
- Mock `IFormFile` with a `MemoryStream` and NSubstitute.
- Inject a custom `extractTextFromPdf` delegate into `SkillExtractorService` to isolate PDF I/O.

---

## Best Practices

### API Design
- Use **Minimal API** syntax with inline route handlers in `Program.cs`.
- Apply `.DisableAntiforgery()` to every file upload endpoint.
- Use `TypedResults` for strongly-typed, OpenAPI-friendly responses.
- Annotate endpoints with `.Produces(...)` / `.ProducesProblem(...)` for accurate schema generation.
- Always validate `IFormFile` for null, zero length, and file extension before processing.

### Error Handling
- `400 Bad Request` – invalid or missing input (null file, empty file, non-PDF extension).
- `500 Internal Server Error` – unrecoverable PDF parsing failure; include the exception message, not the stack trace.
- `200 OK` with an empty skills array – `SKILLS` section not found (not treated as an error).
- Log all exceptions before returning error responses.

### Service Design
- Follow **SOLID** principles; keep each method focused on a single responsibility.
- Register services with **Scoped** lifetime via the built-in DI container.
- Use `const` strings for regex patterns and error messages to avoid magic strings.
- Use `using` statements for all streams and `PdfDocument` instances to ensure prompt disposal.
- Use `HashSet<string>` with `StringComparer.OrdinalIgnoreCase` for case-insensitive deduplication.

### Code Style
- Target **.NET 9** and **C# 13** language features.
- Use **file-scoped namespaces** to reduce indentation.
- Prefer **expression-bodied members** for simple, single-expression methods.
- Use **tuple returns** `(T1, T2, T3)` for methods that need to surface multiple related values.
- `PascalCase` for public members; `camelCase` for parameters and local variables.

---

## Extending the API

- Add a new service using the `.github/copilot-prompts/add-service.prompt` template.
- Register the service in `Program.cs` with the appropriate DI lifetime.
- Add a new endpoint using the `.github/copilot-prompts/add-endpoint.prompt` template.
- Write unit tests using the `.github/copilot-prompts/add-test.prompt` template.
- Follow SOLID and the Single Responsibility Principle for all new code.

---

## Contributing

1. Fork the repository and create a feature branch.
2. Follow the coding standards in `.github/copilot-instructions.md`.
3. Add or update Copilot prompt templates in `.github/copilot-prompts/` as needed.
4. Write unit tests for all new functionality.
5. Submit a pull request with a clear description of the changes.

---

## Learn More

- [PdfPig Library](https://github.com/UglyToad/PdfPig)
- [ASP.NET Core Minimal APIs](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis)
- [.NET 9 – What's New](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-9/overview)
- [xUnit Documentation](https://xunit.net/)
- [NSubstitute Documentation](https://nsubstitute.github.io/)
- [.NET Coding Conventions](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
