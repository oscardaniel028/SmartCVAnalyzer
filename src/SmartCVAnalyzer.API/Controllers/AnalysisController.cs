using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartCVAnalyzer.Application.DTOs.Analysis;
using SmartCVAnalyzer.Application.Features.GetAnalysis;

namespace SmartCVAnalyzer.API.Controllers;

[Authorize]
public class AnalysisController : BaseController
{
    private readonly IMediator _mediator;

    public AnalysisController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>Returns the full details of a specific CV analysis.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CvAnalysisDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAnalysis(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetAnalysisQuery(id, CurrentUserId);
        var result = await _mediator.Send(query, cancellationToken);

        if (result.IsFailure)
            return NotFound(new { message = result.Error });

        return Ok(result.Value);
    }
}