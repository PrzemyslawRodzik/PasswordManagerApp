using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordManagerApp.Models.Entities
{   [Table("paypall_account")]
    public class PaypallAcount
    {   [Key]
        public int Id { get; set; }

        [Required]
        [Column("email")]
        public string Email { get; set; }

        [Required]
        [Column("password")]
        public string Password { get; set; }

        [Required]
        [Column("compromised")]
        public int Compromised { get; set; }

        [Required]
        [Column("modified_date")]
        public DateTime ModifiedDate { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
