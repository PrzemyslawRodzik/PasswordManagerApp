
using System;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace PasswordManagerApp.Models
{   [Table("paypall_account")]
    public class PaypallAcount : UserRelationshipModel
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

        
    }
}
