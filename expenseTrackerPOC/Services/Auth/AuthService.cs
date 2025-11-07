using expenseTrackerPOC.Data;
using expenseTrackerPOC.Data.Dtos;
using expenseTrackerPOC.Data.RequestModels;
using expenseTrackerPOC.Models;
using expenseTrackerPOC.Services.Auth.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace expenseTrackerPOC.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly ExpenseTrackerDbContext dbContext;
        public AuthService(ExpenseTrackerDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public Task<RefreshToken?> GetRefreshTokenAsync(string token)
        {
            throw new NotImplementedException();
        }

        public Task<UserDto?> GetUserByRefreshTokenAsync(string refreshToken)
        {
            throw new NotImplementedException();
        }

        public Task RemoveOldRefreshTokensAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public Task RevokeRefreshTokenAsync(RefreshToken refreshToken, string? reason = null)
        {
            throw new NotImplementedException();
        }

        public Task SaveRefreshTokenAsync(RefreshToken refreshToken)
        {
            throw new NotImplementedException();
        }

        public async Task<UserDto> ValidateCredentialsAsync(LoginRequest loginRequest)
        {
            if(loginRequest == null)
            {
                return null;
            }

            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == loginRequest.Email);

            if (user == null)
            {
                return null;
            }

            if(!PasswordHasher.Verify(loginRequest.Password, user.HashPassword))
            {
                return null;
            }

            return new UserDto
            {
                Id = user.UserId,
                Email = user.Email,
                UserName = user.Username,
                Role = user.Role,
            };
        }

    }
}
