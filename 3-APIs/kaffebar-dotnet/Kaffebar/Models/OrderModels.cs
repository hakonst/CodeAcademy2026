namespace Kaffebar.Models;

public enum CoffeeSize { SMALL, MEDIUM, LARGE };

public enum MilkType { WHOLE, SKIMMED, OAT, SOY };

public record CreateOrderRequest(Guid CoffeeId, CoffeeSize Size, MilkType MilkType, bool? ExtraShot);

public record OrderResponse(Guid OrderId, Guid CoffeeId, CoffeeSize Size, MilkType MilkType, bool? ExtraShot);
