using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPurposeApplication.Server.Data.Models
{
    [Table("Users")]
    public class User
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(50)]
        public required string Username { get; set; }

        public required string Password { get; set; }
    }
}
