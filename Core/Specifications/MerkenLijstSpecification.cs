using System;
using Core.Entities;

namespace Core.Specifications;

public class MerkenLijstSpecification : BaseSpecification<Product, string>
{
    public MerkenLijstSpecification()
    {
        AddSelect(x => x.Merk);
        ApplyDistinct();
    }
}
