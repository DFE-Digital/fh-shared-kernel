using FamilyHubs.SharedKernel.GovLogin.AppStart;

var builder = WebApplication.CreateBuilder(args);

// *****  REQUIRED SECTION START
builder.Services.AddAndConfigureGovUkAuthentication(builder.Configuration); 
// *****  REQUIRED SECTION END

// *****  CALL_API_EXAMPLE SECTION START
builder.Services.AddSecureHttpClient("TestClient", (serviceProvider, httpClient) =>
{
    httpClient.BaseAddress = new Uri("https://localhost:7101/");
});
// *****  CALL_API_EXAMPLE SECTION END


builder.Services.AddRazorPages();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// *****  REQUIRED SECTION START
app.UseGovLoginAuthentication();
// *****  REQUIRED SECTION END

app.MapRazorPages();

app.Run();
