using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPurposeApplication.Application.DTOs
{
    public class ProductCreateDTO
    {
        public string Name { get; set; } = null!;
        public int CategoryId { get; set; }
        public decimal CostPrice { get; set; }
        public decimal SellingPrice { get; set; }
        public bool IsActive { get; set; }
    }

}
