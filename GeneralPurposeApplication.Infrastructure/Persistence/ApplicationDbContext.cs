using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using GeneralPurposeApplication.Infrastructure.Identity;
using GeneralPurposeApplication.Domain.Categories;
using GeneralPurposeApplication.Domain.Expenses;
using GeneralPurposeApplication.Domain.Inventory;
using GeneralPurposeApplication.Domain.Products;
using GeneralPurposeApplication.Domain.Sales;
using GeneralPurposeApplication.Domain.Customers;
using GeneralPurposeApplication.Application.Common.Interfaces;
using GeneralPurposeApplication.Application.Services;

namespace GeneralPurposeApplication.Infrastructure.Persistence
{
    public class ApplicationDbContext: IdentityDbContext<ApplicationUser>, IApplicationDbContext
    {
        private readonly ICurrentUserService? _currentUserService;

        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options,
            ICurrentUserService? currentUserService = null) : base(options)
        {
            _currentUserService = currentUserService;
        }

        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Expense> Expenses => Set<Expense>();
        public DbSet<InventoryLog> InventoryLogs => Set<InventoryLog>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<SalesTransaction> SalesTransactions => Set<SalesTransaction>();
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var auditEntries = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added
                         || e.State == EntityState.Modified
                         || e.State == EntityState.Deleted)
                .Select(e => new AuditLog
                {
                    Action = e.State.ToString(),
                    EntityName = e.Entity.GetType().Name,
                    EntityId = (int)e.Property("Id").CurrentValue,
                    PerformedBy = _currentUserService?.UserId ?? "Unknown",
                    Changes = string.Join(", ", e.Properties
                        .Select(p => $"{p.Metadata.Name}: {p.CurrentValue}"))
                }).ToList();

            AuditLogs.AddRange(auditEntries);
            return await base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
            modelBuilder.Entity<Customer>().HasData(
                new Customer
                {
                    Id = 1,
                    Name = "Walk-in",
                    ContactNumber = "None"
                }
            );
        }
    }
}