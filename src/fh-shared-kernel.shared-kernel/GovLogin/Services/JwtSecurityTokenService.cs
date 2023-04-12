using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FamilyHubs.SharedKernel.GovLogin.Services
{
    public interface IJwtSecurityTokenService
    {
        string CreateToken(string clientId, string audience, ClaimsIdentity claimsIdentity,
            SigningCredentials signingCredentials);
    }

    internal class JwtSecurityTokenService : IJwtSecurityTokenService
    {
        public string CreateToken(string clientId, string audience, ClaimsIdentity claimsIdentity,
            SigningCredentials signingCredentials)
        {
            var handler = new JwtSecurityTokenHandler();
            var value = handler.CreateJwtSecurityToken(clientId, audience, claimsIdentity, DateTime.UtcNow,
                DateTime.UtcNow.AddMinutes(5), DateTime.UtcNow, signingCredentials);

            return value.RawData;
        }
    }
}
