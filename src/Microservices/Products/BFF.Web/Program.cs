using System.IdentityModel.Tokens.Jwt;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

using Duende.Bff.Yarp;

using FW.Landscape.Common;
using FW.Landscape.Common.Microservices;

var builder = WebApplication.CreateBuilder(args);

JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddOpenIdConnectAccessTokenManagement(options =>
{
    options.RefreshBeforeExpiration = TimeSpan.FromSeconds(60);
});

builder.Services.AddBff()
    .AddRemoteApis();

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = "oidc";
        options.DefaultSignOutScheme = "oidc";
    })
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.Cookie.Name = CookieNames.MICROSERVICE_PRODUCTS_HOST_BFF;
        options.Cookie.Path = "/";
        options.Cookie.SameSite = SameSiteMode.None;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.IsEssential = true;
        options.SlidingExpiration = true;
    })
    .AddOpenIdConnect("oidc", options =>
    {
        options.Authority = IDP.Authority;
        options.ClientId = ProductsMicroservice.CLIENT_ID;
        options.ClientSecret = ProductsMicroservice.CLIENT_SECRET;
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

        options.Scope.Add(MicroserviceApiResources.PRODUCTS_API);

        options.CorrelationCookie.SameSite = SameSiteMode.None;
        options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;
        options.NonceCookie.SameSite = SameSiteMode.None;
        options.NonceCookie.SecurePolicy = CookieSecurePolicy.Always;

        options.GetClaimsFromUserInfoEndpoint = true;

        options.TokenValidationParameters.NameClaimType = "name";
        options.TokenValidationParameters.RoleClaimType = "role";

        options.Events.OnAccessDenied = context =>
        {
            return Task.CompletedTask;
        };

        options.Events.OnAuthenticationFailed = context =>
        {
            return Task.CompletedTask;
        };

        options.Events.OnAuthorizationCodeReceived = context =>
        {
            return Task.CompletedTask;
        };

        options.Events.OnMessageReceived = context =>
        {
            return Task.CompletedTask;
        };

        options.Events.OnRedirectToIdentityProvider = context =>
        {
            if (context.Properties.Items.TryGetValue("prompt", out var prompt))
            {
                context.ProtocolMessage.Prompt = prompt;
            }
            return Task.CompletedTask;
        };

        options.Events.OnRedirectToIdentityProviderForSignOut = context =>
        {
            return Task.CompletedTask;
        };

        options.Events.OnRemoteFailure = context =>
        {
            if (context.Failure?.Message?.Contains("login_required") == true ||
                context.Failure?.Message?.Contains("interaction_required") == true)
            {
                context.HandleResponse();

                // Return a page that tells the parent frame authentication is needed
                context.Response.ContentType = "text/html";
                context.Response.WriteAsync(@"
                    <html>
                    <body>
                        <script>
                            window.parent.postMessage({ type: 'AUTH_REQUIRED' }, '*');
                        </script>
                        <p>Authentication required. Please refresh the main application.</p>
                    </body>
                    </html>
                ");
            }
            return Task.CompletedTask;
        };

        options.Events.OnRemoteSignOut = context =>
        {
            return Task.CompletedTask;
        };

        options.Events.OnSignedOutCallbackRedirect = context =>
        {
            return Task.CompletedTask;
        };

        options.Events.OnTicketReceived = context =>
        {
            var isSuccess = context.Success;
            return Task.CompletedTask;
        };

        options.Events.OnTokenResponseReceived = context =>
        {
            // Access the tokens
            var accessToken = context.TokenEndpointResponse.AccessToken;

            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(accessToken) as JwtSecurityToken;
            var claims = jsonToken.Claims;
            var audience = jsonToken.Claims.FirstOrDefault(c => c.Type == "aud")?.Value;
            var scope = jsonToken.Claims.FirstOrDefault(c => c.Type == "scope")?.Value;

            context.HttpContext.Session.SetString("AccessToken", accessToken);
            return Task.CompletedTask;
        };

        options.Events.OnTokenValidated = context =>
        {
            return Task.CompletedTask;
        };

        options.Events.OnUserInformationReceived = context =>
        {
            return Task.CompletedTask;
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers();

builder.Services.AddHttpClient(MicroserviceApiResources.PRODUCTS_API, client =>
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
// FIXME: Is this required?
app.MapControllerRoute(
    name: "logout-all",
    pattern: "bff/logout-all/{action=Index}",
    defaults: new { controller = "LogoutAll" }
);

app.MapBffManagementEndpoints();

//
// Intentionally commented out to enable this Microservice's SPA set directly to the iFrame of the Shell SPA application.
//
// app.MapFallbackToFile("index.html");
//

app.Run();