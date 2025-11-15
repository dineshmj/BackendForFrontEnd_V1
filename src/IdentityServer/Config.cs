using Common;
using Common.Microservices;
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
            // new ApiScope(/* "orders_api" */, "Orders API"),
            new ApiScope(/* "products_api" */MicroserviceApiResources.PRODUCTS_API, "Products API"),
            // new ApiScope(/* "payments_api" */, "Payments API")
        };

    public static IEnumerable<Client> Clients =>
        new List<Client>
        {
            // BFF Client
            new Client
            {
                ClientId = PASShellBFF.CLIENT_ID,  // "bff-client",
                ClientName = PASShellBFF.CLIENT_NAME, // "BFF Application",
                ClientSecrets = { new Secret(PASShellBFF.CLIENT_SECRET.Sha256()) },  // { new Secret("secret".Sha256()) },

                AllowedGrantTypes = GrantTypes.Code,
                
                // RedirectUris = { "https://localhost:44367/signin-oidc" },
                // PostLogoutRedirectUris = { "https://localhost:44367/signout-callback-oidc" },
                // FrontChannelLogoutUri = "https://localhost:44367/signout-oidc",

                RedirectUris = { $"{PASShellBFF.CLIENT_BASE_URL }/signin-oidc" },
				PostLogoutRedirectUris = { $"{PASShellBFF.CLIENT_BASE_URL}/signout-callback-oidc" },
				FrontChannelLogoutUri = $"{PASShellBFF.CLIENT_BASE_URL }/signout-oidc",

				AllowOfflineAccess = true,
                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Email,
                    //"orders_api",         // Commented out because BFF Shell does not directly call APIs.
                    //"products_api",
                    //"payments_api"
                },

                RefreshTokenUsage = TokenUsage.ReUse,
                RefreshTokenExpiration = TokenExpiration.Sliding,
                SlidingRefreshTokenLifetime = 3600
            },
			new Client
			{
				ClientId = ProductsMicroservice.CLIENT_ID,  // "bff-client-products",
				ClientName = ProductsMicroservice.CLIENT_NAME, // "Product BFF Application",
				ClientSecrets = { new Secret(ProductsMicroservice.CLIENT_SECRET.Sha256()) }, // { new Secret("product-secret".Sha256()) },

				AllowedGrantTypes = GrantTypes.Code,

				// RedirectUris = { "https://localhost:54367/signin-oidc" },
				// PostLogoutRedirectUris = { "https://localhost:54367/signout-callback-oidc" },
				// FrontChannelLogoutUri = "https://localhost:54367/signout-oidc",

                RedirectUris = { $"{ ProductsMicroservice.CLIENT_BASE_URL }/signin-oidc" },
				PostLogoutRedirectUris = { $"{ProductsMicroservice.CLIENT_BASE_URL}/signout-callback-oidc" },
				FrontChannelLogoutUri = $"{ProductsMicroservice.CLIENT_BASE_URL }/signout-oidc",

				AllowOfflineAccess = true,
				AllowedScopes =
				{
					IdentityServerConstants.StandardScopes.OpenId,
					IdentityServerConstants.StandardScopes.Profile,
					IdentityServerConstants.StandardScopes.Email,
                    MicroserviceApiResources.PRODUCTS_API
					// "orders_api"
				},

				RefreshTokenUsage = TokenUsage.ReUse,
				RefreshTokenExpiration = TokenExpiration.Sliding,
				SlidingRefreshTokenLifetime = 3600
			}
		};
}
