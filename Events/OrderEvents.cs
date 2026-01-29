namespace WolverineMartenDemo.Events;

// Event = something that already happened
public record OrderStarted(Guid OrderId);

public record OrderCompleted(Guid OrderId);
