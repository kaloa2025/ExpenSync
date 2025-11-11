using expenseTrackerPOC.Models;

namespace expenseTrackerPOC.Data.ResponseModels
{
    public class FetchCategoryResponse
    {
        public Boolean Success { get; set; }
        public Category? Category { get; set; }
        public string Message { get; set; }
        public List<string> Errors {  get; set; }
    }
}
