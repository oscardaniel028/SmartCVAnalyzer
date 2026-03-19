using MediatR;
using SmartCVAnalyzer.Application.Common;
using SmartCVAnalyzer.Application.DTOs.Auth;
using SmartCVAnalyzer.Application.Interfaces;

namespace SmartCVAnalyzer.Application.Features.Auth;

public class LoginQueryHandler : IRequestHandler<LoginQuery, Result<AuthResponseDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;
    private readonly IPasswordHasher _passwordHasher;

    public LoginQueryHandler(
        IUnitOfWork unitOfWork,
        IJwtService jwtService,
        IPasswordHasher passwordHasher)
    {
        _unitOfWork = unitOfWork;
        _jwtService = jwtService;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<AuthResponseDto>> Handle(
        LoginQuery request,
        CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users
            .GetByEmailAsync(request.Email, cancellationToken);

        if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
            return Result<AuthResponseDto>.Failure("Invalid email or password.");

        var accessToken = _jwtService.GenerateAccessToken(user);
        var refreshToken = _jwtService.GenerateRefreshToken();

        return Result<AuthResponseDto>.Success(new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName
            }
        });
    }
}