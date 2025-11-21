using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FW.Microservices.Products.BFFWeb.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/[controller]")]
    public sealed class AuthController
        : Controller
    {
        [HttpGet("silent-login")]
        public IActionResult SilentLogin([FromQuery] string returnUrl = "/")
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = returnUrl,
                Items =
                {
                    { "prompt", "none" }
                }
            };

            return Challenge(properties, "oidc");
        }

        [Authorize]
        [HttpPost("silent-logout")]
        public async Task<IActionResult> SilentLogout()
        {
            try
            {
                if (User.Identity?.IsAuthenticated != true)
                {
                    return Ok(new
                    {
                        Success = true,
                        Message = "User was not authenticated",
                        AlreadyLoggedOut = true
                    });
                }

                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                return Ok(new
                {
                    Success = true,
                    Message = "Successfully logged out from Products Microservice BFF",
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Logout failed",
                    Error = ex.Message
                });
            }
        }

        [HttpGet("silent-logout")]
        public async Task<IActionResult> SilentLogoutGet()
        {
            try
            {
                if (User.Identity?.IsAuthenticated != true)
                {
                    return Ok(new
                    {
                        Success = true,
                        Message = "User was not authenticated",
                        AlreadyLoggedOut = true
                    });
                }

                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                return Ok(new
                {
                    Success = true,
                    Message = "Successfully logged out from Products Microservice BFF",
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Logout failed",
                    Error = ex.Message
                });
            }
        }

        [HttpGet("status")]
        [AllowAnonymous]
        public IActionResult GetAuthStatus()
        {
            return Ok(new
            {
                IsAuthenticated = User.Identity?.IsAuthenticated ?? false,
                UserName = User.Identity?.Name,
                Claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList()
            });
        }
    }
}