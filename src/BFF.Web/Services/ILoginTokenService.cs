using System.Security.Claims;

using BFF.Web.Entities;

public interface ILoginTokenService
{
	string CreateLoginToken (ClaimsPrincipal principal, PasMicroservice pasMicroservice);

	ClaimsPrincipal? ValidateLoginToken (string token, PasMicroservice pasMicroservice);
}