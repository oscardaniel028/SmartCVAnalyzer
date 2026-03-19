using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SmartCVAnalyzer.Application.DTOs.Analysis;
using SmartCVAnalyzer.Application.Features.UploadCv;
using SmartCVAnalyzer.Application.Interfaces;
using SmartCVAnalyzer.Domain.Entities;

namespace SmartCVAnalyzer.UnitTests.Application.Features;

public class UploadCvCommandHandlerTests
{
    // Mocks de todas las dependencias del handler
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<ICvParserService> _cvParserMock = new();
    private readonly Mock<IAiAnalysisService> _aiServiceMock = new();
    private readonly Mock<ILogger<UploadCvCommandHandler>> _loggerMock = new();
    private readonly Mock<ICvAnalysisRepository> _cvAnalysisRepoMock = new();
    private readonly Mock<IUserRepository> _userRepoMock = new();

    private readonly UploadCvCommandHandler _handler;

    // Datos de prueba
    private readonly Guid _userId = Guid.NewGuid();
    private readonly byte[] _validPdfContent = new byte[] { 0x25, 0x50, 0x44, 0x46 };

    public UploadCvCommandHandlerTests()
    {
        // Conectar repositorios al UnitOfWork mock
        _unitOfWorkMock.Setup(u => u.CvAnalyses).Returns(_cvAnalysisRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.Users).Returns(_userRepoMock.Object);

        _handler = new UploadCvCommandHandler(
            _unitOfWorkMock.Object,
            _cvParserMock.Object,
            _aiServiceMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldReturnSuccessResult()
    {
        // Arrange
        var user = User.Create("test@test.com", "Test User", "hashedpassword");
        var command = CreateValidCommand();
        var expectedFeedback = CreateMockFeedback();

        _userRepoMock
            .Setup(r => r.GetByIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _cvAnalysisRepoMock
            .Setup(r => r.GetAnalysisCountTodayAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        _cvParserMock
            .Setup(p => p.IsValidPdf(_validPdfContent))
            .Returns(true);

        _cvParserMock
            .Setup(p => p.ExtractTextAsync(_validPdfContent, It.IsAny<CancellationToken>()))
            .ReturnsAsync("John Doe - Software Engineer...");

        _aiServiceMock
            .Setup(a => a.AnalyzeCvAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedFeedback);

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.OverallScore.Should().Be(expectedFeedback.OverallScore);
        result.Value.IndustryTarget.Should().Be("software");
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ShouldReturnFailure()
    {
        // Arrange
        var command = CreateValidCommand();

        _userRepoMock
            .Setup(r => r.GetByIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null); // Usuario no existe

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("User not found");

        // Verificar que nunca se llamó a la IA
        _aiServiceMock.Verify(
            a => a.AnalyzeCvAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenDailyLimitReached_ShouldReturnFailure()
    {
        // Arrange
        var user = User.Create("test@test.com", "Test User", "hashedpassword");
        var command = CreateValidCommand();

        _userRepoMock
            .Setup(r => r.GetByIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _cvAnalysisRepoMock
            .Setup(r => r.GetAnalysisCountTodayAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(5); // Límite alcanzado

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Daily analysis limit");
    }

    [Fact]
    public async Task Handle_WithInvalidPdf_ShouldReturnFailure()
    {
        // Arrange
        var user = User.Create("test@test.com", "Test User", "hashedpassword");
        var command = CreateValidCommand();

        _userRepoMock
            .Setup(r => r.GetByIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _cvAnalysisRepoMock
            .Setup(r => r.GetAnalysisCountTodayAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        _cvParserMock
            .Setup(p => p.IsValidPdf(It.IsAny<byte[]>()))
            .Returns(false); // PDF inválido

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("not a valid PDF");
    }

    [Fact]
    public async Task Handle_WhenAiServiceFails_ShouldReturnFailure()
    {
        // Arrange
        var user = User.Create("test@test.com", "Test User", "hashedpassword");
        var command = CreateValidCommand();

        _userRepoMock
            .Setup(r => r.GetByIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _cvAnalysisRepoMock
            .Setup(r => r.GetAnalysisCountTodayAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        _cvParserMock
            .Setup(p => p.IsValidPdf(_validPdfContent))
            .Returns(true);

        _cvParserMock
            .Setup(p => p.ExtractTextAsync(_validPdfContent, It.IsAny<CancellationToken>()))
            .ReturnsAsync("CV content...");

        _aiServiceMock
            .Setup(a => a.AnalyzeCvAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException("OpenAI unavailable"));

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("AI analysis failed");
    }

    // ─── Helpers ──────────────────────────────────────────────────────────────

    private UploadCvCommand CreateValidCommand() => new(
        UserId: _userId,
        FileContent: _validPdfContent,
        FileName: "cv.pdf",
        IndustryTarget: "software");

    private static AnalysisFeedbackDto CreateMockFeedback() => new()
    {
        OverallScore = 78,
        SectionScores = new SectionScoresDto
        {
            Experience = 80,
            Education = 70,
            Skills = 85,
            Format = 75
        },
        Strengths = new List<string> { "Strong technical skills" },
        Improvements = new List<string> { "Add more metrics" },
        MissingKeywords = new List<string> { "Docker", "CI/CD" },
        ExecutiveSummary = "Solid CV with room for improvement."
    };
}