namespace SmartCVAnalyzer.Application.Common;

// Representa el resultado de una operación que puede fallar
// T es el tipo del valor en caso de éxito
public class Result<T>
{
    public T? Value { get; }
    public string? Error { get; }
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;

    // Constructor privado: se crea solo con los métodos estáticos
    private Result(T? value, string? error, bool isSuccess)
    {
        Value = value;
        Error = error;
        IsSuccess = isSuccess;
    }

    // Crea un resultado exitoso con un valor
    public static Result<T> Success(T value)
        => new(value, null, true);

    // Crea un resultado fallido con un mensaje de error
    public static Result<T> Failure(string error)
        => new(default, error, false);
}

// Versión sin valor de retorno (para operaciones void)
public class Result
{
    public string? Error { get; }
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;

    private Result(string? error, bool isSuccess)
    {
        Error = error;
        IsSuccess = isSuccess;
    }

    public static Result Success()
        => new(null, true);

    public static Result Failure(string error)
        => new(error, false);
}