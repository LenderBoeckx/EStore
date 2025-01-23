using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Config;

public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
{
    public void Configure(EntityTypeBuilder<IdentityRole> builder)
    {
        builder.HasData(
            new IdentityRole{Id = "ba798536-3db1-4471-923b-ef6e9c0f566c", Name = "Admin", NormalizedName = "ADMIN"},
            new IdentityRole{Id = "bb662bc0-dd28-45fb-8d3a-680ce101f911", Name = "Customer", NormalizedName = "CUSTOMER"}
        );
    }
}
