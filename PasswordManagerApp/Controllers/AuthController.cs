using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PasswordManagerApp.Models;
using PasswordManagerApp.Services;
using UAParser;


namespace PasswordManagerApp.Controllers
{
    [Route("auth")]
    public class AuthController : Controller
    {


        private readonly IUserService userService;
        

      

        public AuthController(IUserService userService)
        {
            this.userService = userService;
            this.userService.EmailSendEvent += UserService_AuthenticationSuccessfullEvent;
            
           
            
        }

        private void UserService_AuthenticationSuccessfullEvent(object sender, string e)
        {
            

            var logPath = System.IO.Path.GetTempFileName();
            var logFile = System.IO.File.Create(logPath);
            var logWriter = new System.IO.StreamWriter(logFile);
            logWriter.WriteLine("Wyslalem maila asasdasdasd @@@@@@@@@@@@@@@");
            logWriter.Dispose();
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
            var user = userService.Authenticate(model.Email, model.Password);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });

        
            

          await HttpContext.SignInAsync("CookieAuth", new ClaimsPrincipal(userService.GetClaimIdentity(user)));


            
          
            

            return RedirectToAction(controllerName: "Home", actionName: "Index");


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
        public  IActionResult Register([Bind] RegisterModel model)
        {

            
            
            try
            {
                // create user
                userService.Create(model.Email, model.Password);
                return Ok();
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
           

           
        }
        [Route("logout")]
        public async Task<IActionResult> Logout()
        {



            await HttpContext.SignOutAsync("CookieAuth");

            return RedirectToAction(controllerName: "Home", actionName: "Index");


        }
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
             
            if (User.Identity.IsAuthenticated)
            // return Ok(userService.GetById(int.Parse(User.Identity.Name)));
            return Ok(User.Claims);

           

            return Forbid();

            


           // var currentUserId = int.Parse(User.Identity.Name);
           // if (id != currentUserId)
            //    return Forbid();

           // var user = userService.GetById(id);

          //  if (user == null)
           //     return NotFound();

           // return Ok(user);
        }

        [Route("agent")]
        [HttpGet]
        public IActionResult agent()
        {

            var ip = HttpContext.Connection.RemoteIpAddress.ToString();

            return Ok(ip);







            



            

            

            
        }




    }
}