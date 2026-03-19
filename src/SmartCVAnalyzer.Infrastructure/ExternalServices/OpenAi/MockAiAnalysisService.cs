using SmartCVAnalyzer.Application.DTOs.Analysis;
using SmartCVAnalyzer.Application.Interfaces;

namespace SmartCVAnalyzer.Infrastructure.ExternalServices.OpenAi;

// Servicio mock para desarrollo y testing sin gastar créditos de OpenAI
public class MockAiAnalysisService : IAiAnalysisService
{
    public Task<AnalysisFeedbackDto> AnalyzeCvAsync(
        string cvText,
        string industryTarget,
        CancellationToken cancellationToken = default)
    {
        var feedback = new AnalysisFeedbackDto
        {
            OverallScore = 72,
            SectionScores = new SectionScoresDto
            {
                Experience = 75,
                Education = 70,
                Skills = 80,
                Format = 65
            },
            Strengths = new List<string>
            {
                "Clear demonstration of technical skills relevant to the industry",
                "Good progression of responsibilities across positions",
                "Quantified achievements that demonstrate real impact"
            },
            Improvements = new List<string>
            {
                "Add more measurable results and metrics to experience section",
                "Include relevant certifications for the target industry",
                "Improve formatting consistency throughout the document"
            },
            MissingKeywords = new List<string>
            {
                "CI/CD", "Docker", "Agile", "REST API", "Microservices"
            },
            ExecutiveSummary =
                $"This CV shows solid foundational experience for the {industryTarget} industry. " +
                "With some targeted improvements in metrics and keywords, it has strong potential " +
                "to pass ATS filters and attract recruiter attention."
        };

        return Task.FromResult(feedback);
    }
}