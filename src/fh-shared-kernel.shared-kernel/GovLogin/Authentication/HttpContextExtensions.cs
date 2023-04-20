﻿using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FamilyHubs.SharedKernel.GovLogin.Authentication
{
    public static class HttpContextExtensions
    {
        public static string GetBearerToken(this HttpContext httpContext)
        {
            if (!httpContext.Items.ContainsKey(AuthenticationConstants.BearerToken))
                return string.Empty;

            var token = httpContext.Items[AuthenticationConstants.BearerToken] as string;

            if(token == null) 
                return string.Empty;

            return token;

        }

        public static async Task<SignOutResult> GovSignOut(this HttpContext httpContext)
        {
            var idToken = await httpContext.GetTokenAsync("id_token");

            var authenticationProperties = new AuthenticationProperties();
            authenticationProperties.Parameters.Clear();
            authenticationProperties.Parameters.Add("id_token", idToken);

            string[] schemes = { CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme };
            return new SignOutResult(schemes, authenticationProperties );
        }
    }
}
