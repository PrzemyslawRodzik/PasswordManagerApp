using PasswordManagerApp.Handlers;
using PasswordManagerApp.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;

namespace PasswordManagerApp.Models
{
    public class HibpCheckRuleAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            
            string password = (string)value;  
            var isPwned = PwnedPasswords.IsPasswordPwnedAsync(password, new CancellationToken(), null).Result;
            if (isPwned != -1)
                return new ValidationResult($"Selected password have been pwned {isPwned} times. Enter another one!", new[] { validationContext.MemberName });
            else
                return ValidationResult.Success;




        }
    }
}