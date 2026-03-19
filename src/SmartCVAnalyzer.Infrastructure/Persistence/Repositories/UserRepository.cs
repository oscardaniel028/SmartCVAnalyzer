using Microsoft.EntityFrameworkCore;
using SmartCVAnalyzer.Application.Interfaces;
using SmartCVAnalyzer.Domain.Entities;

namespace SmartCVAnalyzer.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(
        Guid id, CancellationToken cancellationToken = default)
        => await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

    public async Task<User?> GetByEmailAsync(
        string email, CancellationToken cancellationToken = default)
        => await _context.Users
            .FirstOrDefaultAsync(
                u => u.Email == email.ToLowerInvariant(),
                cancellationToken);

    public async Task<bool> ExistsByEmailAsync(
        string email, CancellationToken cancellationToken = default)
        => await _context.Users
            .AnyAsync(
                u => u.Email == email.ToLowerInvariant(),
                cancellationToken);

    public async Task AddAsync(
        User user, CancellationToken cancellationToken = default)
        => await _context.Users.AddAsync(user, cancellationToken);
}