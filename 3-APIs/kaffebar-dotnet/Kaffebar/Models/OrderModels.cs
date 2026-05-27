namespace Kaffebar.Models;

public record CreateOrderRequest(Guid CoffeeId);

public record OrderResponse(Guid OrderId, Guid CoffeeId);
