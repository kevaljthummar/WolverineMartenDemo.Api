using Microsoft.AspNetCore.Mvc;
using Wolverine;
using WolverineMartenDemo.Commands;

namespace WolverineMartenDemo.Controllers;

[ApiController]
[Route("orders")]
public class OrderController : ControllerBase
{
    private readonly IMessageBus _bus;

    public OrderController(IMessageBus bus)
    {
        _bus = bus;
    }

    [HttpPost("start")]
    public async Task<IActionResult> StartOrder()
    {
        var orderId = Guid.NewGuid().ToString();

        // Send command to Wolverine
        await _bus.SendAsync(new StartOrder(orderId));

        return Ok(new { OrderId = orderId });
    }
}
