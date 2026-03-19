namespace SmartCVAnalyzer.Domain.Enums;

public enum AnalysisStatus
{
    Pending = 0,      // Recién creado, esperando procesamiento
    Processing = 1,   // Siendo analizado por la IA
    Completed = 2,    // Análisis finalizado con éxito
    Failed = 3        // Ocurrió un error durante el análisis
}
