using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace expenseTrackerPOC.Models
{
    public class Transaction
    {
        public int TransactionId { get; set; }
        [MaxLength(255)]
        public string? TransactionDescription { get; set; }
        public string ReciverSenderName { get; set; }

        [Column(TypeName ="decimal(18,2)")]
        public decimal TransactionAmount { get; set; }
        
        //ForeignKeys
        public int UserId { get; set; }
        public int CategoryId { get; set; }
        public int ExpenseTypeId { get; set; }
        public int ModeOfPaymentId { get; set; }

        public DateTime TransactionDate {  get; set; }
        public DateTime CreatedOn{ get; set; } = DateTime.UtcNow;
        public DateTime UpdatedOn { get; set; } = DateTime.UtcNow;

        //Navigation
        public User User { get; set; }
        public Category Category { get; set; }
        public ExpenseType ExpenseType { get; set; }
        public ModeOfPayment ModeOfPayment { get; set; }
    }
}
