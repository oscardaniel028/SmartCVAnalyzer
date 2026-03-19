using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartCVAnalyzer.Application.DTOs.Analysis;
using SmartCVAnalyzer.Application.Features.GetUserHistory;
using SmartCVAnalyzer.Application.Features.UploadCv;

namespace SmartCVAnalyzer.API.Controllers;

[Authorize] // Todos los endpoints requieren autenticación
public class CvController : BaseController
{
    private readonly IMediator _mediator;

    public CvController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Uploads a CV PDF and triggers AI analysis.
    /// </summary>
    /// <remarks>
    /// Accepts a PDF file (max 5MB) and an industry target.
    /// Returns the complete AI analysis synchronously.
    /// Daily limit: 5 analyses per user.
    /// </remarks>
    [HttpPost("analyze")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(CvAnalysisDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AnalyzeCv(IFormFile file,[FromForm] string industryTarget, CancellationToken cancellationToken)
    {
        // Leer el archivo en memoria
        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream, cancellationToken);
        var fileContent = memoryStream.ToArray();

        var command = new UploadCvCommand(
            UserId: CurrentUserId,
            FileContent: fileContent,
            FileName: file.FileName,
            IndustryTarget: industryTarget);

        var result = await _mediator.Send(command, cancellationToken);

        return HandleResult(result, successStatusCode: 201);
    }

    /// <summary>Returns all CV analyses for the authenticated user.</summary>
    [HttpGet("history")]
    [ProducesResponseType(typeof(IReadOnlyList<CvAnalysisSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetHistory(CancellationToken cancellationToken)
    {
        var query = new GetUserHistoryQuery(CurrentUserId);
        var result = await _mediator.Send(query, cancellationToken);

        return HandleResult(result);
    }
}