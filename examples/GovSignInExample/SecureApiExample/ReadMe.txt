Read Me

============= appsettings.json =============
1 value need to be configured for this example to work

BearerTokenSigningKey	- This key is used to sign the Bearer Token



================= Program.cs =================

Only one line is needed for the bearer token authorization -> builder.Services.AddBearerAuthentication(builder.Configuration);


