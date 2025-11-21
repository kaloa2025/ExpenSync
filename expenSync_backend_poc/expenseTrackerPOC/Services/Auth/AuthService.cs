using expenseTrackerPOC.Data;
using expenseTrackerPOC.Data.Dtos;
using expenseTrackerPOC.Data.RequestModels;
using expenseTrackerPOC.Models;
using expenseTrackerPOC.Services.Auth.Interfaces;
using Microsoft.AspNetCore.Identity.Data;
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
        public async Task<RefreshToken?> GetRefreshTokenAsync(string token)
        {
            return await dbContext.RefreshTokens.Include(rt => rt.User).FirstOrDefaultAsync(rt => rt.Token == token);
        }

        public async Task<UserDto?> GetUserByRefreshTokenAsync(string refreshToken)
        {
            var token = await dbContext.RefreshTokens.Include(rt => rt.User).FirstOrDefaultAsync(rt => rt.Token == refreshToken);
            if(token == null)
            {
                return null;
            }
            return new UserDto
            {
                Id = token.User.UserId,
                Email = token.User.Email,
                UserName = token.User.Username,
                Role = token.User.Role,
            };
        }

        public async Task RemoveOldRefreshTokensAsync(int userId, string newToken)
        {
            var tokens = await dbContext.RefreshTokens.Where(rt=>rt.UserId == userId && rt.Token!= newToken).ToListAsync();
            if (tokens.Any())
            {
                dbContext.RefreshTokens.RemoveRange(tokens);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task RevokeRefreshTokenAsync(RefreshToken refreshToken, string? reason = null)
        {
            if(refreshToken == null)
            {
                throw new ArgumentNullException(nameof(RefreshToken));
            }
            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.ReasonRevoked = reason ?? "Revoked";
            refreshToken.IsRevoked.Equals(true);

            dbContext.RefreshTokens.Update(refreshToken);
            await dbContext.SaveChangesAsync();
        }

        public async Task SaveRefreshTokenAsync(RefreshToken refreshToken)
        {
            if (refreshToken == null)
                throw new ArgumentNullException(nameof(refreshToken));

            await dbContext.RefreshTokens.AddAsync(refreshToken);
            await dbContext.SaveChangesAsync();
        }

        public async Task<UserDto?> ValidateCredentialsAsync(Data.RequestModels.LoginRequest loginRequest)
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
        public async Task<(bool exists, string Message)> CheckUserEmailAlreadyExistsAsync(string email)
        {
            if (email == null)
            {
                return (false, "Email is not valid");
            }

            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user != null)
            {
                return (true, "An User with same email exists, Please Login if you are a returning User.");
            }

            return (false, "User with email doesnt exist.");

        }


        public async Task<UserDto> CreateUserAsync(SignUpRequest signUpRequest)
        {
            if (signUpRequest == null)
            {
                return null;
            }

            //1. check if password and repassword is same
            if (signUpRequest.Password != signUpRequest.RePassword || !signUpRequest.Password.Equals(signUpRequest.RePassword))
            {
                return null;
            }

            //2. Hash Password
            var hasedPassword = PasswordHasher.Hash(signUpRequest.Password);

            //3. Create User
            User user = new User
                {
                Email = signUpRequest.Email,
                Username = signUpRequest.UserName,
                HashPassword = hasedPassword,
            };
            //4. Add User
            var userAdded = (await dbContext.Users.AddAsync(user)).Entity;
            await dbContext.SaveChangesAsync();

            if (userAdded == null)
            {
                return null;
            }

            return new UserDto
            {
                Id = userAdded.UserId,
                Email = userAdded.Email,
                UserName = userAdded.Username,
                Role = userAdded.Role,
            };
        }
    }
}
