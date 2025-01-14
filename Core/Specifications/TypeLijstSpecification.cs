using System;
using Core.Entities;

namespace Core.Specifications;

public class TypeLijstSpecification : BaseSpecification<Product, string>
{
    public TypeLijstSpecification()
    {
        AddSelect(x => x.Type);
        ApplyDistinct();
    }
}
