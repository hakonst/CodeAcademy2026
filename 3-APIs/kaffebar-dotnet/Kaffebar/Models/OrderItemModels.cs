using System.Text.Json.Serialization;

namespace Kaffebar.Models;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(CoffeeItem), "coffee")]
[JsonDerivedType(typeof(PastryItem), "pastry")]
public abstract record OrderItem;

public record CoffeeItem(CoffeeSize Size, MilkType MilkType, bool? ExtraShot) : OrderItem;

public record PastryItem(string Name, bool IsVegan) : OrderItem;
