using GeneralPurposeApplication.Server.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPurposeApplication.Server.Data.DTOs
{
    public class InventoryLogCreateInputDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public InventoryChangeType ChangeType { get; set; }
        public string? Remarks { get; set; }
    }
}
