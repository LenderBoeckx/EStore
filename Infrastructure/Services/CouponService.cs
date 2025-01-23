using System;
using Core.Entities;
using Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Stripe;

namespace Infrastructure.Services;

public class CouponService: ICouponService
{
    public CouponService(IConfiguration config)
    {
        StripeConfiguration.ApiKey = config["StripeSettings:SecretKey"];
    }
    public async Task<AppCoupon?> GetCouponFromPromoCode(string code)
    {
        var promotionCodeService = new PromotionCodeService();

        var options = new PromotionCodeListOptions
        {
            Code = code
        };

        var promotionCodes = await promotionCodeService.ListAsync(options);

        var promotioncode = promotionCodes.FirstOrDefault();

        if(promotioncode != null && promotioncode.Coupon != null)
        {
            return new AppCoupon
            {
                CouponId = promotioncode.Coupon.Id,
                AmountOff = promotioncode.Coupon.AmountOff,
                PercentOff = promotioncode.Coupon.PercentOff,
                Name = promotioncode.Coupon.Name,
                PromotionCode = promotioncode.Code
            };
        }

        return null;
    }
}
