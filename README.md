# Overview
- Minimal API in .NET 9 to extract skills from uploaded PDF CVs.
- Endpoint: `/extract-skills` accepts `multipart/form-data` with `file`.
- Uses PdfPig to parse PDF text and regex to locate `SKILLS` section.
- Returns JSON: `{ skills: [ ... ] }` with error handling and Swagger docs.

# Tools / models used (Copilot)
- Copilot Chat GPT-5
- Copilot Agent GPT-5

# Prompt history (key steps)
- Step: Project task definition
  - Prompt: "I want to build .Net minimal API which will extract from pdf CV file skills and return them in a structured format the app should contain one edpoint create me plan of steps how to build the backend part of the app."
  - Result: Provided concise step-by-step plan to build a .NET 9 minimal API backend part.

- Step: Project setup and Swagger
  - Prompt: "Add Swagger"
  - Result: Added `Swashbuckle.AspNetCore`, `AddEndpointsApiExplorer`, `AddSwaggerGen`, and middleware `UseSwagger/UseSwaggerUI`.

- Step: File upload endpoint
  - Prompt: "Create a POST endpoint (/extract-skills) that accepts a PDF file via multipart/form-data."
  - Result: Minimal API endpoint using `IFormFile` with basic validation.

- Step: PDF parsing
  - Prompt: "Use the PDF library PdfPig to extract text from the uploaded file."
  - Result: Integrated PdfPig to join page text.

- Step: Skills extraction (section-based)
  - Prompt: "Find section with name Skills and extract inner content. CV sample provided."
  - Result: Regex to locate `SKILLS` header and slice until next header.

- Step: JSON formatting
  - Prompt: "Format the extracted skills into a structured JSON response."
  - Result: Split section by common separators into unique list; returned `{ skills }`.

- Step: Error handling
  - Prompt: "Handle invalid files, parsing errors, and return appropriate HTTP status codes."
  - Result: 400 for invalid input, 500 with parse details, 200 when section missing.

- Step: SOLID refactor
  - Prompt: "Optimize the code and refactor with SOLID."
  - Result: Added `ISkillExtractorService` and `SkillExtractorService`, registered via DI.

- Step: Antiforgery issue
  - Prompt: "Fix anti-forgery middleware error for endpoint."
  - Result: Added `.DisableAntiforgery()` on endpoint.

- Step: CORS for React
  - Prompt: "Add CORS for origin http://localhost:3000, allow all methods."
  - Result: Added named policy `AllowReactApp` and `UseCors`.

# Insights
- Which prompts worked well?
  - Incremental requests ("add Swagger", "create endpoint", "handle errors") produced focused, reliable changes.
  - Requests to refactor with SOLID led to cleaner architecture and testability.

- Which prompts did not work, and why?
  - Generic regex prompts initially captured too much; specifying examples (e.g., SKILLS then EXPERIENCE) helped refine patterns.

- Prompting patterns that gave best results
  - First provide a plan of needed actions, then implement per file.
  - Provide concrete examples of input (sample CV sections) to tune parsing.
  - Address integration concerns early (antiforgery and CORS) to avoid runtime blockers.
