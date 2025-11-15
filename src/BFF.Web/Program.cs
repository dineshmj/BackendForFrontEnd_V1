using Duende.Bff;
using Duende.Bff.Yarp;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

// Clear default JWT claim mapping
JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

// Add BFF services
builder.Services.AddBff()
    .AddRemoteApis();

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
        options.Cookie.Name = "__Host-bff";
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    })
    .AddOpenIdConnect("oidc", options =>
    {
        options.Authority = builder.Configuration["IdentityServer:Authority"];
        options.ClientId = builder.Configuration["IdentityServer:ClientId"];
        options.ClientSecret = builder.Configuration["IdentityServer:ClientSecret"];
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
        options.Scope.Add("orders_api");
        options.Scope.Add("products_api");
        options.Scope.Add("payments_api");
        options.Scope.Add("offline_access");

        options.TokenValidationParameters.NameClaimType = "name";
        options.TokenValidationParameters.RoleClaimType = "role";
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers();

// Add HTTP clients for microservices
builder.Services.AddHttpClient("orders-api", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Microservices:OrdersApi"]!);
}).AddUserAccessTokenHandler();

builder.Services.AddHttpClient("products-api", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Microservices:ProductsApi"]!);
}).AddUserAccessTokenHandler();

builder.Services.AddHttpClient("payments-api", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Microservices:PaymentsApi"]!);
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
    .RequireAuthorization()
    .AsBffApiEndpoint();

app.MapBffManagementEndpoints();

// Proxy endpoints to microservices
app.MapRemoteBffApiEndpoint("/api/orders", "https://localhost:44349")
    .RequireAccessToken(Duende.Bff.TokenType.User);

app.MapRemoteBffApiEndpoint("/api/products", "https://localhost:44363")
    .RequireAccessToken(Duende.Bff.TokenType.User);

app.MapRemoteBffApiEndpoint("/api/payments", "https://localhost:44309")
    .RequireAccessToken(Duende.Bff.TokenType.User);

app.MapFallbackToFile("index.html");

app.Run();
