namespace expenseTrackerPOC.Data.Dtos
{
    public class RefreshToken
    {
        public int Id { get; set; }                // Primary key (optional, useful if stored in DB)
        public string Token { get; set; }          // The refresh token value
        public int UserId { get; set; }            // Foreign key reference to the user
        public DateTime CreatedDate { get; set; }  // When the token was created
        public DateTime ExpiryDate { get; set; }   // When the token expires
        public bool IsRevoked { get; set; }        // Optional: mark as invalid if revoked
    }
}
