using GeneralPurposeApplication.Domain.Inventory;
using GeneralPurposeApplication.Domain.Sales;
using Microsoft.AspNetCore.Identity;

namespace GeneralPurposeApplication.Infrastructure.Identity
{
    public class ApplicationUser: IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public string? Position { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<SalesTransaction> SalesTransactions { get; set; } = new List<SalesTransaction>();
        public ICollection<InventoryLog> InventoryLogs { get; set; } = new List<InventoryLog>();

    }
}
