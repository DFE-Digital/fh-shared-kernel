using Microsoft.AspNetCore.Http;

namespace FamilyHubs.SharedKernel.Razor.Cookies;

public interface ICookiePage
{
    string CookiePolicyContent { get; }
    bool ShowSuccessBanner { get; }

    void OnPost(bool analytics, HttpRequest request, HttpResponse response);
}