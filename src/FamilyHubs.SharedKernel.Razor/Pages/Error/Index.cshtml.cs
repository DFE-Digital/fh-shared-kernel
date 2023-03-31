using FamilyHubs.SharedKernel.Razor.FamilyHubsUi.Options;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace FamilyHubs.SharedKernel.Razor.Pages.Error;

public class ErrorModel : PageModel
{
    public IOptions<FamilyHubsUiOptions> FamilyHubsUiOptions { get; set; }

    public ErrorModel(IOptions<FamilyHubsUiOptions> familyHubsUiOptions)
    {
        FamilyHubsUiOptions = familyHubsUiOptions;
    }
}