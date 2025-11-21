using expenseTrackerPOC.Data.Dtos;

namespace expenseTrackerPOC.Data.ResponseModels
{
    public class SignUpResponse
    {
        public bool Success { get; set; }

        public string? Token { get; set; }

        public UserDto? User { get; set; }

        public string? Message { get; set; }

        public List<string>? Errors { get; set; }
    }
}
