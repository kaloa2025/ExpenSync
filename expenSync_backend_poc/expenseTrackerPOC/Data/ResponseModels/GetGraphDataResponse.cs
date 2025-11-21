namespace expenseTrackerPOC.Data.ResponseModels
{
    public class GetGraphDataResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public List<string>? Errors { get; set; }
        public Dictionary<string, decimal>? Data { get; set; }
    }
}
