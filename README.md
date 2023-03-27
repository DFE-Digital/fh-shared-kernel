# fh-shared-kernel
DDD shared kernel for the family hubs projects containing artefacts that can be shared across projects. 

# Family Hub web framework and components

This solution contains the source for a NPM package (familyhubs-frontend) and Razor Class Library (FamilyHubs.SharedKernel.Razor), that work together to help quickly create new Family Hubs websites and add common UI components to them.

## Version numbers

To ease testing, we should keep the version number of the NPM package and the Razor Class Library in sync. Consumers should then ensure that both packages are on the same version.

The version of the familyhubs-frontend package is given in its package.json file, as the value of the version property.

The version of the FamilyHubs.SharedKernel.Razor package is given in its FamilyHubs.SharedKernel.Razor.csproj file, as the value of the VersionPrefix property.

## familyhubs-frontend

To publish this npm package, you’ll need to follow these steps:

* Create a user account on the npm website if you don’t already have one.
* In your terminal or command prompt, navigate to the `familyhubs-frontend` directory, containing the package files.
* Run the `npm login` command and enter your npm username, password, and email when prompted.
* Update the package.json file in the package directory with the version number synced to the FamilyHubs.SharedKernel.Razor version.
* Run the `npm publish` command to publish the package to the npm registry.

After publishing the package, it will be available for others to install and use nearly instantaneously.

It's best to reference the package using its exact version number, otherwise it might not pick up the latest, just published version.

## FamilyHubs.SharedKernel.Razor

The package is automatically built when the solution is built.

It is currently not published automatically to the NuGet feed, and needs to be automatically uploaded to NuGet.
