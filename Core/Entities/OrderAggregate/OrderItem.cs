using System;

namespace Core.Entities.OrderAggregate;

public class OrderItem : BaseEntity
{
    public ProductItemOrdered ItemBesteld {get; set;} = null!;
    public decimal Prijs {get; set;}
    public int Hoeveelheid {get; set;}
}
