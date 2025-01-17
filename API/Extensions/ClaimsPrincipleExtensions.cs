using System;
using System.Security.Authentication;
using System.Security.Claims;
using Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions;

public static class ClaimsPrincipleExtensions
{
    //een user zoeken in de database aan de hand van de gevonden email
    public static async Task<AppUser> GetUserByEmail(this UserManager<AppUser> userManager, ClaimsPrincipal user)
    {
        //de eerste user zoeken in de database waarvan de email gelijk is aan de email van de geclaimde user
        var userToReturn = await userManager.Users.FirstOrDefaultAsync(x => x.Email == user.GetEmail());

        if(userToReturn == null) throw new AuthenticationException("User niet gevonden.");

        return userToReturn;
    }

    public static async Task<AppUser> GetUserByEmailWithAddress(this UserManager<AppUser> userManager, ClaimsPrincipal user)
    {
        //de eerste user zoeken in de database waarvan de email gelijk is aan de email van de geclaimde user
        var userToReturn = await userManager.Users.Include(x => x.Address).FirstOrDefaultAsync(x => x.Email == user.GetEmail());

        if(userToReturn == null) throw new AuthenticationException("User niet gevonden.");

        return userToReturn;
    }


    //email uit de geclaimde user halen
    public static string GetEmail(this ClaimsPrincipal user)
    {
        var email = user.FindFirstValue(ClaimTypes.Email);

        if(email == null) throw new AuthenticationException("Email niet gevonden.");

        //de gevonden email als returnwaarde geven
        return email;
    }
}
