namespace expenseTrackerPOC.Data.RequestModels
{
    public class VerifyOtpRequest
    {
        public string Email {  get; set; }
        public int Otp { get; set; }
    }
}
