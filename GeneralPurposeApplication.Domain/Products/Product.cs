using GeneralPurposeApplication.Domain.Categories;
using GeneralPurposeApplication.Domain.Inventory;
using GeneralPurposeApplication.Domain.Sales;

namespace GeneralPurposeApplication.Domain.Products
{
    public class Product
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public int CategoryId { get; set; }
        public decimal CostPrice { get; set; }
        public decimal SellingPrice { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime DateAdded { get; private set; }
        public DateTime LastUpdated { get; private set; }
        public int Stock { get; set; } = 0;
        public Category? Category { get; set; }
        public ICollection<InventoryLog> InventoryLogs { get; set; } = new List<InventoryLog>();
        public ICollection<SalesTransactionItem> SalesTransactionItems { get; set; } = new List<SalesTransactionItem>();
        public void SetCreated(DateTime now)
        {
            DateAdded = now;
            LastUpdated = now;
        }
        public void SetUpdated(DateTime now)
        {
            LastUpdated = now;
        }
    }
}
