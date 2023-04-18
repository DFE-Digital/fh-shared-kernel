using FamilyHubs.SharedKernel.GovLogin.AppStart;
using FamilyHubs.SharedKernel.GovLogin.Configuration;
using GovSignInExample.AppStart;

var builder = WebApplication.CreateBuilder(args);


// *****  REQUIRED SECTION START
builder.Services.AddHttpContextAccessor();
builder.Services.Configure<GovUkOidcConfiguration>(builder.Configuration.GetSection(nameof(GovUkOidcConfiguration)));
builder.Services.AddServiceRegistration(builder.Configuration);
// *****  REQUIRED SECTION END

// *****  CALL_API_EXAMPLE SECTION START
builder.Services.AddSecureClient("TestClient", (serviceProvider, httpClient) =>
{
    httpClient.BaseAddress = new Uri("https://localhost:7101/");
});
// *****  CALL_API_EXAMPLE SECTION END


// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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
