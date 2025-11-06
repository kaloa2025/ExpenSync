using expenseTrackerPOC.Data.Dtos;
using expenseTrackerPOC.Data.RequestModels;
using expenseTrackerPOC.Services.Auth.Interfaces;

namespace expenseTrackerPOC.Services.Auth
{
    public class AuthService : IAuthService
    {
        public Task SaveRefreshTokenAsync(RefreshToken refreshToken)
        {
            throw new NotImplementedException();
        }

        public Task<UserDto> ValidateCredentialsAsync(LoginRequest loginRequest)
        {
            throw new NotImplementedException();
        }
    }
}
