using GeneralPurposeApplication.Server.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPurposeApplication.Server.Data.Configurations
{
    public class SalesTransactionEntityTypeConfiguration : IEntityTypeConfiguration<SalesTransaction>
    {
        public void Configure(EntityTypeBuilder<SalesTransaction> builder)
        {
            builder.ToTable("SalesTransactions");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).IsRequired();
            builder.Property(x => x.TotalAmount).HasColumnType("decimal(10,2)");
            builder.Property(x => x.PaymentMethod).IsRequired();
            builder.Property(x => x.ProcessedBy).IsRequired();
            builder.Property(x => x.Date).IsRequired();
        }
    }
}
