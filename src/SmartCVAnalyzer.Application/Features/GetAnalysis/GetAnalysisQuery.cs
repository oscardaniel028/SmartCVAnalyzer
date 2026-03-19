using MediatR;
using SmartCVAnalyzer.Application.Common;
using SmartCVAnalyzer.Application.DTOs.Analysis;

namespace SmartCVAnalyzer.Application.Features.GetAnalysis;

// Una Query representa una consulta que NO cambia el estado del sistema
public record GetAnalysisQuery(
    Guid AnalysisId,
    Guid UserId   // Para verificar que el análisis pertenece a este usuario
) : IRequest<Result<CvAnalysisDto>>;