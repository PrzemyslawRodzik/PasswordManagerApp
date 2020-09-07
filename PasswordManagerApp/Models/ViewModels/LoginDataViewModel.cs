﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordManagerApp.Models.ViewModels
{
    public class LoginDataViewModel
    {
        
        
        [Required]
        [StringLength(20, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 3)]
        public string Name { get; set; }
        [Required(ErrorMessage = "Please enter password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        
        [Required(ErrorMessage = "Please enter confirm password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password and confirm password does not match")]
        public string ConfirmPassword { get; set; }


        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        
        public string Login { get; set; }

        [DataType(DataType.Url)]
        [Remote(action: "VerifyLogin", controller: "Wallet", AdditionalFields = nameof(Login))]
        public string Website { get; set; }
        
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime ModifiedDate { get; set; }
    }
}
