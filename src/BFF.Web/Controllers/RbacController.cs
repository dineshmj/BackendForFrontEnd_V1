using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ShellBff.Controllers;

[Authorize]
[ApiController]
[Route ("bff/rbac")]
public sealed class RbacController
	: ControllerBase
{
	[HttpGet ("menu")]
	public IActionResult GetMenu ()
	{
		// Hard-coded example for now; replace later with real RBAC logic.
		var response = new
		{
			user = "alice",
			hasAccessTo = new []
			{
				new
				{
					microService = "Products",
					menuItems = new[]
					{
						new {
							menuItemTitle = "Show Products",
							targetURL = "/bff/forward/products"
						}
					}
				}
			}
		};

		return Ok (response);
	}
}