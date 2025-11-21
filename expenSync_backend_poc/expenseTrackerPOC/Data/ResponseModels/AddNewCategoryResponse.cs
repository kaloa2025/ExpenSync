using expenseTrackerPOC.Models;

namespace expenseTrackerPOC.Data.ResponseModels
{
    public class AddNewCategoryResponse
    {
        public Category? Category { get; set; }
        public string Message { get; set; }
        public Boolean Success {  get; set; }
        public List<string> Errors { get; set; }
    }
}
