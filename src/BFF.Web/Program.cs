using System.IdentityModel.Tokens.Jwt;

using Microsoft.AspNetCore.Authentication.Cookies;

using Common;
using Common.Microservices;
using Duende.Bff.Yarp;

var builder = WebApplication.CreateBuilder(args);

// Clear default JWT claim mapping
JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

// Add BFF services
builder.Services.AddBff()
    .AddRemoteApis();

builder.Services.AddSingleton<ILoginTokenService, LoginTokenService> ();

// Add authentication
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = "oidc";
        options.DefaultSignOutScheme = "oidc";
    })
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.Cookie.Name = CookieNames.PAS_SHELL_HOST_BFF; // "__PAS-Shell-Host-bff";
		options.Cookie.SameSite = SameSiteMode.Lax; // SameSiteMode.Strict is not used becuse the app will not render inside an iFrame. Lax is used to allow iframe use case.
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    })
    .AddOpenIdConnect("oidc", options =>
    {
        // NOTE: Replace with true configuration values in a production deployment. The "constants" here are for demo purposes only.

        options.Authority = IDP.Authority;  // builder.Configuration["IdentityServer:Authority"];
        options.ClientId = PASShellBFF.CLIENT_ID; // builder.Configuration["IdentityServer:ClientId"];
        options.ClientSecret =  PASShellBFF.CLIENT_SECRET; // builder.Configuration["IdentityServer:ClientSecret"];
        options.ResponseType = "code";
        options.ResponseMode = "query";

        options.GetClaimsFromUserInfoEndpoint = true;
        options.MapInboundClaims = false;
        options.SaveTokens = true;

        // Scopes
        options.Scope.Clear();
        options.Scope.Add("openid");
        options.Scope.Add("profile");
        options.Scope.Add("email");

		options.Scope.Add("offline_access");

        options.TokenValidationParameters.NameClaimType = "name";
        options.TokenValidationParameters.RoleClaimType = "role";
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers();

builder.Services.AddHttpClient("products-api", client =>
{
    client.BaseAddress = new Uri (ProductsMicroservice.API_BASE_URL);    // builder.Configuration["Microservices:ProductsApi"]!);
}).AddUserAccessTokenHandler();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseBff();
app.UseAuthorization();

app.MapControllers()
    .RequireAuthorization();

app.MapBffManagementEndpoints();

app.MapFallbackToFile("index.html");

app.Run();