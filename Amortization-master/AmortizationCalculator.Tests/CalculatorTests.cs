using System.ComponentModel.DataAnnotations;
using AmortizationCalculator.Models;

namespace AmortizationCalculator.Tests
{
    public class CalculatorTests
    {
        private List<ValidationResult> ValidateModel(LoanInputModel loanInputModel)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(loanInputModel, null, null);
            Validator.TryValidateObject(loanInputModel, context, results, true);
            return results;
        }

        private void DisplayValidationResults(List<ValidationResult> results)
        {
            if (results.Count == 0)
            {
                Console.WriteLine("Aucune erreur de validation retourn�e");
            }
            else
            {
                foreach (var error in results)
                {
                    Console.WriteLine("Erreur trouv�e : " + error.ErrorMessage);
                }
            }
        }


        [Theory]
        [InlineData(10000, 0, 0, false, 25, 2025, 9, 19, 0, "Le taux du pr�t doit �tre entre 0.01 et 20%.")]
        [InlineData(10000, 25, 0, false, 25, 2025, 9, 19, 0, "Le taux du pr�t doit �tre entre 0.01 et 20%.")]
        [InlineData(10000, -25, 0, false, 25, 2025, 9, 19, 0, "Le taux du pr�t doit �tre entre 0.01 et 20%.")]

        public void Calculator_ShouldBeInvalid_WhenInterestRateIsInvalid(decimal loanamount, double interestrateDouble, double insurancerateDouble, bool hasinsurance, int durationinyears, int year, int month, int day, int deferredmonths, string expectedMessage)
        {
            decimal interestrate = (decimal)interestrateDouble;
            decimal insurancerate = (decimal)insurancerateDouble;

            Console.WriteLine($"test de validation du modele avec un taux � {interestrate}%");
            var startdate = new DateTime(year, month, day);

            var calculator = new LoanInputModel
            {
                LoanAmount = loanamount,
                InterestRate = interestrate,
                InsuranceRate = insurancerate,
                HasInsurance = hasinsurance,
                DurationInYears = durationinyears,
                StartDate = startdate,
                DeferredMonths = deferredmonths
            };

            var results = ValidateModel(calculator);
            DisplayValidationResults(results);

            Assert.Contains(results, r => r.ErrorMessage == expectedMessage);
        }

        [Theory]
        [InlineData(0, 2.45, 0, false, 25, 2025, 9, 19, 0, "Le montant du pr�t doit �tre compris entre 1 000 et 1 000 000.")]
        [InlineData(-50, 2.45, 0, false, 25, 2025, 9, 19, 0, "Le montant du pr�t doit �tre compris entre 1 000 et 1 000 000.")]
        [InlineData(10000000, 2.45, 0, false, 25, 2025, 9, 19, 0, "Le montant du pr�t doit �tre compris entre 1 000 et 1 000 000.")]

        public void Calculator_ShouldBeInvalid_WhenLoanAmountIsInvalid(decimal loanamount, double interestrateDouble, double insurancerateDouble, bool hasinsurance, int durationinyears, int year, int month, int day, int deferredmonths, string expectedMessage)
        {
            decimal interestrate = (decimal)interestrateDouble;
            decimal insurancerate = (decimal)insurancerateDouble;

            Console.WriteLine($"test de validation du modele avec un pr�t � {loanamount} euros");
            var startdate = new DateTime(year, month, day);

            var calculator = new LoanInputModel
            {
                LoanAmount = loanamount,
                InterestRate = interestrate,
                InsuranceRate = insurancerate,
                HasInsurance = hasinsurance,
                DurationInYears = durationinyears,
                StartDate = startdate,
                DeferredMonths = deferredmonths
            };

            var results = ValidateModel(calculator);
            DisplayValidationResults(results);

            Assert.Contains(results, r => r.ErrorMessage == expectedMessage);
        }
    }
}