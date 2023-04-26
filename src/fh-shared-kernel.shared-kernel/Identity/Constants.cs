namespace FamilyHubs.SharedKernel.Identity
{
    internal static class AuthenticationConstants
    {
        internal const string BearerToken = "BearerToken";
        internal const string IdToken = "id_token";

        /// <summary>
        /// This is the path called from the browser to trigger the sign-out process
        /// </summary>
        internal const string SignOutPath = "/Account/signout";

        /// <summary>
        /// This is the path one-login will return to after logout. The oidc library will then catch this, perform some tasks then 
        /// redirect to the path specified in the app settings
        /// </summary>
        internal const string AccountLogoutCallback = "/Account/logout-callback";  
    }

    internal static class StubConstants
    {
        internal const string LoginPagePath = "/account/stub/loginpage/";
        internal const string RoleSelectedPath = "/account/stub/roleSelected";
    }
}
