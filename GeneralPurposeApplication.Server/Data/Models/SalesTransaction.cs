using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPurposeApplication.Server.Data.Models
{
    [Table("SalesTransaction")]
    public class SalesTransaction
    {
        [Key]
        public int Id { get; set; }        

        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalAmount { get; set; }

        public required string PaymentMethod { get; set; }

        public string ProcessedByUserId { get; set; } = string.Empty;

        public DateTime Date { get; set; } = DateTime.Now;

        public ICollection<SalesTransactionItem> SalesTransactionItems { get; set; } = new List<SalesTransactionItem>();

        [ForeignKey(nameof(ProcessedByUserId))]
        public ApplicationUser ProcessedByUser { get; set; } = null!;
    }
}
