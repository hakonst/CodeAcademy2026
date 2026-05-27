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
        var order = new OrderResponse(Guid.NewGuid(), request.CoffeeId, request.Size, request.MilkType, request.ExtraShot, request.CustomerName, request.Quantity);
        orders[order.OrderId] = order;
        return CreatedAtAction(nameof(CreateOrder), new { orderId = order.OrderId }, order);
    }

    [HttpGet("{orderId:guid}")]
    [ProducesResponseType<OrderResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetOrder(Guid orderId)
    {
        if (!orders.TryGetValue(orderId, out var order))
            return NotFound();

        return Ok(order);
    }
}
