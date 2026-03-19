using FluentAssertions;
using SmartCVAnalyzer.Domain.Entities;
using SmartCVAnalyzer.Domain.Enums;
using SmartCVAnalyzer.Domain.Exceptions;

namespace SmartCVAnalyzer.UnitTests.Domain;

public class CvAnalysisTests
{
    // Datos de prueba reutilizables
    private readonly Guid _userId = Guid.NewGuid();
    private const string FileName = "my-cv.pdf";
    private const string ExtractedText = "John Doe - Software Engineer with 3 years experience...";
    private const string Industry = "software";

    [Fact]
    public void Create_WithValidData_ShouldCreateAnalysis()
    {
        // Act
        var analysis = CvAnalysis.Create(_userId, FileName, ExtractedText, Industry);

        // Assert
        analysis.UserId.Should().Be(_userId);
        analysis.FileName.Should().Be(FileName);
        analysis.ExtractedText.Should().Be(ExtractedText);
        analysis.IndustryTarget.Should().Be(Industry);
        analysis.Status.Should().Be(AnalysisStatus.Pending);
        analysis.OverallScore.Should().BeNull();
        analysis.Id.Should().NotBeEmpty();
        analysis.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_WithEmptyFileName_ShouldThrowDomainException(string? fileName)
    {
        // Act
        var act = () => CvAnalysis.Create(_userId, fileName!, ExtractedText, Industry);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("*file name*");
    }

    [Fact]
    public void Create_WithEmptyUserId_ShouldThrowDomainException()
    {
        // Act
        var act = () => CvAnalysis.Create(Guid.Empty, FileName, ExtractedText, Industry);

        // Assert
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void MarkAsProcessing_WhenPending_ShouldChangeStatus()
    {
        // Arrange
        var analysis = CvAnalysis.Create(_userId, FileName, ExtractedText, Industry);

        // Act
        analysis.MarkAsProcessing();

        // Assert
        analysis.Status.Should().Be(AnalysisStatus.Processing);
    }

    [Fact]
    public void MarkAsProcessing_WhenNotPending_ShouldThrowDomainException()
    {
        // Arrange
        var analysis = CvAnalysis.Create(_userId, FileName, ExtractedText, Industry);
        analysis.MarkAsProcessing();

        // Act: intentar marcar como processing de nuevo
        var act = () => analysis.MarkAsProcessing();

        // Assert
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void MarkAsCompleted_WhenProcessing_ShouldSetScoreAndStatus()
    {
        // Arrange
        var analysis = CvAnalysis.Create(_userId, FileName, ExtractedText, Industry);
        analysis.MarkAsProcessing();
        var resultJson = """{"overallScore": 85}""";

        // Act
        analysis.MarkAsCompleted(85, resultJson);

        // Assert
        analysis.Status.Should().Be(AnalysisStatus.Completed);
        analysis.OverallScore!.Value.Should().Be(85);
        analysis.AnalysisResultJson.Should().Be(resultJson);
        analysis.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void MarkAsFailed_WhenProcessing_ShouldSetErrorAndStatus()
    {
        // Arrange
        var analysis = CvAnalysis.Create(_userId, FileName, ExtractedText, Industry);
        analysis.MarkAsProcessing();

        // Act
        analysis.MarkAsFailed("OpenAI service unavailable");

        // Assert
        analysis.Status.Should().Be(AnalysisStatus.Failed);
        analysis.ErrorMessage.Should().Be("OpenAI service unavailable");
    }

    [Fact]
    public void MarkAsFailed_WhenCompleted_ShouldThrowDomainException()
    {
        // Arrange
        var analysis = CvAnalysis.Create(_userId, FileName, ExtractedText, Industry);
        analysis.MarkAsProcessing();
        analysis.MarkAsCompleted(80, """{"score": 80}""");

        // Act
        var act = () => analysis.MarkAsFailed("error");

        // Assert
        act.Should().Throw<DomainException>();
    }
}