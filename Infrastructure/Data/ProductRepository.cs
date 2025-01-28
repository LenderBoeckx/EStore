using System;
using System.Diagnostics;
using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class ProductRepository(StoreContext context) : IProductRepository
{
    //vanuit deze repository wordt de context aangesproken om verschillende CRUD operaties uit te voeren
    public void AddProduct(Product product)
    {
        context.Products.Add(product);
    }

    public void DeleteProduct(Product product)
    {
        context.Products.Remove(product);
    }

    public async Task<IReadOnlyList<string>> GetMerkenAsync(){
        return await context.Products.Select(x => x.Merk).Distinct().ToListAsync();
    }

    public async Task<IReadOnlyList<string>> GetTypesAsync(){
        return await context.Products.Select(x => x.Type).Distinct().ToListAsync();
    }

    public async Task<Product?> GetProductByIdAsync(int id)
    {
        return await context.Products.FindAsync(id);
    }

    public async Task<IReadOnlyList<Product>> GetProductsAsync(string? merk, string? type, string? sort)
    {
        var query = context.Products.AsQueryable();

        if(!string.IsNullOrWhiteSpace(merk))
            query = query.Where(x => x.Merk == merk);

        if(!string.IsNullOrWhiteSpace(type))
            query = query.Where(x => x.Type == type);
        
        query = sort switch{
            "prijsAsc" => query.OrderBy(x => x.Prijs),
            "prijsDesc" => query.OrderByDescending(x => x.Prijs),
             _ => query.OrderBy(x => x.Naam)
        };
        
        return await query.ToListAsync();
    }

    public bool ProductExists(int id)
    {
        return context.Products.Any(x => x.Id == id);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await context.SaveChangesAsync() > 0;
    }

    public void UpdateProduct(Product product)
    {
        context.Entry(product).State = EntityState.Modified;
    }

    public async Task<IReadOnlyList<string>> GetAfbeeldingenAsync()
    {
        return await context.Products.Select(x => x.FotoURL).Distinct().ToListAsync();
    }
}
