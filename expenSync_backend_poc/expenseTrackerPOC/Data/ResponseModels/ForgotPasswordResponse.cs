namespace expenseTrackerPOC.Data.ResponseModels
{
    public class ForgotPasswordResponse
    {
        public string Message { get; set; }
        public bool Success { get; set; }
        public string? Email { get; set; }
        public int? OtpExpirySec { get; set; }
        public List<string>? Errors { get; set; }
    }
}
