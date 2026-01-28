namespace WolverineMartenDemo.Events;

// Event = something that already happened
public record OrderStarted(string OrderId);

public record OrderCompleted(string OrderId);
