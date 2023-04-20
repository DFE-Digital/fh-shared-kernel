using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace FamilyHubs.SharedKernel.GovLogin.Services.Interfaces
{
    public interface IJwtSecurityTokenService
    {
        string CreateToken(string clientId, string audience, ClaimsIdentity claimsIdentity,
            SigningCredentials signingCredentials);
    }
}
