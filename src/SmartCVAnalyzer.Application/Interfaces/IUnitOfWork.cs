namespace SmartCVAnalyzer.Application.Interfaces;

// Agrupa todos los repositorios y confirma los cambios en una sola transacción
public interface IUnitOfWork : IDisposable
{
    ICvAnalysisRepository CvAnalyses { get; }
    IUserRepository Users { get; }

    // Persiste todos los cambios pendientes en la base de datos
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}