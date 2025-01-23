using System;
using System.ComponentModel.DataAnnotations;
using Core.Entities.OrderAggregate;

namespace API.DTOs;

public class CreateOrderDto
{
    [Required]
    public string WinkelwagenId {get; set;} = string.Empty;
    [Required]
    public int LeveringsMethodeId {get; set;}
    [Required]
    public ShippingAddress LeveringsAdres {get; set;} = null!;
    [Required]
    public PaymentSummary BetalingsOverzicht {get; set;} = null!;
    public decimal Korting {get; set;}
}
