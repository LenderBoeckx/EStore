using System;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class CreateProductDTO
{
    [Required(ErrorMessage = "Het veld naam moet ingevuld zijn.")]
    public string Naam { get; set;} = string.Empty;

    [Required(ErrorMessage = "Het veld Beschrijving moet ingevuld zijn.")]
    public string Beschrijving {get; set;} = string.Empty;

    [Range(0.01, double.MaxValue, ErrorMessage = "De ingevoerde prijs moet hoger dan 0 zijn.")]
    public decimal Prijs {get; set;}

    [Required(ErrorMessage = "Het veld type moet ingevuld zijn.")]
    public string Type {get; set;} = string.Empty;

    [Required(ErrorMessage = "Het veld merk moet ingevuld zijn.")]
    public string Merk {get; set;} = string.Empty;

    [Range(0, int.MaxValue, ErrorMessage = "De hoeveelheid in voorraad moet minstens 0 zijn.")]
    public int HoeveelheidInVoorraad {get; set;}
    
    [Required(ErrorMessage = "Er is geen bestand bijgevoegd.")]
    public required IFormFile Image { get; set; }

    public string? FotoURL {get; set;}
}
