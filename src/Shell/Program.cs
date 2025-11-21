using System.IdentityModel.Tokens.Jwt;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

using Duende.Bff.Yarp;

using FW.Landscape.Common;
using FW.Landscape.Common.Microservices;
using FW.PAS.BFFWeb.DBAccess;

var builder = WebApplication.CreateBuilder(args);

JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddDbContext<MenuDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("MenuDbConnection")));

builder.Services.AddBff()
    .AddRemoteApis();

builder.Services.AddScoped<IMenuRepository, MenuRepository>();

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = "oidc";
        options.DefaultSignOutScheme = "oidc";
    })
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.Cookie.Name = CookieNames.PAS_SHELL_HOST_BFF;
        options.Cookie.Path = "/";
        options.Cookie.SameSite = SameSiteMode.None;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    })
    .AddOpenIdConnect("oidc", options =>
    {
        options.Authority = IDP.Authority;
        options.ClientId = PASShellBFF.CLIENT_ID;
        options.ClientSecret = PASShellBFF.CLIENT_SECRET;
        options.ResponseType = "code";
        options.ResponseMode = "query";

        options.MapInboundClaims = false;
        options.ClaimActions.MapJsonKey("role", "role", "role");
        options.SaveTokens = true;

        options.Scope.Clear();
        options.Scope.Add("openid");
        options.Scope.Add("profile");
        options.Scope.Add("email");
        options.Scope.Add("roles");
        options.Scope.Add("offline_access");

        options.CorrelationCookie.SameSite = SameSiteMode.None;
        options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;
        options.NonceCookie.SameSite = SameSiteMode.None;
        options.NonceCookie.SecurePolicy = CookieSecurePolicy.Always;

        options.GetClaimsFromUserInfoEndpoint = true;

        options.TokenValidationParameters.NameClaimType = "name";
        options.TokenValidationParameters.RoleClaimType = "role";
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers();

builder.Services.AddHttpClient("products-api", client =>
{
    client.BaseAddress = new Uri(ProductsMicroservice.API_BASE_URL);
}).AddUserAccessTokenHandler();

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();
else
    app.UseHsts();

app.UseSession();

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();

app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/bff/api"))
    {
        // Rewrite the path to remove /bff
        context.Request.Path = context.Request.Path.Value.Replace("/bff", "");
        context.SetEndpoint(null);
    }
    await next();
});

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseBff();

// Protect all controllers by default except LogoutAllController.
app.MapControllers()
    .RequireAuthorization();

// LogoutAll action method must be public and hence it is not protected by default authorization.
app.MapControllerRoute(
    name: "logout-all",
    pattern: "bff/logout-all/{action=Index}",
    defaults: new { controller = "LogoutAll" }
);

app.MapBffManagementEndpoints();

//
// Intentionally commented out to enable this Microservice's SPA set directly to the iFrame of the Shell SPA application and avoid unnecessary redirects
// within the PAS Shell BFF.
//
// app.MapFallbackToFile("index.html");
//

app.Run();