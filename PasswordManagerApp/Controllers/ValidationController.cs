using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PasswordManagerApp.Interfaces;
using PasswordManagerApp.Models;
using PasswordManagerApp.Services;
//using PasswordManagerApp.Models;

namespace PasswordManagerApp.Controllers
{
    public class ValidationController : Controller
    {   
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        public ValidationController(IUnitOfWork unitOfWork, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
        }

        [Route("VerifyEmail")]
        [AcceptVerbs("GET", "POST")]
        public IActionResult VerifyEmail(string email)
        {
            if (_userService.VerifyEmail(email))
            {
                return Json($"Email {email} is already in use.");
            }

            return Json(true);
        }
        [Authorize]
        [Route("VerifyPassword")]
        [AcceptVerbs("GET", "POST")]
        public IActionResult VerifyPassword(string oldpassword)
        {   int authUserId = Int32.Parse(HttpContext.User.Identity.Name);
            var user = _unitOfWork.Users.Find<User>(authUserId);
            if (!_userService.VerifyPasswordHash(oldpassword, Convert.FromBase64String(user.Password), Convert.FromBase64String(user.PasswordSalt)))
            {
                return Json($"Password  is incorrect.");
            }

            return Json(true);
        }

        [Authorize]
        [Route("VerifyLogin")]
        [AcceptVerbs("GET", "POST")]
        public IActionResult VerifyLogin(string website,string login,int id)
        {
            if (_unitOfWork.Context.LoginDatas.Any(l => l.Website == website && l.Login == login))
            {
                return Json($"You already set login {login} for that website {website}");
            }

            return Json(true);
        }
        
        



    }
}