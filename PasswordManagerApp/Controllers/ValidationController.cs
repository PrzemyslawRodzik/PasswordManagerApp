using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PasswordManagerApp.Services;


namespace PasswordManagerApp.Controllers
{
    public class ValidationController : Controller
    {   
        
        private readonly ApiService _apiService;

        public ValidationController(ApiService apiService)
        {
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
            if (!VerifyPasswordHash(password, Convert.FromBase64String(user.Password), Convert.FromBase64String(user.PasswordSalt)))
            {
                return Json($"Password  is incorrect.");
            }

            return Json(true);
        }

        [Authorize]
        [Route("VerifyLogin")]
        [AcceptVerbs("GET", "POST")]
        public IActionResult VerifyLogin(string website,string login,string Id)
        {
            if (!Id.Equals("0"))
                return Json(true);
            var isDuplicateLogin = _apiService.CheckLoginDuplicate(website,login);
             if (isDuplicateLogin)
             {
                 return Json($"You already set login {login} for that website {website}");
             }
             
            return Json(true);
            
        }



        private bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
            if (storedHash.Length != 64) throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");
            if (storedSalt.Length != 128) throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordHash");

            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i]) return false;
                }
            }

            return true;
        }





    }
}