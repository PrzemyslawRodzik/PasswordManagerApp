using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordManagerApp.Models.ViewModels
{
    public class CreditCardViewModel
    {
        
        

        [Required]
        [StringLength(45, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 3)]
        public string Name { get; set; }

        [Required]
        public string CardHolderName { get; set; }

        [Required]
        [DataType(DataType.CreditCard)]
        public string CardNumber { get; set; }

        [Required]
        [RegularExpression(@"^[0-9]{3,4}$",ErrorMessage = "CVV number seems to be wrong, it consists of 3 to 4 numbers!")]
        public string SecurityCode { get; set; }

        [Required]
        public string ExpirationDate { get; set; }


        
    }
}
