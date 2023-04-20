Read Me

============= appsettings.json =============
4 values need to be configured for this example to work

BaseUrl - This is the url to the gov oidc service
PrivateKey - Key obtained from the private_key.pem
ClientId - ClientId provided by the gov oneLogin team
SignedOutRedirect - Must match the path to the signed-out page when the app is running, 
					this is where the ui will redirect to once sign out is complete

Optional
IdamsApiBaseUrl - This endpoint provides custom claims, if not populated set StubAuthentication.UseStubClaims


================= sign in =================

AddAndConfigureGovUkAuthentication - serviceCollection extension registers all required functionality for 
									sign in to function.

Decorate the page endpoint with [Authorize] attribute. 

Upon navigating to the page the you will be redirected to the Gov Login pages.

After login you will be redirected back to the page with the [Authorize] attribute.

NOTE- If using the integration environment the a popup will appear requesting username and password. 
	This is an extra step which will not occur in the production environment


================ sign out =================

For Sign out to function direct the users to a page (path must be /Account/signout) which will return the response from 
HttpContext.GovSignOut().
This page will not get rendered. The page the user will land on will be the one specified in appsettings - SignedOutRedirect