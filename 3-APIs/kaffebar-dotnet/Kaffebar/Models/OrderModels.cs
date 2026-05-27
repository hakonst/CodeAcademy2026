using System.ComponentModel.DataAnnotations;

namespace Kaffebar.Models;

public enum CoffeeSize { SMALL, MEDIUM, LARGE };

public enum MilkType { WHOLE, SKIMMED, OAT, SOY };

public enum OrderStatus { PENDING, BREWING, READY };

public record CreateOrderRequest(
    [Required]
    [StringLength(50, MinimumLength = 2)]
    string CustomerName,
    [MinLength(1)]
    List<OrderItem> Items
);

public record UpdateOrderStatusRequest([Required] OrderStatus Status);

public record OrderQuery(
    OrderStatus? Status,
    [Range(1, 100)] int Limit = 20,
    [Range(0, int.MaxValue)] int Offset = 0
);

public record PagedResponse<T>(IEnumerable<T> Items, int Total, int Limit, int Offset);

public record OrderResponse(
    Guid OrderId,
    string CustomerName,
    List<OrderItem> Items,
    OrderStatus Status
);
