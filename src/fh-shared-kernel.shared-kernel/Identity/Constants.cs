namespace FamilyHubs.SharedKernel.Identity
{
    public static class FamilyHubsClaimTypes
    {
        public const string Role = "role";
        public const string OrganisationId = "OrganisationId";
        public const string AccountId = "AccountId";
        public const string AccountStatus = "AccountStatus";
        public const string ClaimsValidTillTime = "ClaimsValidTillTime";
        public const string FullName = "Name";
        public const string PhoneNumber = "PhoneNumber";
        public const string TermsAndConditionsAccepted = "TermsAndConditionsAccepted";
    }

    public static class RoleTypes
    {
        public const string DfeAdmin = "DfeAdmin";
        public const string LaManager = "LaManager";
        public const string VcsManager = "VcsManager";
        public const string LaProfessional = "LaProfessional";
        public const string VcsProfessional = "VcsProfessional";
        public const string VcsDualRole = "VcsDualRole";
        public const string LaDualRole = "LaDualRole";
        public const string ServiceAccount = "ServiceAccount";
    }

    public static class RoleGroups
    {
        public const string LaProfessionalOrDualRole = RoleTypes.LaProfessional + "," + RoleTypes.LaDualRole;
        public const string VcsProfessionalOrDualRole = RoleTypes.VcsProfessional + "," + RoleTypes.VcsDualRole;
    }

    internal static class AuthenticationConstants
    {
        internal const string BearerToken = "BearerToken";
        internal const string IdToken = "id_token";
        internal const string RefreshClaimsCookie = "RefreshClaims";

        internal const string AccountPaths = "Account/";

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
