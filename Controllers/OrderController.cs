using Microsoft.AspNetCore.Mvc;
using Wolverine;
using Wolverine.Http;
using WolverineMartenDemo.Commands;

namespace WolverineMartenDemo.Controllers;

public class OrderController
{
    private readonly IMessageBus _bus;

    public OrderController(IMessageBus bus)
    {
        _bus = bus;
    }

    [WolverinePost("orders/start")]
    public async Task<IResult> StartOrder()
    {
        var orderId = Guid.NewGuid();

        // Send command to Wolverine
        await _bus.SendAsync(new StartOrder(orderId));

        return Results.Ok(new { OrderId = orderId });
    }
}
