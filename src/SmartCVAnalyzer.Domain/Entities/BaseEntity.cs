namespace SmartCVAnalyzer.Domain.Entities;

// Clase base que todas las entidades heredan
// Centraliza el Id y las fechas de auditoría
public abstract class BaseEntity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; protected set; }

    protected void SetUpdatedAt() => UpdatedAt = DateTime.UtcNow;
}