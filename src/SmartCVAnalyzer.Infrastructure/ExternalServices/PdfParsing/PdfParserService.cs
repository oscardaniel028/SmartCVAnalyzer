using SmartCVAnalyzer.Application.Interfaces;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

namespace SmartCVAnalyzer.Infrastructure.ExternalServices.PdfParsing;

public class PdfParserService : ICvParserService
{
    public Task<string> ExtractTextAsync(byte[] fileContent,CancellationToken cancellationToken = default)
    {
        // PdfPig es síncrono internamente, lo envolvemos en Task.Run
        // para no bloquear el hilo del request
        return Task.Run(() =>
        {
            using var document = PdfDocument.Open(fileContent);
            var textBuilder = new System.Text.StringBuilder();

            foreach (Page page in document.GetPages())
            {
                // Extraemos palabras en orden de lectura y las unimos
                var words = page.GetWords();
                textBuilder.AppendLine(string.Join(" ", words.Select(w => w.Text)));
            }

            return textBuilder.ToString().Trim();

        }, cancellationToken);
    }

    public bool IsValidPdf(byte[] fileContent)
    {
        // Los archivos PDF siempre comienzan con la firma %PDF-
        if (fileContent.Length < 4)
            return false;

        return fileContent[0] == 0x25 && // %
               fileContent[1] == 0x50 && // P
               fileContent[2] == 0x44 && // D
               fileContent[3] == 0x46;   // F
    }
}