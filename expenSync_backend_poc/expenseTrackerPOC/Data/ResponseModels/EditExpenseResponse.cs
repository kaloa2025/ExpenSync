
using expenseTrackerPOC.Models;

namespace expenseTrackerPOC.Data.ResponseModels
{
    public class EditExpenseResponse
    {
        public bool Success { get; internal set; }
        public string Message { get; internal set; }
        public List<string> Errors { get; internal set; }
        public Transaction? transaction { get; internal set; }
    }
}
