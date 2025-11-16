using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

using Duende.IdentityModel;

using FW.Landscape.Common.Microservices;

namespace FW.PAS.BFFWeb.Controllers;

[ApiController]
[Route ("bff/logout-all")]
public sealed class LogoutAllController
	: Controller
{
	private readonly IHttpClientFactory _httpClientFactory;

	private static readonly string [] MicroserviceBases = [
		ProductsMicroservice.CLIENT_BASE_URL, // Add more.
    ];

	private const string InternalSecret = "YOUR_HIGHLY_SECURE_INTERNAL_API_KEY";

	public LogoutAllController (IHttpClientFactory httpClientFactory)
	{
		_httpClientFactory = httpClientFactory;
	}

	[HttpGet]
	public async Task<IActionResult> LogoutAll ()
	{
		var idToken = User.FindFirst ("id_token")?.Value;
		var sid = User.FindFirst (JwtClaimTypes.SessionId)?.Value;
		var subjectId = User.FindFirst (JwtClaimTypes.Subject)?.Value;

		// Ask each microservice BFF to perform local logout
		if (!string.IsNullOrEmpty (sid) && !string.IsNullOrEmpty (subjectId))
		{
			foreach (var baseUrl in MicroserviceBases)
			{
				await TriggerMicroserviceLogoutAsync (baseUrl, sid, subjectId);
			}
		}

		// Logout at Shell BFF level
		if (User.Identity?.IsAuthenticated == true)
		{
			await HttpContext.SignOutAsync (CookieAuthenticationDefaults.AuthenticationScheme);
		}

		string finalRedirectUri = "/logged-out";
		return Redirect (finalRedirectUri);
	}

	private async Task TriggerMicroserviceLogoutAsync (string microserviceBaseUrl, string sessionId, string subjectId)
	{
		var logoutUrl = $"{microserviceBaseUrl}/api/backchannel/logout-trigger";

		using var client = _httpClientFactory.CreateClient ();

		// Pass the internal secret for authentication
		client.DefaultRequestHeaders.Add ("X-Internal-Secret", InternalSecret);

		var content = new FormUrlEncodedContent ([
			new KeyValuePair<string, string>("sid", sessionId),
			new KeyValuePair<string, string>("sub", subjectId)
		]);

		// The Shell BFF makes a direct, trusted HTTP call to the Microservice BFF
		var response = await client.PostAsync (logoutUrl, content);

		// ↓↓ NOT WORKING!
		// Although the response.IsSuccessStatusCode is true, somehow this back-channel C# to C# call does not reach the Microservice's controller.
		if (!response.IsSuccessStatusCode)
		{
			// You should implement robust logging here for failed cleanup calls
			Console.WriteLine ($"Failed to trigger logout on {microserviceBaseUrl}: {response.StatusCode}");
		}
	}
}