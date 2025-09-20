using AmortizationCalculator.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

public class DownloadController : Controller
{
    [HttpGet]
    public IActionResult DownloadPdf()
    {
        var json = HttpContext.Session.GetString("AmortizationResult");
        if (json == null) return RedirectToAction("Index");

        var model = System.Text.Json.JsonSerializer.Deserialize<AmortizationResultViewModel>(json);

        // Déclarer la licence Community
        QuestPDF.Settings.License = LicenseType.Community;

        var document = new AmortizationPdfDocument(model);
        var pdfBytes = document.GeneratePdf();

        return File(pdfBytes, "application/pdf", "TableauAmortissement.pdf");
    }
}
