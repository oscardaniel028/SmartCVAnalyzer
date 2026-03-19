using MediatR;
using Microsoft.Extensions.Logging;
using SmartCVAnalyzer.Application.Common;
using SmartCVAnalyzer.Application.DTOs.Analysis;
using SmartCVAnalyzer.Application.Interfaces;
using SmartCVAnalyzer.Domain.Entities;

namespace SmartCVAnalyzer.Application.Features.UploadCv;

public class UploadCvCommandHandler : IRequestHandler<UploadCvCommand, Result<CvAnalysisDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICvParserService _cvParser;
    private readonly IAiAnalysisService _aiService;
    private readonly ILogger<UploadCvCommandHandler> _logger;
    private const int DailyLimit = 5;

    public UploadCvCommandHandler(
        IUnitOfWork unitOfWork,
        ICvParserService cvParser,
        IAiAnalysisService aiService,
        ILogger<UploadCvCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _cvParser = cvParser;
        _aiService = aiService;
        _logger = logger;
    }

    public async Task<Result<CvAnalysisDto>> Handle(
        UploadCvCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
            return Result<CvAnalysisDto>.Failure("User not found.");

        var todayCount = await _unitOfWork.CvAnalyses
            .GetAnalysisCountTodayAsync(request.UserId, cancellationToken);

        if (todayCount >= DailyLimit)
            return Result<CvAnalysisDto>.Failure(
                $"Daily analysis limit of {DailyLimit} reached. Try again tomorrow.");

        if (!_cvParser.IsValidPdf(request.FileContent))
            return Result<CvAnalysisDto>.Failure("The uploaded file is not a valid PDF.");

        string extractedText;
        try
        {
            extractedText = await _cvParser.ExtractTextAsync(
                request.FileContent, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "PDF parsing failed: {Message}", ex.Message);
            return Result<CvAnalysisDto>.Failure($"Could not read PDF content: {ex.Message}");
        }

        if (string.IsNullOrWhiteSpace(extractedText))
            return Result<CvAnalysisDto>.Failure("The PDF appears to be empty or contains only images.");

        var analysis = CvAnalysis.Create(
            request.UserId,
            request.FileName,
            extractedText,
            request.IndustryTarget);

        await _unitOfWork.CvAnalyses.AddAsync(analysis, cancellationToken);
        analysis.MarkAsProcessing();

        AnalysisFeedbackDto feedback;
        try
        {
            feedback = await _aiService.AnalyzeCvAsync(
                extractedText,
                request.IndustryTarget,
                cancellationToken);
        }
        catch (Exception ex)
        {
            // Log del error real para debugging
            _logger.LogError(ex, "AI analysis failed. Type: {Type}, Message: {Message}",
                ex.GetType().Name, ex.Message);

            // Log del inner exception si existe
            if (ex.InnerException != null)
                _logger.LogError("Inner exception: {Inner}", ex.InnerException.Message);

            analysis.MarkAsFailed($"AI service error: {ex.Message}");
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<CvAnalysisDto>.Failure("AI analysis failed. Please try again.");
        }

        var resultJson = System.Text.Json.JsonSerializer.Serialize(feedback);
        analysis.MarkAsCompleted(feedback.OverallScore, resultJson);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<CvAnalysisDto>.Success(MapToDto(analysis, feedback));
    }

    private static CvAnalysisDto MapToDto(CvAnalysis analysis, AnalysisFeedbackDto feedback)
        => new()
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
}