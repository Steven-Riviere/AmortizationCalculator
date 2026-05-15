using Microsoft.Playwright;
using System;
using System.Threading.Tasks;

namespace PlaywrightTests
{
    [Parallelizable(ParallelScope.Self)]
    [TestFixture]
    public class Tests : PageTest
    {
        //Principe de DRY : Don't Repeat Yourself
        private async Task FillBaseLoanAsync()
        {
            await Page.GotoAsync("http://localhost:5057");
            await Page.GetByLabel("Montant du prêt").FillAsync("200000");
            await Page.GetByLabel("Taux d'intérêt (%)").FillAsync("3.45");
            await Page.GetByLabel("Durée (années)").FillAsync("25");

        }

        [Test]
        public async Task Should_Calculate_Loan_With_All_Valid_Inputs()
        {

            await FillBaseLoanAsync();

            await Page.GetByLabel("Taux assurance (%)").FillAsync("0.25");
            await Page.GetByRole(AriaRole.Button, new() { Name = "Calculer" }).ClickAsync();

            var rows = Page.Locator("table tbody tr")
                .Filter(new() { HasText = "Mois" });

            var lastRow = rows.Last;

            var lastCapital = Page.GetByTestId("remaining-capital").Last;

            await Expect(lastCapital)
                .ToContainTextAsync("0");

            var download = await Page.RunAndWaitForDownloadAsync(async () =>
            {
                await Page.GetByRole(AriaRole.Link, new() { Name = "Télécharger PDF" }).ClickAsync();
            });

            Assert.That(download.SuggestedFilename, Does.EndWith(".pdf"));
        }

        [Test]
        public async Task Should_Calculate_Loan_Without_Insurance()
        {

            await FillBaseLoanAsync();

            await Page.GetByLabel("Assurance active").UncheckAsync();
            await Page.GetByRole(AriaRole.Button, new() { Name = "Calculer" }).ClickAsync();

            var rows = Page.Locator("table tbody tr")
                .Filter(new() { HasText = "Mois" });

            var lastRow = rows.Last;

            var lastCapital = Page.GetByTestId("remaining-capital").Last;

            await Expect(lastCapital)
                .ToContainTextAsync("0");

            var download = await Page.RunAndWaitForDownloadAsync(async () =>
            {
                await Page.GetByRole(AriaRole.Link, new() { Name = "Télécharger PDF" }).ClickAsync();
            });

            Assert.That(download.SuggestedFilename, Does.EndWith(".pdf"));
        }

        [Test]
        public async Task Should_Calculate_Loan_With_Deferred_Capital()
        {

            await FillBaseLoanAsync();

            await Page.GetByLabel("Taux assurance (%)").FillAsync("0.25");
            await Page.GetByLabel("Différé du capital rembours").FillAsync("6");

            await Page.GetByRole(AriaRole.Button, new() { Name = "Calculer" }).ClickAsync();

            var rows = Page.Locator("table tbody tr")
                .Filter(new() { HasText = "Mois" });

            var lastRow = rows.Last;

            var lastCapital = Page.GetByTestId("remaining-capital").Last;

            await Expect(lastCapital)
                .ToContainTextAsync("0");

            var download = await Page.RunAndWaitForDownloadAsync(async () =>
            {
                await Page.GetByRole(AriaRole.Link, new() { Name = "Télécharger PDF" }).ClickAsync();
            });

            Assert.That(download.SuggestedFilename, Does.EndWith(".pdf"));
        }
    }
}