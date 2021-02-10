using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace PasswordManagerApp.Models.ViewModels
{
    public class PasswordChangeViewModel 
    {

       
        [Required(ErrorMessage = "Please enter old password")]
        [Display(Name = "Old master password")]
        [DataType(DataType.Password)]
        [Remote(action: "VerifyPassword", controller: "Validation")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please enter new password")]
        [StringLength(40, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [RegularExpression
            (@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[#$^+=!*()@%&]).{8,40}$",
            ErrorMessage = "Passwords must be from 8 to 40 characters and contain at least one of the folowing: " +
                            "upper case (A-Z), lower case (a-z), number (0-9) and special character (#$^+=!*()@%&)"
            )]
        [OldNewPasswordCheckRule]
        [HibpCheckRule]
        [Display(Name = "New password")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }


        [Required(ErrorMessage = "Please enter confirm password")]
        [Display(Name = "Confirm new password")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "New password and confirm password does not match")]
        public string RepeatPassword { get; set; }
    }
}
