using MediatR;
using SmartCVAnalyzer.Application.Common;
using SmartCVAnalyzer.Application.DTOs.Auth;
using SmartCVAnalyzer.Application.Interfaces;
using SmartCVAnalyzer.Domain.Entities;

namespace SmartCVAnalyzer.Application.Features.Auth;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<AuthResponseDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterCommandHandler(
        IUnitOfWork unitOfWork,
        IJwtService jwtService,
        IPasswordHasher passwordHasher)
    {
        _unitOfWork = unitOfWork;
        _jwtService = jwtService;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<AuthResponseDto>> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken)
    {
        // Verificar que el email no esté en uso
        var exists = await _unitOfWork.Users
            .ExistsByEmailAsync(request.Email, cancellationToken);

        if (exists)
            return Result<AuthResponseDto>.Failure("An account with this email already exists.");

        // Hashear contraseña y crear usuario
        var passwordHash = _passwordHasher.Hash(request.Password);
        var user = User.Create(request.Email, request.FullName, passwordHash);

        await _unitOfWork.Users.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Generar tokens JWT
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