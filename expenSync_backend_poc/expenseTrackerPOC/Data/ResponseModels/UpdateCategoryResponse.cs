using expenseTrackerPOC.Models;

namespace expenseTrackerPOC.Data.ResponseModels
{
    public class UpdateCategoryResponse
    {
        public Boolean Success {  get; set; }
        public String Message { get; set; }
        public List<string> Errors { get; set; }
        public Category category { get; set; }

    }
}
