using System;
using System.Security.Claims;
using API.DTOs;
using API.Extensions;
using Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController(SignInManager<AppUser> signInManager) : BaseApiController
{
    //methode om een user te registreren
    [HttpPost("register")]
    public async Task<ActionResult> Register(RegisterDto registerDto)
    {
        //nieuwe user aanmaken
        var user = new AppUser {
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName,
            Email = registerDto.Email,
            UserName = registerDto.Email
        };

        //user aanmaken in de database
        var result = await signInManager.UserManager.CreateAsync(user, registerDto.Password);

        //als het aanmaken niet lukt, de verschillende error codes en beschrijvingen toevoegen aan de Modelstate
        if(!result.Succeeded)
        {
            foreach(var error in result.Errors){
                ModelState.AddModelError(error.Code, error.Description);
            }

            return ValidationProblem();
        }

        return Ok();
    }

    //methode om een user uit te loggen
    //user moet authorized zijn om deze functie te kunnen gebruiken
    [Authorize]
    [HttpPost("logout")]
    public async Task<ActionResult> LogOut()
    {
        await signInManager.SignOutAsync();

        return NoContent();
    }

    //info over de gebruiker opvragen
    [HttpGet("user-info")]
    public async Task<ActionResult> GetUserInfo()
    {
        //als de gebruiker niet geauthenticeerd is, dan een nocontent terugsturen
        if(User.Identity?.IsAuthenticated == false) return NoContent();

        //user zoeken in de database met de email die wordt meegegeven in het User object --> extensions
        var user = await signInManager.UserManager.GetUserByEmailWithAddress(User);

        //bij een gevonden user, worden de gegevens terug gestuurd
        return Ok(new {
            user.FirstName,
            user.LastName,
            user.Email,
            Address = user.Address?.ToDto(),
            Roles = User.FindFirstValue(ClaimTypes.Role)
        });
    }

    //controleren of een gebruiker geauthenticeerd is
    [HttpGet("auth-status")]
    public ActionResult GetAuthState()
    {
        return Ok(new {IsAuthenticated = User.Identity?.IsAuthenticated ?? false});
    }

    [Authorize]
    [HttpPost("address")]
    public async Task<ActionResult<Address>> CreateOrUpdateAddress(AddressDto addressDto)
    {
        var user = await signInManager.UserManager.GetUserByEmailWithAddress(User);

        if(user.Address == null)
        {
            user.Address = addressDto.ToEntity();
        }
        else
        {
            user.Address.UpdateFromDto(addressDto);
        }

        var result = await signInManager.UserManager.UpdateAsync(user);

        if(!result.Succeeded) return BadRequest("Fout bij het aanpassen van het adres.");

        return Ok(user.Address.ToDto());


    }
}
