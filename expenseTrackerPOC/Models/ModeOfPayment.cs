using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace expenseTrackerPOC.Models
{
    public class ModeOfPayment
    {
        public int ModeOfPaymentId { get; set; }
        public string ModeOfPaymentName { get; set; }
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
