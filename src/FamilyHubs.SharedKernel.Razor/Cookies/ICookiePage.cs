using Microsoft.AspNetCore.Http;

namespace FamilyHubs.SharedKernel.Razor.Cookies;

public interface ICookiePage
{
    string CookiePolicyContent { get; }
    bool ShowSuccessBanner { get; }
    //todo: remove these from here and view
    public bool ShowPreviousPageLink { get; set; }
    public string? LastPage { get; set; }

    void OnPost(bool analytics, HttpRequest request, HttpResponse response);
}