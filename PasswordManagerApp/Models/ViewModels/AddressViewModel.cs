using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordManagerApp.Models.ViewModels
{
    public class AddressViewModel
    {
        
       

        [Required]
        [StringLength(45, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 3)]
        
        public string AddressName { get; set; }

        [Required]
        public string Street { get; set; }

        [Required]
        [DataType(DataType.PostalCode, ErrorMessage = "Please enter a valid PIN/ZIP code")]
        
        public string ZipCode { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string Country { get; set; }


        
    }
}
