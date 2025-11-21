using expenseTrackerPOC.Data.Dtos;
using expenseTrackerPOC.Models;

namespace expenseTrackerPOC.Data.ResponseModels
{
    public class FetchAllIconsResponse
    {
        public bool Success { get; set; }
        public List<CategoryIcon>? icons { get; set; }

        public string? Message { get; set; }

        public List<string>? Errors { get; set; }
    }
}
