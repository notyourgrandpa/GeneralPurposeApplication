using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.Metrics;
using Microsoft.EntityFrameworkCore;

namespace GeneralPurposeApplication.Server.Data.Models
{
    [Table("Products")]
    [Index(nameof(Name))]
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(100)]
        public required string Name { get; set; }

        public int CategoryId { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal CostPrice { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal SellingPrice { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime DateAdded { get; set; } = DateTime.Now;

        public DateTime LastUpdated { get; set; } = DateTime.Now;

        public int Stock { get; set; } = 0;

        [ForeignKey(nameof(CategoryId))]
        public Category? Category { get; set; }

        public ICollection<InventoryLog> InventoryLogs { get; set; } = new List<InventoryLog>();
        public ICollection<SalesTransactionItem> SalesTransactionItems { get; set; } =  new List<SalesTransactionItem>();
    }
}
