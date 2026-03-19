using MediatR;
using SmartCVAnalyzer.Application.Common;
using SmartCVAnalyzer.Application.DTOs.Analysis;
using SmartCVAnalyzer.Application.Interfaces;

namespace SmartCVAnalyzer.Application.Features.GetUserHistory;

public class GetUserHistoryQueryHandler: IRequestHandler<GetUserHistoryQuery, Result<IReadOnlyList<CvAnalysisSummaryDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetUserHistoryQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IReadOnlyList<CvAnalysisSummaryDto>>> Handle(GetUserHistoryQuery request,CancellationToken cancellationToken)
    {
        var analyses = await _unitOfWork.CvAnalyses.GetByUserIdAsync(request.UserId, cancellationToken);

        var summaries = analyses
            .OrderByDescending(a => a.CreatedAt)
            .Select(a => new CvAnalysisSummaryDto
            {
                Id = a.Id,
                FileName = a.FileName,
                IndustryTarget = a.IndustryTarget,
                Status = a.Status,
                OverallScore = a.OverallScore?.Value,
                CreatedAt = a.CreatedAt
            })
            .ToList()
            .AsReadOnly();

        return Result<IReadOnlyList<CvAnalysisSummaryDto>>.Success(summaries);
    }
}