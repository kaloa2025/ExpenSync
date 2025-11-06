using expenseTrackerPOC.Data.Dtos;
using expenseTrackerPOC.Data.RequestModels;

namespace expenseTrackerPOC.Services.Auth.Interfaces
{
    public interface IAuthService
    {
        Task SaveRefreshTokenAsync(RefreshToken refreshToken);
        Task<UserDto> ValidateCredentialsAsync(LoginRequest loginRequest);
    }
}
