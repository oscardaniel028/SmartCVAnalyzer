using SmartCVAnalyzer.Domain.Enums;

namespace SmartCVAnalyzer.Application.DTOs.Analysis;

// Versión resumida para listar el historial (sin el feedback completo)
public class CvAnalysisSummaryDto
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string IndustryTarget { get; set; } = string.Empty;
    public AnalysisStatus Status { get; set; }
    public int? OverallScore { get; set; }
    public DateTime CreatedAt { get; set; }
}