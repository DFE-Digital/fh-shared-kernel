using FamilyHubs.SharedKernel.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GovSignInExample.Pages
{
    public class TermsAndConditionsPageModel : PageModel
    {
        private readonly ITermsAndConditionsService _termsAndConditionsService;

        public TermsAndConditionsPageModel(ITermsAndConditionsService termsAndConditionsService)
        {
            _termsAndConditionsService = termsAndConditionsService;
        }

        public async Task<IActionResult> OnPost()
        {
            await _termsAndConditionsService.AcceptTermsAndConditions();
            return RedirectToPage("SecurePage");
        }
    }
}
