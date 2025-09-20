namespace AmortizationCalculator.Models
{
    public class AmortizationEntryModel
    {
        public int Month { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal MonthlyPayment { get; set; }
        public decimal Interest { get; set; }
        public decimal Insurance { get; set; }
        public decimal PrincipalPaid { get; set; }
        public decimal RemainingPrincipal { get; set; }
    }
}
