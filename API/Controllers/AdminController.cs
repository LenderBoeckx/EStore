using System;
using API.DTOs;
using API.Extensions;
using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController(IUnitOfWork uow, IPaymentService paymentService) : BaseApiController
{
    [HttpGet("orders")]
    public async Task<ActionResult<IReadOnlyList<Order>>> GetOrders([FromQuery]OrderSpecParams specParams)
    {
        var spec = new OrderSpecification(specParams);

        return await CreatepagedResult(uow.Repository<Order>(), spec, specParams.PageIndex, specParams.PageSize, o => o.ToDto());
    }

    [HttpGet("orders/{id:int}")]
    public async Task<ActionResult<OrderDto>> GetOrderById(int id)
    {
        var spec = new OrderSpecification(id);
        var order = await uow.Repository<Order>().GetEntityWithSpec(spec);

        if(order == null) return BadRequest("Geen bestelling met het meegegeven id.");

        return order.ToDto();
    }

    [HttpPost("orders/refund/{id:int}")]
    public async Task<ActionResult<OrderDto>> RefundOrder(int id)
    {
        var spec = new OrderSpecification(id);

        var order = await uow.Repository<Order>().GetEntityWithSpec(spec);

        if(order == null) return BadRequest("Geen bestellingen gevonden.");
        if(order.BestellingsStatus == OrderStatus.Pending) return BadRequest("De betaling is niet ontvangen voor deze bestelling.");

        var result = await paymentService.RefundPayment(order.BetalingsIntentId);

        if(result == "succeeded")
        {
            order.BestellingsStatus = OrderStatus.Refunded;

            await uow.Complete();

            return order.ToDto();
        }

        return BadRequest("Probleem met het terugbetalen van de bestelling");
    }
}
