using AmortizationCalculator.Models;
using AmortizationCalculator.Models.ViewModels;
using System;
using System.Collections.Generic;

namespace AmortizationCalculator.Services
{
    public class AmortizationService
    {
        public bool ValidateInput(LoanInputModel input, out List<string> errors)
        {
            errors = new List<string>();

            if (input.HasInsurance)
            {
                if (input.InsuranceRate < 0.01m || input.InsuranceRate > 5)
                    errors.Add("Le taux d'assurance doit être entre 0.01 et 5%.");
            }
            else
            {
                if (input.InsuranceRate != 0)
                    errors.Add("Le taux d'assurance doit être 0 lorsque l'assurance est désactivée.");
            }

            return errors.Count == 0;
        }

        public AmortizationResultViewModel CalculateAmortization(LoanInputModel input)
        {
            var result = new AmortizationResultViewModel
            {
                InputParameters = input,
                Entries = new List<AmortizationEntryModel>()
            };

            decimal monthlyInterestRate = (decimal)(input.InterestRate / 100) / 12;
            decimal monthlyInsuranceRate = (decimal)(input.InsuranceRate / 100) / 12;
            int totalMonths = input.DurationInYears * 12;
            decimal remainingPrincipal = input.LoanAmount;

            //Calcul hors différé
            decimal monthlyPayment = 0;
            if (monthlyInterestRate > 0)
            {
                monthlyPayment = input.LoanAmount * monthlyInterestRate / (1 - (decimal)Math.Pow(1 + (double)monthlyInterestRate, -totalMonths));
            }
            else
            {
                // Si taux 0%
                monthlyPayment = input.LoanAmount / totalMonths;
            }

            int currentMonth = 1;
            DateTime paymentDate = input.StartDate;

            while (currentMonth <= totalMonths)
            {
                var entry = new AmortizationEntryModel
                {
                    Month = currentMonth,
                    PaymentDate = paymentDate,
                };

                if (currentMonth <= input.DeferredMonths)
                {
                    // Différé : on paie seulement intérêts + assurance (si active)
                    decimal interest = remainingPrincipal * monthlyInterestRate;
                    decimal insurance = input.HasInsurance ? remainingPrincipal * monthlyInsuranceRate : 0m;
                    entry.Interest = decimal.Round(interest, 2);
                    entry.Insurance = decimal.Round(insurance, 2);
                    entry.MonthlyPayment = entry.Interest + entry.Insurance;
                    entry.PrincipalPaid = 0;
                    // Le capital ne diminue pas
                }
                else
                {
                    // Période normale
                    decimal interest = remainingPrincipal * monthlyInterestRate;
                    decimal insurance = input.HasInsurance ? remainingPrincipal * monthlyInsuranceRate : 0m;
                    decimal principalPaid = monthlyPayment - interest;

                    entry.Interest = decimal.Round(interest, 2);
                    entry.Insurance = decimal.Round(insurance, 2);
                    entry.PrincipalPaid = decimal.Round(principalPaid, 2);
                    entry.MonthlyPayment = decimal.Round(monthlyPayment + entry.Insurance, 2);

                    remainingPrincipal -= principalPaid;
                    if (remainingPrincipal < 0) remainingPrincipal = 0;
                }

                entry.RemainingPrincipal = decimal.Round(remainingPrincipal, 2);

                result.Entries.Add(entry);

                // Totaux
                result.TotalInterestCost += entry.Interest;
                result.TotalInsuranceCost += entry.Insurance;
                result.TotalCost += entry.MonthlyPayment;

                // Next month
                paymentDate = paymentDate.AddMonths(1);
                currentMonth++;
            }

            // Round totals
            result.TotalInterestCost = decimal.Round(result.TotalInterestCost, 2);
            result.TotalInsuranceCost = decimal.Round(result.TotalInsuranceCost, 2);
            result.TotalCost = decimal.Round(result.TotalCost, 2);

            return result;
        }
    }
}
