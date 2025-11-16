using System.IdentityModel.Tokens.Jwt;

using Microsoft.AspNetCore.Authentication.Cookies;

using Duende.Bff.Yarp;

using FW.Landscape.Common;
using FW.Landscape.Common.Microservices;
using FW.PAS.BFFWeb.Services;

var builder = WebApplication.CreateBuilder (args);

JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

builder.Services.AddBff ()
	.AddRemoteApis ();

builder.Services.AddSingleton<ILoginTokenService, LoginTokenService> ();

builder.Services
	.AddAuthentication (options =>
	{
		options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
		options.DefaultChallengeScheme = "oidc";
		options.DefaultSignOutScheme = "oidc";
	})
	.AddCookie (CookieAuthenticationDefaults.AuthenticationScheme, options =>
	{
		options.Cookie.Name = CookieNames.PAS_SHELL_HOST_BFF;
		options.Cookie.Path = "/";
		options.Cookie.SameSite = SameSiteMode.Lax;
		options.Cookie.HttpOnly = true;
		options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
	})
	.AddOpenIdConnect ("oidc", options =>
	{
		options.Authority = IDP.Authority;
		options.ClientId = PASShellBFF.CLIENT_ID;
		options.ClientSecret = PASShellBFF.CLIENT_SECRET;
		options.ResponseType = "code";
		options.ResponseMode = "query";

		options.GetClaimsFromUserInfoEndpoint = true;
		options.MapInboundClaims = false;
		options.SaveTokens = true;

		options.Scope.Clear ();
		options.Scope.Add ("openid");
		options.Scope.Add ("profile");
		options.Scope.Add ("email");
		options.Scope.Add ("offline_access");

		options.TokenValidationParameters.NameClaimType = "name";
		options.TokenValidationParameters.RoleClaimType = "role";
	});

builder.Services.AddAuthorization ();
builder.Services.AddControllers ();

builder.Services.AddHttpClient ("products-api", client =>
{
	client.BaseAddress = new Uri (ProductsMicroservice.API_BASE_URL);
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

// Protect all controllers by default except LogoutAllController.
app.MapControllers ()
	.RequireAuthorization ();

// LogoutAll action method must be public and hence it is not protected by default authorization.
app.MapControllerRoute (
	name: "logout-all",
	pattern: "bff/logout-all/{action=Index}",
	defaults: new { controller = "LogoutAll" }
);

app.MapBffManagementEndpoints ();
app.MapFallbackToFile ("index.html");

app.Run ();