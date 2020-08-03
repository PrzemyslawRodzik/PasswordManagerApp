using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordManagerApp.Models
{
    [Table("credit_card")]
    public class CreditCard: UserRelationshipModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Column("name")]
        public string Name { get; set; }
        [Required]
        [Column("cardholder_name")]
        public string CardHolderName { get; set; }
        [Required]
        [Column("card_number")]
        public string CardNumber { get; set; }
        [Required]
        [Column("security_code")]
        public string SecurityCode { get; set; }
        [Required]
        [Column("expiration_date")]
        public string ExpirationDate { get; set; }

        

    }
}
