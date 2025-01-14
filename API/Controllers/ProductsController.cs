using System;
using API.RequestHelpers;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]

public class ProductsController(IGenericRepository<Product> repo) : BaseApiController
{

    [HttpGet] //een lijst van alle producten ophalen die binnen de ingegeven pagination vallen
    public async Task<ActionResult<IReadOnlyList<Product>>> GetProducts([FromQuery]ProductSpecParams specParams)
    {
        var spec = new ProductSpecification(specParams);
        
        return await CreatepagedResult(repo, spec, specParams.PageIndex, specParams.PageSize);
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

    //filteren op de ingegeven merken
    [HttpGet("merken")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetMerken()
    {
        var spec = new MerkenLijstSpecification();
        return Ok(await repo.ListAsync(spec));
    }

    //filteren op de ingegeven types
    [HttpGet("types")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetTypes()
    {
        var spec = new TypeLijstSpecification();

        return Ok(await repo.ListAsync(spec));
    }

    //filteren op de ingegeven prijzen
    [HttpGet("prijzen")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetPrijzen()
    {
        var spec = new PrijzenLijstSpecification();

        return Ok(await repo.ListAsync(spec));
    }

    private bool ProductExists(int id) //controleren of een product voor de meegegeven id bestaat in de database
    {
        return repo.Exists(id);
    }

}
