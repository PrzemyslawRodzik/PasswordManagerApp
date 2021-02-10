
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
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        
        public string Password { get; set; }

        
        
        public int Compromised { get; set; }

        
        
        public DateTime ModifiedDate { get; set; }

        
    }
}
