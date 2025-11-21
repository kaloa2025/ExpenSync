namespace expenseTrackerPOC.Data.ResponseModels
{
    public class RefreshTokenResponse
    {
        public bool Success { get; set; }
        public string? AccessToken { get; set; }
        public string? Message { get; set; }
        public List<string>? Errors { get; set; }
    }
}
