using expenseTrackerPOC.Data.Dtos;
using expenseTrackerPOC.Models;

namespace expenseTrackerPOC.Services.Core.Interfaces
{
    public interface IEmailService
    {
        Task<(bool Success, string Message)> SendForgotPasswordOtpMail(string email, int? otpValue, int? expirySec);
        Task<(bool Success, string Message)> SendLoginAttemptMail(UserDto user);
        Task<(bool Success, string Message)> SendPasswordUpdateMail(string email);
        Task<(bool Success, string Message)> SendWelcomeMail(UserDto user);
    }
}
