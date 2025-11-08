using expenseTrackerPOC.Data.Dtos;
using expenseTrackerPOC.Data.RequestModels;
using expenseTrackerPOC.Models;

namespace expenseTrackerPOC.Services.Auth.Interfaces
{
    public interface IAuthService
    {
        Task<UserDto?> ValidateCredentialsAsync(LoginRequest loginRequest);

        //Refresh Token
        Task SaveRefreshTokenAsync(RefreshToken refreshToken);
        Task<RefreshToken?> GetRefreshTokenAsync(string token);
        Task<UserDto?> GetUserByRefreshTokenAsync(string refreshToken);
        Task RevokeRefreshTokenAsync(RefreshToken refreshToken, string? reason = null);
        Task RemoveOldRefreshTokensAsync(int userId);
        Task<bool> CheckUserEmailAlreadyExistsAsync(string email);
        Task<UserDto> CreateUserAsync(SignUpRequest signUpRequest);
    }
}
