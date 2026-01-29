using Marten;
using Wolverine;
using WolverineMartenDemo.Commands;
using WolverineMartenDemo.Domain;
using WolverineMartenDemo.Events;

namespace WolverineMartenDemo.Handlers;

// Wolverine automatically finds this class
public class OrderCommandHandler
{
    // Handles StartOrder command
    public async Task Handle(StartOrder cmd, IDocumentSession session, IMessageBus bus)
    {
        Console.WriteLine("Start handler hit");

        // Create first event
        var started = new OrderStarted(cmd.OrderId);

        // Start event stream (Event Sourcing)
        session.Events.StartStream<Order>(
            cmd.OrderId,
            started
        );

        // Persist event to mt_events table
        await session.SaveChangesAsync();
    }

    // Handles CompleteOrder command
    public async Task Handle(CompleteOrder cmd, IDocumentSession session)
    {
        // Load aggregate by replaying events
        var stream = await session.Events.FetchForWriting<Order>(cmd.Id);

        // Append new event
        stream.AppendOne(new OrderCompleted(cmd.Id));

        await session.SaveChangesAsync();
    }
}
