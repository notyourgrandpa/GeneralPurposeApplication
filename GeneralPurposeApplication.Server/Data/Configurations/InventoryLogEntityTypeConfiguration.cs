﻿using GeneralPurposeApplication.Server.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPurposeApplication.Server.Data.Configurations
{
    public class InventoryLogEntityTypeConfiguration : IEntityTypeConfiguration<InventoryLog>
    {
        public void Configure(EntityTypeBuilder<InventoryLog> builder)
        {
            builder.ToTable("InventoryLogs");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).IsRequired();
            builder.Property(x => x.ProductId);
            builder.Property(x => x.QuantityChange);
            builder.Property(x => x.Remarks);
            builder.Property(x => x.Date);
            builder
                .HasOne(x => x.Product)
                .WithMany(x => x.InventoryLogs)
                .HasForeignKey(x => x.ProductId);
        }
    }
}
