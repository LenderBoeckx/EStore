using System;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities;

public class Product: BaseEntity
{
    public required string Naam { get; set;}
    public required string Beschrijving {get; set;}
    public decimal Prijs {get; set;}
    public required string FotoURL {get; set;}
    public required string Type {get; set;}
    public required string Merk {get; set;}
    public int HoeveelheidInVoorraad {get; set;}
}
