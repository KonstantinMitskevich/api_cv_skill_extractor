# Services Layer Instructions

Apply to: `*Service.cs`, `Services/*.cs`

## Interface Design
- Always define an interface (I<ServiceName>) before implementation
- Place interfaces and implementations in the same file for small services
- Register services in Program.cs with appropriate lifetime:
  - **Scoped**: Per-request services (database contexts, services that maintain request state)
  - **Singleton**: Stateless, app-lifetime services (configuration, caching)
  - **Transient**: Lightweight, created each time (simple utilities, stateless workers)

## SkillExtractorService Specifics
- PDF text extraction should join pages with `\n` separator
- Return tuple: `(List<string> skills, string? error, int statusCode)`
- Regex patterns should be const fields for reusability
- Section header pattern: `(?m)^[A-Z][A-Za-z\s\-]+:?$`
- Skills header pattern: `(?i)skills\s*:?`
- Use HashSet with `StringComparer.OrdinalIgnoreCase` for unique, case-insensitive skills

## Method Organization
- Public methods: Interface contract methods
- Private static methods: Pure functions without dependencies (validation, parsing)
- Keep methods small and focused on single responsibility
- Extract validation logic into separate methods (e.g., `IsValidPdfFile`, `GetFileValidationError`)

## Error Handling
- Return error tuples instead of throwing exceptions for business validation
- Catch exceptions during PDF parsing and wrap in tuple response
- Include original exception message in error details for 500 responses
- Use `try-catch` only around external library calls (PdfPig)

## Performance Considerations
- Use `file.OpenReadStream()` instead of reading entire file to byte array
- Dispose streams properly with `using` statements
- Use `StringComparison.OrdinalIgnoreCase` for case-insensitive string operations
- Prefer LINQ for concise collection operations

## Regex Best Practices
- Define regex patterns as const strings with descriptive names
- Use inline options: `(?i)` for case-insensitive, `(?m)` for multiline
- Test patterns with edge cases (extra whitespace, special characters)
- Document what each pattern matches with comments

## Logging (Future Enhancement)
- Inject `ILogger<T>` for all services when logging is needed
- Log at Information level for successful operations
- Log at Warning level when sections are missing
- Log at Error level with exception details for failures
- Never log sensitive user data (file contents, personal info from CVs)
