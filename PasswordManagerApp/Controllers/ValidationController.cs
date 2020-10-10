using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using PasswordManagerApp.Models;
using PasswordManagerApp.Services;
//using PasswordManagerApp.Models;

namespace PasswordManagerApp.Controllers
{
    public class ValidationController : Controller
    {   
        
        private readonly IUserService _userService;
        private readonly ApiService _apiService;

        public ValidationController(IUserService userService,ApiService apiService)
        {
            
            _userService = userService;
            _apiService = apiService;
        }

        [Route("VerifyEmail")]
        [AcceptVerbs("GET", "POST")]
        public IActionResult VerifyEmail(string email)
        {
            var isEmailAvailable = _apiService.CheckEmailAvailability(email);
            if (!isEmailAvailable)
            {
                return Json($"Email {email} is already in use.");
            }

            return Json(true);
        }
        [Authorize]
        [Route("VerifyPassword")]
        [AcceptVerbs("GET", "POST")]
        public IActionResult VerifyPassword(string password)
        {   
            var user = _apiService.GetAuthUser();
            if (!_userService.VerifyPasswordHash(password, Convert.FromBase64String(user.Password), Convert.FromBase64String(user.PasswordSalt)))
            {
                return Json($"Password  is incorrect.");
            }

            return Json(true);
        }

        [Authorize]
        [Route("VerifyLogin")]
        [AcceptVerbs("GET", "POST")]
        public IActionResult VerifyLogin(string website,string login)
        {
            var isDuplicateLogin = _apiService.CheckLoginDuplicate(website,login);
             if (isDuplicateLogin)
             {
                 return Json($"You already set login {login} for that website {website}");
             }
             
            return Json(true);
            
        }
        
        



    }
}