using Wolverine;
using WolverineMartenDemo.Commands;
using WolverineMartenDemo.Events;

namespace WolverineMartenDemo.Sagas;

// Implicit saga (Id-based)
public class OrderSaga : Saga
{
    // This is the saga correlation id
    public Guid Id { get; set; }

    // Saga STARTS when OrderStarted event is published
    public static OrderSaga Start(
        OrderStarted e,
        IMessageBus bus)
    {
        Console.WriteLine("🔥 OrderSaga STARTED");

        // Send next command
        bus.SendAsync(new CompleteOrder(e.OrderId));

        // Create saga state
        return new OrderSaga
        {
            Id = e.OrderId
        };
    }

    // Saga CONTINUES when OrderCompleted event arrives
    public void Handle(OrderCompleted e)
    {
        Console.WriteLine("✅ SAGA COMPLETED");

        // Workflow is finished
        MarkCompleted();
    }
}
