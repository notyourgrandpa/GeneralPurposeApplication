using GeneralPurposeApplication.Domain.Categories;
using GeneralPurposeApplication.Domain.Customers;
using GeneralPurposeApplication.Domain.Expenses;
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
        DbSet<Product> Products { get; }
        DbSet<Category> Categories { get; }
        DbSet<SalesTransaction> SalesTransactions { get; }
        DbSet<InventoryLog> InventoryLogs { get; }
        DbSet<Expense> Expenses { get; }
        DbSet<Customer> Customers { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
