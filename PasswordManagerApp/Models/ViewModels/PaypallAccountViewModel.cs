using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordManagerApp.Models.ViewModels
{
    public class PaypallAccountViewModel
    {
        

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        
        public string Password { get; set; }

        [Required]
        public int Compromised { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime ModifiedDate { get; set; }

        
      
    }
}
