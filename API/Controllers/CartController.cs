using System;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class CartController(ICartService cartService) : BaseApiController
{
    //één bepaalde winkelwagen ophalen aan de hand van de meegegeven Id, als er geen winkelwagen met dat Id bestaat dan wordt er een nieuwe winkelwagen teruggestuurd
    [HttpGet]
    public async Task<ActionResult<ShoppingCart>> GetCartById(string id){
        var cart = await cartService.GetCardAsync(id);

        return Ok(cart ?? new ShoppingCart{Id = id});
    }

    //een bestaande winkelwagen updaten door de nieuwe winkelwagen mee te geven
    [HttpPost]
    public async Task<ActionResult<ShoppingCart>> UpdateCart(ShoppingCart cart){
        var updatedCart = await cartService.SetCardAsync(cart);

        if(updatedCart == null) return BadRequest("Probleem met winkelwagen.");

        return updatedCart;
    }

    //één bepaalde winkelwagen verwijderen aan de hand van een meegegeven Id
    [HttpDelete]
    public async Task<ActionResult> DeleteCart(string id){
        var result = await cartService.DeleteCardAsync(id);

        if(!result) return BadRequest("Probleem met het verwijderen van de winkelwagen.");

        return Ok();
    }
}
