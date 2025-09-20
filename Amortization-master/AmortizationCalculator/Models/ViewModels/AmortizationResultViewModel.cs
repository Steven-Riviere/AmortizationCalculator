namespace AmortizationCalculator.Models.ViewModels
{
    public class AmortizationResultViewModel
    {
        public LoanInputModel InputParameters { get; set; } = new LoanInputModel();
        public List<AmortizationEntryModel> Entries { get; set; } = new List<AmortizationEntryModel>();

        public decimal TotalInterestCost { get; set; }
        public decimal TotalInsuranceCost { get; set; }
        public decimal TotalCost { get; set; }
    }

}
