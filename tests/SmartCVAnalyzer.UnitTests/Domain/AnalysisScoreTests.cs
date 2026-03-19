using FluentAssertions;
using SmartCVAnalyzer.Domain.ValueObjects;

namespace SmartCVAnalyzer.UnitTests.Domain;

public class AnalysisScoreTests
{
    [Fact]
    public void Create_WithValidValue_ShouldCreateScore()
    {
        // Arrange & Act
        var score = AnalysisScore.Create(75);

        // Assert
        score.Value.Should().Be(75);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(50)]
    [InlineData(100)]
    public void Create_WithBoundaryValues_ShouldCreateScore(int value)
    {
        // Act
        var score = AnalysisScore.Create(value);

        // Assert
        score.Value.Should().Be(value);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    [InlineData(-100)]
    public void Create_WithInvalidValue_ShouldThrowException(int value)
    {
        // Act
        var act = () => AnalysisScore.Create(value);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Equals_WithSameValue_ShouldBeEqual()
    {
        // Arrange
        var score1 = AnalysisScore.Create(80);
        var score2 = AnalysisScore.Create(80);

        // Assert
        score1.Should().Be(score2);
    }

    [Fact]
    public void ImplicitConversion_ToInt_ShouldReturnValue()
    {
        // Arrange
        var score = AnalysisScore.Create(90);

        // Act
        int value = score;

        // Assert
        value.Should().Be(90);
    }
}