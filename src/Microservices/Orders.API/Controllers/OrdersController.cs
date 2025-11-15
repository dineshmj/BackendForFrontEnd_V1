using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Orders.API.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class OrdersController : ControllerBase
{
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(ILogger<OrdersController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Get()
    {
        var userId = User.FindFirst("sub")?.Value ?? "unknown";
        var userName = User.FindFirst("name")?.Value ?? "Unknown User";
        
        _logger.LogInformation("Orders requested by user: {UserId}", userId);
        
        return Ok(new
        {
            Message = "Orders retrieved successfully",
            UserId = userId,
            UserName = userName,
            Orders = new[]
            {
                new { Id = 1, Product = "Laptop", Amount = 1299.99, Status = "Shipped", Date = "2024-10-15" },
                new { Id = 2, Product = "Mouse", Amount = 29.99, Status = "Delivered", Date = "2024-10-20" },
                new { Id = 3, Product = "Keyboard", Amount = 89.99, Status = "Processing", Date = "2024-11-01" }
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
            Product = "Sample Product",
            Amount = 99.99,
            Status = "Active"
        });
    }

    [HttpPost]
    public IActionResult Create([FromBody] CreateOrderRequest request)
    {
        var userId = User.FindFirst("sub")?.Value ?? "unknown";
        
        return Ok(new
        {
            Message = "Order created successfully",
            OrderId = Random.Shared.Next(1000, 9999),
            UserId = userId,
            Product = request.Product,
            Amount = request.Amount
        });
    }
}

public record CreateOrderRequest(string Product, decimal Amount);
