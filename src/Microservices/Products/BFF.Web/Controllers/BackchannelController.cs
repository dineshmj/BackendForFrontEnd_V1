using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace FW.Microservices.Products.BFFWeb.Controllers
{
	[ApiController]
	[Route ("api/backchannel")]
	public sealed class BackchannelController
		: ControllerBase
	{
		private readonly ILogger<BackchannelController> _logger;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public BackchannelController (
			ILogger<BackchannelController> logger,
			IHttpContextAccessor httpContextAccessor)
		{
			_logger = logger;
			_httpContextAccessor = httpContextAccessor;
		}

		[HttpPost ("logout-trigger")]
		public async Task<IActionResult> LogoutTrigger ([FromBody] LogoutTriggerRequest request)
		{
			if (string.IsNullOrEmpty (request.Sub))
			{
				return BadRequest ("Missing 'sub'");
			}

			var httpContext = _httpContextAccessor.HttpContext;

			if (httpContext == null)
			{
				return StatusCode (500, "No HttpContext");
			}

			try
			{
				await httpContext.SignOutAsync ();
				return Ok (new { message = "Local logout completed." });
			}
			catch (Exception ex)
			{
				_logger.LogError (ex, "Error during local logout");
				return StatusCode (500, "Logout failed");
			}
		}
	}

	public sealed class LogoutTriggerRequest
	{
		public string Sub { get; set; } = string.Empty;

		public string? Sid { get; set; }
	}
}