namespace SmartCVAnalyzer.Application.Interfaces;

public interface ICvParserService
{
    // Extrae el texto de un PDF y lo retorna como string
    Task<string> ExtractTextAsync(byte[] fileContent, CancellationToken cancellationToken = default);

    // Valida que el archivo sea un PDF válido
    bool IsValidPdf(byte[] fileContent);
}