namespace SmartCVAnalyzer.Application.DTOs.Analysis;

    // Estructura del feedback que devuelve la IA
    public class AnalysisFeedbackDto
    {
        public int OverallScore { get; set; }
        public SectionScoresDto SectionScores { get; set; } = null!;
        public List<string> Strengths { get; set; } = new();
        public List<string> Improvements { get; set; } = new();
        public List<string> MissingKeywords { get; set; } = new();
        public string ExecutiveSummary { get; set; } = string.Empty;
    }

