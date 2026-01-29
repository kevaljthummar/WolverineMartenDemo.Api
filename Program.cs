using JasperFx;
using JasperFx.Resources;
using Marten;
using Weasel.Core;
using Wolverine;
using Wolverine.Http;
using Wolverine.Marten;
using WolverineMartenDemo.Sagas;
using WolverineMartenDemo.Events;

var builder = WebApplication.CreateBuilder(args);

// --------------------
// Marten + Wolverine
// --------------------
builder.Services.AddMarten(opts =>
{
    opts.Connection(builder.Configuration.GetConnectionString("postgres"));
    opts.DatabaseSchemaName = "public";

    // Let Marten create tables automatically
    opts.AutoCreateSchemaObjects = AutoCreate.All;

    // We are using Guid stream ids
    opts.Events.StreamIdentity = JasperFx.Events.StreamIdentity.AsGuid;
})
// Integrate Marten with Wolverine's transactional outbox
.IntegrateWithWolverine()

// 🔥 EXPLICIT EVENT FORWARDING (THIS IS THE FIX)
.EventForwardingToWolverine(cfg =>
{
    // Forward Marten events as Wolverine messages
    cfg.SubscribeToEvent<OrderStarted>()
       .TransformedTo(e => e.Data);

    cfg.SubscribeToEvent<OrderCompleted>()
       .TransformedTo(e => e.Data);
});

// Ensure resources (DB schema, queues) are ready on startup
builder.Services.AddResourceSetupOnStartup();

// --------------------
// Wolverine host
// --------------------
builder.Host.UseWolverine(opts =>
{
    // 🔥 Tell Wolverine to scan this assembly for handlers & sagas
    opts.Discovery.IncludeAssembly(typeof(OrderSaga).Assembly);

    // Apply DB transactions automatically
    opts.Policies.AutoApplyTransactions();

    // ⚠️ IMPORTANT: Do NOT enable durable queues while debugging
    // opts.Policies.UseDurableLocalQueues();
});

// --------------------
// HTTP + Swagger
// --------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Wolverine HTTP endpoints
builder.Services.AddWolverineHttp();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Map Wolverine HTTP routes
app.MapWolverineEndpoints();

// Run host
return await app.RunJasperFxCommands(args);
