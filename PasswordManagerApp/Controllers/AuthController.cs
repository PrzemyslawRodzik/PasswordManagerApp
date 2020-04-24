using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PasswordManagerApp.Models;

namespace PasswordManagerApp.Controllers
{
    [Route("auth")]
    public class AuthController : Controller
    {

        public AuthController()
        {

        }
        [Route("login")]
        [HttpGet]
        public IActionResult LogIn()
        {
            return View(new LoginModel());
        }

        [Route("login")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> LogIn(LoginModel model)
        {
            return null;
        }

        [Route("register")]
        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterModel());
        }

        [Route("register")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            return null;
        }



    }
}