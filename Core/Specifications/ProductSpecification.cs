using System;
using Core.Entities;

namespace Core.Specifications;

public class ProductSpecification : BaseSpecification<Product>
{
    public ProductSpecification(ProductSpecParams specParams) : base(x=>
        (string.IsNullOrEmpty(specParams.Search) || x.Naam.ToLower().Contains(specParams.Search)) &&
        (!specParams.Merken.Any() || specParams.Merken.Contains(x.Merk)) &&
        (!specParams.Types.Any() || specParams.Types.Contains(x.Type))
    )
    {
        ApplyPaging(specParams.PageSize * (specParams.PageIndex - 1), specParams.PageSize);

        switch(specParams.Sort)
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
