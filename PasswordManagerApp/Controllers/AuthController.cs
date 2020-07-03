﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Claims;
using System.Threading.Tasks;
using EmailService;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PasswordManagerApp.Handlers;
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
        private readonly IDataProtectionProvider _provider;
        public CookieHandler cookieHandler;

        
        public AuthController(IUserService userService, IEmailSender emailSender, IDataProtectionProvider provider)
        {
            this.userService = userService;
            this.userService.EmailSendEvent += UserService_EmailSendEvent;
            _emailSender = emailSender;
            _provider = provider;
            cookieHandler = new CookieHandler(new HttpContextAccessor(), _provider);



        }

        private void UserService_EmailSendEvent(object sender, Message e)
        {
            _emailSender.SendEmailAsync(e);
        }
        [AllowAnonymous]
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
        [AllowAnonymous]
        [Route("register")]
        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterModel());
        }
        
        [Route("register")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public  async Task<IActionResult> Register([Bind] RegisterModel model)
        {

            
            
            try
            {
                // create user
                var newUser = userService.Create(model.Email, model.Password);
                // log in new created user
                await HttpContext.SignInAsync("CookieAuth", new ClaimsPrincipal(userService.GetClaimIdentity(newUser)));

                return RedirectToAction(controllerName: "Home", actionName: "Index");
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

            var claims = new List<Claim>
{           new Claim(ClaimTypes.Name,User.Identity.Name),
            
           
          new Claim("OsHash", "asd123zxc123asjshjekehjdnmsamxk" )


        };

            // HttpContext.SignInAsync("DeviceAuth", new ClaimsPrincipal(new ClaimsIdentity(claims, "AuthorizedDeviceCookieAuth")),new AuthenticationProperties
            // {IsPersistent = true,});

            
            // cookieHandler.CreateCookie("UserXDeviceAuth", "1234",null);

            //cookieHandler.RemoveCookie("UserXDeviceAuth");

            //string wynik = cookieHandler.ReadCookie("UserXDeviceAuth");
            //string wynikDec = cookieHandler.DecryptCookie("UserXDeviceAuth");



            //Response.WriteAsync(wynikDec);






            //if (User.Identity.IsAuthenticated)
            // return Ok(userService.GetById(int.Parse(User.Identity.Name)));
           // return Ok(User);

           

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

          /*
           *  -> jeśli zalogowany to nie może przejsc do two_factor
           *  -> jeśli nie authenticated też nie może przejść
           *  -> jeśli authenticated ,ale nie zalogowany(w trakcie procedury) to może przejść
           *  Trzeba chyba stworzyć ciasteczko, które przydzieli dostęp  , claim podczas authenticated
           * 
           * 
           * 
           * 
           */
        
        [Route("twofactor")]
        [HttpGet]
        public IActionResult TwoFactorLogIn(int id)
        {   if(HttpContext.User.Identity.IsAuthenticated)
                return RedirectToAction(controllerName: "Home", actionName: "Index");
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