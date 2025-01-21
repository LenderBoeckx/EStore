using System;

namespace Core.Entities.OrderAggregate;

public class ShippingAddress
{
    public required string Naam {get; set;}
    public required string Straat {get; set;}
    public string? Toevoeging {get; set;}
    public required string Plaats {get; set;}
    public string? Provincie {get; set;}
    public required string Postcode {get; set;}
    public required string Land {get; set;}
}

