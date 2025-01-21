using System;
using API.DTOs;
using Core.Entities.OrderAggregate;

namespace API.Extensions;

public static class OrderMappingExtensions
{
    public static OrderDto ToDto(this Order order)
    {
        return new OrderDto
        {
            Id = order.Id,
            KoperEmail = order.KoperEmail,
            BestellingsDatum = order.BestellingsDatum,
            LeverAddress = order.LeverAddress,
            BetalingsOverzicht = order.BetalingsOverzicht,
            LeveringsMethode = order.LeveringsMethode.Beschrijving,
            LeveringsPrijs = order.LeveringsMethode.Prijs,
            BestellingsItems = order.BestellingsItems.Select(x => x.ToDto()).ToList(),
            Subtotaal = order.Subtotaal,
            BestellingsStatus = order.BestellingsStatus.ToString(),
            BetalingsIntentId = order.BetalingsIntentId,
            Totaal = order.GetTotaal()
        };
    }

    public static OrderItemDto ToDto(this OrderItem orderItem)
    {
        return new OrderItemDto
        {
            ProductId = orderItem.Id,
            ProductNaam = orderItem.ItemBesteld.ProductNaam,
            FotoUrl = orderItem.ItemBesteld.FotoUrl,
            Prijs = orderItem.Prijs,
            Hoeveelheid = orderItem.Hoeveelheid
        };
    }
}
