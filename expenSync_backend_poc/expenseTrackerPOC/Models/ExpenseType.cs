using System.ComponentModel.DataAnnotations;

namespace expenseTrackerPOC.Models
{
    public class ExpenseType
    {
        public int ExpenseTypeId { get; set; }
        [Required, MaxLength(50)]
        public string ExpenseTypeName{ get; set; }

        public ICollection<Transaction> Transactions { get; set; }=new List<Transaction>();
    }
}
