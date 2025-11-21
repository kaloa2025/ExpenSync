using System.ComponentModel.DataAnnotations;

namespace expenseTrackerPOC.Data.RequestModels
{
    public class UpdateCategoryRequest
    {
        [Required, MaxLength(50)]
        public string CategoryName { get; set; }
        public int IconId { get; set; }
    }
}
