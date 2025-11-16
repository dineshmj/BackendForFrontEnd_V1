using Duende.IdentityServer;
using Duende.IdentityServer.Models;

using FW.Landscape.Common;
using FW.Landscape.Common.Microservices;

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
            new ApiScope(MicroserviceApiResources.PRODUCTS_API, "Products API"),
        };

    public static IEnumerable<Client> Clients =>
        new List<Client>
        {
            // Shell BFF Client
            new Client
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
                },

                RefreshTokenUsage = TokenUsage.ReUse,
                RefreshTokenExpiration = TokenExpiration.Sliding,
                SlidingRefreshTokenLifetime = 3600
            },

            // Products Microservice BFF Client
			new Client
			{
				ClientId = ProductsMicroservice.CLIENT_ID,
				ClientName = ProductsMicroservice.CLIENT_NAME,
				ClientSecrets = { new Secret(ProductsMicroservice.CLIENT_SECRET.Sha256()) },

				AllowedGrantTypes = GrantTypes.Code,

                RedirectUris = { $"{ ProductsMicroservice.CLIENT_BASE_URL }/signin-oidc" },
				PostLogoutRedirectUris = { $"{ProductsMicroservice.CLIENT_BASE_URL}/signout-callback-oidc" },

				FrontChannelLogoutUri = $"{ProductsMicroservice.CLIENT_BASE_URL }/signout-oidc",
                // ↓↓ NOT WORKING!
                // FrontChannelLogoutUri = $"{ProductsMicroservice.CLIENT_BASE_URL }/bff/logout/frontchannel",
				FrontChannelLogoutSessionRequired = true,       // to include sid claim in id_token

                BackChannelLogoutUri = $"{ProductsMicroservice.CLIENT_BASE_URL}/bff/backchannel",
	            BackChannelLogoutSessionRequired = true,

				AllowOfflineAccess = true,
				AllowedScopes =
				{
					IdentityServerConstants.StandardScopes.OpenId,
					IdentityServerConstants.StandardScopes.Profile,
					IdentityServerConstants.StandardScopes.Email,
                    MicroserviceApiResources.PRODUCTS_API
				},

				RefreshTokenUsage = TokenUsage.ReUse,
				RefreshTokenExpiration = TokenExpiration.Sliding,
				SlidingRefreshTokenLifetime = 3600
			}
		};
}