using Duende.IdentityServer;
using Duende.IdentityServer.Events;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Test;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace IdentityServer.Pages.Account;

[SecurityHeaders]
[AllowAnonymous]
public class LoginModel : PageModel
{
    private readonly TestUserStore _users;
    private readonly IIdentityServerInteractionService _interaction;
    private readonly IEventService _events;
    private readonly IAuthenticationSchemeProvider _schemeProvider;

    public LoginModel(
        IIdentityServerInteractionService interaction,
        IAuthenticationSchemeProvider schemeProvider,
        IEventService events,
        TestUserStore? users = null)
    {
        _users = users ?? new TestUserStore(TestUsers.Users);
        _interaction = interaction;
        _schemeProvider = schemeProvider;
        _events = events;
    }

    [BindProperty]
    public InputModel Input { get; set; } = default!;

    public class InputModel
    {
        public string Username { get; set; } = default!;
        public string Password { get; set; } = default!;
        public bool RememberLogin { get; set; }
        public string ReturnUrl { get; set; } = default!;
        public string Button { get; set; } = default!;
    }

    public async Task<IActionResult> OnGet(string? returnUrl)
    {
        Input = new InputModel { ReturnUrl = returnUrl ?? "~/" };
        return Page();
    }

    public async Task<IActionResult> OnPost()
    {
        var context = await _interaction.GetAuthorizationContextAsync(Input.ReturnUrl);

        if (Input.Button != "login")
        {
            if (context != null)
            {
                await _interaction.DenyAuthorizationAsync(context, AuthorizationError.AccessDenied);
                if (context.IsNativeClient())
                {
                    return this.LoadingPage(Input.ReturnUrl);
                }
                return Redirect(Input.ReturnUrl ?? "~/");
            }
            else
            {
                return Redirect("~/");
            }
        }

        if (ModelState.IsValid)
        {
            if (_users.ValidateCredentials(Input.Username, Input.Password))
            {
                var user = _users.FindByUsername(Input.Username);
                await _events.RaiseAsync(new UserLoginSuccessEvent(user.Username, user.SubjectId, user.Username));

                AuthenticationProperties? props = null;
                if (Input.RememberLogin)
                {
                    props = new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.Add(TimeSpan.FromDays(30))
                    };
                }

                var isuser = new IdentityServerUser(user.SubjectId)
                {
                    DisplayName = user.Username
                };

                await HttpContext.SignInAsync(isuser, props);

                if (context != null)
                {
                    if (context.IsNativeClient())
                    {
                        return this.LoadingPage(Input.ReturnUrl);
                    }

                    return Redirect(Input.ReturnUrl ?? "~/");
                }

                if (Url.IsLocalUrl(Input.ReturnUrl))
                {
                    return Redirect(Input.ReturnUrl);
                }
                else if (string.IsNullOrEmpty(Input.ReturnUrl))
                {
                    return Redirect("~/");
                }
                else
                {
                    throw new ArgumentException("invalid return URL");
                }
            }

            await _events.RaiseAsync(new UserLoginFailureEvent(Input.Username, "invalid credentials"));
            ModelState.AddModelError(string.Empty, "Invalid username or password");
        }

        return Page();
    }
}
