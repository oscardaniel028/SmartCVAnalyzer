using MediatR;
using SmartCVAnalyzer.Application.Common;
using SmartCVAnalyzer.Application.DTOs.Analysis;

namespace SmartCVAnalyzer.Application.Features.GetUserHistory;

public record GetUserHistoryQuery(Guid UserId) : IRequest<Result<IReadOnlyList<CvAnalysisSummaryDto>>>;