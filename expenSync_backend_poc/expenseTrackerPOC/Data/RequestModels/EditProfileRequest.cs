using System.ComponentModel.DataAnnotations;

namespace expenseTrackerPOC.Data.RequestModels
{
    public class EditProfileRequest
    {
        [Required, MinLength(3), MaxLength(15)]
        public string UserName { get; set; }
        [Required, EmailAddress]
        public string Email { get; set; }
    }
}
