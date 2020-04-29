using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordManagerApp.Models
{
    public class RegisterModel
    {   [Required]
        public string Email { get; set; }
        [Required]
        [Display(Name = "Master password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        public string  RepeatPassword { get; set; }

    }
}
