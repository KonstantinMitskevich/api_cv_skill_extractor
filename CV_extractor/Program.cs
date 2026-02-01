using CV_extractor;

var builder = WebApplication.CreateBuilder(args);

// Add CORS policy for React app
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ISkillExtractorService, SkillExtractorService>();

var app = builder.Build();

// Use CORS policy
app.UseCors("AllowReactApp");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapPost("/extract-skills", async (ISkillExtractorService skillExtractor, IFormFile file) =>
{
	var (skills, error, statusCode) = skillExtractor.ExtractSkillsFromPdf(file);
	if (statusCode == 400)
        return Results.BadRequest(new { error });
    if (statusCode == 500)
        return Results.Json(new { error }, statusCode: StatusCodes.Status500InternalServerError);
    if (skills.Count == 0)
        return Results.Ok(new { file.FileName, skills, message = error });
    return Results.Ok(new { file.FileName, skills });
})
.DisableAntiforgery()
.Accepts<IFormFile>("multipart/form-data", "application/pdf")
.Produces(StatusCodes.Status200OK, typeof(object))
.Produces(StatusCodes.Status400BadRequest, typeof(object))
.Produces(StatusCodes.Status500InternalServerError, typeof(object))
.WithName("ExtractSkills")
.WithDescription("Extracts skills from the 'Skills' section of an uploaded PDF CV file.");

app.Run();
