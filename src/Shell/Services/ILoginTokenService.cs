using System.Security.Claims;

using FW.PAS.BFFWeb.Entities;

namespace FW.PAS.BFFWeb.Services;

public interface ILoginTokenService
{
	string CreateLoginToken (ClaimsPrincipal principal, PasMicroservice pasMicroservice);

	ClaimsPrincipal? ValidateLoginToken (string token, PasMicroservice pasMicroservice);
}