using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

using FW.Landscape.Common;

namespace FW.Microservices.Products.BFFWeb.Controllers;

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
	[IgnoreAntiforgeryToken]   // required for redirect-based login
	public async Task<IActionResult> ReceiveLogin ([FromQuery] string token)
	{
		if (string.IsNullOrWhiteSpace (token))
			return BadRequest ("Missing token");

		var key = new SymmetricSecurityKey (
			Encoding.UTF8.GetBytes (PASShellBFF.SHELL_BFF_TOKEN_SIGNING_KEY)
		);

		var handler = new JwtSecurityTokenHandler ();

		try
		{
			var principal = handler.ValidateToken (
				token,
				new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidIssuer = PASShellBFF.SHELL_BFF_TOKEN_ISSUER,
					ValidAudience = PASShellBFF.SHELL_BFF_TOKEN_AUDIENCE_FOR_PRODUCTS,
					IssuerSigningKey = key,
					ValidateLifetime = true,
					ClockSkew = TimeSpan.FromSeconds (5)
				},
				out var securityToken
			);

			// Create cookie identity
			var claims = principal.Claims.ToList ();
			var identity = new ClaimsIdentity (
				claims,
				CookieAuthenticationDefaults.AuthenticationScheme
			);

			// IMPORTANT: await SignInAsync so cookie is written to response
			await HttpContext.SignInAsync (
				CookieAuthenticationDefaults.AuthenticationScheme,              // Duende BFF expects the cookie under the default scheme ("Cookies").
				new ClaimsPrincipal (identity)
			);

			// redirect to Products UI home
			return Redirect ("~/");
		}
		catch (Exception ex)
		{
			Console.WriteLine ("Token validation failed: " + ex.Message);
			return Unauthorized ("Invalid token");
		}
	}
}