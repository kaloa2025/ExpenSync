namespace expenseTrackerPOC.Data.Dtos
{
    public class TransactionReportDto
    {
        public int TransactionId { get; set; }
        public string? TransactionDescription { get; set; }
        public string ReciverSenderName { get; set; }
        public decimal TransactionAmount { get; set; }
        public DateTime TransactionDate { get; set; }
        public string CategoryName { get; set; }
        public string ExpenseTypeName { get; set; }
        public string ModeOfPaymentName { get; set; }
    }
}
