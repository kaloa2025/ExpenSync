using expenseTrackerPOC.Data.Dtos;
using expenseTrackerPOC.Models;

namespace expenseTrackerPOC.Data.ResponseModels
{
    public class GetReportResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public List<string>? Errors { get; set; }
        public Dictionary<DateTime, List<TransactionReportDto>> transactions { get; set; }
    }
}
