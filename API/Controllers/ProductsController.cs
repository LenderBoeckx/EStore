using System;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]

public class ProductsController(IProductRepository repo) : ControllerBase
{

    [HttpGet] //een lijst van alle producten ophalen
    public async Task<ActionResult<IReadOnlyList<Product>>> GetProducts(string? merk, string? type, string? sort)
    {
        
        return Ok(await repo.GetProductsAsync(merk, type, sort));
    }

    [HttpGet("{id:int}")] //specifiek product zoeken op basis van id
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await repo.GetProductByIdAsync(id);

        if(product == null) return NotFound();

        return product;
    }

    [HttpPost] //een product aanmaken in de database
    public async Task<ActionResult<Product>> CreateProduct(Product product){
        repo.AddProduct(product);
        
       if(await repo.SaveChangesAsync())
       {
        return CreatedAtAction("GetProduct", new {id = product.Id}, product);
       } 

        return BadRequest("Het product kon niet aangemaakt worden.");
    }

    [HttpPut("{id:int}")] //een product uit de database aanpassen
    public async Task<ActionResult> UpdateProduct(int id, Product product)
    {
        if(product.Id != id || !ProductExists(id)) return BadRequest("Dit product kan niet aangepast worden.");

        repo.UpdateProduct(product);

        if(await repo.SaveChangesAsync()){
            return NoContent();
        }

        return BadRequest("Er is een probleem met het aanpassen van het product.");
    }

    [HttpDelete("{id:int}")] //een product uit de database verwijderen
    public async Task<ActionResult> DeleteProduct(int id)
    {
        var product = await repo.GetProductByIdAsync(id);

        if(product == null) return NotFound();

        repo.DeleteProduct(product);

        if(await repo.SaveChangesAsync()){
            return NoContent();
        }

        return BadRequest("Het product kon niet verwijderd worden.");
    }

    [HttpGet("brands")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetMerken(){
        return Ok(await repo.GetMerkenAsync());
    }

    [HttpGet("types")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetTypes(){
        return Ok(await repo.GetTypesAsync());
    }

    private bool ProductExists(int id) //controleren of een product voor de meegegeven id bestaat
    {
        return repo.ProductExists(id);
    }

}
