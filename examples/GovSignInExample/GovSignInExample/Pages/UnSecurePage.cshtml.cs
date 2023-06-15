using FamilyHubs.SharedKernel.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GovSignInExample.Pages
{
    public class UnSecurePageModel : PageModel
    {
        public void OnGet()
        {
            Console.WriteLine($"IsUserDfeAdmin:{HttpContext.IsUserDfeAdmin()}");
            Console.WriteLine($"IsUserLaManager:{HttpContext.IsUserLaManager()}");
            Console.WriteLine($"IsUserLoggedIn:{HttpContext.IsUserLoggedIn()}");
        }
    }
}
