using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using System.Security.Claims;

namespace FamilyHubs.SharedKernel.GovLogin.Services
{
    public interface ICustomClaims
    {
        Task<IEnumerable<Claim>> GetClaims(TokenValidatedContext tokenValidatedContext);
    }
}
