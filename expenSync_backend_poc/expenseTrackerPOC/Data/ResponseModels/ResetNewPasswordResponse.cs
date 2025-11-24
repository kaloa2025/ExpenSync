
namespace expenseTrackerPOC.Data.ResponseModels
{
    public class ResetNewPasswordResponse
    {
        public bool Success { get; internal set; }
        public string Message { get; internal set; }
        public List<string>? Errors { get; internal set; }
    }
}
