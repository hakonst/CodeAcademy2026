using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

// In-memory order store
var orders = new Dictionary<Guid, OrderResponse>();

app.MapGet("/menu", () => new[]
{
    new Coffee(Guid.NewGuid(), "Kaffe Latte", 48.50m),
    new Coffee(Guid.NewGuid(), "Cappuccino", 45.00m),
    new Coffee(Guid.NewGuid(), "Espresso", 35.00m)
})
.WithName("GetMenu")
.WithSummary("Hent kaffemeny")
.WithDescription("Returnerer en liste over alle tilgjengelige kaffedrikker i kaffebaren.");

app.MapPost("/orders", (CreateOrderRequest request) =>
{
    var order = new OrderResponse(Guid.NewGuid(), request.CoffeeId);
    orders[order.OrderId] = order;
    return TypedResults.Created($"/orders/{order.OrderId}", order);
})
.WithName("CreateOrder")
.WithSummary("Legg inn bestilling")
.WithDescription("Oppretter en ny kaffebestilling og returnerer orderen med autogenerert OrderId.");

app.Run();

public record Coffee(Guid Id, string Name, decimal Price);

public record CreateOrderRequest(Guid CoffeeId);

public record OrderResponse(Guid OrderId, Guid CoffeeId);
