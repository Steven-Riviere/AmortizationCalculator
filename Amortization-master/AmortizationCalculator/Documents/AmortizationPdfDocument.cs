using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using AmortizationCalculator.Models.ViewModels;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;

public class AmortizationPdfDocument : IDocument
{
    private readonly AmortizationResultViewModel _model;

    public AmortizationPdfDocument(AmortizationResultViewModel model)
    {
        _model = model;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
        // Grouper par année
        var groupedByYear = _model.Entries
            .GroupBy(e => e.PaymentDate.Year)
            .OrderBy(g => g.Key)
            .ToList();

        container.Page(page =>
        {
            page.Margin(15);
            page.Size(PageSizes.A4);
            page.PageColor(Colors.White);
            page.DefaultTextStyle(x => x.FontSize(12));

            page.Header()
                .Text("Tableau d'amortissement")
                .SemiBold()
                .FontSize(18)
                .AlignCenter();

            page.Content().Column(col =>
            {
                // Détails du prêt + total global
                if (_model.InputParameters != null)
                {
                    col.Item().PaddingTop(5).Text("Détails du prêt").SemiBold().FontSize(14);

                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        table.Cell().PaddingTop(3).Text("Montant du prêt");
                        table.Cell().PaddingTop(3).Text(_model.InputParameters.LoanAmount.ToString("C"));

                        table.Cell().PaddingTop(3).Text("Taux d'intérêt");
                        table.Cell().PaddingTop(3).Text(_model.InputParameters.InterestRate.ToString("F2") + " %");

                        table.Cell().PaddingTop(3).Text("Taux d'assurance");
                        table.Cell().PaddingTop(3).Text(_model.InputParameters.InsuranceRate.ToString("F2") + " %");

                        table.Cell().PaddingTop(3).Text("Durée");
                        table.Cell().PaddingTop(3).Text(_model.InputParameters.DurationInYears + " années");

                        table.Cell().PaddingTop(3).Text("Date début");
                        table.Cell().PaddingTop(3).Text(_model.InputParameters.StartDate.ToString("dd-MM-yyyy"));

                        table.Cell().PaddingTop(3).Text("Différé du capital remboursé");
                        table.Cell().PaddingTop(3).Text(_model.InputParameters.DeferredMonths + " mois");

                        table.Cell().PaddingTop(3).Text("Assurance active");
                        table.Cell().PaddingTop(3).Text(_model.InputParameters.HasInsurance ? "Oui" : "Non");

                        table.Cell().PaddingTop(10);
                        table.Cell().PaddingTop(10);

                        // Total global
                        table.Cell().PaddingTop(3).Text("Total Intérêts");
                        table.Cell().PaddingTop(3).Text(_model.TotalInterestCost.ToString("C"));

                        table.Cell().PaddingTop(3).Text("Total Assurance");
                        table.Cell().PaddingTop(3).Text(_model.TotalInsuranceCost.ToString("C"));

                        table.Cell().PaddingTop(3).Text("Coût total");
                        table.Cell().PaddingTop(3).Text(_model.TotalCost.ToString("C"));
                    });

                    col.Item().PaddingBottom(10);

                    col.Item().PaddingTop(5).Background(Colors.Grey.Lighten4).Text(txt =>
                    {
                        txt.Span("⚠️ Attention : ");
                        txt.Span("Ce document est une simulation générée automatiquement en fonction des données saisies. " +
                                 "Il est fourni à titre purement informatif et ne constitue pas un document contractuel. " +
                                 "Pour tout calcul officiel et décision financière, veuillez consulter votre banque ou un courtier.").Italic();
                    });
                }

                // Boucle par année
                foreach (var yearGroup in groupedByYear)
                {
                    col.Item().PaddingBottom(15);

                    col.Item()
                    .PaddingTop(18)
                    .Text($"Échéances - Année {yearGroup.Key}")
                    .SemiBold()
                    .FontSize(14);

                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(40); // Mois
                            columns.RelativeColumn();   // Date paiement
                            columns.RelativeColumn();   // Paiement mensuel
                            columns.RelativeColumn();   // Intérêts
                            columns.RelativeColumn();   // Assurance
                            columns.RelativeColumn();   // Capital remboursé
                            columns.RelativeColumn();   // Capital restant dû
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("Mois");
                            header.Cell().Text("Date paiement");
                            header.Cell().Text("Paiement mensuel");
                            header.Cell().Text("Intérêts");
                            header.Cell().Text("Assurance");
                            header.Cell().Text("Capital remboursé");
                            header.Cell().Text("Capital restant dû");
                        });

                        // Lignes des paiements
                        foreach (var entry in yearGroup)
                        {
                            table.Cell().Text(entry.Month.ToString());
                            table.Cell().Text(entry.PaymentDate.ToString("dd-MM-yyyy"));
                            table.Cell().Text(entry.MonthlyPayment.ToString("C"));
                            table.Cell().Text(entry.Interest.ToString("C"));
                            table.Cell().Text(entry.Insurance.ToString("C"));
                            table.Cell().Text(entry.PrincipalPaid.ToString("C"));
                            table.Cell().Text(entry.RemainingPrincipal.ToString("C"));
                        }

                        // Total de l'année
                        table.Cell().ColumnSpan(2).Background(Colors.Grey.Lighten3).Padding(3)
                        .Text($"Total année {yearGroup.Key}").SemiBold();
                        table.Cell().Background(Colors.Grey.Lighten3).PaddingTop(3).Text(yearGroup.Sum(e => e.MonthlyPayment).ToString("C")).SemiBold();
                        table.Cell().Background(Colors.Grey.Lighten3).PaddingTop(3).Text(yearGroup.Sum(e => e.Interest).ToString("C")).SemiBold();
                        table.Cell().Background(Colors.Grey.Lighten3).PaddingTop(3).Text(yearGroup.Sum(e => e.Insurance).ToString("C")).SemiBold();
                        table.Cell().Background(Colors.Grey.Lighten3).PaddingTop(3).Text(yearGroup.Sum(e => e.PrincipalPaid).ToString("C")).SemiBold();

                    });
                }
            });

            page.Footer()
                .AlignCenter()
                .Text(txt =>
                {
                    txt.Span("Page ");
                    txt.CurrentPageNumber();
                    txt.Span(" / ");
                    txt.TotalPages();
                });
        });
    }
}
