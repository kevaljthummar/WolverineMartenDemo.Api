using Marten;
using Wolverine;
using Wolverine.Http;
using Wolverine.Marten;

var builder = WebApplication.CreateBuilder(args);

// Marten configuration
builder.Services.AddMarten(opts =>
{
    opts.Connection(builder.Configuration.GetConnectionString("postgres"));
})
.IntegrateWithWolverine();

// Wolverine usage is required for WolverineFx.Http
builder.Host.UseWolverine(opts =>
{
    // This middleware will apply to the HTTP
    // endpoints as well
    opts.Policies.AutoApplyTransactions();

    // Setting up the outbox on all locally handled
    // background tasks
    opts.Policies.UseDurableLocalQueues();
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
