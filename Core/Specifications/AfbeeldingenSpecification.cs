using System;
using Core.Entities;

namespace Core.Specifications;

public class AfbeeldingenSpecification : BaseSpecification<Product, string>
{
    public AfbeeldingenSpecification()
    {
        AddSelect(x => x.FotoURL);
        ApplyDistinct();
    }
}
