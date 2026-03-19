namespace SmartCVAnalyzer.Domain.Exceptions;

// Se lanza cuando un recurso no existe en la base de datos
public class NotFoundException : DomainException
{
    public NotFoundException(string entityName, object id)
        : base($"{entityName} with id '{id}' was not found.") { }
}