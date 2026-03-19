using FluentValidation;

namespace SmartCVAnalyzer.Application.Features.UploadCv;

public class UploadCvCommandValidator : AbstractValidator<UploadCvCommand>
{
    private const int MaxFileSizeBytes = 5 * 1024 * 1024; // 5 MB

    public UploadCvCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");

        RuleFor(x => x.FileContent)
            .NotEmpty().WithMessage("File content is required.")
            .Must(content => content.Length <= MaxFileSizeBytes)
            .WithMessage("File size must not exceed 5 MB.");

        RuleFor(x => x.FileName)
            .NotEmpty().WithMessage("File name is required.")
            .Must(name => name.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            .WithMessage("Only PDF files are accepted.");

        RuleFor(x => x.IndustryTarget)
            .NotEmpty().WithMessage("Industry target is required.")
            .MaximumLength(100).WithMessage("Industry target must not exceed 100 characters.");
    }
}