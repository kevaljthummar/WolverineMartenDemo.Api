namespace WolverineMartenDemo.Commands;

// Command = intent (what user wants to do)
public record StartOrder(Guid OrderId);

// Sent later by Saga
public record CompleteOrder(Guid Id);
