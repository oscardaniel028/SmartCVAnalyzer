using SmartCVAnalyzer.Domain.Entities;

namespace SmartCVAnalyzer.Application.Interfaces;

public interface ICvAnalysisRepository
{
    Task<CvAnalysis?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    // Solo retorna análisis del usuario especificado (seguridad: un usuario no ve los de otro)
    Task<CvAnalysis?> GetByIdAndUserIdAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CvAnalysis>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<int> GetAnalysisCountTodayAsync(Guid userId, CancellationToken cancellationToken = default);

    Task AddAsync(CvAnalysis analysis, CancellationToken cancellationToken = default);

    Task UpdateAsync(CvAnalysis analysis, CancellationToken cancellationToken = default);
}