using System;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class AddressDto
{
    [Required]
    public string Straat {get; set;} = string.Empty;
    public string? Toevoeging {get; set;}
    [Required]
    public string Plaats {get; set;} = string.Empty;
    public string? Provincie {get; set;}
    [Required]
    public string Postcode {get; set;} = string.Empty;
    [Required]
    public string Land {get; set;} = string.Empty;
}
