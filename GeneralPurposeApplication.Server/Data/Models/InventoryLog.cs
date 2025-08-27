using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPurposeApplication.Server.Data.Models
{
    [Table("InventoryLogs")]
    public class InventoryLog
    {
        [Key]
        public int Id { get; set; }

        public int ProductId { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Required]
        public InventoryChangeType ChangeType { get; set; }

        public string? Remarks { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;

        [ForeignKey(nameof(ProductId))]
        public Product? Product { get; set; }
    }

    public enum InventoryChangeType
    {
        StockIn = 1,
        StockOut = 2,
        Adjustment = 3
    }
}
