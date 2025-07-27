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
        
        public int QuantityChange { get; set; }

        public string? Remarks { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;

        [ForeignKey(nameof(ProductId))]
        public Product? Product { get; set; }
    }
}
