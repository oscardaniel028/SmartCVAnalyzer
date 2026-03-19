using SmartCVAnalyzer.Domain.Entities;

namespace SmartCVAnalyzer.Application.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();

    // Extrae el UserId de un token (útil para refresh tokens)
    Guid? GetUserIdFromToken(string token);
}