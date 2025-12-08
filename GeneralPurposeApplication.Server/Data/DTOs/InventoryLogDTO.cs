using GeneralPurposeApplication.Server.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPurposeApplication.Server.Data.DTOs
{
    public class InventoryLogDTO
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public InventoryChangeType ChangeType { get; set; }
        public string? Remarks { get; set; }
        public DateTime Date { get; set; }
        public string ProductName { get; set; } = null!;
        public bool IsVoided { get; set; }
    }
}
