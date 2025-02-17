using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Core.Entities;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Data;

public class StoreContextSeed
{
    //dummy data toevoegen aan de database bij opstarten van de applicatie
    public static async Task SeedAsync(StoreContext context, UserManager<AppUser> userManager)
    {
        if(!userManager.Users.Any(x => x.UserName == "admin@test.com"))
        {
            var user = new AppUser
            {
                UserName = "admin@test.com",
                Email = "admin@test.com",
            };

            await userManager.CreateAsync(user, "Pa$$w0rd");
            await userManager.AddToRoleAsync(user, "Admin");
        }

        var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        
        //als er geen data aanwezig is in de producten entiteit dan wordt deze toegevoegd vanuit het json bestand
        if(!context.Products.Any())
        {
            var productsData = await File.ReadAllTextAsync(path + @"/Data/SeedData/products.json");

            var products = JsonSerializer.Deserialize<List<Product>>(productsData);

            if(products == null) return;

            context.Products.AddRange(products);

            //data opslaan in de database
            await context.SaveChangesAsync();
        }

        //als er geen data aanwezig is in de deliverymethods entiteit dan wordt deze toegevoegd vanuit het json bestand
        if(!context.DeliveryMethods.Any())
        {
            var deliveryData = await File.ReadAllTextAsync(path + @"/Data/SeedData/delivery.json");

            var methods = JsonSerializer.Deserialize<List<DeliveryMethod>>(deliveryData);

            if(methods == null) return;

            context.DeliveryMethods.AddRange(methods);

            //data opslaan in de database
            await context.SaveChangesAsync();
        }
    }
}
