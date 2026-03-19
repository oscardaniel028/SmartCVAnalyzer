using SmartCVAnalyzer.Domain.Enums;

namespace SmartCVAnalyzer.Application.DTOs.Analysis;

// DTO de respuesta con el resultado completo del análisis
public class CvAnalysisDto
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string IndustryTarget { get; set; } = string.Empty;
    public AnalysisStatus Status { get; set; }
    public int? OverallScore { get; set; }
    public AnalysisFeedbackDto? Feedback { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
