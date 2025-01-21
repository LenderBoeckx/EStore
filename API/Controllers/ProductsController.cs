using System;
using API.RequestHelpers;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]

public class ProductsController(IUnitOfWork uow) : BaseApiController
{

    [HttpGet] //een lijst van alle producten ophalen die binnen de ingegeven pagination vallen
    public async Task<ActionResult<IReadOnlyList<Product>>> GetProducts([FromQuery]ProductSpecParams specParams)
    {
        var spec = new ProductSpecification(specParams);
        
        return await CreatepagedResult(uow.Repository<Product>(), spec, specParams.PageIndex, specParams.PageSize);
    }

    [HttpGet("{id:int}")] //specifiek product zoeken op basis van id
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await uow.Repository<Product>().GetByIdAsync(id);

        if(product == null) return NotFound();

        return product;
    }

    [HttpPost] //een product aanmaken in de database
    public async Task<ActionResult<Product>> CreateProduct(Product product){
        uow.Repository<Product>().Add(product);
        
       if(await uow.Complete())
       {
        return CreatedAtAction("GetProduct", new {id = product.Id}, product);
       } 

        return BadRequest("Het product kon niet aangemaakt worden.");
    }

    [HttpPut("{id:int}")] //een product uit de database aanpassen
    public async Task<ActionResult> UpdateProduct(int id, Product product)
    {
        if(product.Id != id || !ProductExists(id)) return BadRequest("Dit product kan niet aangepast worden.");

        uow.Repository<Product>().Update(product);

        if(await uow.Complete()){
            return NoContent();
        }

        return BadRequest("Er is een probleem met het aanpassen van het product.");
    }

    [HttpDelete("{id:int}")] //een product uit de database verwijderen
    public async Task<ActionResult> DeleteProduct(int id)
    {
        var product = await uow.Repository<Product>().GetByIdAsync(id);

        if(product == null) return NotFound();

        uow.Repository<Product>().Remove(product);

        if(await uow.Complete()){
            return NoContent();
        }

        return BadRequest("Het product kon niet verwijderd worden.");
    }

    //filteren op de ingegeven merken
    [HttpGet("merken")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetMerken()
    {
        var spec = new MerkenLijstSpecification();
        return Ok(await uow.Repository<Product>().ListAsync(spec));
    }

    //filteren op de ingegeven types
    [HttpGet("types")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetTypes()
    {
        var spec = new TypeLijstSpecification();

        return Ok(await uow.Repository<Product>().ListAsync(spec));
    }

    //filteren op de ingegeven prijzen
    [HttpGet("prijzen")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetPrijzen()
    {
        var spec = new PrijzenLijstSpecification();

        return Ok(await uow.Repository<Product>().ListAsync(spec));
    }

    private bool ProductExists(int id) //controleren of een product voor de meegegeven id bestaat in de database
    {
        return uow.Repository<Product>().Exists(id);
    }

}
