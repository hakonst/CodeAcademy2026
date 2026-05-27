using Kaffebar.Models;
using Microsoft.AspNetCore.Mvc;

namespace Kaffebar.Controllers;

[ApiController]
[Route("orders")]
public class OrdersController(Dictionary<Guid, OrderResponse> orders) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<OrderResponse>(StatusCodes.Status201Created)]
    public IActionResult CreateOrder(CreateOrderRequest request)
    {
        var order = new OrderResponse(Guid.NewGuid(), request.CoffeeId, request.Size, request.MilkType, request.ExtraShot, request.CustomerName, request.Quantity, OrderStatus.PENDING);
        orders[order.OrderId] = order;
        return CreatedAtAction(nameof(GetOrder), new { orderId = order.OrderId }, order);
    }

    [HttpGet("{orderId:guid}")]
    [ProducesResponseType<OrderResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public IActionResult GetOrder(Guid orderId)
    {
        if (!orders.TryGetValue(orderId, out var order))
            return Problem(
                title: "Bestilling ikke funnet",
                detail: $"Fant ingen bestilling med id '{orderId}'.",
                statusCode: StatusCodes.Status404NotFound);

        return Ok(order);
    }

    [HttpPatch("{orderId:guid}/status")]
    [ProducesResponseType<OrderResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public IActionResult UpdateOrderStatus(Guid orderId, UpdateOrderStatusRequest request)
    {
        if (!orders.TryGetValue(orderId, out var order))
            return Problem(
                title: "Bestilling ikke funnet",
                detail: $"Fant ingen bestilling med id '{orderId}'.",
                statusCode: StatusCodes.Status404NotFound);

        var updated = order with { Status = request.Status };
        orders[orderId] = updated;
        return Ok(updated);
    }
}
