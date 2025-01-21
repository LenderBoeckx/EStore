using System;

namespace Core.Entities.OrderAggregate;

public class ProductItemOrdered
{
    public int ProductId {get; set;}
    public required string ProductNaam {get; set;}
    public required string FotoUrl {get; set;}
}
