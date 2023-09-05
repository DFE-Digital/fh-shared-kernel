using FamilyHubs.SharedKernel.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GovSignInExample.Pages
{
    public class TermsAndConditionsPageModel : PageModel
    {
        private readonly ITermsAndConditionsService _termsAndConditionsService;

        [BindProperty]
        public string ReturnPath { get; set; } = string.Empty;

        public TermsAndConditionsPageModel(ITermsAndConditionsService termsAndConditionsService)
        {
            _termsAndConditionsService = termsAndConditionsService;
        }

        public void OnGet(string returnPath)
        {
            ReturnPath = returnPath;
        }

        public async Task<IActionResult> OnPost()
        {
            await _termsAndConditionsService.AcceptTermsAndConditions();
            return Redirect(ReturnPath);
        }
    }
}
