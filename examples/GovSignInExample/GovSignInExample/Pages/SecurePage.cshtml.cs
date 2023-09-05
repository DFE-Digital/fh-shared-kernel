using FamilyHubs.SharedKernel.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GovSignInExample.Pages
{
    [Authorize]
    public class SecurePageModel : PageModel
    {
        public void OnGet()
        {
            var user = HttpContext.GetFamilyHubsUser();
            Console.WriteLine($"Role:{user.Role}");
            Console.WriteLine($"OrganisationId:{user.OrganisationId}");
            Console.WriteLine($"AccountStatus:{user.AccountStatus}");
            Console.WriteLine($"ClaimsValidTillTime:{user.ClaimsValidTillTime}");
            Console.WriteLine($"FullName:{user.FullName}");
            Console.WriteLine($"Email:{user.Email}");
            Console.WriteLine($"PhoneNumber:{user.PhoneNumber}");
            Console.WriteLine($"TermsAndConditionsAccepted:{user.TermsAndConditionsAccepted}");

            Console.WriteLine($"IsUserDfeAdmin:{HttpContext.IsUserDfeAdmin()}");
            Console.WriteLine($"IsUserLaManager:{HttpContext.IsUserLaManager()}");
            Console.WriteLine($"IsUserLoggedIn:{HttpContext.IsUserLoggedIn()}");
            Console.WriteLine($"GetUserOrganisationId:{HttpContext.GetUserOrganisationId()}");
        }
    }
}
