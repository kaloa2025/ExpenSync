using expenseTrackerPOC.Data.Dtos;
using expenseTrackerPOC.Models;
using System.Security.Claims;

namespace expenseTrackerPOC.Services.Auth.Interfaces
{
    public interface IJwtService
    {
        Task<(string accessToken, RefreshToken refreshToken)> GenerateTokenAsync(UserDto user);
        string GenerateJwtToken(IEnumerable<Claim> claims, TimeSpan expiry);
        RefreshToken GenerateRefreshToken(int userId);
        ClaimsPrincipal? ValidateToken(string token);
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);

    }
}
