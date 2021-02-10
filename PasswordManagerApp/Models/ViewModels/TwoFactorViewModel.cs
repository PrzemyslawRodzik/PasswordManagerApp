using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordManagerApp.Models.ViewModels
{
    public class TwoFactorViewModel
    {
        [Required]
        [StringLength(6, ErrorMessage = "The {0} must be {2} characters long.", MinimumLength = 6)]
       
        public string Token { get; set; }
       
    }
}
