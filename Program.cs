using JasperFx.CodeGeneration;
using JasperFx.Core;
using Marten;
using Weasel.Core;
using Wolverine;
using Wolverine.Http;
using Wolverine.Marten;
using WolverineMartenDemo.Sagas;
using WolverineMartenDemo.Events;
using JasperFx.Resources;
using JasperFx;

var builder = WebApplication.CreateBuilder(args);

// --------------------
// 1. Marten + Wolverine Integration
// --------------------
builder.Services.AddMarten(opts =>
{
    var connectionString = builder.Configuration.GetConnectionString("postgres");
    opts.Connection(connectionString);
    opts.DatabaseSchemaName = "public";

    // DEV ONLY: Auto-create schema to avoid "relation does not exist" errors
    if (builder.Environment.IsDevelopment())
    {
        opts.AutoCreateSchemaObjects = AutoCreate.All;
    }

    // Use Guid for Stream Identity (Required for standard Wolverine Saga mapping)
    opts.Events.StreamIdentity = JasperFx.Events.StreamIdentity.AsGuid;
})
// This single line sets up the Transactional Outbox for Marten
.IntegrateWithWolverine()
.EventForwardingToWolverine(opts =>
{
    // Forward Marten events as Wolverine messages
    // The Saga MUST implement `Start(OrderStarted)` or `IAmStartedBy<OrderStarted>`
    opts.SubscribeToEvent<OrderStarted>().TransformedTo(e => e.Data);
    opts.SubscribeToEvent<OrderCompleted>().TransformedTo(e => e.Data);
});

// Setup resources (queues/tables) on startup
builder.Services.AddResourceSetupOnStartup();

// --------------------
// 2. Wolverine Host Configuration
// --------------------
builder.Host.UseWolverine(opts =>
{
    // FIX: Ensure both the Saga assembly AND the current Program assembly are scanned
    opts.Discovery.IncludeAssembly(typeof(OrderSaga).Assembly);
    opts.Discovery.IncludeAssembly(typeof(Program).Assembly);

    // AUTO-TRANSACTIONS: This ensures Handle(StartOrder) wraps in a Marten Session
    opts.Policies.AutoApplyTransactions();

    // DEBUGGING AID: In Dev, you might want durable queues to ensure messages persist 
    // if the debugger pauses too long, but usually Local queues are fine.
    opts.Policies.UseDurableLocalQueues();

    // CODE GEN: Auto allows you to step through generated code if needed
    opts.CodeGeneration.TypeLoadMode = TypeLoadMode.Auto;
});

// --------------------
// 3. HTTP + Swagger
// --------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddWolverineHttp();

var app = builder.Build();

// --------------------
// 4. Pipeline
// --------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Map Wolverine HTTP Endpoints
app.MapWolverineEndpoints();

// Initialize usage of JasperFx (Wolverine/Marten) command line tools
return await app.RunJasperFxCommands(args);