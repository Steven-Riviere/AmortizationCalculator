using System.ComponentModel.DataAnnotations;

namespace AmortizationCalculator.Models
{
    public class LoanInputModel
    {
        [Required(ErrorMessage = "Veuillez saisir un montant de prêt.")]
        [Range(1000, 1000000, ErrorMessage = "Le montant du prêt doit être compris entre 1 000 et 1 000 000.")]
        public decimal LoanAmount { get; set; }

        [Required(ErrorMessage = "Veuillez saisir taux d'intêret.")]
        [Range(0.01, 20, ErrorMessage = "Le taux du prêt doit être entre 0.01 et 20%.")]
        public decimal InterestRate{ get; set; } // en %

        [Range(0, 15, ErrorMessage = "Le taux assurance doit être entre 0 et 15%.")]
        public decimal InsuranceRate { get; set; } // en %

        public bool HasInsurance { get; set; } = true;

        [Required(ErrorMessage = "Veuillez saisir une durée d'emprunt.")]
        [Range(1, 40, ErrorMessage = "La durée d'emprunt doit être entre 1 et 40 ans.")]
        public int DurationInYears { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; } = DateTime.Now;

        [Range(0, 60)]
        [Display(Name = "Différé d'amortissement (en mois)")]
        public int DeferredMonths { get; set; } = 0;

    }
}
