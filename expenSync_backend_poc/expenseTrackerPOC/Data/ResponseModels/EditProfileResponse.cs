using expenseTrackerPOC.Data.Dtos;
using System.ComponentModel.DataAnnotations;

namespace expenseTrackerPOC.Data.ResponseModels
{
    public class EditProfileResponse
    {
        public bool Success { get; set; }

        public string? Token { get; set; }

        public UserDto? User { get; set; }

        public string? Message { get; set; }

        public List<string>? Errors { get; set; }
    }
}
