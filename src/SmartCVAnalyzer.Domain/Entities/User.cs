using SmartCVAnalyzer.Domain.Exceptions;

namespace SmartCVAnalyzer.Domain.Entities;

public class User : BaseEntity
{
    public string Email { get; private set; } = string.Empty;
    public string FullName { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;

    // EF Core necesita un constructor sin parámetros (puede ser privado o protected)
    private User() { }

    public static User Create(string email, string fullName, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Email cannot be empty.");

        if (string.IsNullOrWhiteSpace(fullName))
            throw new DomainException("Full name cannot be empty.");

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new DomainException("Password hash cannot be empty.");

        return new User
        {
            Email = email.Trim().ToLowerInvariant(),
            FullName = fullName.Trim(),
            PasswordHash = passwordHash
        };
    }

    public void UpdateFullName(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new DomainException("Full name cannot be empty.");

        FullName = fullName.Trim();
        SetUpdatedAt();
    }
}