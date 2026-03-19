using System.Text.Json;
using Microsoft.Extensions.Options;
using OpenAI;
using OpenAI.Chat;
using SmartCVAnalyzer.Application.DTOs.Analysis;
using SmartCVAnalyzer.Application.Interfaces;

namespace SmartCVAnalyzer.Infrastructure.ExternalServices.OpenAi;

public class OpenAiService : IAiAnalysisService
{
    private readonly OpenAiOptions _options;
    private readonly ChatClient _chatClient;

    public OpenAiService(IOptions<OpenAiOptions> options)
    {
        _options = options.Value;

        var openAiClient = new OpenAIClient(_options.ApiKey);
        _chatClient = openAiClient.GetChatClient(_options.Model);
    }

    public async Task<AnalysisFeedbackDto> AnalyzeCvAsync(string cvText,string industryTarget,CancellationToken cancellationToken = default)
    {
        var systemPrompt = BuildSystemPrompt();
        var userPrompt = BuildUserPrompt(cvText, industryTarget);

        var messages = new List<ChatMessage>
        {
            new SystemChatMessage(systemPrompt),
            new UserChatMessage(userPrompt)
        };

        var completionOptions = new ChatCompletionOptions
        {
            MaxOutputTokenCount = _options.MaxTokens,
            // Le decimos a GPT que responda únicamente en JSON
            ResponseFormat = ChatResponseFormat.CreateJsonObjectFormat()
        };

        var response = await _chatClient.CompleteChatAsync(messages, completionOptions, cancellationToken);

        var jsonContent = response.Value.Content[0].Text;

        // Deserializamos la respuesta JSON de la IA al DTO
        var result = JsonSerializer.Deserialize<AnalysisFeedbackDto>(jsonContent,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (result is null)
            throw new InvalidOperationException("AI returned an empty or invalid response.");

        return result;
    }

    private static string BuildSystemPrompt() => """
        You are a senior technical recruiter and career coach with 15 years of experience
        reviewing resumes for top technology companies.

        Your task is to analyze a resume and return a structured JSON response.
        Be specific, actionable, and honest in your feedback.

        You MUST respond ONLY with a valid JSON object following this exact structure:
        {
          "overallScore": <integer 0-100>,
          "sectionScores": {
            "experience": <integer 0-100>,
            "education": <integer 0-100>,
            "skills": <integer 0-100>,
            "format": <integer 0-100>
          },
          "strengths": [<string>, <string>, ...],
          "improvements": [<string>, <string>, ...],
          "missingKeywords": [<string>, <string>, ...],
          "executiveSummary": <string>
        }

        Scoring criteria:
        - overallScore: weighted average of all sections
        - experience: relevance, impact metrics, progression
        - education: relevance to target industry, certifications
        - skills: technical and soft skills alignment with industry
        - format: clarity, length, ATS compatibility, structure

        Provide 3-5 items for strengths, improvements, and missingKeywords.
        ExecutiveSummary should be 2-3 sentences maximum.
        """;

    private static string BuildUserPrompt(string cvText, string industryTarget) => $"""
        Please analyze the following resume for the {industryTarget} industry.

        RESUME CONTENT:
        {cvText}

        Remember to respond ONLY with the JSON structure specified. No additional text.
        """;
}