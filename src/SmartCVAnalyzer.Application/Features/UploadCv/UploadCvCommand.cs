using MediatR;
using SmartCVAnalyzer.Application.Common;
using SmartCVAnalyzer.Application.DTOs.Analysis;

namespace SmartCVAnalyzer.Application.Features.UploadCv;

// Un Command representa una intención de cambiar el estado del sistema
// IRequest<T> indica que este command retorna Result<CvAnalysisDto>
public record UploadCvCommand(
    Guid UserId,
    byte[] FileContent,
    string FileName,
    string IndustryTarget
) : IRequest<Result<CvAnalysisDto>>;