using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using FW.Landscape.Common.Microservices;
using FW.PAS.BFFWeb.Entities;
using FW.PAS.BFFWeb.Services;

namespace FW.PAS.BFFWeb.Controllers;

[Authorize]
[ApiController]
[Route ("bff/forward")]
public sealed class ForwardController
	: ControllerBase
{
	private readonly ILoginTokenService _loginTokenService;

	public ForwardController (ILoginTokenService loginTokenService)
	{
		_loginTokenService = loginTokenService;
	}

	[HttpGet ("products")]
	public IActionResult ForwardProducts ()
	{
		// Mint short-lived token
		var loginToken = _loginTokenService.CreateLoginToken (base.User, PasMicroservice.Products);

		// Redirect to Products BFF step
		var redirectUrl =
			$"{ProductsMicroservice.CLIENT_BASE_URL}/receive-login?token={loginToken}";

		return Redirect (redirectUrl);
	}
}