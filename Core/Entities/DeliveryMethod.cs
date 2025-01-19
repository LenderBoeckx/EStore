using System;

namespace Core.Entities;

public class DeliveryMethod : BaseEntity
{
    public required string Naam {get; set;}
    public required string LeveringsTijd {get; set;}
    public required string Beschrijving {get; set;}
    public decimal Prijs {get; set;}
}
