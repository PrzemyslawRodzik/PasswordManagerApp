using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordManagerApp.Models
{
    [Table("breached_password")]
    public class BreachedPassword
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column("password")]
        public string Password { get; set; }

        [Required]
        [Column("occuring")]
        public long Occuring { get; set; }




    }
}
