using System;
using API.DTOs;
using API.RequestHelpers;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]

public class ProductsController(IUnitOfWork uow, BlobStorageService _storageService) : BaseApiController
{
    [Cache(600)]
    [HttpGet] //een lijst van alle producten ophalen die binnen de ingegeven pagination vallen
    public async Task<ActionResult<IReadOnlyList<Product>>> GetProducts([FromQuery]ProductSpecParams specParams)
    {
        var spec = new ProductSpecification(specParams);
        
        return await CreatepagedResult(uow.Repository<Product>(), spec, specParams.PageIndex, specParams.PageSize);
    }

    [Cache(600)]
    [HttpGet("{id:int}")] //specifiek product zoeken op basis van id
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await uow.Repository<Product>().GetByIdAsync(id);

        if(product == null) return NotFound();

        return product;
    }

    [InvalidateCache("api/products|")]
    [Authorize(Roles = "Admin")]
    [HttpPost] //een product aanmaken in de database
    public async Task<ActionResult<Product>> CreateProduct(CreateProductDTO productDto)
    {
        if (productDto.Image != null)
        {
            // Sla de URL op in productDto
            var result = await AddFoto(productDto.Image);
            if (result.Result is BadRequestObjectResult badRequest)
            {
                return BadRequest(badRequest.Value);
            }

            productDto.FotoURL = result.Value;
        }

        if(string.IsNullOrEmpty(productDto.FotoURL) || productDto.FotoURL == "Er is al een afbeelding met dezelfde naam in de opslag.")
        {    
            return BadRequest("Er is al een bestand met deze naam in de database.");
        }

        var product = new Product
        {
            Naam = productDto.Naam,
            Beschrijving = productDto.Beschrijving,
            Prijs = productDto.Prijs,
            Merk = productDto.Merk,
            Type = productDto.Type,
            HoeveelheidInVoorraad = productDto.HoeveelheidInVoorraad,
            FotoURL = productDto.FotoURL,
  
        };
        

        uow.Repository<Product>().Add(product);

            if(await uow.Complete())
            {
                return CreatedAtAction("GetProduct", new {id = product.Id}, product);
            }
        
            return BadRequest("Het product kon niet aangemaakt worden.");
    }

    [InvalidateCache("api/products|")]
    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")] //een product uit de database aanpassen
    public async Task<ActionResult> UpdateProduct(int id, UpdateProductDto productDto)
    {
        if(productDto.Id != id || !ProductExists(id)) return BadRequest("Dit product kan niet aangepast worden.");

        var product = await uow.Repository<Product>().GetByIdAsync(id);
        if (product == null) return NotFound("Product niet gevonden.");

        if (productDto.Image != null)
        {
            // Sla de URL op in productDto
            var result = await AddFoto(productDto.Image);
            if (result.Result is BadRequestObjectResult badRequest)
            {
                return BadRequest(badRequest.Value);
            }
            
            //oude foto verwijderen
            await DeleteFoto(product.FotoURL);

            productDto.FotoURL = result.Value!;
        }

        if(productDto.FotoURL == "Er is al een afbeelding met dezelfde naam in de opslag.")
        {    
            return BadRequest("Er is al een bestand met deze naam in de database.");
        }

        product.Naam = productDto.Naam;
        product.Beschrijving = productDto.Beschrijving;
        product.Prijs = productDto.Prijs;
        product.Merk = productDto.Merk;
        product.Type = productDto.Type;
        product.HoeveelheidInVoorraad = productDto.HoeveelheidInVoorraad;
        product.FotoURL = productDto.FotoURL;

        uow.Repository<Product>().Update(product);

        if(await uow.Complete()){
            return Ok(product);
        }

        return BadRequest("Er is een probleem met het aanpassen van het product.");
    }

    [InvalidateCache("api/products|")]
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")] //een product uit de database verwijderen
    public async Task<ActionResult> DeleteProduct(int id)
    {
        var product = await uow.Repository<Product>().GetByIdAsync(id);

        if(product == null) return NotFound();

        var deleted = await DeleteFoto(product.FotoURL);

        if(deleted)
        {
            uow.Repository<Product>().Remove(product);

            if(await uow.Complete()){
                return NoContent();
            }

            return BadRequest("Het product kon niet verwijderd worden.");
        }

        return BadRequest("De afbeelding van het product kon niet verwijderd worden.");
    }

    [Authorize(Roles = "Admin")]
    public async Task<bool> DeleteFoto(string fotoUrl) {
        string path = fotoUrl;
        string fileName = path.Split('/').Last();

        var deleted = await _storageService.DeleteFileAsync(fileName);

        return deleted;
    }

    [Authorize(Roles ="Admin")]
    public async Task<ActionResult<string>> AddFoto(IFormFile image)
    {
        List<string> extentions = new List<string>() { ".jpg", ".jpeg", ".png" };
        string extention = Path.GetExtension(image.FileName).ToLower();
        long size = image.Length;
        string url;

        if(!extentions.Contains(extention))
        {
            return BadRequest("Het bestand is niet voorzien van de juiste bestandsextentie.");
        }
        if(size > (5 * 1024 * 1024)) 
        {
            return BadRequest("Het bestand is te groot. De maximale bestandsgrootte is 5MB.");
        }

        using (var stream = image.OpenReadStream())
        {
            // Upload het bestand naar Blob Storage
            var imageUrl = await _storageService.UploadFileAsync(stream, image.FileName);

            // Sla de URL op in productDto
            url = imageUrl;
        }

        return url;
    }

    [Cache(10000)]
    [HttpGet("merken")] //filteren op de ingegeven merken
    public async Task<ActionResult<IReadOnlyList<string>>> GetMerken()
    {
        var spec = new MerkenLijstSpecification();
        return Ok(await uow.Repository<Product>().ListAsync(spec));
    }

    [Cache(10000)]
    [HttpGet("types")] //filteren op de ingegeven types
    public async Task<ActionResult<IReadOnlyList<string>>> GetTypes()
    {
        var spec = new TypeLijstSpecification();

        return Ok(await uow.Repository<Product>().ListAsync(spec));
    }

    [Cache(10000)]
    [HttpGet("prijzen")] //filteren op de ingegeven prijzen
    public async Task<ActionResult<IReadOnlyList<string>>> GetPrijzen()
    {
        var spec = new PrijzenLijstSpecification();

        return Ok(await uow.Repository<Product>().ListAsync(spec));
    }

    [HttpGet("afbeeldingen")] //filteren op de ingegeven prijzen
    public async Task<ActionResult<IReadOnlyList<string>>> GetAfbeeldingen()
    {
        var spec = new AfbeeldingenSpecification();

        return Ok(await uow.Repository<Product>().ListAsync(spec));
    }

    private bool ProductExists(int id) //controleren of een product voor de meegegeven id bestaat in de database
    {
        return uow.Repository<Product>().Exists(id);
    }

}
