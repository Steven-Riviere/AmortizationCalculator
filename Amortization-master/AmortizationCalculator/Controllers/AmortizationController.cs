using AmortizationCalculator.Models;
using AmortizationCalculator.Models.ViewModels;
using AmortizationCalculator.Services;
using Microsoft.AspNetCore.Mvc;
using System.Linq;


namespace AmortizationCalculator.Controllers
{
    public class AmortizationController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View(new AmortizationResultViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(AmortizationResultViewModel model)
        {
            var input = model.InputParameters;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (!input.HasInsurance)
            {
                input.InsuranceRate = 0;
            }

            var service = new AmortizationService();
            if (!service.ValidateInput(input, out var errors))
            {
                foreach (var error in errors)
                    ModelState.AddModelError("InputParameters.InsuranceRate", error);
                return View(model);
            }


            var result = service.CalculateAmortization(input);

            if (result == null || !result.Entries.Any())
            {
                ModelState.AddModelError("", "Le calcul n’a pas généré d’échéances.");
                model.Entries = new List<AmortizationEntryModel>();
                return View(model);
            }

            HttpContext.Session.SetString("AmortizationResult", System.Text.Json.JsonSerializer.Serialize(result));

            return View(result);
        }

    }
}
