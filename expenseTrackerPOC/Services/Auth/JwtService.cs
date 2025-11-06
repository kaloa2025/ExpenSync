using expenseTrackerPOC.Data.Dtos;
using expenseTrackerPOC.Models;
using expenseTrackerPOC.Services.Auth.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace expenseTrackerPOC.Services.Auth
{
    public class JwtService : IJwtService
    {
        private IAuthService authService;

        public JwtService(IAuthService authService)
        {
            this.authService = authService;
        }

        public async Task<(string accessToken, string refreshToken)> GenerateTokenAsync(UserDto user)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Name, user.UserName),
                new(ClaimTypes.Role, user.Role),
                new("userId", user.Id.ToString()),
                new("sessionId", Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

            var accessToken = GenerateJwtToken(claims, TimeSpan.FromMinutes(30));
            var refreshToken = await GenerateRefreshTokenAsync(user.Id);
            
            return (accessToken, refreshToken);
        }

        public string GenerateJwtToken(IEnumerable<Claim> claims, TimeSpan expiry)
        {
            var key = ;
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtToken(
                issuer : "",
                audience :"",
                claims : claims,
                expires : DateTime.UtcNow.Add(expiry),
                signingCredentials :creds
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<string> GenerateRefreshTokenAsync(int userId)
        {
            var refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                UserId = userId,
                ExpiryDate = DateTime.UtcNow.AddDays(30),
                CreatedDate = DateTime.UtcNow
            };
            await authService.SaveRefreshTokenAsync(refreshToken);
            return refreshToken.Token;
        }

        public Task<string> RefreshTokenAsync(string refreshToken)
        {
            throw new NotImplementedException();
        }

        public ClaimsPrincipal ValidateToken(string token)
        {
            throw new NotImplementedException();
        }
    }
}
