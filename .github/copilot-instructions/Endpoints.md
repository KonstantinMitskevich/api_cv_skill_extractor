# API Endpoints Instructions

Apply to: `Program.cs` (endpoint definitions)

## Minimal API Endpoint Structure
```csharp
app.MapPost("/endpoint-name", async (IFormFile file, IService service) =>
{
    // Validation and processing
    return TypedResults.Ok(result);
})
.DisableAntiforgery()
.Produces<ResponseType>(200)
.ProducesProblem(400)
.WithName("EndpointName")
.WithOpenApi();
```

## File Upload Endpoints
- Accept `IFormFile` with parameter name matching frontend expectations (e.g., `file`)
- Validation order:
  1. Check `file != null`
  2. Check `file.Length > 0`
  3. Check file extension: `file.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase)`
- Use `.DisableAntiforgery()` fluent method for file uploads
- Consider adding file size limit validation (e.g., max 10MB)

## Response Patterns
- **Success (200)**: Return data object, e.g., `TypedResults.Ok(new { skills })`
- **Client Error (400)**: Return error message, e.g., `TypedResults.BadRequest(new { error = "message" })`
- **Server Error (500)**: Return error with details, e.g., `TypedResults.Problem("message", statusCode: 500)`
- Always use `TypedResults` for strongly-typed responses

## OpenAPI Documentation
- Add `.Produces<T>(statusCode)` for all success responses
- Add `.ProducesProblem(statusCode)` for error responses
- Use `.WithName()` for route name (used in link generation)
- Use `.WithOpenApi()` to include in Swagger documentation
- Add XML summary comments above endpoint definitions

## Endpoint Organization
- Group related endpoints together
- Add comments to separate endpoint sections
- Keep endpoint handlers concise; delegate complex logic to services
- Extract inline lambdas to local functions if they exceed 10 lines

## CORS
- Maintain existing CORS policy "AllowReactApp" for http://localhost:3000
- Apply CORS before authorization: `app.UseCors("AllowReactApp")`
- Add new origins only when explicitly required
- Document allowed origins in comments above CORS configuration

## Request Validation
- Validate all inputs before calling services
- Return 400 for invalid inputs with descriptive error messages
- Use pattern matching and null checks for concise validation
- Example: `if (file is null or { Length: 0 }) return TypedResults.BadRequest(...)`

## Anti-Patterns to Avoid
- Don't use `Results.Ok()` - prefer `TypedResults.Ok<T>()`
- Don't handle business logic in endpoints - delegate to services
- Don't ignore antiforgery errors - explicitly disable for file uploads
- Don't return raw exceptions to client - wrap in error objects
- Don't use synchronous I/O - always use async methods

## Example Endpoint
```csharp
/// <summary>
/// Extracts skills from an uploaded PDF CV file.
/// </summary>
app.MapPost("/extract-skills", async (IFormFile file, ISkillExtractorService service) =>
{
    var (skills, error, statusCode) = service.ExtractSkillsFromPdf(file);
    
    if (error != null && statusCode != StatusCodes.Status200OK)
        return TypedResults.Problem(error, statusCode: statusCode);
    
    return TypedResults.Ok(new { skills });
})
.DisableAntiforgery()
.Produces<object>(200)
.ProducesProblem(400)
.ProducesProblem(500)
.WithName("ExtractSkills")
.WithOpenApi();
```
