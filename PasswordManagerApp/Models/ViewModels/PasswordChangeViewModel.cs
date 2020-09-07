using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordManagerApp.Models.ViewModels
{
    public class PasswordChangeViewModel
    {

        [Key]
        public int Key { get; set; }

        [Required(ErrorMessage = "Please enter master password")]
        [StringLength(40, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [RegularExpression
            (@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[#$^+=!*()@%&]).{8,40}$",
            ErrorMessage = "Passwords must be from 8 to 40 characters and contain at least one of the folowing: " +
                            "upper case (A-Z), lower case (a-z), number (0-9) and special character (#$^+=!*()@%&)"
            )]
        [HibpCheckRule]
        [Display(Name = "Master password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }


        [Required(ErrorMessage = "Please enter confirm master password")]
        [Display(Name = "Confirm master password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password and confirm password does not match")]
        public string RepeatPassword { get; set; }
    }
}
