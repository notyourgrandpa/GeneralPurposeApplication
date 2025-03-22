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
    [Table("SalesTransactionItems")]
    public class SalesTransactionItem
    {
        [Key]
        public int Id { get; set; }

        public int SalesTransactionId { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        [ForeignKey(nameof(SalesTransactionId))]
        public required SalesTransaction SalesTransaction { get; set; }

        [ForeignKey(nameof(ProductId))]
        public required Product Product { get; set; }
    }
}
