using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using BFF.Web.Entities;
using Common.Microservices;

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