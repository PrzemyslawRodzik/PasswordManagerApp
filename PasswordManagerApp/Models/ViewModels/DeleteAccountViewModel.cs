using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordManagerApp.Models.ViewModels
{
    public class DeleteAccountViewModel
    {
       
        public string Token { get; set; }
        
        [Required(ErrorMessage = "Please enter password")]
        [Display(Name = "Master password")]
        [DataType(DataType.Password)]
        [Remote(action: "VerifyPassword", controller: "Validation")]
        public string Password { get; set; }
    }
}
