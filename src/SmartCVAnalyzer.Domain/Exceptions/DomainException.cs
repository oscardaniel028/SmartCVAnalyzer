namespace SmartCVAnalyzer.Domain.Exceptions;

// Excepción base para cualquier violación de regla de negocio
public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}