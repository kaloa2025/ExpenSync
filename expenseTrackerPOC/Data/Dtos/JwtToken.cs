using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace expenseTrackerPOC.Data.Dtos
{
    public class JwtToken : JwtSecurityToken
    {
        public JwtToken(
           string issuer,
           string audience,
           IEnumerable<Claim> claims,
           DateTime expires,
           SigningCredentials signingCredentials
       ) : base(
           issuer: issuer,
           audience: audience,
           claims: claims,
           notBefore: DateTime.UtcNow,
           expires: expires,
           signingCredentials: signingCredentials
       )
        {
        }
    }
}