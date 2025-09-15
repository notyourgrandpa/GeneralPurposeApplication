using Microsoft.EntityFrameworkCore;
using GeneralPurposeApplication.Server.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace GeneralPurposeApplication.Server.Data
{
    public class ApplicationDbContext: IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext() : base()
        {
        }

        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Expense> Expenses => Set<Expense>();
        public DbSet<InventoryLog> InventoryLogs => Set<InventoryLog>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<SalesTransaction> SalesTransactions => Set<SalesTransaction>();
        public DbSet<SalesTransactionItem> SalesTransactionItems => Set<SalesTransactionItem>();
        public DbSet<Customer> Customers => Set<Customer>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            modelBuilder.Entity<Customer>().HasData(
                new Customer
                {
                    Id = 1,
                    Name = "Walk-in",
                    ContactNumber = "09096846407"
                }
            );
        }
    }
}
