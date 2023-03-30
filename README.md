# fh-shared-kernel
DDD shared kernel for the family hubs projects containing artefacts that can be shared across projects. 

# Family Hub web framework and components

This solution contains the source for a NPM package (familyhubs-frontend) and Razor Class Library (FamilyHubs.SharedKernel.Razor), that work together to help quickly create new Family Hubs websites and add common UI components to them.

## Consuming the packages

Install the familyhubs-frontend package into the website project using the following command:

```
npm install familyhubs-frontend
```

Installing the package, will add files to the wwwroot folder. (todo document which files)

In the styles/application.scss file, add the following line:

```
@import "../node_modules/familyhubs-frontend/styles/all";
```

Add the FamilyHubs.SharedKernel.Razor package to the website project.

The FamilyHubs.SharedKernel.Razor package contains:

* the layout
* common shared partial views
* todo add rest here

Check that the npm package and the Razor Class Library are on the same version.

Add the configuration section to the appsettings.json file of the website project.

### Configuration

Here's an example configuration section that should be added to the appsettings.json file of a Family Hubs website:

```json
  "FamilyHubsUi": {
    "ServiceName": "Manage family support services and accounts",
    "Phase": "Beta",
    "FeedbackUrl": "https://example.com/feedback",
    "Analytics": {
      "CookieName": "manage_family_support_cookies_policy",
      "CookieVersion": 1,
      "MeasurementId": "",
      "ContainerId": ""
    },
    "Footer": {
      "Links": [
        { "Text": "Accessibility" },
        { "Text": "Contact Us" },
        { "Text": "Cookies" },
        { "Text": "Feedback", "ConfigUrl": "FamilyHubsUi:FeedbackUrl" },
        { "Text": "Terms and conditions" }
      ] 
    } 
```

Notes:

* Google Analytics is only enabled if the MeasurementId and ContainerId are set.

* The Options classes have XML documentation on the properties.

## Version numbers

To ease testing, we should keep the version number of the NPM package and the Razor Class Library in sync. Consumers should then ensure that both packages are on the same version.

The version of the familyhubs-frontend package is given in its package.json file, as the value of the version property.

The version of the FamilyHubs.SharedKernel.Razor package is given in its FamilyHubs.SharedKernel.Razor.csproj file, as the value of the VersionPrefix property.

## familyhubs-frontend

To publish this npm package, you�ll need to follow these steps:

* Create a user account on the npm website if you don�t already have one.
* In your terminal or command prompt, navigate to the `familyhubs-frontend` directory, containing the package files.
* Run the `npm login` command and enter your npm username, password, and email when prompted.
* Update the package.json file in the package directory with the version number synced to the FamilyHubs.SharedKernel.Razor version.
* Run the `npm publish` command to publish the package to the npm registry.

After publishing the package, it will be available for others to install and use nearly instantaneously.

It's best to reference the package using its exact version number, otherwise it might not pick up the latest, just published version.

## FamilyHubs.SharedKernel.Razor

The package is automatically built when the solution is built.

It is not currently published automatically to the NuGet feed, and needs to be manually uploaded to NuGet.

## Components

### Cookie page

Call `AddCookiePage()` on your `IServiceCollection`, like so...

```
    services.AddCookiePage(configuration);
```

Create a new Razor Page. Inject ICookiePage into the PageModel's constructor, stash it away, then pass it to the cookie page partial in the View.

To add support for users running without Javascript, add an OnPost method as per the example.

E.g.

```
public class IndexModel : PageModel
{
    public readonly ICookiePage CookiePage;

    public IndexModel(ICookiePage cookiePage)
    {
        CookiePage = cookiePage;
    }

    public void OnPost(bool analytics)
    {
        CookiePage.OnPost(analytics, Request, Response);
    }
}
```

and add in your view...

```
    <partial name="~/Pages/Shared/_CookiePage.cshtml" model="Model.CookiePage"/>
```

Add a partial view called `Pages/Shared/_CookiePolicy.cshtml` and add the cookie policy content into it.

If you want to pick up the cookie policy content from a different partial view, pass its name into `AddCookiePage()`, e.g.

```
    services.AddCookiePage(configuration, "SomeOtherView.cshtml");
```
