using SmartCVAnalyzer.Application.Interfaces;
using SmartCVAnalyzer.Infrastructure.Persistence.Repositories;

namespace SmartCVAnalyzer.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    // Repositorios inicializados de forma lazy (solo cuando se necesitan)
    private ICvAnalysisRepository? _cvAnalyses;
    private IUserRepository? _users;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public ICvAnalysisRepository CvAnalyses
        => _cvAnalyses ??= new CvAnalysisRepository(_context);

    public IUserRepository Users
        => _users ??= new UserRepository(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);

    public void Dispose() => _context.Dispose();
}