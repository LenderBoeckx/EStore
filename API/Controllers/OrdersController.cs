using System;
using API.DTOs;
using API.Extensions;
using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Core.Specifications;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
public class OrdersController(ICartService cartService, IUnitOfWork uow) : BaseApiController
{
    [HttpPost]
    public async Task<ActionResult<Order>> CreateOrder(CreateOrderDto createOrderDto)
    {
        //email van de gebruiker en winkelwagen ophalen
        var email = User.GetEmail();
        var cart = await cartService.GetCardAsync(createOrderDto.WinkelwagenId);

        if(cart == null) return BadRequest("Winkelwagen niet gevonden");

        if(cart.PaymentIntentId == null) return BadRequest("Geen betalingsintent voor dit order gevonden");

        var items = new List<OrderItem>();

        //lijst met items opvullen met de items in de winkelwagen
        foreach(var item in cart.Items) 
        {
            var productItem = await uow.Repository<Product>().GetByIdAsync(item.ProductId);

            if(productItem == null) return BadRequest("Probleem met het order.");

            var itemOrdered = new ProductItemOrdered
            {
                ProductId = item.ProductId,
                ProductNaam = item.ProductNaam,
                FotoUrl = item.FotoUrl
            };

            var orderItem = new OrderItem
            {
                ItemBesteld = itemOrdered,
                Prijs = item.Prijs,
                Hoeveelheid = item.Hoeveelheid
            };

            items.Add(orderItem);
        }

        //leveringsmethode ophalen aan de hand van de meegeleverde id
        var deliveryMethod = await uow.Repository<DeliveryMethod>().GetByIdAsync(createOrderDto.LeveringsMethodeId);

        if(deliveryMethod == null) return BadRequest("Geen leveringsmethode geselecteerd.");

        //nieuw order aanmaken met de voorgaand opgehaalde gegevens
        var order = new Order
        {
            BestellingsItems = items,
            LeveringsMethode = deliveryMethod,
            LeverAddress = createOrderDto.LeveringsAdres,
            Subtotaal = items.Sum(x => x.Prijs * x.Hoeveelheid),
            BetalingsOverzicht = createOrderDto.BetalingsOverzicht,
            BetalingsIntentId = cart.PaymentIntentId,
            KoperEmail = email,
            Korting = createOrderDto.Korting
        };

        //aangemaakt order naar de database sturen
        uow.Repository<Order>().Add(order);

        //wijzigingen in de database opslaan
        if(await uow.Complete())
        {
            return order;
        }
        return BadRequest("Probleem met het aanmaken van de bestelling.");
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<OrderDto>>> GetOrdersForUser()
    {
        var spec = new OrderSpecification(User.GetEmail());

        var orders = await uow.Repository<Order>().ListAsync(spec);

        var OrdersToReturn = orders.Select(o => o.ToDto()).ToList();

        return Ok(OrdersToReturn);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<OrderDto>> GetOrderById (int id)
    {
        var spec = new OrderSpecification(User.GetEmail(), id);
        var order = await uow.Repository<Order>().GetEntityWithSpec(spec);

        if(order == null) return NotFound();

        return order.ToDto();
    }
}
