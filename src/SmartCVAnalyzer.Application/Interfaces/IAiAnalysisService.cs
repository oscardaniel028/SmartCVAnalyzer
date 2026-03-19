using SmartCVAnalyzer.Application.DTOs.Analysis;

namespace SmartCVAnalyzer.Application.Interfaces;

public interface IAiAnalysisService
{
    // Envía el texto del CV a la IA y retorna el feedback estructurado
    Task<AnalysisFeedbackDto> AnalyzeCvAsync(string cvText, string industryTarget, CancellationToken cancellationToken = default);
}