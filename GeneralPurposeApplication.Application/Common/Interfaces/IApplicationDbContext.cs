using GeneralPurposeApplication.Domain.Categories;
using GeneralPurposeApplication.Domain.Inventory;
using GeneralPurposeApplication.Domain.Products;
using GeneralPurposeApplication.Domain.Sales;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPurposeApplication.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Product> Products { get; set; }
        DbSet<Category> Categories { get; set; }
        DbSet<SalesTransaction> SalesTransactions { get; set; }
        DbSet<InventoryLog> InventoryLogs { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
