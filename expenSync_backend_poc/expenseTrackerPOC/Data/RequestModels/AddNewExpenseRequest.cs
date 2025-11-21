using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace expenseTrackerPOC.Data.RequestModels
{
    public class AddNewExpenseRequest
    {
        public string? TransactionDescription { get; set; }
        public string ReciverSenderName { get; set; }
        public decimal TransactionAmount { get; set; }
        public int CategoryId { get; set; }
        public int ExpenseTypeId { get; set; }
        public int ModeOfPaymentId { get; set; }
        public DateTime TransactionDate { get; set; }

    }
}