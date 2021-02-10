using PasswordManagerApp.Handlers;
using PasswordManagerApp.Models.ViewModels;
using PasswordManagerApp.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;

namespace PasswordManagerApp.Models
{
    public class OldNewPasswordCheckRuleAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            
            
            
            PasswordChangeViewModel model = (PasswordChangeViewModel)validationContext.ObjectInstance;
            string oldPassword = model.Password;
            string newPassword = (string)value;  
            if(oldPassword.Equals(newPassword))
                return new ValidationResult($"Selected password is the same as the previous one . Enter another one!", new[] { validationContext.MemberName });
            else
                return ValidationResult.Success;
        }   
        
        



    }
}