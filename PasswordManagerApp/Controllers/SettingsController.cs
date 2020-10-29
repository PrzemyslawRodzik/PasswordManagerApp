using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using PasswordGenerator;
using PasswordManagerApp.ApiResponses;
using PasswordManagerApp.Handlers;
using PasswordManagerApp.Models;
using PasswordManagerApp.Models.ViewModels;
using PasswordManagerApp.Services;

namespace PasswordManagerApp.Controllers
{   
    [Authorize]
    [Route("settings")]
    public class SettingsController : Controller
    {
       
        private readonly ApiService _apiService;
        private readonly JwtHelper _jwtHelper;
        private readonly LogInHandler _logInHandler;
        private readonly EncryptionService _encryptionService;

        public SettingsController(ApiService apiService, JwtHelper jwtHelper, LogInHandler logInHandler,EncryptionService encryptionService)
        {
             _apiService = apiService;
            _jwtHelper = jwtHelper;
            _logInHandler = logInHandler;
            _encryptionService = encryptionService;
        }

        public string AuthUserId { get { return HttpContext.User.Identity.Name; } }

        public IActionResult Index()
        {
            PopulateForm();

           
            return View();
        }
        private void  PopulateForm()
        {
            ViewData["2F"] = HttpContext.User.FindFirst("TwoFactorAuth").Value;
            ViewData["PassNot"] = HttpContext.User.FindFirst("PasswordNotifications").Value;
            ViewData["valueTime"] = HttpContext.User.FindFirst("AuthTime").Value;
        }






        [HttpGet]
        [Route("passwordgenerate")]
        public IActionResult PasswordGenerator()
        {

            ViewBag.Password = "";
            return PartialView("PasswordGenerator", new PassGeneratorViewModel());
        }
        
        [HttpGet]
        [Route("passwordgeneratequick")]
        public string PasswordGeneratorQuick() => new Password(true,true,true,true,13).Next();

        [HttpPost]
        [Route("hibpcheck")]
        public string HibpCheck(string password)
        {
           var hibpResult =  PwnedPasswords.IsPasswordPwnedAsync(password, new CancellationToken(), null).Result;
            if (hibpResult <= 0)
                return "Your password is OK :)";
            else
                return $"Your password have been pwned {hibpResult}. Please, change your password.";
        }

        


        [HttpPost]
        [Route("passwordgenerate")]
        public IActionResult PasswordGenerator(PassGeneratorViewModel model)
        {
            string result;

            if (!(model.Length >= 8 && model.Length <= 128))
                model.Length = 16;

            if (model.IncludeLowercase == false && model.IncludeUppercase == false && model.IncludeSpecial == false && model.IncludeNumeric == false)
            {
                result = new Password().LengthRequired(model.Length).Next();
            }
            else
            {
                var pwd = new Password(model.IncludeLowercase, model.IncludeUppercase, model.IncludeNumeric, model.IncludeSpecial, model.Length);

                result = pwd.Next();
            }


            ViewBag.Password = result;
            return PartialView("PasswordGenerator", model);





        }
       
        [HttpGet]
        [Route("passwordchange")]

        public IActionResult PasswordChange() => PartialView("~/Views/Auth/PasswordChange.cshtml", new PasswordChangeViewModel());
           
             
       

        [Route("passwordchange")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> PasswordChange(PasswordChangeViewModel model)  
        {
            
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("Error", "Change highlighted fields.");
                return PartialView("~/Views/Auth/PasswordChange.cshtml", new PasswordChangeViewModel());
            }
           ApiResponse apiResponse =  await _apiService.ChangeMasterPassword(model);
            
             if(apiResponse.Success)
                {
                _encryptionService.AddOrUpdateEncryptionKey(AuthUserId, model.NewPassword);
                    ViewBag.Message = apiResponse.Messages.First();
                    return PartialView("~/Views/Shared/_NotificationAlert.cshtml");
                }
            else
            {
                ModelState.AddModelError("Error", apiResponse.Messages.First());
                return PartialView("~/Views/Auth/PasswordChange.cshtml", new PasswordChangeViewModel());
            }
        }





        [HttpPost]
        [Route("updatepreferences")]
        public async Task<IActionResult> UpdatePreferences(string switch2F,string switchPnot, string sliderVerTime)
        {
            

            var apiResponse = await _apiService.UpdateUserPreferences(switch2F, switchPnot, sliderVerTime);


            if (apiResponse.Success)
            {
                ClaimsPrincipal claimsPrincipal;
                AuthenticationProperties authProperties;
                _jwtHelper.ValidateToken(apiResponse.AccessToken,out claimsPrincipal, out authProperties);
                await _logInHandler.LogInUser(claimsPrincipal, authProperties);
                
                ViewBag.Message = apiResponse.Messages.First();
                return PartialView("~/Views/Shared/_NotificationAlert.cshtml");
            }
            else
            {
                ViewBag.Message = apiResponse.Messages.First();
                return PartialView("~/Views/Shared/_NotificationAlert.cshtml");
            }
        }

        
        
              
            
            
            
        
        







    }
}
