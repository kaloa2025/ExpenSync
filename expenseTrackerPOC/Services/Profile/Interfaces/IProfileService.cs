
using expenseTrackerPOC.Data.Dtos;
using expenseTrackerPOC.Data.RequestModels;

namespace expenseTrackerPOC.Services.Profile.Interfaces
{
    public interface IProfileService
    {
        Task<(bool exists, string Message)> CheckUserExistsAsync(string email, int userId);
        Task<(bool exists, string Message)> CheckEmailExistsAsync(string email);
        Task<(UserDto? user, string Message)> EditUserDetailsAsync(EditProfileRequest editProfileRequest, int userId);
        Task<(bool Success, int? otpValue, int? expirySec, string Message)> CreateOtpForEmail(string email);
        Task<(bool Success, string Message)> VerifyOtp(VerifyOtpRequest verifyOtpRequest);
        Task<(bool Success, string Message)> ResetUserPassword(ResetNewPasswordRequest resetPasswordRequest);
    }
}
