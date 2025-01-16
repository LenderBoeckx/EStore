using System;
using API.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class BugController : BaseApiController
{
    [HttpGet("unauthorized")]
    public IActionResult GetUnauthorized() 
    {
        return Unauthorized();
    }

    [HttpGet("badrequest")]
    public IActionResult GetBadRequest() 
    {
        return BadRequest("Geen geldige aanvraag.");
    }

    [HttpGet("notfound")]
    public IActionResult GetNotFound() 
    {
        return NotFound();
    }

    [HttpGet("internalerror")]
    public IActionResult GetInternalError() 
    {
        throw new Exception("Er treed een interne fout op.");
    }

    [HttpPost("validationerror")]
    public IActionResult GetValidationError(CreateProductDTO product) 
    {
        return Ok();
    }
}
