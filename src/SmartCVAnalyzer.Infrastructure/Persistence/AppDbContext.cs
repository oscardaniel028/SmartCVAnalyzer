using Microsoft.EntityFrameworkCore;
using SmartCVAnalyzer.Domain.Entities;

namespace SmartCVAnalyzer.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<CvAnalysis> CvAnalyses => Set<CvAnalysis>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        // Forzar que todos los enums se guarden como string globalmente
        configurationBuilder
            .Properties<Enum>()
            .HaveConversion<string>();
    }
}