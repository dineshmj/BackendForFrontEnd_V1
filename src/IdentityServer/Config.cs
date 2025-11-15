using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace IdentityServer;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new List<IdentityResource>
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResources.Email()
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new List<ApiScope>
        {
            new ApiScope("orders_api", "Orders API"),
            new ApiScope("products_api", "Products API"),
            new ApiScope("payments_api", "Payments API")
        };

    public static IEnumerable<Client> Clients =>
        new List<Client>
        {
            // BFF Client
            new Client
            {
                ClientId = "bff-client",
                ClientName = "BFF Application",
                ClientSecrets = { new Secret("secret".Sha256()) },

                AllowedGrantTypes = GrantTypes.Code,
                
                RedirectUris = { "https://localhost:44367/signin-oidc" },
                PostLogoutRedirectUris = { "https://localhost:44367/signout-callback-oidc" },
                FrontChannelLogoutUri = "https://localhost:44367/signout-oidc",

                AllowOfflineAccess = true,
                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Email,
                    "orders_api",
                    "products_api",
                    "payments_api"
                },

                RefreshTokenUsage = TokenUsage.ReUse,
                RefreshTokenExpiration = TokenExpiration.Sliding,
                SlidingRefreshTokenLifetime = 3600
            }
        };
}
