using System.ComponentModel.DataAnnotations;

namespace Kaffebar.Models;

public enum CoffeeSize { SMALL, MEDIUM, LARGE };

public enum MilkType { WHOLE, SKIMMED, OAT, SOY };

public record CreateOrderRequest(
    Guid CoffeeId,
    CoffeeSize Size,
    MilkType MilkType,
    bool? ExtraShot,
    [Required]
    [StringLength(50, MinimumLength = 2)]
    string CustomerName,
    [Range(1, 10)]
    int Quantity
);

public record OrderResponse(
    Guid OrderId,
    Guid CoffeeId,
    CoffeeSize Size,
    MilkType MilkType,
    bool? ExtraShot,
    string CustomerName,
    int Quantity
);
