using System.Net.Http;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FW.Microservices.Products.BFFWeb.Controllers;

[ApiController]
[Route ("bff/products")]
[Authorize]                     // Only users authenticated (via Shell BFF forwarded login)
public sealed class ProductsController
	: ControllerBase
{
	[HttpGet]
	public IActionResult GetProducts ()
	{
		// Hard-coded for now — later you'll call the microservice via HttpClient + access token
		var response = new
		{
			message = "Products fetched from Products BFF Experience API",
			products = new []
			{
				new { id = 1, name = "Laptop", price = 1200, stock = 14, category = "Electronics" },
				new { id = 2, name = "Mouse", price = 25, stock = 200, category = "Accessories" },
				new { id = 3, name = "Keyboard", price = 45, stock = 150, category = "Accessories" },
				new { id = 4, name = "Monitor", price = 350, stock = 30, category = "Electronics" }
			}
		};

		// Later:
		// var api = _httpClientFactory.CreateClient ("products-api");
		// var realProducts = await api.GetFromJsonAsync<List<Product>> ("/products");

		return Ok (response);
	}
}