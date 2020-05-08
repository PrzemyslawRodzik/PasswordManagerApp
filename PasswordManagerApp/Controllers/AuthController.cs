using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Claims;
using System.Threading.Tasks;
using EmailService;
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
        private readonly IEmailSender _emailSender;

        
        public AuthController(IUserService userService, IEmailSender emailSender)
        {
            this.userService = userService;
            this.userService.EmailSendEvent += UserService_EmailSendEvent;
            _emailSender = emailSender;




        }

        private void UserService_EmailSendEvent(object sender, Message e)
        {
            _emailSender.SendEmailAsync(e);
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

            if (user.TwoFactorAuthorization == 1)
            {
                userService.SendTotpToken(user);
                return RedirectToAction(actionName: "TwoFactorLogIn", new { id = user.Id });
            }
            else
            {
                await HttpContext.SignInAsync("CookieAuth", new ClaimsPrincipal(userService.GetClaimIdentity(user)));

                return RedirectToAction(controllerName: "Home", actionName: "Index");
            }




            



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
        
        [Route("twofactor")]
        [HttpGet]
        public IActionResult TwoFactorLogIn(int id)
        {
            ViewBag.AuthUserId = id;
            
            return View();
            
        }

        [Route("twofactor")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> TwoFactorLogIn(int id,string token)
        {
            var user = userService.GetById(id);
            
            var verificationStatus = userService.VerifyTotpToken(user, token);
            if(verificationStatus != 1)
            {
                if (verificationStatus == 0)
                {
                    return RedirectToAction(actionName: "TwoFactorLogIn", new { id = user.Id });
                }
                else
                {
                    return RedirectToAction(actionName: "LogIn");

                }
            }
            else
            {
                await HttpContext.SignInAsync("CookieAuth", new ClaimsPrincipal(userService.GetClaimIdentity(user)));
                return RedirectToAction(controllerName: "Home", actionName: "Index");
            }
                
          
            
                
            


            

            

            



        }





















    }
}