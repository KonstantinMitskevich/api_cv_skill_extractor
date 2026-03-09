# GitHub Copilot Customization

This directory contains custom instructions and reusable prompts for GitHub Copilot to provide better, project-specific code suggestions.

## Structure

```
.github/
??? copilot-instructions.md          # Repository-wide instructions
??? copilot-instructions/             # Path-specific instructions
?   ??? Services.md                   # Service layer guidelines
?   ??? Endpoints.md                  # API endpoint patterns
?   ??? Configuration.md              # Configuration file rules
?   ??? Testing.md                    # Testing standards
??? copilot-prompts/                  # Reusable prompt templates
    ??? add-endpoint.prompt           # Template for new API endpoints
    ??? add-service.prompt            # Template for new services
    ??? add-tests.prompt              # Template for unit tests
    ??? add-logging.prompt            # Error handling and logging guide
    ??? add-configuration.prompt      # Configuration setup guide
```

## Repository-Wide Instructions

**File:** `copilot-instructions.md`

Applies to all files in the repository. Contains:
- Project overview and architecture
- General coding guidelines (SOLID, DI, naming conventions)
- Error handling patterns
- API design standards
- PDF processing specifics
- CORS configuration
- Testing requirements
- Code style preferences

## Path-Specific Instructions

These instructions apply to specific files or directories:

### Services.md
**Applies to:** `*Service.cs`, `Services/*.cs`

Guidelines for:
- Interface and implementation patterns
- Service registration with DI
- Error handling strategies
- Method organization
- Regex best practices
- Performance considerations

### Endpoints.md
**Applies to:** `Program.cs` (endpoint definitions)

Guidelines for:
- Minimal API structure
- File upload endpoints
- Response patterns
- OpenAPI documentation
- CORS configuration
- Request validation

### Configuration.md
**Applies to:** `appsettings*.json`, `launchSettings.json`

Guidelines for:
- Configuration file structure
- Environment-specific settings
- User secrets management
- Launch profiles

### Testing.md
**Applies to:** `*Tests.cs`, `Tests/*.cs`, `*.Tests/*.cs`

Guidelines for:
- Test organization and naming
- Arrange-Act-Assert pattern
- Mocking strategies
- Test coverage requirements
- Assertion best practices

## Reusable Prompts

Prompt files provide templates for common tasks. Use them in Copilot Chat by referencing with `#`:

### add-endpoint.prompt
Template for creating new API endpoints with:
- File upload handling
- JSON request/response
- Validation
- OpenAPI documentation

**Usage:** `#add-endpoint Create a POST /analyze endpoint that accepts a CSV file`

### add-service.prompt
Template for creating service classes with:
- Interface definition
- Dependency injection
- Error handling patterns
- Service registration

**Usage:** `#add-service Create a CsvParserService for parsing CSV files`

### add-tests.prompt
Template for generating unit tests with:
- Happy path scenarios
- Edge cases
- Error handling tests
- Mock patterns
- Parameterized tests

**Usage:** `#add-tests Generate tests for SkillExtractorService`

### add-logging.prompt
Guide for adding logging and error handling:
- ILogger injection
- Logging levels
- Structured logging
- Exception handling patterns

**Usage:** `#add-logging Add logging to SkillExtractorService`

### add-configuration.prompt
Guide for adding configuration settings:
- Options pattern
- Validation
- Environment-specific config
- IOptions injection

**Usage:** `#add-configuration Add configuration for max file size and timeout`

## How to Use

### 1. Automatic Application
GitHub Copilot automatically reads and applies:
- `copilot-instructions.md` for all files
- Path-specific instructions based on file location

No action needed - just start coding!

### 2. Using Prompt Files in Copilot Chat
Reference prompts with `#` followed by the prompt name:

```
#add-service Create an EmailValidationService
```

or

```
#add-tests for the current file
```

### 3. Inline Suggestions
When writing code, Copilot will suggest code that follows your custom instructions automatically.

## Updating Instructions

When project patterns evolve:

1. **Update relevant instruction file** in `.github/copilot-instructions/`
2. **Commit changes** to repository
3. **Copilot will use new instructions** immediately

## Best Practices

### For Instructions
- Keep concise and example-driven
- Include anti-patterns to avoid
- Update as project evolves
- Document rationale for complex rules

### For Prompts
- Provide complete, working templates
- Include checklists for verification
- Show multiple patterns where applicable
- Add comments explaining key decisions

## Project-Specific Context

This Copilot customization is tailored for:
- **.NET 9** Minimal API
- **C# 13.0** features
- **PdfPig** for PDF processing
- **xUnit** for testing
- **SOLID principles** and clean architecture
- **Skills extraction** from CV/resume PDFs

## Contributing

When adding new patterns or features:
1. Update relevant instruction files
2. Create prompt templates for common tasks
3. Include examples from actual codebase
4. Document in this README

## Learn More

- [GitHub Copilot Documentation](https://docs.github.com/en/copilot)
- [Copilot Custom Instructions](https://github.blog/changelog/2024-05-21-copilot-instructions-in-github-copilot-chat/)
- [.NET Coding Conventions](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
