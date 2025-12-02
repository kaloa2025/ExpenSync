namespace expenseTrackerPOC.Data.ResponseModels
{
    public class ScanReceiptResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string[]? Errors { get; set; }
        public DateTime TransactionDate { get; set; }
        public string TransactionDescription { get; set; }
        public string ReciverSenderName { get; set; }
        public decimal TransactionAmount { get; set; }
        public int CategoryId { get; set; }
        public int ModeOfPaymentId { get; set; }
        public int ExpenseTypeId { get; set; }
    }
}
