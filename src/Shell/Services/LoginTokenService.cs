using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.IdentityModel.Tokens;

using FW.PAS.BFFWeb.Entities;
using FW.Landscape.Common;

namespace FW.PAS.BFFWeb.Services;

public sealed class LoginTokenService
	: ILoginTokenService
{
	private readonly IConfiguration _configReader;
	private readonly string _issuer;
	private readonly string _signingKey;
	private readonly Dictionary<PasMicroservice, string> _audienceDictionary =
		new ()
		{
			{ PasMicroservice.Products, PASShellBFF.SHELL_BFF_TOKEN_AUDIENCE_FOR_PRODUCTS },
		};

	public LoginTokenService (IConfiguration config)
	{
		_configReader = config;     // Use it later after demo. As of now, the details are in PASShellBFF static class.

		_issuer = PASShellBFF.SHELL_BFF_TOKEN_ISSUER; // config ["BffLoginToken:Issuer"] ?? "shell-bff";
		_signingKey = PASShellBFF.SHELL_BFF_TOKEN_SIGNING_KEY;  //  config ["BffLoginToken:SigningKey"] ?? throw new Exception ("Missing signing key.");
	}

	public string CreateLoginToken (ClaimsPrincipal principal, PasMicroservice pasMicroservice)
	{
		var audience = _audienceDictionary [pasMicroservice];

		var key = new SymmetricSecurityKey (Encoding.UTF8.GetBytes (_signingKey));
		var creds = new SigningCredentials (key, SecurityAlgorithms.HmacSha256);

		var claims = principal.Claims
			.Where (c => c.Type != "nbf" && c.Type != "exp")
			.ToList ();

		var tokenDescriptor = new SecurityTokenDescriptor
		{
			Issuer = _issuer,
			Audience = audience,
			Subject = new ClaimsIdentity (claims),
			Expires = DateTime.UtcNow.AddSeconds (30),
			NotBefore = DateTime.UtcNow,
			SigningCredentials = creds
		};

		var handler = new JwtSecurityTokenHandler ();
		var token = handler.CreateToken (tokenDescriptor);

		return handler.WriteToken (token);
	}

	public ClaimsPrincipal? ValidateLoginToken (string token, PasMicroservice pasMicroservice)
	{
		try
		{
			var audience = _audienceDictionary [pasMicroservice];

			var handler = new JwtSecurityTokenHandler ();
			var parameters = new TokenValidationParameters
			{
				ValidateLifetime = true,
				ValidateIssuer = true,
				ValidateAudience = true,
				ValidateIssuerSigningKey = true,

				ValidIssuer = _issuer,
				ValidAudience = audience,
				IssuerSigningKey = new SymmetricSecurityKey (
					Encoding.UTF8.GetBytes (_signingKey))
			};

			var principal = handler.ValidateToken (token, parameters, out _);
			return principal;
		}
		catch
		{
			return null;
		}
	}
}