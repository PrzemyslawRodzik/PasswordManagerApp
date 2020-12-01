using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordManagerApp.Models
{   [Table("phone_number")]
    public class PhoneNumber
    {
        [Key]
        public int Id { get; set; }
        [Required]
       
        public string NickName { get; set; }
        [Required]
        
        public string TelNumber { get; set; }
        [Required]
        
        public string Type { get; set; }
        
        public int PersonalInfoId { get; set; }
        public int UserId { get; set; }
        


    }
}
