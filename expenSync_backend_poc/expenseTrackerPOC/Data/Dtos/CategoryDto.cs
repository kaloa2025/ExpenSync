using expenseTrackerPOC.Models;

namespace expenseTrackerPOC.Data.Dtos
{
    public class CategoryDto
    {
        public int CategoryId {  get; set; }
        public string CategoryName { get; set; }
        public int? UserId { get; set; }
        public int IconId { get; set; }
        public string IconUrl { get; set; }
        public bool IsDefault { get; set; }
    }
}
