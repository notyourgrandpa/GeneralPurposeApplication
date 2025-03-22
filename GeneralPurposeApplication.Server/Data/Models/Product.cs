using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPurposeApplication.Server.Data.Models
{
    public class Product
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public required string Name { get; set; }

        public required int CategoryId { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public required decimal CostPrice { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public required decimal SellingPrice { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime DateAdded { get; set; } = DateTime.Now;

        public DateTime LastUpdated { get; set; } = DateTime.Now;
    }
}
