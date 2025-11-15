using System.IdentityModel.Tokens.Jwt;

using Microsoft.AspNetCore.Authentication.Cookies;

using Duende.Bff.Yarp;

using Common;
using Common.Microservices;

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
        options.Cookie.Name = CookieNames.MICROSERVICE_PRODUCTS_HOST_BFF;  // "__Host-products-bff";
		options.Cookie.SameSite = SameSiteMode.None;    // "Strict" can be used only if this app is NOT going to be embedded in an iframe on a different domain.
		options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    })
    .AddOpenIdConnect("oidc", options =>
    {
        options.Authority = IDP.Authority;  //  builder.Configuration["IdentityServer:Authority"];
        options.ClientId = ProductsMicroservice.CLIENT_ID;  // builder.Configuration["IdentityServer:ClientId"];
        options.ClientSecret = ProductsMicroservice.CLIENT_SECRET; // builder.Configuration["IdentityServer:ClientSecret"];
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

        options.Scope.Add (MicroserviceApiResources.PRODUCTS_API);  //  "products_api");

        options.TokenValidationParameters.NameClaimType = "name";
        options.TokenValidationParameters.RoleClaimType = "role";
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers();

builder.Services.AddHttpClient("products-api", client =>
{
    client.BaseAddress = new Uri (ProductsMicroservice.API_BASE_URL!);  //  builder.Configuration["Microservices:ProductsApi"]!);
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

app.MapControllers ();

app.MapBffManagementEndpoints();

// TODO: Is the below correct?
app.MapRemoteBffApiEndpoint("/api/products", "https://localhost:44363")
    .RequireAccessToken(Duende.Bff.TokenType.User);

app.MapFallbackToFile ("/products/webUI/{*path:nonfile}", "index.html");
app.MapFallbackToFile("index.html");

app.Run();