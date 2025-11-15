using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

using Common;

[ApiController]
[AllowAnonymous]
[Route ("receive-login")]
public sealed class ReceiveLoginController
	: Controller
{
	private readonly IConfiguration _config;

	public ReceiveLoginController (IConfiguration config)
	{
		_config = config;
	}

	[HttpGet]
	[IgnoreAntiforgeryToken]        // Required, because `ReceiveLogin` is triggered by a redirect, not an AJAX POST.
	public IActionResult ReceiveLogin ([FromQuery] string token)
	{
		if (string.IsNullOrWhiteSpace (token))
		{
			return BadRequest ("Missing token");
		}

		var key = new SymmetricSecurityKey (
			Encoding.UTF8.GetBytes (PASShellBFF.SHELL_BFF_TOKEN_SIGNING_KEY) //_config ["ForwardLogin:SigningKey"])
		);

		var handler = new JwtSecurityTokenHandler ();
		try
		{
			var principal = handler.ValidateToken (token, new TokenValidationParameters
			{
				ValidateIssuer = true,
				ValidateAudience = true,
				ValidIssuer = PASShellBFF.SHELL_BFF_TOKEN_ISSUER,
				ValidAudience = PASShellBFF.SHELL_BFF_TOKEN_AUDIENCE_FOR_PRODUCTS,
				IssuerSigningKey = key,
				ValidateLifetime = true,
				ClockSkew = TimeSpan.FromSeconds (5)
			}, out var securityToken);

			var claims = principal.Claims.ToList ();

			var id = new ClaimsIdentity (claims, "Cookies");
			HttpContext.SignInAsync (CookieNames.MICROSERVICE_PRODUCTS_HOST_BFF, new ClaimsPrincipal (id));

			// Redirect user to Products web UI frontend
			return Redirect ("~/");
		}
		catch (Exception ex)
		{
			Console.WriteLine ("Token validation failed: " + ex.Message);
			return Unauthorized ("Invalid token");
		}
	}
}