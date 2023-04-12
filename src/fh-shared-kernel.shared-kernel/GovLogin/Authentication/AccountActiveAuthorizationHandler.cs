﻿using FamilyHubs.SharedKernel.GovLogin.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace FamilyHubs.SharedKernel.GovLogin.Authentication
{
    public class AccountActiveAuthorizationHandler : AuthorizationHandler<AccountActiveRequirement>
    {
        private readonly GovUkOidcConfiguration _configuration;

        public AccountActiveAuthorizationHandler(IConfiguration configuration)
        {
            _configuration = configuration.GetGovUkOidcConfiguration();
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AccountActiveRequirement requirement)
        {
            if (context.Resource is HttpContext httpContext)
            {
                var isAccountSuspended = context.User.Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.AuthorizationDecision))?.Value;
                if (isAccountSuspended != null && isAccountSuspended.Equals("Suspended", StringComparison.CurrentCultureIgnoreCase))
                {
                    httpContext.Response.Redirect(_configuration.Urls.AccountSuspendedRedirect);
                }
            }
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}
