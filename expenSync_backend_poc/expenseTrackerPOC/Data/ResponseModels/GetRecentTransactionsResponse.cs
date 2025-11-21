using expenseTrackerPOC.Models;

namespace expenseTrackerPOC.Data.ResponseModels
{
    public class GetRecentTransactionsResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public List<string>? Errors { get; set; }
        public Dictionary<DateTime, List<Transaction>> transactionsByDate { get; set; }
    }
}
