using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BFF.Web.Controllers;

[Authorize]
[Route("api")]
[ApiController]
public sealed class ApiController
    : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ApiController> _logger;

    public ApiController(IHttpClientFactory httpClientFactory, ILogger<ApiController> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    [HttpGet("user-info")]
    public async Task<IActionResult> GetUserInfo()
    {
        var claims = User.Claims.Select(c => new { c.Type, c.Value });
        var accessToken = await HttpContext.GetTokenAsync("access_token");
        
        return Ok(new
        {
            Claims = claims,
            HasAccessToken = !string.IsNullOrEmpty(accessToken)
        });
    }
}