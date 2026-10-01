using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Core.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Infrastructure.EntityConfig
{
    public class OrderConfig : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.UserName).IsRequired(true).HasMaxLength(100);
            builder.Property(p => p.FirstName).IsRequired(true).HasMaxLength(100);
            builder.Property(p => p.LastName).IsRequired(true).HasMaxLength(100);
            builder.Property(p => p.EmailAddress).IsRequired(false).HasMaxLength(100);
            builder.Property(p => p.AddressLine).IsRequired(false).HasMaxLength(100);
            builder.Property(p => p.Country).IsRequired(false).HasMaxLength(100);
            builder.Property(p => p.State).IsRequired(false).HasMaxLength(100);
            builder.Property(p => p.ZipCode).IsRequired(false).HasMaxLength(100);
            builder.Property(p => p.TotalPrice).HasColumnType("decimal(18, 2)");
            builder.ToTable("Order");
        }
    }
}
