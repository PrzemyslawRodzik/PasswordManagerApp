﻿
using System.ComponentModel.DataAnnotations;


namespace PasswordManagerApp.Models
{
    public class LoginModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        [Display(Name = "Master password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

    }
}
