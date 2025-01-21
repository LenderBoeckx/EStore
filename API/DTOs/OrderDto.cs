using System;
using Core.Entities;
using Core.Entities.OrderAggregate;

namespace API.DTOs;

public class OrderDto
{
    public int Id {get; set;}
    public DateTime BestellingsDatum {get; set;} = DateTime.UtcNow;
    public required string KoperEmail {get; set;}
    public required ShippingAddress LeverAddress {get; set;}
    public required string LeveringsMethode {get; set;}
    public decimal LeveringsPrijs {get; set;}
    public required PaymentSummary BetalingsOverzicht {get; set;}
    public required List<OrderItemDto> BestellingsItems {get; set;}
    public decimal Subtotaal {get; set;}
    public required string BestellingsStatus {get; set;}
    public decimal Totaal {get; set;}
    public required string BetalingsIntentId {get; set;}
}
