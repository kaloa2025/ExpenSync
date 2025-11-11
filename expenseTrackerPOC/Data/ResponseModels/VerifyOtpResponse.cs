namespace expenseTrackerPOC.Data.ResponseModels
{
    public class VerifyOtpResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<string> Errors { get; set; }

        public string Email { get; set; }

    }
}
