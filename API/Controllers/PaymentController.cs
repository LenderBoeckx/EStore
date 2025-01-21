using System;
using API.Extensions;
using API.SignalR;
using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Stripe;

namespace API.Controllers;

public class PaymentController(IPaymentService paymentService, IUnitOfWork uow, ILogger<PaymentController> logger, IConfiguration config, IHubContext<NotificationHub> hubContext) : BaseApiController
{
    private readonly string _whSecret = config["StripeSettings:WhSecret"]!;
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

    [HttpPost("webhook")]
    public async Task<IActionResult> StripeWebhook()
    {
        var json = await new StreamReader(Request.Body).ReadToEndAsync();

        try
        {
            var stripeEvent = ConstructStripeEvent(json);

            if(stripeEvent.Data.Object is not PaymentIntent intent)
            {
                return BadRequest("Onvolledige event data.");
            }

            await HandlePaymentIntentSucceeded(intent);

            return Ok();
        }
        catch(StripeException ex)
        {
            logger.LogError(ex, "Stripe webhook fout.");
            return StatusCode(StatusCodes.Status500InternalServerError, "a webhook error occurred.");
        }
        catch(Exception ex)
        {
            logger.LogError(ex, "Er is een onverwachte fout opgetreden.");
            return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
        }
    }

    private async Task HandlePaymentIntentSucceeded(PaymentIntent intent)
    {
        if(intent.Status == "succeeded")
        {
            var spec = new OrderSpecification(intent.Id, true);

            //order ophalen uit de database aan de hand van het meegegeven id
            var order = await uow.Repository<Core.Entities.OrderAggregate.Order>().GetEntityWithSpec(spec) ?? throw new Exception("Bestelling niet gevonden.");

            //controleren of het bedrag van het afgewerkt order hetzelfde is als het bedrag dat Stripe terug geeft
            if((long)order.GetTotaal() * 100 != intent.Amount)
            {
                order.BestellingsStatus = OrderStatus.PaymentMismatch;
            }
            else
            {
                order.BestellingsStatus = OrderStatus.PaymentReceived;
            }

            //wijzigingen opslaan in de database
            await uow.Complete();

            var connectionId = NotificationHub.GetConnectionIdByEmail(order.KoperEmail);

            if(!string.IsNullOrEmpty(connectionId))
            {
                await hubContext.Clients.Client(connectionId).SendAsync("OrderCompleteNotification", order.ToDto());
            }
        }
    }

    private Event ConstructStripeEvent(string json)
    {
        try
        {
            return EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], _whSecret);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Stripe event aanmaken is niet gelukt.");
            throw new StripeException("foutieve signature.");
        }
    }
}
