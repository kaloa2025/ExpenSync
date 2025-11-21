namespace expenseTrackerPOC.Data.RequestModels
{
    public class ResetNewPasswordRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
