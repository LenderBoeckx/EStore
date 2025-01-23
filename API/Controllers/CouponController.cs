using System;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class CouponController(ICouponService couponService) : BaseApiController
{
    [HttpGet("{code}")]
    public async Task<ActionResult<AppCoupon>> ValidateCoupon(string code)
    {
        var coupon = await couponService.GetCouponFromPromoCode(code);

        if(coupon == null) return BadRequest("Ongeldige kortingscode");

        return coupon;
    }
}
