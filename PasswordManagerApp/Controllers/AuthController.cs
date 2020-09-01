﻿using System;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using EmailService;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PasswordManagerApp.Handlers;
using PasswordManagerApp.Models.ViewModels;
using PasswordManagerApp.Repositories;
using PasswordManagerApp.Services;



namespace PasswordManagerApp.Controllers
{   
    [Route("auth")]
    public class AuthController : Controller
    {


        private readonly IUserService userService;
        private readonly IEmailSender _emailSender;
        private readonly IHttpClientFactory _httpClientFactory; // potem do usuniecia !!!
        public CookieHandler cookieHandler;
        public DataProtectionHelper dataProtectionHelper;

        
        public AuthController(IUserService userService, IEmailSender emailSender, IDataProtectionProvider provider, IHttpClientFactory httpClientFactory)
        {
            this.userService = userService;
            this.userService.EmailSendEvent += UserService_EmailSendEvent;
            _httpClientFactory = httpClientFactory;
            _emailSender = emailSender;
            dataProtectionHelper = new DataProtectionHelper(provider);
            cookieHandler = new CookieHandler(new HttpContextAccessor(), provider);
            



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
            return View(new LoginViewModel());
        }

        [AcceptVerbs("GET", "POST")]
        public IActionResult VerifyEmail(string email)
{
    if (userService.VerifyEmail(email))
    {
        return Json($"Email {email} is already in use.");
    }

    return Json(true);
}
        
        [Route("login")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> LogIn(LoginViewModel model)
        {
            var user = userService.Authenticate(model.Email, model.Password);

            if (user == null)
           {
                //return BadRequest(new { message = "Username or password is incorrect" });
                ModelState.AddModelError("Error", "Username or password is incorrect");
                return View(new LoginViewModel());
            }
            if (user.TwoFactorAuthorization == 1)
            {
                userService.SendTotpToken(user);

                return RedirectToAction(actionName: "TwoFactorLogIn", new { id = dataProtectionHelper.Encrypt(user.Id.ToString(),"QueryStringsEncryptions")  });
            }
            else
            {
                
                await HttpContext.SignInAsync("CookieAuth", new ClaimsPrincipal(userService.GetClaimIdentity(user)));

                return RedirectToAction(controllerName: "Wallet", actionName: "Index");
            }

        
        }
        [AllowAnonymous]
        [Route("register")]
        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }
        
        [Route("register")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public  async Task<IActionResult> Register([Bind] RegisterViewModel model)
        {



            if (!ModelState.IsValid)
             
                return View(model);
            try
            {
                // create user
                var newUser = userService.Create(model.Email, model.Password);
                // log in new created user
                await HttpContext.SignInAsync("CookieAuth", new ClaimsPrincipal(userService.GetClaimIdentity(newUser)));

                return RedirectToAction(controllerName: "Wallet", actionName: "Index");
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

        [Route("[action]/{passwordToCheck}")]
        [HttpGet]

        public IActionResult hibp(string passwordToCheck)
        {

           
            if (passwordToCheck is null)
                return Ok("Brak wprowadzonego hasla");
            var client = _httpClientFactory.CreateClient();
            var wynik = PwnedPasswords.IsPasswordPwnedAsync(passwordToCheck, new CancellationToken(), client).Result;

            if (wynik != -1)
                // return Ok(wynik.Result);
                return Ok(wynik);
            else
                return Ok("-1");

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
        public IActionResult TwoFactorLogIn(string id)
        {   if(HttpContext.User.Identity.IsAuthenticated || id is null)
                return RedirectToAction(controllerName: "Home", actionName: "Index");

            ViewBag.AuthUserId = id;
            
            return View();
            
            
        }

        [Route("twofactor")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> TwoFactorLogIn(string id,string token)    
        {
            int idUser = Int32.Parse(dataProtectionHelper.Decrypt(id, "QueryStringsEncryptions"));
            var user = userService.GetById(idUser);
            
            var verificationStatus = userService.VerifyTotpToken(user, token);
            if(verificationStatus != 1)
            {
                if (verificationStatus == 0)
                {
                    return RedirectToAction(actionName: "TwoFactorLogIn", new { id = dataProtectionHelper.Encrypt(user.Id.ToString(), "QueryStringsEncryptions") });
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