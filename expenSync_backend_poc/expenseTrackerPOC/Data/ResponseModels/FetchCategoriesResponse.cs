using expenseTrackerPOC.Data.Dtos;
using expenseTrackerPOC.Models;

namespace expenseTrackerPOC.Data.ResponseModels
{
    public class FetchCategoriesResponse
    {
        public bool Success { get; set; }
        public List<Category>? categories { get; set; }

        public string? Message { get; set; }

        public List<string>? Errors { get; set; }
    }
}
