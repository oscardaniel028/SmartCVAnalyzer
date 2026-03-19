using System.Text.Json;
using MediatR;
using SmartCVAnalyzer.Application.Common;
using SmartCVAnalyzer.Application.DTOs.Analysis;
using SmartCVAnalyzer.Application.Interfaces;

namespace SmartCVAnalyzer.Application.Features.GetAnalysis;

public class GetAnalysisQueryHandler : IRequestHandler<GetAnalysisQuery, Result<CvAnalysisDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAnalysisQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CvAnalysisDto>> Handle(GetAnalysisQuery request,CancellationToken cancellationToken)
    {
        var analysis = await _unitOfWork.CvAnalyses
            .GetByIdAndUserIdAsync(request.AnalysisId, request.UserId, cancellationToken);

        if (analysis is null)
            return Result<CvAnalysisDto>.Failure("Analysis not found.");

        // Deserializar el JSON guardado en la entidad
        AnalysisFeedbackDto? feedback = null;
        if (!string.IsNullOrEmpty(analysis.AnalysisResultJson))
        {
            feedback = JsonSerializer.Deserialize<AnalysisFeedbackDto>(
                analysis.AnalysisResultJson);
        }

        var dto = new CvAnalysisDto
        {
            Id = analysis.Id,
            FileName = analysis.FileName,
            IndustryTarget = analysis.IndustryTarget,
            Status = analysis.Status,
            OverallScore = analysis.OverallScore?.Value,
            Feedback = feedback,
            CreatedAt = analysis.CreatedAt,
            UpdatedAt = analysis.UpdatedAt
        };

        return Result<CvAnalysisDto>.Success(dto);
    }
}