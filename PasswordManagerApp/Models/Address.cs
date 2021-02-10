using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordManagerApp.Models
{
    [Table("address")]
    public class Address
    {
        [Key]
        public int Id { get; set; }

        [Required]
       
        public string AddressName { get; set; }

        [Required]
        
        public string Street { get; set; }
        [Required]
       
        public string StreetNumber { get; set; }

        [Required]
        
        public string ZipCode { get; set; }

        [Required]
        
        public string City { get; set; }

        public int UserId { get; set; }
        public int PersonalInfoId { get; set; }
        

    }
}
