using Duende.IdentityServer;
using Duende.IdentityServer.Models;

using FW.Landscape.Common;
using FW.Landscape.Common.Microservices;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        [
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResources.Email(),
            new (name: "roles", displayName: "User Roles", userClaims: [ "role" ])
        ];

    public static IEnumerable<ApiScope> ApiScopes =>
        [
            new (MicroserviceApiResources.PRODUCTS_API, "Products API")
            {
                UserClaims = { "role", "name", "email" }
            }
        ];

    public static IEnumerable<ApiResource> ApiResources =>
        [
            new (MicroserviceApiResources.PRODUCTS_API, "Products API")
            {
                Scopes = { MicroserviceApiResources.PRODUCTS_API },
                UserClaims = { "role", "name", "email" }
            }
        ];

    public static IEnumerable<Client> Clients =>
        [
            // Shell BFF Client
            new()
            {
                ClientId = PASShellBFF.CLIENT_ID,
                ClientName = PASShellBFF.CLIENT_NAME,
                ClientSecrets = { new Secret(PASShellBFF.CLIENT_SECRET.Sha256()) },

                AllowedGrantTypes = GrantTypes.Code,
                
                RedirectUris = { $"{PASShellBFF.CLIENT_BASE_URL }/signin-oidc" },
				PostLogoutRedirectUris = { $"{PASShellBFF.CLIENT_BASE_URL}/signout-callback-oidc" },
				FrontChannelLogoutUri = $"{PASShellBFF.CLIENT_BASE_URL }/signout-oidc",

				AllowOfflineAccess = true,
                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Email,
                    "roles"
                },

                // FIXME: Enable consent later
                // RequireConsent = true,

                RefreshTokenUsage = TokenUsage.ReUse,
                RefreshTokenExpiration = TokenExpiration.Sliding,
                SlidingRefreshTokenLifetime = 3600
            },

            new()
            {
                ClientId = ProductsMicroservice.CLIENT_ID,
                ClientName = ProductsMicroservice.CLIENT_NAME,
                ClientSecrets = { new Secret(ProductsMicroservice.CLIENT_SECRET.Sha256()) },

                AllowedGrantTypes = GrantTypes.Code,

                RedirectUris = { $"{ProductsMicroservice.CLIENT_BASE_URL }/signin-oidc" },
                PostLogoutRedirectUris = { $"{ProductsMicroservice.CLIENT_BASE_URL}/signout-callback-oidc" },
                FrontChannelLogoutUri = $"{ProductsMicroservice.CLIENT_BASE_URL }/signout-oidc",

                AllowOfflineAccess = true,
                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Email,
                    "roles",
                    MicroserviceApiResources.PRODUCTS_API
                },

                RefreshTokenUsage = TokenUsage.ReUse,
                RefreshTokenExpiration = TokenExpiration.Sliding,
                SlidingRefreshTokenLifetime = 3600
            }
		];
}