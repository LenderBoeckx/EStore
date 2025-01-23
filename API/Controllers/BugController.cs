using System;
using System.Security.Claims;
using API.DTOs;
using Microsoft.AspNetCore.Authorization;
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

    [Authorize]
    [HttpGet("secret")]
    public IActionResult GetSecret()
    {
        var name = User.FindFirst(ClaimTypes.Name)?.Value;
        var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        return Ok("Hallo " + name + " met id: " + id);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("admin-secret")]
    public IActionResult GetAdminSecret()
    {
        var name = User.FindFirst(ClaimTypes.Name)?.Value;
        var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var isAdmin = User.IsInRole("Admin");
        var roles = User.FindFirstValue(ClaimTypes.Role);

        return Ok(new
        {
            name,
            id,
            isAdmin,
            roles
        });
    }
}
