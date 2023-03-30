using FamilyHubs.SharedKernel.Razor.FamilyHubsUi.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Globalization;

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

//public interface ICookiePageModel
//{
//    string CookiePolicyContent { get; }
//    bool ShowSuccessBanner { get; }
//    //todo: remove these from here and view
//    public bool ShowPreviousPageLink { get; set; }
//    public string? LastPage { get; set; }
//}

public class CookiePage : ICookiePage
{
    public string CookiePolicyContent { get; }
    private readonly AnalyticsOptions? _analyticsOptions;

    public CookiePage(IOptions<FamilyHubsUiOptions> familyHubsUiOptions, string cookiePolicyContent = "_CookiePolicy.cshtml")
    {
        CookiePolicyContent = cookiePolicyContent;
        _analyticsOptions = familyHubsUiOptions.Value.Analytics;
    }

    public bool ShowSuccessBanner { get; set; }
    //todo: remove these from here and view
    public bool ShowPreviousPageLink { get; set; }
    public string? LastPage { get; set; }

    public void OnPost(bool analytics, HttpRequest request, HttpResponse response)
    {
        if (_analyticsOptions == null)
            return;

        SetConsentCookie(request, response, analytics);
        if (!analytics)
        {
            ResetAnalyticCookies(request, response);
        }

        ShowSuccessBanner = true;

        // user doesn't see the cookie banner if javascript is disabled, so there'll never be a page to go back to
        //SetPreviousPageLink();
    }

    //private void SetPreviousPageLink()
    //{
    //    var refererUri = new Uri(Request.Headers.Referer);
    //    if (refererUri.LocalPath != Request.Path)
    //    {
    //        LastPage = refererUri.LocalPath;
    //        ShowPreviousPageLink = true;
    //    }
    //    else
    //    {
    //        ShowPreviousPageLink = false;
    //    }
    //}

    /// <summary>
    /// Note: this needs to be compatible with our javascript cookie code, such as cookie-functions.ts
    /// </summary>
    private void SetConsentCookie(HttpRequest request, HttpResponse response, bool analyticsAllowed)
    {
        //todo: Response.Cookies has a static EnableCookieNameEncoding - can we use that and switch to Append??
        var cookieOptions = new CookieOptions
        {
            Expires = DateTime.Now.AddDays(365),
            Path = "/",
            SameSite = SameSiteMode.Strict
        };

        if (request.IsHttps)
        {
            cookieOptions.Secure = true;
        }

        response.AppendRawCookie(_analyticsOptions!.CookieName,
            $$"""{"analytics": {{analyticsAllowed.ToString(CultureInfo.InvariantCulture).ToLowerInvariant()}}, "version": {{_analyticsOptions!.CookieVersion}}}""", cookieOptions);
    }

    private void ResetAnalyticCookies(HttpRequest request, HttpResponse response)
    {
        foreach (var uaCookie in request.Cookies.Where(c => c.Key.StartsWith("_ga")))
        {
            DeleteCookies(response, uaCookie.Key);
        }

        DeleteCookies(response, "_gid");
    }

    /// <summary>
    /// Asks the browser to deletes the supplied cookies.
    /// </summary>
    private void DeleteCookies(HttpResponse response, params string[] cookies)
    {
        foreach (var cookie in cookies)
        {
            //todo: cookieoptions for domain?
            response.Cookies.Delete(cookie);
        }
    }
}