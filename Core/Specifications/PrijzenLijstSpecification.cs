using System;
using Core.Entities;

namespace Core.Specifications;

public class PrijzenLijstSpecification : BaseSpecification<Product, decimal>
{
    public PrijzenLijstSpecification()
    {
        AddSelect(x => x.Prijs);
        ApplyDistinct();
    }
}
