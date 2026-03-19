using FluentAssertions;
using SmartCVAnalyzer.Application.Features.UploadCv;

namespace SmartCVAnalyzer.UnitTests.Application.Features;

public class UploadCvCommandValidatorTests
{
    private readonly UploadCvCommandValidator _validator = new();

    [Fact]
    public async Task Validate_WithValidCommand_ShouldPassValidation()
    {
        // Arrange
        var command = new UploadCvCommand(
            UserId: Guid.NewGuid(),
            FileContent: new byte[100],
            FileName: "cv.pdf",
            IndustryTarget: "software");

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_WithEmptyUserId_ShouldFailValidation()
    {
        // Arrange
        var command = new UploadCvCommand(
            UserId: Guid.Empty,
            FileContent: new byte[100],
            FileName: "cv.pdf",
            IndustryTarget: "software");

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "UserId");
    }

    [Fact]
    public async Task Validate_WithNonPdfFile_ShouldFailValidation()
    {
        // Arrange
        var command = new UploadCvCommand(
            UserId: Guid.NewGuid(),
            FileContent: new byte[100],
            FileName: "cv.docx", // No es PDF
            IndustryTarget: "software");

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "FileName");
    }

    [Fact]
    public async Task Validate_WithFileTooLarge_ShouldFailValidation()
    {
        // Arrange: archivo de 6MB (límite es 5MB)
        var command = new UploadCvCommand(
            UserId: Guid.NewGuid(),
            FileContent: new byte[6 * 1024 * 1024],
            FileName: "cv.pdf",
            IndustryTarget: "software");

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "FileContent");
    }

    [Fact]
    public async Task Validate_WithEmptyIndustryTarget_ShouldFailValidation()
    {
        // Arrange
        var command = new UploadCvCommand(
            UserId: Guid.NewGuid(),
            FileContent: new byte[100],
            FileName: "cv.pdf",
            IndustryTarget: ""); // Vacío

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "IndustryTarget");
    }
}