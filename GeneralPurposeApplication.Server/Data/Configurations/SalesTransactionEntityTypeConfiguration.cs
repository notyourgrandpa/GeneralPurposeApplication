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
            builder.Property(x => x.ProcessedByUserId).IsRequired();
            builder.Property(x => x.Date).IsRequired();
            builder.Property(x => x.IsVoided).HasDefaultValue(false);
            //builder.HasOne(x => x.ProcessedByUser)
            //   .WithMany(u => u.SalesTransactions)
            //   .HasForeignKey(x => x.ProcessedByUserId)
            //   .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.Customer)
                .WithMany(s => s.SalesTransactions)
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
