using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FW.PAS.BFFWeb.Controllers;

[Authorize]
[ApiController]
[Route ("bff/forward")]
public sealed class ForwardController
	: ControllerBase
{
	public ForwardController ()
	{
	}

    [HttpGet("to-microservice")]
    public IActionResult ForwardToTargetMicroserviceBFF([FromQuery] string baseUrl, [FromQuery] string relativePath)
    {
        relativePath = "";
        var redirectUrl = $"{baseUrl}/signinoidc/signin?redirect_uri={baseUrl}{relativePath}";
        return Redirect(redirectUrl);
    }

    [HttpGet("{**any}")]
    public IActionResult ForwardingNotFound()
    {
        string htmlContent = "<div style='padding: 20px; border: 1px solid #1a2c4e; background-color: #e6f0ff; color: #1a2c4e; border-radius: 5px; font-family: sans-serif;'>" +
                             "<strong>❗ Task Not Implemented:</strong> This microservice task is yet to be implemented or the route is incorrect." +
                             "</div>";

        return new ContentResult
        {
            Content = htmlContent,
            ContentType = "text/html",
            StatusCode = StatusCodes.Status404NotFound
        };
    }
}