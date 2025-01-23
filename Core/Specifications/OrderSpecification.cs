using System;
using System.Security.Cryptography.X509Certificates;
using Core.Entities.OrderAggregate;

namespace Core.Specifications;

public class OrderSpecification : BaseSpecification<Order>
{
    public OrderSpecification(string email) : base(x => x.KoperEmail == email)
    {
        //items in de winkelwagen en de leveringsmethode mee opvragen
        AddInclude(x => x.BestellingsItems);
        AddInclude(x => x.LeveringsMethode);
        
        //resultaat sorteren op bestellingsdatum
        AddOrderBy(x => x.BestellingsDatum);
    }
    public OrderSpecification(string email, int id) : base(x => x.KoperEmail == email && x.Id == id)
    {
        AddInclude("BestellingsItems");
        AddInclude("LeveringsMethode");
    }

    public OrderSpecification(string betalingsIntentId, bool isBetalingsIntent): base(x => x.BetalingsIntentId == betalingsIntentId)
    {
        AddInclude("BestellingsItems");
        AddInclude("LeveringsMethode");
    }

    public OrderSpecification(OrderSpecParams specParams) : base(x => string.IsNullOrEmpty(specParams.Status) || x.BestellingsStatus == ParseStatus(specParams.Status))
    {
        AddInclude("BestellingsItems");
        AddInclude("LeveringsMethode");
        ApplyPaging(specParams.PageSize * (specParams.PageIndex - 1), specParams.PageSize);
        AddOrderByDesc(x => x.BestellingsDatum);
    }

    public OrderSpecification(int id) : base(x => x.Id == id)
    {
        AddInclude("BestellingsItems");
        AddInclude("LeveringsMethode");
    }

    private static OrderStatus? ParseStatus(string status)
    {
        if(Enum.TryParse<OrderStatus>(status, true, out var result)) return result;
        return null;
    }
}
