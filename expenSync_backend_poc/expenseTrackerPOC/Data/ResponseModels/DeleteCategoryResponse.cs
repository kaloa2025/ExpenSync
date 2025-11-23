using expenseTrackerPOC.Data.Dtos;

namespace expenseTrackerPOC.Data.ResponseModels
{
    public class DeleteCategoryResponse
    {
        public Boolean Success { get; set; }
        public String Message { get; set; }
        public List<string> Errors { get; set; }
    }
}
