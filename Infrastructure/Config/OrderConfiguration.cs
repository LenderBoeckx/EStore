using System;
using Core.Entities.OrderAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Config;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    //relaties van entiteit Order configureren
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.OwnsOne(x => x.LeverAddress, o => o.WithOwner());
        builder.OwnsOne(x => x.BetalingsOverzicht, o => o.WithOwner());
        builder.Property(x => x.BestellingsStatus).HasConversion(
            o => o.ToString(),
            o => (OrderStatus)Enum.Parse(typeof(OrderStatus), o)
        );
        builder.Property(x => x.Subtotaal).HasColumnType("decimal(18,2)");
        //als een order verwijderd wordt, dan worden ook de bijhorende order items verwijderd
        builder.HasMany(x => x.BestellingsItems).WithOne().OnDelete(DeleteBehavior.Cascade);
        builder.Property(x => x.BestellingsDatum).HasConversion(
            d => d.ToUniversalTime(),
            d => DateTime.SpecifyKind(d, DateTimeKind.Utc)
        );
    }
}
