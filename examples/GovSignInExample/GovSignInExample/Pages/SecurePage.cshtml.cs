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
            Console.WriteLine($"FirstName:{user.FirstName}");
            Console.WriteLine($"LastName:{user.LastName}");
            Console.WriteLine($"Email:{user.Email}");
            Console.WriteLine($"PhoneNumber:{user.PhoneNumber}");
        }
    }
}
