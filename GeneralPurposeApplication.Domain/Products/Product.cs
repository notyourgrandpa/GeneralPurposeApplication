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
        public DateTime DateAdded { get; set; } = DateTime.Now;
        public DateTime LastUpdated { get; set; } = DateTime.Now;
        public int Stock { get; set; } = 0;
        public Category? Category { get; set; }
        public ICollection<InventoryLog> InventoryLogs { get; set; } = new List<InventoryLog>();
        public ICollection<SalesTransactionItem> SalesTransactionItems { get; set; } =  new List<SalesTransactionItem>();
    }
}
