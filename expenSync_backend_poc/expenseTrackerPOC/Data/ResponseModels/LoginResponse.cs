using expenseTrackerPOC.Data.Dtos;
using expenseTrackerPOC.Models;

namespace expenseTrackerPOC.Data.ResponseModels
{
    public class LoginResponse
    {
        public bool Success { get; set; }

        public string? Token { get; set; }

        public UserDto? User { get; set; }

        public string? Message { get; set; }

        public List<string>? Errors { get; set; }
    }
}
