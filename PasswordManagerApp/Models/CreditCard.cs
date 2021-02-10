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
        
        public string Name { get; set; }
        [Required]
        
        public string CardHolderName { get; set; }
        [Required]
        
        public string CardNumber { get; set; }
        [Required]
        public string SecurityCode { get; set; }
        [Required]
        public string ExpirationDate { get; set; }

        

    }
}
