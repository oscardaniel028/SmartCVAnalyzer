using MediatR;
using SmartCVAnalyzer.Application.Common;
using SmartCVAnalyzer.Application.DTOs.Auth;

namespace SmartCVAnalyzer.Application.Features.Auth;

public record LoginQuery(
    string Email,
    string Password
) : IRequest<Result<AuthResponseDto>>;