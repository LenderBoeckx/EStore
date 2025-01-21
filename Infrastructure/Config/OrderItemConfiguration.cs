using System;
using Core.Entities.OrderAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Config;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    //relaties van entiteit OrderItem configureren
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.OwnsOne(x => x.ItemBesteld, o => o.WithOwner());
        builder.Property(x => x.Prijs).HasColumnType("decimal(18,2)");
    }
}
