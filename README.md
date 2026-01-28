# Wolverine + Marten CQRS Saga Demo

This repository demonstrates a **minimal but complete** implementation of:

- CQRS
- Event Sourcing
- Saga (Workflow)
- Wolverine
- Marten
- .NET 8 Web API

---

## Architecture Overview
-> HTTP Request
-> Command (StartOrder)
-> Command Handler
-> Event Stored (OrderStarted)
-> Saga Starts
-> Saga Sends Command (CompleteOrder)
-> Command Handler
-> Event Stored (OrderCompleted)
-> Saga Completes

## Key Concepts
### Command
Represents **intent** (what user wants).

### Event
Represents **fact** (what already happened).

### Marten
- Stores events in `mt_events`
- Rebuilds aggregates by replaying events

### Wolverine
- Dispatches commands & events
- Runs Saga automatically

### Saga
- Coordinates long-running workflow
- Uses `Id` for correlation
- Is deleted after completion

---

## How to Run

1. Start PostgreSQL
2. Update connection string
3. Run project

```bash
dotnet run