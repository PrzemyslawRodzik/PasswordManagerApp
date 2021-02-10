﻿using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PasswordManagerApp.ApiResponses;
using PasswordManagerApp.Handlers;
using PasswordManagerApp.Models;
using PasswordManagerApp.Models.ViewModels;
using PasswordManagerApp.Services;



namespace PasswordManagerApp.Controllers
{   
    [Route("auth")]
    public class AuthController : Controller
    {


        

        private readonly ApiService _apiService; 
        public CookieHandler cookieHandler;
        public DataProtectionHelper dataProtectionHelper;
        public LogInHandler _logInHandler;
        private readonly JwtHelper _jwtHelper;
        private readonly EncryptionService _encryptionService;
        private readonly NotificationService _notify;

        public AuthController(IDataProtectionProvider provider, ApiService apiService, JwtHelper jwtHelper, LogInHandler logInHandler,IConfiguration config, EncryptionService encryptionService, NotificationService notify)
        {
           
            _apiService = apiService;
            dataProtectionHelper = new DataProtectionHelper(provider);
            cookieHandler = new CookieHandler(new HttpContextAccessor(), provider, config);
            _jwtHelper = jwtHelper;
            _logInHandler = logInHandler;
            _encryptionService = encryptionService;
            _notify = notify;





        }
        

        /*
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
        */




        [Route("login")]
        [HttpGet]
        public IActionResult LogIn() => View(new LoginViewModel());
        


        [Route("login")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> LogIn(LoginViewModel model)
        {   
            var apiResponse = await _apiService.LogIn(model);
            if (!apiResponse.Success)
            {
                ModelState.AddModelError("Error", apiResponse.Messages.First());
                return View(new LoginViewModel());
            }
            ClaimsPrincipal claimsPrincipal;
            AuthenticationProperties authProperties;
            var isSuccess = _jwtHelper.ValidateToken(apiResponse.AccessToken, out claimsPrincipal, out authProperties);
            if (!isSuccess)
            { 
               ModelState.AddModelError("Error", "Json Web Token is invalid.");
               return View(new LoginViewModel());
            }
             await _logInHandler.LogInUser(claimsPrincipal, authProperties);
             _encryptionService.AddOrUpdateEncryptionKey(claimsPrincipal.Identity.Name, model.Password);
            OnPasswordSave(claimsPrincipal.Identity.Name, model.Password);
            
            return RedirectToAction(controllerName: "Wallet", actionName: "Index");
        }

        private async void OnPasswordSave(string userId, string password)
        {
            await Task.Delay(3000);
            _notify.UserPasswordSaveEvent(new UserCredentials { UserId = userId, Password = password });
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
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("Error", "There was an error. Try again or contact support.");
                return PartialView("~/Views/Auth/DeleteAccount.cshtml");
            }
            var responseFromApi = _apiService.DeleteAccountProcess(model,"1step").Result;

            if(!responseFromApi.Success)
                {
                    ModelState.AddModelError("Error",responseFromApi.Messages.First());
                    return PartialView("~/Views/Auth/DeleteAccount.cshtml");
                }
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
            var userIsAuth = HttpContext.User.Identity.IsAuthenticated;

            if(!userIsAuth)
            {
                ModelState.AddModelError("Error","You need to be log in.");
                return PartialView("~/Views/Auth/DeleteAccountConfirmation.cshtml");
            }
            
            var responseFromApi = await _apiService.DeleteAccountProcess(model, "2step");

            if(!responseFromApi.Success)
            {
                    ModelState.AddModelError("Error",responseFromApi.Messages.First());
                    return PartialView("~/Views/Auth/DeleteAccountConfirmation.cshtml");
            }
            
            if(responseFromApi.Success)
            {
                _encryptionService.RemoveEncryptionKey(HttpContext.User.Identity.Name);
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                TempData["logoutMessage"] = "You account has been deleted.";
               
            }
            return RedirectToAction(controllerName: "Home", actionName: "Index");


          
            
            


        }
        
        
        
        [Route("register")]
        [HttpGet]
        public IActionResult Register() => View(new RegisterViewModel());
        
        
        [Route("register")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public  async Task<IActionResult> Register([Bind] RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
  
            var authRegisterResponse = await _apiService.RegisterUser(model);

            if(!authRegisterResponse.Success)
               {
                   ModelState.AddModelError("Error", authRegisterResponse.Messages.First());
                   return View(model);
               }
            ClaimsPrincipal claimsPrincipal;
            AuthenticationProperties authProperties;
            var isTokenValid = _jwtHelper.ValidateToken(authRegisterResponse.AccessToken, out claimsPrincipal, out authProperties);
            if(!isTokenValid)
              {     
                    ModelState.AddModelError("Error", "Token is invalid");
                    return View(model);
              }
            await _logInHandler.LogInUser(claimsPrincipal, authProperties);
            _encryptionService.AddOrUpdateEncryptionKey(claimsPrincipal.Identity.Name, model.Password);
            return RedirectToAction(controllerName: "Wallet", actionName: "Index");  
        }
        [Authorize]
        [Route("logout")]
        public async Task<IActionResult> Logout()
        {
            _encryptionService.RemoveEncryptionKey(HttpContext.User.Identity.Name);
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
            

            ApiTwoFactorResponse apiResponse = await _apiService.TwoFactorLogIn(idUser,model.Token);
            
            
            if(apiResponse.VerificationStatus != 1)
            {
                if (apiResponse.VerificationStatus == 0)
                {
                    
                    ViewBag.AuthUserId = id;
                    ModelState.AddModelError("Error", apiResponse.Messages.First());
                    
                    return View(new TwoFactorViewModel());

                }
                else
                {
                    TempData["logoutMessage"] = "Your code has expired. Try log in again.";
                    _encryptionService.RemoveEncryptionKey(idUser.ToString());
                    return RedirectToAction(controllerName: "Home", actionName: "Index");
                }
            }
            
            
                ClaimsPrincipal claimsPrincipal;
                AuthenticationProperties authProperties;
                var isSuccess = _jwtHelper.ValidateToken(apiResponse.AccessToken, out claimsPrincipal,out authProperties);
                if(!isSuccess)
                {
                    ViewBag.AuthUserId = id;
                    ModelState.AddModelError("Error", "Token is invalid.");

                    return View(new TwoFactorViewModel());

                }
                await _logInHandler.LogInUser(claimsPrincipal, authProperties);
            
            return RedirectToAction("Index", "Wallet");




            
                
         

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
        public async Task<IActionResult> ReSendCode(string id)
        {
            int idUser = Int32.Parse(dataProtectionHelper.Decrypt(id, "QueryStringsEncryptions"));
            
            ApiResponse apiResponse = await _apiService.SendTotpByEmail(idUser);
            if(!apiResponse.Success)
            {
                ViewBag.Message = apiResponse.Messages.First();
                return PartialView("~/Views/Shared/_NotificationAlert.cshtml");
            }
            
            ViewBag.Message = "Your code has been resend.";
            return PartialView("~/Views/Shared/_NotificationAlert.cshtml");
            





        }

        

    }

}
        
      