namespace SmartCVAnalyzer.Infrastructure.ExternalServices.OpenAi;

public class OpenAiOptions
{
    // Nombre de la sección en appsettings.json
    public const string SectionName = "OpenAI";

    public string ApiKey { get; set; } = string.Empty;
    public string Model { get; set; } = "gpt-4o";
    public int MaxTokens { get; set; } = 2000;
}