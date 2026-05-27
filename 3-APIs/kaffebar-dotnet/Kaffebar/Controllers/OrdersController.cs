using Kaffebar.Models;
using Microsoft.AspNetCore.Mvc;

namespace Kaffebar.Controllers;

[ApiController]
[Route("orders")]
public class OrdersController(Dictionary<Guid, OrderResponse> orders) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<PagedResponse<OrderResponse>>(StatusCodes.Status200OK)]
    public IActionResult GetOrders([FromQuery] OrderQuery query)
    {
        var filtered = orders.Values
            .Where(o => query.Status == null || o.Status == query.Status)
            .ToList();

        var items = filtered.Skip(query.Offset).Take(query.Limit);

        return Ok(new PagedResponse<OrderResponse>(items, filtered.Count, query.Limit, query.Offset));
    }

    [HttpPost]
    [ProducesResponseType<OrderResponse>(StatusCodes.Status201Created)]
    public IActionResult CreateOrder(CreateOrderRequest request)
    {
        var order = new OrderResponse(Guid.NewGuid(), request.CustomerName, request.Items, OrderStatus.PENDING);
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
