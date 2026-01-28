using WolverineMartenDemo.Events;

namespace WolverineMartenDemo.Domain;

// Aggregate rebuilt from events
public class Order
{
    public string Id { get; private set; } = default!;
    public bool IsCompleted { get; private set; }

    // Apply methods are used by Marten
    public void Apply(OrderStarted e)
    {
        Id = e.OrderId;
    }

    public void Apply(OrderCompleted e)
    {
        IsCompleted = true;
    }
}
