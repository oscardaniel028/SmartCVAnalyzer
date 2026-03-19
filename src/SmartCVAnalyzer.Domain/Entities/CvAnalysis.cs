using SmartCVAnalyzer.Domain.Enums;
using SmartCVAnalyzer.Domain.Exceptions;
using SmartCVAnalyzer.Domain.ValueObjects;

namespace SmartCVAnalyzer.Domain.Entities;

// Entidad principal del sistema
// Representa un análisis de CV realizado por la IA
public class CvAnalysis : BaseEntity
{
    public Guid UserId { get; private set; }

    // Nombre original del archivo subido
    public string FileName { get; private set; } = string.Empty;

    // Texto extraído del PDF
    public string ExtractedText { get; private set; } = string.Empty;

    // La industria para la que se está analizando el CV
    public string IndustryTarget { get; private set; } = string.Empty;

    // Estado actual del análisis
    public AnalysisStatus Status { get; private set; }

    // Score general del CV (null hasta que se complete el análisis)
    public AnalysisScore? OverallScore { get; private set; }

    // Resultado completo en JSON tal como lo devuelve la IA
    // Lo guardamos como string para flexibilidad
    public string? AnalysisResultJson { get; private set; }

    // Mensaje de error si el análisis falló
    public string? ErrorMessage { get; private set; }

    // Navegación hacia el usuario dueño de este análisis (para EF Core)
    public User? User { get; private set; }

    private CvAnalysis() { }

    // Factory method: única forma válida de crear un análisis nuevo
    public static CvAnalysis Create(Guid userId, string fileName, string extractedText, string industryTarget)
    {
        if (userId == Guid.Empty)
            throw new DomainException("UserId cannot be empty.");

        if (string.IsNullOrWhiteSpace(fileName))
            throw new DomainException("File name cannot be empty.");

        if (string.IsNullOrWhiteSpace(extractedText))
            throw new DomainException("Extracted text cannot be empty.");

        if (string.IsNullOrWhiteSpace(industryTarget))
            throw new DomainException("Industry target cannot be empty.");

        return new CvAnalysis
        {
            UserId = userId,
            FileName = fileName.Trim(),
            ExtractedText = extractedText.Trim(),
            IndustryTarget = industryTarget.Trim().ToLowerInvariant(),
            Status = AnalysisStatus.Pending  // siempre empieza como Pending
        };
    }

    // Métodos que representan transiciones de estado válidas
    // Solo la entidad puede cambiar su propio estado (encapsulamiento)

    public void MarkAsProcessing()
    {
        if (Status != AnalysisStatus.Pending)
            throw new DomainException($"Cannot start processing an analysis with status '{Status}'.");

        Status = AnalysisStatus.Processing;
        SetUpdatedAt();
    }

    public void MarkAsCompleted(int score, string analysisResultJson)
    {
        if (Status != AnalysisStatus.Processing)
            throw new DomainException($"Cannot complete an analysis with status '{Status}'.");

        if (string.IsNullOrWhiteSpace(analysisResultJson))
            throw new DomainException("Analysis result cannot be empty.");

        OverallScore = AnalysisScore.Create(score);
        AnalysisResultJson = analysisResultJson;
        Status = AnalysisStatus.Completed;
        SetUpdatedAt();
    }

    public void MarkAsFailed(string errorMessage)
    {
        if (Status == AnalysisStatus.Completed)
            throw new DomainException("Cannot fail an already completed analysis.");

        ErrorMessage = errorMessage;
        Status = AnalysisStatus.Failed;
        SetUpdatedAt();
    }
}