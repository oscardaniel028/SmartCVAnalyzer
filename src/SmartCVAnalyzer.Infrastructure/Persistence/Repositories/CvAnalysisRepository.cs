using Microsoft.EntityFrameworkCore;
using SmartCVAnalyzer.Application.Interfaces;
using SmartCVAnalyzer.Domain.Entities;

namespace SmartCVAnalyzer.Infrastructure.Persistence.Repositories;

public class CvAnalysisRepository : ICvAnalysisRepository
{
    private readonly AppDbContext _context;

    public CvAnalysisRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<CvAnalysis?> GetByIdAsync(
        Guid id, CancellationToken cancellationToken = default)
        => await _context.CvAnalyses
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    public async Task<CvAnalysis?> GetByIdAndUserIdAsync(
        Guid id, Guid userId, CancellationToken cancellationToken = default)
        => await _context.CvAnalyses
            .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId, cancellationToken);

    public async Task<IReadOnlyList<CvAnalysis>> GetByUserIdAsync(
        Guid userId, CancellationToken cancellationToken = default)
        => await _context.CvAnalyses
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task<int> GetAnalysisCountTodayAsync(
        Guid userId, CancellationToken cancellationToken = default)
    {
        var startOfDay = DateTime.UtcNow.Date;
        return await _context.CvAnalyses
            .CountAsync(
                a => a.UserId == userId && a.CreatedAt >= startOfDay,
                cancellationToken);
    }

    public async Task AddAsync(
        CvAnalysis analysis, CancellationToken cancellationToken = default)
        => await _context.CvAnalyses.AddAsync(analysis, cancellationToken);

    public Task UpdateAsync(
        CvAnalysis analysis, CancellationToken cancellationToken = default)
    {
        // EF Core rastrea los cambios automáticamente
        // Solo necesitamos marcar la entidad como modificada
        _context.CvAnalyses.Update(analysis);
        return Task.CompletedTask;
    }
}