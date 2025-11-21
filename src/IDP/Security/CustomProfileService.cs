using System.Security.Claims;

using Duende.IdentityModel;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;

using FW.IDP.DBAccess;

namespace FW.IDP.Services;

public sealed class CustomProfileService
    : IProfileService
{
    private readonly IUserRepository _userRepository;

    public CustomProfileService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        var sub = context.Subject.GetSubjectId();
        var user = await _userRepository.FindBySubjectIdAsync(sub);

        if (user == null)
        {
            context.IssuedClaims = new List<Claim>();
            return;
        }

        var claims = new List<Claim>
        {
            new Claim(JwtClaimTypes.Subject, user.ID.ToString()),
            new Claim(JwtClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
            new Claim(JwtClaimTypes.GivenName, user.FirstName),
            new Claim(JwtClaimTypes.FamilyName, user.LastName),
            new Claim(JwtClaimTypes.Email, user.Email),
            new Claim(JwtClaimTypes.EmailVerified, "true")
        };

        if (context.RequestedClaimTypes.Contains(JwtClaimTypes.Role))
        {
            var roles = await _userRepository.GetRolesByUserIDAsync(user.ID);

            foreach (var role in roles)
            {
                claims.Add(new Claim(JwtClaimTypes.Role, role));
            }
        }

        context.IssuedClaims = claims;
    }

    public Task IsActiveAsync(IsActiveContext context)
    {
        context.IsActive = !string.IsNullOrWhiteSpace(context.Subject.GetSubjectId());
        return Task.CompletedTask;
    }
}