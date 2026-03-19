using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartCVAnalyzer.Application.DTOs.Auth;
using SmartCVAnalyzer.Application.Features.Auth;

namespace SmartCVAnalyzer.API.Controllers;

public class AuthController : BaseController
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>Registers a new user account.</summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request,CancellationToken cancellationToken)
    {
        var command = new RegisterCommand(
            request.Email,
            request.FullName,
            request.Password);

        var result = await _mediator.Send(command, cancellationToken);

        return HandleResult(result, successStatusCode: 201);
    }

    /// <summary>Authenticates a user and returns JWT tokens.</summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequestDto request,
        CancellationToken cancellationToken)
    {
        var query = new LoginQuery(request.Email, request.Password);
        var result = await _mediator.Send(query, cancellationToken);

        return HandleResult(result);
    }
}