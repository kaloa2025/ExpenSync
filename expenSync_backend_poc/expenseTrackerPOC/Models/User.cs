using System.ComponentModel.DataAnnotations;

namespace expenseTrackerPOC.Models
{
    public class User
    {
        public int UserId { get; set; }
        [Required, MaxLength(50)]
        public string Username { get; set; }
        [Required, EmailAddress, MaxLength(100)]
        public string Email { get; set; }
        [Required]
        public string HashPassword { get; set; }
        public string Role { get; set; } = "User";

        public int? PasswordResetOtp {  get; set; }
        public DateTime? PasswordResetExpiry { get; set; }
        public bool IsOtpVerified { get; set; } = false;
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
        public List<RefreshToken> RefreshTokens { get; set; } = new();
        public ICollection<Category>? Categories { get; set; }
    }
}
