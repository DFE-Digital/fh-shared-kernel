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
            Console.WriteLine($"LoginTime:{user.LoginTime}");
            Console.WriteLine($"FullName:{user.FullName}");
            Console.WriteLine($"Email:{user.Email}");
            Console.WriteLine($"PhoneNumber:{user.PhoneNumber}");
        }
    }
}
