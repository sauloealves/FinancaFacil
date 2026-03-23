using ControleFinanceiro.Application.Interfaces;

using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser.Listener;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace ControleFinanceiro.Application.UseCases.Invoice {
    public class PdfTextExtractor : IPdfTextExtractor {
        public async Task<string> ExtractTextAsync(Stream pdfStream) {
            using var reader = new PdfReader(pdfStream);
            using var pdf = new PdfDocument(reader);

            var text = new StringBuilder();

            for(int i = 1; i <= pdf.GetNumberOfPages(); i++) {
                var page = pdf.GetPage(i);
                var strategy = new SimpleTextExtractionStrategy();
                var currentText = iText.Kernel.Pdf.Canvas.Parser.PdfTextExtractor.GetTextFromPage(page, strategy);

                text.AppendLine(currentText);
            }

            return text.ToString();
        }
    }
}
