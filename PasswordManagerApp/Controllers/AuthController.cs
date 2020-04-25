using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PasswordManagerApp.Models;
using PasswordManagerApp.Services;

namespace PasswordManagerApp.Controllers
{
    [Route("auth")]
    public class AuthController : Controller
    {

        private readonly IUserService userService;

        public AuthController(IUserService userService)
        {
            this.userService = userService;
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