namespace SmartCVAnalyzer.Application.DTOs.Analysis;

public class UploadCvRequestDto
{
    // El archivo PDF como array de bytes
    public byte[] FileContent { get; set; } = Array.Empty<byte>();
    public string FileName { get; set; } = string.Empty;

    // La industria para la que se evalúa el CV
    // Ejemplos: "software", "finance", "marketing"
    public string IndustryTarget { get; set; } = string.Empty;
}