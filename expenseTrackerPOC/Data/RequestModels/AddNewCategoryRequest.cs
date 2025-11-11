using expenseTrackerPOC.Models;
using System.ComponentModel.DataAnnotations;

namespace expenseTrackerPOC.Data.RequestModels
{
    public class AddNewCategoryRequest
    {
        [Required, MaxLength(50)]
        public string CategoryName { get; set; }
        public int IconId { get; set; }
    }
}