using System.ComponentModel.DataAnnotations;

namespace expenseTrackerPOC.Data.RequestModels
{
    public class SignUpRequest
    {
        [Required, MinLength(3), MaxLength(15)]
        public string UserName { get; set; }
        [Required, EmailAddress]
        public string Email { get; set; }
        [Required, MinLength(5)]
        public string Password { get; set; }
        [Required, MinLength(5)]
        public string RePassword { get; set; }
    }
}
