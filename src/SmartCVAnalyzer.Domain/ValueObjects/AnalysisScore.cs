namespace SmartCVAnalyzer.Domain.ValueObjects;

// Representa un puntaje entre 0 y 100
// Es un Value Object: su identidad depende de su valor, no de un ID
public sealed class AnalysisScore
{
    public int Value { get; }

    // Constructor privado: la única forma de crear uno es con el método Create
    private AnalysisScore(int value)
    {
        Value = value;
    }

    public static AnalysisScore Create(int value)
    {
        if (value < 0 || value > 100)
            throw new ArgumentOutOfRangeException(nameof(value),
                "Score must be between 0 and 100.");

        return new AnalysisScore(value);
    }

    // Dos scores son iguales si tienen el mismo valor
    public override bool Equals(object? obj)
        => obj is AnalysisScore other && Value == other.Value;

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();

    // Conversión implícita para facilitar el uso
    public static implicit operator int(AnalysisScore score) => score.Value;
}