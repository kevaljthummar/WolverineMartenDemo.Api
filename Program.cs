using JasperFx;
using JasperFx.Resources;
using Marten;
using Wolverine;
using Wolverine.Http;
using Wolverine.Marten;
using WolverineMartenDemo.Sagas;

var builder = WebApplication.CreateBuilder(args);

// Marten configuration
builder.Services.AddMarten(opts =>
{
    opts.Connection(builder.Configuration.GetConnectionString("postgres"));
    opts.DatabaseSchemaName = "public";
})
// This adds configuration with Wolverine's transactional outbox and
// Marten middleware support to Wolverine
.IntegrateWithWolverine();

builder.Services.AddResourceSetupOnStartup();

// Wolverine usage is required for WolverineFx.Http
builder.Host.UseWolverine(opts =>
{
    opts.Services.AddResourceSetupOnStartup();
    // Tell Wolverine to scan this assembly
    opts.Discovery.IncludeAssembly(typeof(OrderSaga).Assembly);

    // This middleware will apply to the HTTP
    // endpoints as well
    opts.Policies.AutoApplyTransactions();

    // Setting up the outbox on all locally handled
    // background tasks
    opts.Policies.UseDurableLocalQueues();
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddWolverineHttp();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Let's add in Wolverine HTTP endpoints to the routing tree
app.MapWolverineEndpoints();

return await app.RunJasperFxCommands(args);
