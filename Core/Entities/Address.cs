using System;

namespace Core.Entities;

public class Address : BaseEntity
{
    public required string Straat {get; set;}
    public string? Toevoeging {get; set;}
    public required string Plaats {get; set;}
    public required string Provincie {get; set;}
    public required string Postcode {get; set;}
    public required string Land {get; set;}
}
