using FluentAssertions;
using Moq;
using SmartCVAnalyzer.Application.Features.Auth;
using SmartCVAnalyzer.Application.Interfaces;
using SmartCVAnalyzer.Domain.Entities;

namespace SmartCVAnalyzer.UnitTests.Application.Features;

public class RegisterCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IJwtService> _jwtServiceMock = new();
    private readonly Mock<IPasswordHasher> _passwordHasherMock = new();
    private readonly Mock<IUserRepository> _userRepoMock = new();
    private readonly Mock<ICvAnalysisRepository> _cvAnalysisRepoMock = new();

    private readonly RegisterCommandHandler _handler;

    public RegisterCommandHandlerTests()
    {
        _unitOfWorkMock.Setup(u => u.Users).Returns(_userRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.CvAnalyses).Returns(_cvAnalysisRepoMock.Object);

        _handler = new RegisterCommandHandler(
            _unitOfWorkMock.Object,
            _jwtServiceMock.Object,
            _passwordHasherMock.Object);
    }

    [Fact]
    public async Task Handle_WithNewEmail_ShouldReturnSuccessWithTokens()
    {
        // Arrange
        var command = new RegisterCommand("new@test.com", "John Doe", "Password1!");

        _userRepoMock
            .Setup(r => r.ExistsByEmailAsync("new@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _passwordHasherMock
            .Setup(p => p.Hash("Password1!"))
            .Returns("hashed_password");

        _jwtServiceMock
            .Setup(j => j.GenerateAccessToken(It.IsAny<User>()))
            .Returns("access_token_123");

        _jwtServiceMock
            .Setup(j => j.GenerateRefreshToken())
            .Returns("refresh_token_456");

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.AccessToken.Should().Be("access_token_123");
        result.Value.RefreshToken.Should().Be("refresh_token_456");
        result.Value.User.Email.Should().Be("new@test.com");
        result.Value.User.FullName.Should().Be("John Doe");
    }

    [Fact]
    public async Task Handle_WithExistingEmail_ShouldReturnFailure()
    {
        // Arrange
        var command = new RegisterCommand("existing@test.com", "John Doe", "Password1!");

        _userRepoMock
            .Setup(r => r.ExistsByEmailAsync("existing@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true); // Email ya existe

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("already exists");

        // Verificar que nunca se guardó nada
        _unitOfWorkMock.Verify(
            u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }
}