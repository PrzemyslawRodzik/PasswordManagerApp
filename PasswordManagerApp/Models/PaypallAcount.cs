
using System;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace PasswordManagerApp.Models
{   
    public class PaypalAccount : UserRelationshipModel,ICompromisedModel, IPasswordModel
    {   
        [Key]
        public int Id { get; set; }
        [Required]
        
        public string Name { get; set; }

        [Required]
        
        public string Email { get; set; }

        [Required]
        
        public string Password { get; set; }

        [Required]
        
        public int Compromised { get; set; }

        [Required]
        
        public DateTime ModifiedDate { get; set; }

        
    }
}
