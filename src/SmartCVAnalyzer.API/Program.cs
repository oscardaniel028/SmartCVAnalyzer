using Microsoft.EntityFrameworkCore;
using SmartCVAnalyzer.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using SmartCVAnalyzer.API.Extensions;
using SmartCVAnalyzer.API.Middleware;
using SmartCVAnalyzer.Application;
using SmartCVAnalyzer.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// ─── Servicios ────────────────────────────────────────────────────────────────

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();

builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAuthorization();
builder.Services.AddCorsPolicy(builder.Configuration);

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, ct) =>
    {
        document.Info = new OpenApiInfo
        {
            Title = "SmartCV Analyzer API",
            Version = "v1",
            Description = """
                AI-powered REST API that analyzes resumes and provides
                actionable feedback for job seekers.
                ## Authentication
                Use the `/api/v1/auth/register` or `/api/v1/auth/login` endpoints
                to obtain a JWT token, then click **Authorize** and enter:
                `Bearer {your-token}`
                """,
            Contact = new OpenApiContact
            {
                Name = "SmartCV Analyzer",
                Url = new Uri("https://github.com/yourusername/SmartCVAnalyzer")
            }
        };

        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = JwtBearerDefaults.AuthenticationScheme,
            BearerFormat = "JWT",
            Description = "Enter your JWT token (without the 'Bearer' prefix)"
        };

        return Task.CompletedTask;
    });
});

builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 10 * 1024 * 1024;
});

// ─── Pipeline HTTP ────────────────────────────────────────────────────────────

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

// Scalar disponible en Development y en Production dentro de Docker
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Title = "SmartCV Analyzer API";
        options.Theme = ScalarTheme.Purple;
        options.DefaultHttpClient = new(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
}

// Solo redirigir HTTPS fuera de Docker (en Docker usamos HTTP en el puerto 8080)
if (!app.Environment.IsProduction())
    app.UseHttpsRedirection();

app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/health", () => Results.Ok(new
{
    status = "healthy",
    timestamp = DateTime.UtcNow,
    version = "1.0.0"
})).AllowAnonymous();

// ─── Migraciones con reintentos (necesario para Docker) ───────────────────────
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider
        .GetRequiredService<ILogger<Program>>();

    var retries = 5;
    while (retries > 0)
    {
        try
        {
            var dbContext = scope.ServiceProvider
                .GetRequiredService<AppDbContext>();
            await dbContext.Database.MigrateAsync();
            logger.LogInformation("Database migrations applied successfully.");
            break;
        }
        catch (Exception ex)
        {
            retries--;
            logger.LogWarning(
                "Failed to apply migrations. Retries left: {Retries}. Error: {Error}",
                retries, ex.Message);

            if (retries == 0)
                logger.LogError("Could not apply migrations after all retries.");
            else
                await Task.Delay(5000);
        }
    }
}

app.Run();

public partial class Program { }