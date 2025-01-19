using System;
using Core.Entities;
using Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Stripe;

namespace Infrastructure.Services;

public class PaymentService(IConfiguration config, ICartService cartService, IGenericRepository<Core.Entities.Product> productRepo, IGenericRepository<DeliveryMethod> dmRepo) : IPaymentService
{
    //functie om een payment intent aan te maken
    //valideren dat de inhoud van het winkelwagentje overeenstemt met de gegevens in de database (gebruikermanipulatie: eventueel prijs aangepast?)
    public async Task<ShoppingCart?> CreateOrUpdatePaymentIntent(string cartId)
    {
        StripeConfiguration.ApiKey = config["StripeSettings:SecretKey"];

        //wachten op de response van de get card functie in de cart service (winkelwagen zoeken in de database)
        var cart = await cartService.GetCardAsync(cartId);

        if(cart == null) return null;

        //leveringsprijs op 0 zetten, 'm' om mee te geven dat het een decimal type is
        var leveringsPrijs = 0m;

        //controleren of het winkelwagentje een leveringsmethode id heeft
        if(cart.DeliveryMethodId.HasValue)
        {
            //leveringsmethode ophalen aan de hand van het leveringsmethode id van het cart object
            var deliveryMethod = await dmRepo.GetByIdAsync((int)cart.DeliveryMethodId);
            
            if(deliveryMethod == null) return null;

            leveringsPrijs = deliveryMethod.Prijs;
        }

        //voor elk item in het winkelwagentje controleren of de prijs overeenkomt met de prijs van het product in de database
        foreach(var item in cart.Items){
            var productItem = await productRepo.GetByIdAsync(item.ProductId);

            if(productItem == null) return null;

            if(item.Prijs != productItem.Prijs){
                item.Prijs = productItem.Prijs;
            }
        }

        var service = new PaymentIntentService();
        PaymentIntent? intent = null;

        //controleren dat de payment intent id in het winkelwagen object null of leeg is
        if(string.IsNullOrEmpty(cart.PaymentIntentId))
        {
            //nieuw payment intent object aanmaken
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)cart.Items.Sum(x => x.Hoeveelheid * (x.Prijs * 100)) + (long)leveringsPrijs * 100,
                Currency = "eur",
                PaymentMethodTypes = ["card"]
            };

            //aangemaakt paymentintent laten creëren door de payment intent service
            intent = await service.CreateAsync(options);
            cart.PaymentIntentId = intent.Id;
            cart.ClientSecret = intent.ClientSecret;
        }
        //als er al een payment intent id is in het winkelwagentje dan wordt deze aangepast
        else
        {
            //payment intent object aanpassen
            var options = new PaymentIntentUpdateOptions
            {
                Amount = (long)cart.Items.Sum(x => x.Hoeveelheid * (x.Prijs * 100)) + (long)leveringsPrijs * 100
            };
            //aangepast paymentintent laten updaten door de payment intent service
            intent = await service.UpdateAsync(cart.PaymentIntentId, options);
        }

        await cartService.SetCardAsync(cart);

        return cart;
    }
}
