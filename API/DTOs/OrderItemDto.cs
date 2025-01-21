namespace API.DTOs;

public class OrderItemDto
{
    public int ProductId {get; set;}
    public required string ProductNaam {get; set;}
    public required string FotoUrl {get; set;}
    public decimal Prijs {get; set;}
    public int Hoeveelheid {get; set;}

}