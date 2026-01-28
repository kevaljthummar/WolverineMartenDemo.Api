namespace WolverineMartenDemo.Commands;

// Command = intent (what user wants to do)
public record StartOrder(string OrderId);

// Sent later by Saga
public record CompleteOrder(string Id);
