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
}
