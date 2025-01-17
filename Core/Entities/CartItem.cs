using System;

namespace Core.Entities;

public class CartItem
{
    public int ProductId {get; set;}
    public required string ProductNaam {get; set;}
    public decimal Prijs {get; set;}
    public int Hoeveelheid {get; set;}
    public required string FotoUrl {get; set;}
    public required string Merk {get; set;}
    public required string Type {get; set;}
}
