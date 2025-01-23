using System;
using Core.Entities;
using Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Stripe;

namespace Infrastructure.Services;

public class PaymentService : IPaymentService
{
    private readonly ICartService cartService;
    private readonly IUnitOfWork uow;
    public PaymentService(IConfiguration config, ICartService cartService, IUnitOfWork uow)
    {
        StripeConfiguration.ApiKey = config["StripeSettings:SecretKey"];
        this.cartService = cartService;
        this.uow = uow;
    }
    //functie om een payment intent aan te maken
    //valideren dat de inhoud van het winkelwagentje overeenstemt met de gegevens in de database (gebruikermanipulatie: eventueel prijs aangepast?)
    public async Task<ShoppingCart?> CreateOrUpdatePaymentIntent(string cartId)
    {
        //wachten op de response van de get card functie in de cart service (winkelwagen zoeken in de database)
        var cart = await cartService.GetCardAsync(cartId) ?? throw new Exception("Winkelwagen is niet beschikbaar");

        //controleren of er al een leveringsprijs gekozen is, anders wordt de waarde 0 gebruikt
        var leveringsPrijs = await GetShippingPriceAsync(cart) ?? 0;

        await ValidateCartItemsInCartAsync(cart);

        //subtotaal berekenen (alle items in winkelwagen * hoeveelheid per item)
        var subtotal = CalculateSubtotal(cart);

        //controleren of er een kortingsbon van toepassing is
        if (cart.Coupon != null)
        {
            subtotal = await ApplyDiscountAsync(cart.Coupon, subtotal);
        }

        //totale prijs berekenen
        var total = subtotal + leveringsPrijs;

        //payment intent voor stripe aanmaken of updaten bij een bestaand intent
        await CreateUpdatePaymentIntentAsync(cart, total);

        await cartService.SetCardAsync(cart);

        return cart;
    }

    public async Task<string> RefundPayment(string paymentIntentId)
    {
        var refundOptions = new RefundCreateOptions
        {
            PaymentIntent = paymentIntentId
        };

        var refundService = new RefundService();
        var result = await refundService.CreateAsync(refundOptions);

        return result.Status;
    }

    private async Task CreateUpdatePaymentIntentAsync(ShoppingCart cart, long total)
    {
        var service = new PaymentIntentService();

        //controleren dat de payment intent id in het winkelwagen object null of leeg is
        if(string.IsNullOrEmpty(cart.PaymentIntentId))
        {
            //nieuw payment intent object aanmaken
            var options = new PaymentIntentCreateOptions
            {
                Amount = total,
                Currency = "eur",
                PaymentMethodTypes = ["card"]
            };

            //aangemaakt paymentintent laten creëren door de payment intent service
            var intent = await service.CreateAsync(options);
            cart.PaymentIntentId = intent.Id;
            cart.ClientSecret = intent.ClientSecret;
        }
        //als er al een payment intent id is in het winkelwagentje dan wordt deze aangepast
        else
        {
            //payment intent object aanpassen
            var options = new PaymentIntentUpdateOptions
            {
                Amount = total
            };
            //aangepast paymentintent laten updaten door de payment intent service
            await service.UpdateAsync(cart.PaymentIntentId, options);
        }
    }

    //functie om de korting te berekenen op het subtotaal
    private async Task<long> ApplyDiscountAsync(AppCoupon appCoupon, long subtotal)
    {
        var couponService = new Stripe.CouponService();

        var coupon = await couponService.GetAsync(appCoupon.CouponId);

        if(coupon.AmountOff.HasValue)
        {
            subtotal -= (long)coupon.AmountOff.Value;
        }
        if(coupon.PercentOff.HasValue)
        {
            var discount = subtotal * (coupon.PercentOff.Value / 100);
            subtotal -= (long)discount;
        }
        
        return subtotal;
    }

    //functie om het subtotaal te berekenen
    private long CalculateSubtotal(ShoppingCart cart)
    {
        var subtotal = cart.Items.Sum(x => x.Hoeveelheid * (x.Prijs * 100));

        //subtotaal terugsturen
        return (long)subtotal;
    }

    //functie om de items in het winkelwagentje te controleren
    private async Task ValidateCartItemsInCartAsync(ShoppingCart cart)
    {
        //voor elk item in het winkelwagentje controleren of de prijs overeenkomt met de prijs van het product in de database
        foreach(var item in cart.Items){
            var productItem = await uow.Repository<Core.Entities.Product>().GetByIdAsync(item.ProductId);

            if(productItem == null) throw new Exception("Product niet gevonden.");

            if(item.Prijs != productItem.Prijs){
                item.Prijs = productItem.Prijs;
            }
        }
    }

    private async Task<long?> GetShippingPriceAsync(ShoppingCart cart)
    {
        //controleren of het winkelwagentje een leveringsmethode id heeft
        if(cart.DeliveryMethodId.HasValue)
        {
            //leveringsmethode ophalen aan de hand van het leveringsmethode id van het cart object
            var deliveryMethod = await uow.Repository<DeliveryMethod>().GetByIdAsync((int)cart.DeliveryMethodId);
            
            if(deliveryMethod == null) throw new Exception("Geen leveringsmethode gevonden.");

            return (long)deliveryMethod.Prijs * 100;
        }

        //als er geen leveringsmethode id in het winkelwagentje zit, dan null terugsturen
        return null;
    }

    
}
