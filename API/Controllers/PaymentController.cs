using System;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class PaymentController(IPaymentService paymentService, IUnitOfWork uow) : BaseApiController
{
    [Authorize]
    [HttpPost("{cartId}")]
    //endpoint creëren om een payment intent op te stellen
    public async Task<ActionResult<ShoppingCart>> CreateOrUpdatePaymentIntent(string cartId)
    {
        var cart = await paymentService.CreateOrUpdatePaymentIntent(cartId);

    if(cart == null) return BadRequest("Probleem met uw winkelmandje.");

    return Ok(cart);
    }

    [HttpGet("delivery-methods")]
    //endpoint creëren om de verschillende leveringsmogelijkheden op te halen
    public async Task<ActionResult<IReadOnlyList<DeliveryMethod>>> GetDeliveryMethods()
    {
        return Ok(await uow.Repository<DeliveryMethod>().ListAllAsync());
    }
}
