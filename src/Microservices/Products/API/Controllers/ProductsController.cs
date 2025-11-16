using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FW.Microservices.Products.API.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public sealed class ProductsController
    : ControllerBase
{
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(ILogger<ProductsController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Get()
    {
        var userId = User.FindFirst("sub")?.Value ?? "unknown";
        var userName = User.FindFirst("name")?.Value ?? "Unknown User";
        
        _logger.LogInformation("Products requested by user: {UserId}", userId);
        
        return Ok(new
        {
            Message = "Products retrieved successfully",
            UserId = userId,
            UserName = userName,
            Products = new[]
            {
                new { Id = 1, Name = "Laptop Pro", Price = 1299.99, Stock = 45, Category = "Electronics" },
                new { Id = 2, Name = "Wireless Mouse", Price = 29.99, Stock = 150, Category = "Accessories" },
                new { Id = 3, Name = "Mechanical Keyboard", Price = 89.99, Stock = 78, Category = "Accessories" },
                new { Id = 4, Name = "USB-C Hub", Price = 49.99, Stock = 92, Category = "Accessories" },
                new { Id = 5, Name = "Monitor 27\"", Price = 399.99, Stock = 23, Category = "Electronics" }
            }
        });
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var userId = User.FindFirst("sub")?.Value ?? "unknown";
        
        return Ok(new
        {
            Id = id,
            UserId = userId,
            Name = "Sample Product",
            Price = 99.99,
            Stock = 100,
            Category = "General"
        });
    }
}
