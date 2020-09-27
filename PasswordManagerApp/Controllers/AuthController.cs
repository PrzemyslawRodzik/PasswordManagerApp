﻿using System;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using EmailService;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using PasswordManagerApp.Handlers;
using PasswordManagerApp.Interfaces;
using PasswordManagerApp.Models;
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
        private readonly IUnitOfWork unitOfWork;

        
        public AuthController(IUserService userService, IEmailSender emailSender, IDataProtectionProvider provider, IHttpClientFactory httpClientFactory,IUnitOfWork unitOfWork)
        {
            this.userService = userService;
            this.userService.EmailSendEvent += UserService_EmailSendEvent;
            _httpClientFactory = httpClientFactory;
            _emailSender = emailSender;
            dataProtectionHelper = new DataProtectionHelper(provider);
            cookieHandler = new CookieHandler(new HttpContextAccessor(), provider);
            this.unitOfWork = unitOfWork;
            



        }
        
        public void OldPasswordsCheck()
        {
                 var allloginDatasList = unitOfWork.Context.LoginDatas.ToList();
                if(allloginDatasList is null)
                    return ;
                var loginDatasListWithOldPasswords = allloginDatasList.Where(x => (DateTime.UtcNow.ToLocalTime() - x.ModifiedDate).Days>=30 ).ToList();
                if(loginDatasListWithOldPasswords is null)
                    return;
                string websitesList = "";
                foreach (var item in loginDatasListWithOldPasswords.GroupBy(x => x.UserId))
                {   
                    websitesList="";
                    var userEmail = userService.GetById(item.Key).Email;
                    item.ToList().ForEach(x => websitesList+=x.Website+", "   );


                    string message = $"wykryto {item.Count()} hasła nie zmieniane od 30 dni dla podanych stron internetowych : {websitesList}!";


                    _emailSender.SendEmailAsync(new Message(new string[] { userEmail },"PasswordManagerApp stare hasła", message));


                }
                return;
        }

        private void UserService_EmailSendEvent(object sender, Message e)
        {
            _emailSender.SendEmailAsync(e);
        }
      
        
        [Route("login")]
        [HttpGet]
        public IActionResult LogIn()
        {
            return View(new LoginViewModel());
        }
       
        
        [Route("login")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> LogIn(LoginViewModel model)
        {
            var user = userService.Authenticate(model.Email, model.Password);

            if (user == null)
            {
                
                ModelState.AddModelError("Error", "Incorrect username or password.");
                return View(new LoginViewModel());
            }
            if (user.TwoFactorAuthorization == 1)
            {
                
                userService.SendTotpToken(user);
                TempData["id"] = dataProtectionHelper.Encrypt(user.Id.ToString(),"QueryStringsEncryptions");
                return RedirectToAction(actionName: "TwoFactorLogIn");
            }
            else
            {
                
                var authProperties = GetAuthTokenExpireTime(user);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,new ClaimsPrincipal(userService.GetClaimIdentity(user)),authProperties);

                return RedirectToAction(controllerName: "Wallet", actionName: "Index");
            }

        
        }
        [Authorize]
        [HttpGet]
        [Route("deleteaccount1step")]
        public IActionResult DeleteAccount1Step() =>  PartialView("~/Views/Auth/DeleteAccount.cshtml",new DeleteAccountViewModel());
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("deleteaccount1step")]
        public IActionResult DeleteAccount1Step(DeleteAccountViewModel model) 
        {   
            int authUserId = Int32.Parse( HttpContext.User.Identity.Name);
            User authUser = userService.GetById(authUserId);

            if(!userService.VerifyPasswordHash(model.Password,Convert.FromBase64String(authUser.Password),Convert.FromBase64String(authUser.PasswordSalt)))
                {
                    ModelState.AddModelError("Error","Password is incorrect");
                    return PartialView("~/Views/Auth/DeleteAccount.cshtml");
                }
            if(!ModelState.IsValid)
                {
                    ModelState.AddModelError("Error","There was an error. Try again or contact support.");
                    return PartialView("~/Views/Auth/DeleteAccount.cshtml");
                }
            
            userService.CreateAndSendAuthorizationToken(authUserId,model.Password);
            return PartialView("~/Views/Auth/DeleteAccountNotification.cshtml");
           
            

        }
        [Authorize]
        [HttpGet]
        [Route("deleteaccount2step")]
        public IActionResult DeleteAccount2step(string token)
        {
            ViewBag.Token = token;
            return View("~/Views/Auth/DeleteAccountConfirmation.cshtml");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("deleteaccount2step")]
        public async Task<IActionResult> DeleteAccount2step(DeleteAccountViewModel model)
        {
            ViewBag.Token = model.Token;
            int authUserId = Int32.Parse(HttpContext.User.Identity.Name);
            if(authUserId==-1)
            {
                ModelState.AddModelError("Error","You need to be log in.");
                return PartialView("~/Views/Auth/DeleteAccountConfirmation.cshtml");
            }
            User authUser = userService.GetById(authUserId);

            if(!userService.VerifyPasswordHash(model.Password,Convert.FromBase64String(authUser.Password),Convert.FromBase64String(authUser.PasswordSalt)))
            {
                    ModelState.AddModelError("Error","Password is incorrect");
                    return PartialView("~/Views/Auth/DeleteAccountConfirmation.cshtml");
            }
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("Error","There was an error. Try again or contact support.");
                return PartialView("~/Views/Auth/DeleteAccountConfirmation.cshtml",new DeleteAccountViewModel());
            }

            bool tokenIsValid = userService.ValidateToken(model.Token,model.Password);
            if(!tokenIsValid)
            {
                ModelState.AddModelError("Error", "Verification token is invalid or expired.");
                return View("~/Views/Auth/DeleteAccountConfirmation.cshtml",new DeleteAccountViewModel());
            }
                
             bool isDeleted = userService.DeleteUser(authUserId);
             if(isDeleted)
                {
                    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    TempData["logoutMessage"] = "You account has been deleted.";
                    return RedirectToAction(controllerName: "Home", actionName: "Index");
                }
            ModelState.AddModelError("Error","There was an error during user delete. Try again or contact support.");
            return PartialView("~/Views/Auth/DeleteAccountConfirmation.cshtml",new DeleteAccountViewModel());
            
            
        }
        private AuthenticationProperties GetAuthTokenExpireTime(User authUser){
            var authProperties = new AuthenticationProperties
                {
                    

                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(authUser.AuthenticationTime),
                    // The time at which the authentication ticket expires. A 
                    // value set here overrides the ExpireTimeSpan option of 
                    // CookieAuthenticationOptions set with AddCookie.

                };
                return authProperties;
        }
        
        
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
                 var authProperties = GetAuthTokenExpireTime(newUser);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(userService.GetClaimIdentity(newUser)),authProperties);

                return RedirectToAction(controllerName: "Wallet", actionName: "Index");
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
           

           
        }
        [Authorize]
        [Route("logout")]
        public async Task<IActionResult> Logout()
        {



            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
             TempData["logoutMessage"] = "You were logged out.";
            return RedirectToAction(controllerName: "Home", actionName: "Index");


        }


        [Route("twofactorlogin")]
        [HttpGet]
        public IActionResult TwoFactorLogIn()
        {  
            var encryptedId = TempData["id"] as string;
            if(HttpContext.User.Identity.IsAuthenticated || encryptedId is null)
               return RedirectToAction(controllerName: "Home", actionName: "Index");

            
            ViewBag.AuthUserId = encryptedId;
            return View(new TwoFactorViewModel());
            
            
            
        }


        [Route("twofactorlogin")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> TwoFactorLogIn(string id,TwoFactorViewModel model)    
        {

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("Error", "Code can not be null.");
                ViewBag.AuthUserId = id;
                return View(new TwoFactorViewModel());
            }
                 
            
            
            int idUser = Int32.Parse(dataProtectionHelper.Decrypt(id, "QueryStringsEncryptions"));
            var user = userService.GetById(idUser);
            
            var verificationStatus = userService.VerifyTotpToken(user, model.Token);
            if(verificationStatus != 1)
            {
                if (verificationStatus == 0)
                {
                    
                    ViewBag.AuthUserId = id;
                    ModelState.AddModelError("Error", "Wrong code.");
                    
                    return View(new TwoFactorViewModel());

                }
                else
                {
                    
                    return RedirectToAction(actionName: "VerificationFailed");

                }
            }
            else
            {       var authProperties = GetAuthTokenExpireTime(user); 
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(userService.GetClaimIdentity(user)),authProperties);
                    return RedirectToAction(controllerName: "Wallet", actionName: "Index");
                

            }
                
         

        }
        [Route("verificationfailed")]
        public IActionResult VerificationFailed()
        {
            ViewBag.Message = "The code has expired.";
            return View();
        }

        [Route("resendcode")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult ReSendCode(string id)
        {
            int idUser = Int32.Parse(dataProtectionHelper.Decrypt(id, "QueryStringsEncryptions"));
            var user = userService.GetById(idUser);
            userService.SendTotpToken(user);
            ViewBag.Message = "Your code has been resent.";
            
            return PartialView("~/Views/Shared/_NotificationAlert.cshtml");
            
        }
        
        























    }
}