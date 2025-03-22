using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPurposeApplication.Server.Data.Models
{
    [Table("AuditLogs")]
    public class AuditLog
    {
        [Key]
        public int Id { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public required string Action { get; set; }
        public required string EntityName { get; set; }
        public int EntityId { get; set; }
        public required string PerformedBy { get; set; }
        public required string Changes { get; set; }
    }
}
