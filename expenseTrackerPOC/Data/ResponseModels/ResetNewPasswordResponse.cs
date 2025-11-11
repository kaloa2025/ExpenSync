
namespace expenseTrackerPOC.Data.ResponseModels
{
    public class ResetNewPasswordResponse
    {
        public bool Success { get; internal set; }
        public object Message { get; internal set; }
        public List<string> Errors { get; internal set; }
    }
}
