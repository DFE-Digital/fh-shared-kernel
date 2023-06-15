Read Me

============= appsettings.json =============
5 values need to be configured for this example to work

BearerTokenSigningKey	- This key is used to sign the Bearer Token
BaseUrl					- This is the url to the gov oidc service
PrivateKey				- Key obtained from the private_key.pem
ClientId				- ClientId provided by the gov oneLogin team
SignedOutRedirect		- Must match the path to the signed-out page when the app is running, 
						this is where the ui will redirect to once sign out is complete
AppHost					- This is the Host path of the current app, used to redirect back to after login

Optional
IdamsApiBaseUrl			- This endpoint provides custom claims, if not populated set StubAuthentication.UseStubClaims
Urls.NoClaimsRedirect	- If this is populated the middleware will redirect to this url when no claims are present

Stub Settings
Note - Stub settings will only be used by a UI logging in. However, a user who has logged in via the stubbed settings will still
be able to call an API with the generated bearer token as long as the StubPrivateKey matches the Private Key configured in the API

StubAuthentication.UseStubAuthentication	-	If True will on login will display a list of stub users to choose from and log in with
StubAuthentication.StubUsers				-	If above setting is true the users (along with their claims) configured in this setting 
											will be returned. This does not need to be configured in the appsettings.json but can be
											configured in the stubUsers.json to keep the appsettings.json file more readable
StubAuthentication.UseStubClaims			-	If true will append stubbed claims to a one-login user
StubAuthentication.StubClaims				-	Only used if the UseStubAuthentication = false and UseStubClaims = true



================= sign in =================

AddAndConfigureGovUkAuthentication - serviceCollection extension registers all required functionality for 
									sign in to function.

Decorate the page endpoint with [Authorize] attribute. 

Upon navigating to the page the you will be redirected to the Gov Login pages.

After login you will be redirected back to the page with the [Authorize] attribute.

NOTE- If using the integration environment the a popup will appear requesting username and password. 
	This is an extra step which will not occur in the production environment


================ sign out =================

For Sign out direct the users to /Account/signout which will redirect to the page specified in appsettings - SignedOutRedirect