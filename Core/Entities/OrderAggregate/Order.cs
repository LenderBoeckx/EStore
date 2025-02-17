using System;
using Core.Interfaces;

namespace Core.Entities.OrderAggregate;

public class Order : BaseEntity, IDtoConvertible
{
    public DateTime BestellingsDatum {get; set;} = DateTime.UtcNow;
    public required string KoperEmail {get; set;}
    public ShippingAddress LeverAddress {get; set;} = null!;
    public DeliveryMethod LeveringsMethode {get; set;} = null!;
    public PaymentSummary BetalingsOverzicht {get; set;} = null!;
    public List<OrderItem> BestellingsItems {get; set;} = [];
    public decimal Subtotaal {get; set;}
    public OrderStatus BestellingsStatus {get; set;} = OrderStatus.Pending;
    public required string BetalingsIntentId {get; set;}
    public decimal Korting {get; set;}

    public decimal GetTotaal()
    {
         return Subtotaal - Korting + LeveringsMethode.Prijs;
    }

}
