using System.IdentityModel.Tokens.Jwt;

using Microsoft.AspNetCore.Authentication.Cookies;

using Duende.Bff.Yarp;

using FW.Landscape.Common;
using FW.Landscape.Common.Microservices;

var builder = WebApplication.CreateBuilder (args);

JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

builder.Services.AddBff ()
	.AddRemoteApis ();

builder.Services.AddAuthentication (options =>
{
	options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = "oidc";
	options.DefaultSignOutScheme = "oidc";
})
.AddCookie (CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
	options.Cookie.Name = CookieNames.MICROSERVICE_PRODUCTS_HOST_BFF;
	// options.Cookie.Path = "/";
	options.Cookie.SameSite = SameSiteMode.None;
	options.Cookie.HttpOnly = true;
	options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
})
.AddOpenIdConnect ("oidc", options =>
{
	options.Authority = IDP.Authority;
	options.ClientId = ProductsMicroservice.CLIENT_ID;
	options.ClientSecret = ProductsMicroservice.CLIENT_SECRET;

	options.ResponseType = "code";
	options.GetClaimsFromUserInfoEndpoint = true;
	options.MapInboundClaims = false;
	options.SaveTokens = true;

	options.Scope.Clear ();
	options.Scope.Add ("openid");
	options.Scope.Add ("profile");
	options.Scope.Add ("email");
	options.Scope.Add (MicroserviceApiResources.PRODUCTS_API);

	options.TokenValidationParameters.NameClaimType = "name";
	options.TokenValidationParameters.RoleClaimType = "role";
});

builder.Services.AddAuthorization ();
builder.Services.AddControllers ();

builder.Services.AddHttpClient ("products-api", client =>
{
	client.BaseAddress = new Uri (ProductsMicroservice.API_BASE_URL!);
}).AddUserAccessTokenHandler ();

var app = builder.Build ();

if (app.Environment.IsDevelopment ())
	app.UseDeveloperExceptionPage ();
else
	app.UseHsts ();

app.UseHttpsRedirection ();
app.UseDefaultFiles ();
app.UseStaticFiles ();

app.UseRouting ();
app.UseAuthentication ();
app.UseBff ();
app.UseAuthorization ();

app.MapBffManagementEndpoints ();

// Secure all controllers (except ReceiveLoginController) by default.
app.MapControllers ()
   .RequireAuthorization ();

// Allow /receive-login anonymously
app.MapControllerRoute (
	name: "receive-login",
	pattern: "receive-login",
	defaults: new { controller = "ReceiveLogin", action = "ReceiveLogin" }
); // no RequireAuthorization()

app.MapFallbackToFile ("/products/webUI/{*path:nonfile}", "index.html")
   .RequireAuthorization ();

app.MapFallbackToFile ("index.html");

app.Run ();