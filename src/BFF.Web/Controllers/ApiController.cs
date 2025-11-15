using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BFF.Web.Controllers;

[Authorize]
[Route("api")]
[ApiController]
public class ApiController : ControllerBase
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

    [HttpGet("orders")]
    public async Task<IActionResult> GetOrders()
    {
        var client = _httpClientFactory.CreateClient("orders-api");
        var response = await client.GetAsync("/orders");
        
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            return Ok(content);
        }

        return StatusCode((int)response.StatusCode);
    }

    [HttpGet("products")]
    public async Task<IActionResult> GetProducts()
    {
        var client = _httpClientFactory.CreateClient("products-api");
        var response = await client.GetAsync("/products");
        
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            return Ok(content);
        }

        return StatusCode((int)response.StatusCode);
    }

    [HttpGet("payments")]
    public async Task<IActionResult> GetPayments()
    {
        var client = _httpClientFactory.CreateClient("payments-api");
        var response = await client.GetAsync("/payments");
        
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            return Ok(content);
        }

        return StatusCode((int)response.StatusCode);
    }
}
