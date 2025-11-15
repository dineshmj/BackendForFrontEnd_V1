using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Payments.API.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly ILogger<PaymentsController> _logger;

    public PaymentsController(ILogger<PaymentsController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Get()
    {
        var userId = User.FindFirst("sub")?.Value ?? "unknown";
        var userName = User.FindFirst("name")?.Value ?? "Unknown User";
        
        _logger.LogInformation("Payments requested by user: {UserId}", userId);
        
        return Ok(new
        {
            Message = "Payments retrieved successfully",
            UserId = userId,
            UserName = userName,
            Payments = new[]
            {
                new { Id = 1, Amount = 1299.99, Status = "Completed", Method = "Credit Card", Date = "2024-10-15" },
                new { Id = 2, Amount = 29.99, Status = "Completed", Method = "PayPal", Date = "2024-10-20" },
                new { Id = 3, Amount = 89.99, Status = "Pending", Method = "Debit Card", Date = "2024-11-01" }
            }
        });
    }

    [HttpPost("process")]
    public IActionResult ProcessPayment([FromBody] ProcessPaymentRequest request)
    {
        var userId = User.FindFirst("sub")?.Value ?? "unknown";
        
        return Ok(new
        {
            Message = "Payment processed successfully",
            TransactionId = Guid.NewGuid().ToString(),
            UserId = userId,
            Amount = request.Amount,
            Status = "Completed"
        });
    }
}

public record ProcessPaymentRequest(decimal Amount, string Method);
