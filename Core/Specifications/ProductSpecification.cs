using System;
using Core.Entities;

namespace Core.Specifications;

public class ProductSpecification : BaseSpecification<Product>
{
    public ProductSpecification(string? merk, string? type, string? sort) : base(x=>
        (string.IsNullOrWhiteSpace(merk) || x.Merk == merk) &&
        (string.IsNullOrWhiteSpace(type) || x.Type == type)
    )
    {
        switch(sort)
        {
            case "prijsAsc":
                AddOrderBy(x => x.Prijs);
                break;
            case "prijsDesc":
                AddOrderByDesc(x => x.Prijs);
                break;
            default:
                AddOrderBy(x => x.Naam);
                break;
        }
    }
}
