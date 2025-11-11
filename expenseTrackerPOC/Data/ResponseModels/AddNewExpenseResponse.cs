using expenseTrackerPOC.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace expenseTrackerPOC.Data.ResponseModels
{
    public class AddNewExpenseResponse
    {
        public Boolean Success { get; set; }
        public List<String>? Errors { get; set; }
        public string Message { get; set; }
        public Transaction transaction { get; set; }

    }
}
