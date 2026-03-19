using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartCVAnalyzer.Application.Interfaces;
using SmartCVAnalyzer.Infrastructure.ExternalServices;
using SmartCVAnalyzer.Infrastructure.ExternalServices.OpenAi;
using SmartCVAnalyzer.Infrastructure.ExternalServices.PdfParsing;
using SmartCVAnalyzer.Infrastructure.ExternalServices.Security;
using SmartCVAnalyzer.Infrastructure.Persistence;

namespace SmartCVAnalyzer.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Base de datos
        services.AddDbContext<AppDbContext>(options =>options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
            b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

        // Unit of Work y repositorios
        // Scoped: una instancia por request HTTP
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Servicios externos
        // Para desarrollo sin API key de OpenAI, usa MockAiAnalysisService:
        // services.AddScoped<IAiAnalysisService, MockAiAnalysisService>();

        // Para producción con OpenAI real:
        services.AddScoped<IAiAnalysisService, OpenAiService>();
        services.AddScoped<ICvParserService, PdfParserService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtService, JwtService>();

        // Configuración tipada (Options pattern)
        services.Configure<OpenAiOptions>(
            configuration.GetSection(OpenAiOptions.SectionName));

        services.Configure<JwtOptions>(
            configuration.GetSection(JwtOptions.SectionName));

        return services;
    }
}