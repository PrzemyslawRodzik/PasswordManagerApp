﻿using System;
using System.Linq;
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
       
        
        private readonly JwtHelper _jwtHelper;

        public AuthController(IDataProtectionProvider provider, ApiService apiService, JwtHelper jwtHelper)
        {
           
            _apiService = apiService;
            dataProtectionHelper = new DataProtectionHelper(provider);
            cookieHandler = new CookieHandler(new HttpContextAccessor(), provider);
            _jwtHelper = jwtHelper;
            



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
        public IActionResult LogIn()
        {
            return View(new LoginViewModel());
        }


        [Route("login")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> LogIn(LoginViewModel model)
        {   
            User user = await _apiService.AuthenticateUser(model);

              if (user == null)
              {

                  ModelState.AddModelError("Error", "Incorrect username or password.");
                  return View(new LoginViewModel());
              }
              if (user.TwoFactorAuthorization == 1)
              {

                  
                  TempData["id"] = dataProtectionHelper.Encrypt(user.Id.ToString(),"QueryStringsEncryptions");
                  return RedirectToAction(actionName: "TwoFactorLogIn");
              }
              else
              {
                var apiResponse = await _apiService.GetAccessToken(model);
                
                if (!apiResponse.Success)
                {
                    ModelState.AddModelError("Error", apiResponse.Messages.First());
                    return View(new LoginViewModel());
                }
                
                var isSuccess = await _jwtHelper.ValidateTokenAndSignIn(apiResponse.AccessToken);
                if (!isSuccess)
                {
                    ModelState.AddModelError("Error", "Token is invalid.");
                    return View(new LoginViewModel());
                }

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
        public async Task<IActionResult> DeleteAccount1Step(DeleteAccountViewModel model) 
        {   
           // int authUserId = Int32.Parse( HttpContext.User.Identity.Name);
            var responseFromApi = await _apiService.DeleteAccountProcess(model,"1step");

            if(!responseFromApi.Success)
                {
                    ModelState.AddModelError("Error",responseFromApi.Messages.First());
                    return PartialView("~/Views/Auth/DeleteAccount.cshtml");
                }
            if(!ModelState.IsValid)
                {
                    ModelState.AddModelError("Error","There was an error. Try again or contact support.");
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
            int authUserId;
            try
            {
                authUserId = Int32.Parse(HttpContext.User.Identity.Name);
            }
            catch (Exception)
            {
                authUserId = -1;
            }
            
            if(authUserId==-1)
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
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("Error","There was an error. Try again or contact support.");
                return PartialView("~/Views/Auth/DeleteAccountConfirmation.cshtml",new DeleteAccountViewModel());
            }
           
            if(responseFromApi.Success)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                TempData["logoutMessage"] = "You account has been deleted.";
               
            }
            return RedirectToAction(controllerName: "Home", actionName: "Index");


          
            
            


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
                var authRegisterResponse = await _apiService.RegisterUser(model);

                // log in new created user
                var isSuccess = await _jwtHelper.ValidateTokenAndSignIn(authRegisterResponse.AccessToken);
                if(!isSuccess)
                {
                    ModelState.AddModelError("Error", "Token is invalid");
                    return View(model);
                }

                return RedirectToAction(controllerName: "Wallet", actionName: "Index");
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                ModelState.AddModelError("Error", ex.Message);
                return View(model);
            }
           

           
        }
        [Authorize]
        [Route("logout")]
        public async Task<IActionResult> Logout()
        {
            // trzeba jakos zrevoke ' owac jwt token
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
                    
                    return RedirectToAction(actionName: "VerificationFailed");

                }
            }
            else
            {
                
                var isSuccess = await _jwtHelper.ValidateTokenAndSignIn(apiResponse.AccessToken);
                if(!isSuccess)
                {
                    ViewBag.AuthUserId = id;
                    ModelState.AddModelError("Error", "Token is invalid.");

                    return View(new TwoFactorViewModel());

                }
                return RedirectToAction("Index", "Wallet");




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

        [Route("generatersa")]
        public IActionResult generatersa()    // potem do usuniecia !!!
        {
            using RSA rsa = RSA.Create();


            return Ok(new
            {
                PrivateKey = Convert.ToBase64String(rsa.ExportRSAPrivateKey()),
                PublicKey = Convert.ToBase64String(rsa.ExportRSAPublicKey()),
            });
            
            
        }
    }

}
        
      