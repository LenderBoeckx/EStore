using System;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]

public class ProductsController(IGenericRepository<Product> repo) : ControllerBase
{

    [HttpGet] //een lijst van alle producten ophalen
    public async Task<ActionResult<IReadOnlyList<Product>>> GetProducts(string? merk, string? type, string? sort)
    {
        var spec = new ProductSpecification(merk, type, sort);

        var products = await repo.ListAsync(spec);
        
        return Ok(products);
    }

    [HttpGet("{id:int}")] //specifiek product zoeken op basis van id
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await repo.GetByIdAsync(id);

        if(product == null) return NotFound();

        return product;
    }

    [HttpPost] //een product aanmaken in de database
    public async Task<ActionResult<Product>> CreateProduct(Product product){
        repo.Add(product);
        
       if(await repo.SaveAllAsync())
       {
        return CreatedAtAction("GetProduct", new {id = product.Id}, product);
       } 

        return BadRequest("Het product kon niet aangemaakt worden.");
    }

    [HttpPut("{id:int}")] //een product uit de database aanpassen
    public async Task<ActionResult> UpdateProduct(int id, Product product)
    {
        if(product.Id != id || !ProductExists(id)) return BadRequest("Dit product kan niet aangepast worden.");

        repo.Update(product);

        if(await repo.SaveAllAsync()){
            return NoContent();
        }

        return BadRequest("Er is een probleem met het aanpassen van het product.");
    }

    [HttpDelete("{id:int}")] //een product uit de database verwijderen
    public async Task<ActionResult> DeleteProduct(int id)
    {
        var product = await repo.GetByIdAsync(id);

        if(product == null) return NotFound();

        repo.Remove(product);

        if(await repo.SaveAllAsync()){
            return NoContent();
        }

        return BadRequest("Het product kon niet verwijderd worden.");
    }

    [HttpGet("merken")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetMerken()
    {
        var spec = new MerkenLijstSpecification();
        return Ok(await repo.ListAsync(spec));
    }

    [HttpGet("types")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetTypes()
    {
        var spec = new TypeLijstSpecification();

        return Ok(await repo.ListAsync(spec));
    }
    [HttpGet("prijzen")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetPrijzen()
    {
        var spec = new PrijzenLijstSpecification();

        return Ok(await repo.ListAsync(spec));
    }

    private bool ProductExists(int id) //controleren of een product voor de meegegeven id bestaat
    {
        return repo.Exists(id);
    }

}
