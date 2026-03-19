using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartCVAnalyzer.Domain.Entities;
using SmartCVAnalyzer.Domain.ValueObjects;

namespace SmartCVAnalyzer.Infrastructure.Persistence.Configurations;

public class CvAnalysisConfiguration : IEntityTypeConfiguration<CvAnalysis>
{
    public void Configure(EntityTypeBuilder<CvAnalysis> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.FileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(a => a.IndustryTarget)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.ExtractedText)
            .IsRequired()
            .HasColumnType("nvarchar(max)");

        builder.Property(a => a.AnalysisResultJson)
            .HasColumnType("nvarchar(max)");

        builder.Property(a => a.Status)
            .IsRequired();

        builder.Property(a => a.OverallScore)
            .HasConversion(
                score => score != null ? score.Value : (int?)null,
                value => value != null ? AnalysisScore.Create(value.Value) : null)
            .HasColumnName("OverallScore");

        builder.Property(a => a.ErrorMessage)
            .HasMaxLength(1000);

        builder.Property(a => a.CreatedAt)
            .IsRequired();

        builder.HasIndex(a => a.UserId);
        builder.HasIndex(a => new { a.UserId, a.CreatedAt });
    }
}