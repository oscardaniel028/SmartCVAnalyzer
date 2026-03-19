using MediatR;
using SmartCVAnalyzer.Application.Common;
using SmartCVAnalyzer.Application.DTOs.Auth;

namespace SmartCVAnalyzer.Application.Features.Auth;

public record RegisterCommand(
    string Email,
    string FullName,
    string Password
) : IRequest<Result<AuthResponseDto>>;