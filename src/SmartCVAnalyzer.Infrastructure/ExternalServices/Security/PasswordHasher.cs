using SmartCVAnalyzer.Application.Interfaces;

namespace SmartCVAnalyzer.Infrastructure.ExternalServices.Security;

public class PasswordHasher : IPasswordHasher
{
    // BCrypt es el estándar de industria para hashing de contraseñas
    // Incluye salt automático y es resistente a ataques de fuerza bruta
    public string Hash(string password)
        => BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);

    public bool Verify(string password, string hash)
        => BCrypt.Net.BCrypt.Verify(password, hash);
}