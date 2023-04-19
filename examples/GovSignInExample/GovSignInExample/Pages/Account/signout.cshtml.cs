using FamilyHubs.SharedKernel.GovLogin.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GovSignInExample.Pages.Account
{
    /// <summary>
    /// NOTE - This page never gets rendered. 
    /// HttpContext.GovSignOut() will redirect the browser to Gov logout which will redirect back to 
    /// signed-out.cshtml (this is configurable in appsettings)
    /// The path for this page must be /Account/signout
    /// </summary>
    public class signoutModel : PageModel
    {
        public async Task<SignOutResult> OnGet()
        {
            return await HttpContext.GovSignOut();
        }
    }
}
